<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <div id="videoContainer" style="display: inline-block; border: 4px solid transparent; border-radius: 8px; transition: border-color 0.3s ease;">
            <video id="video" width="320" height="240" autoplay muted playsinline></video>
            <img id="capturedImage" style="display:none; width: 320px; height: 240px; border-radius: 8px;" />
        </div>
        <canvas id="canvas" style="display:none;"></canvas>
        <p id="statusText" style="font-weight: bold; margin-top: 10px; color: #444;"></p>
    </div>

    <input type="hidden" name="Type" id="EntryType" />

    <div class="mt-5 form-group">
        <div class="col d-flex justify-content-center mb-4">
            @if (ViewBag.InOut == "I")
            {
                <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">Punch In</button>
            }
        </div>
        <div class="col d-flex justify-content-center">
            @if (ViewBag.InOut == "O")
            {
                <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">Punch Out</button>
            }
        </div>
    </div>
</form>

<script>
    window.addEventListener("DOMContentLoaded", async () => {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");
        const capturedImage = document.getElementById("capturedImage");
        const EntryTypeInput = document.getElementById("EntryType");
        const successSound = document.getElementById("successSound");
        const errorSound = document.getElementById("errorSound");
        const statusText = document.getElementById("statusText");
        const videoContainer = document.getElementById("videoContainer");

        let blinkCount = 0;
        let blinked = false;
        let blinkValidUntil = 0;
        let blinkCountdownInterval;

        const EAR_THRESHOLD = 0.26;
        const BLINK_GAP = 300;
        const DOUBLE_BLINK_WINDOW = 1500;
        const ALLOW_SUBMIT_DURATION = 10000;

        let lastEARBelowThresholdTime = 0;
        let firstBlinkTime = 0;

        const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

        await Promise.all([
            faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
            faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi')
        ]);
        console.log("Models loaded");
        startVideo();

        function startVideo() {
            navigator.mediaDevices.getUserMedia({
                video: { facingMode: "user", width: { ideal: 640 }, height: { ideal: 480 } }
            })
                .then(stream => {
                    video.srcObject = stream;
                    video.play();
                    video.addEventListener("loadeddata", () => {
                        const checkReady = setInterval(() => {
                            if (video.videoWidth > 0 && video.videoHeight > 0) {
                                clearInterval(checkReady);
                                detectBlink();
                            }
                        }, 100);
                    });
                })
                .catch(console.error);
        }

        function getEAR(eye) {
            const a = distance(eye[1], eye[5]);
            const b = distance(eye[2], eye[4]);
            const c = distance(eye[0], eye[3]);
            return (a + b) / (2.0 * c);
        }

        function distance(p1, p2) {
            return Math.hypot(p1.x - p2.x, p1.y - p2.y);
        }

        async function detectBlink() {
            const now = Date.now();

            if (blinked && now < blinkValidUntil) {
                requestAnimationFrame(detectBlink);
                return;
            }

            const detection = await faceapi.detectSingleFace(video, detectorOptions).withFaceLandmarks();

            if (detection) {
                const leftEye = detection.landmarks.getLeftEye();
                const rightEye = detection.landmarks.getRightEye();
                const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

                if (avgEAR < EAR_THRESHOLD) {
                    if (now - lastEARBelowThresholdTime > BLINK_GAP) {
                        blinkCount++;

                        if (blinkCount === 1) {
                            firstBlinkTime = now;
                        }

                        if (blinkCount === 2 && now - firstBlinkTime <= DOUBLE_BLINK_WINDOW) {
                            blinked = true;
                            blinkValidUntil = now + ALLOW_SUBMIT_DURATION;
                            blinkCount = 0;
                            showGreenBorder();
                            captureImage(); // <-- Captures image on double blink
                            startCountdown();
                        } else if (blinkCount > 2 || now - firstBlinkTime > DOUBLE_BLINK_WINDOW) {
                            blinkCount = 0;
                        }

                        lastEARBelowThresholdTime = now;
                    }
                }

                if (!blinked) {
                    statusText.textContent = "Please double blink to verify liveness";
                    videoContainer.style.borderColor = "red";
                }
            } else {
                statusText.textContent = "No face detected";
                videoContainer.style.borderColor = "gray";
                blinked = false;
                blinkCount = 0;
            }

            requestAnimationFrame(detectBlink);
        }

        function showGreenBorder() {
            videoContainer.style.borderColor = "limegreen";
        }

        function captureImage() {
            const context = canvas.getContext("2d");
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            const imageData = canvas.toDataURL("image/jpeg");
            capturedImage.src = imageData;
            capturedImage.style.display = "block";
            video.style.display = "none";
        }

        function startCountdown() {
            let remaining = ALLOW_SUBMIT_DURATION / 1000;
            statusText.textContent = `Double blink detected! You can proceed. (${remaining}s)`;

            clearInterval(blinkCountdownInterval);
            blinkCountdownInterval = setInterval(() => {
                remaining--;
                if (remaining > 0) {
                    statusText.textContent = `You can proceed. (${remaining}s)`;
                } else {
                    clearInterval(blinkCountdownInterval);
                    blinked = false;
                    videoContainer.style.borderColor = "red";
                    statusText.textContent = "Please double blink to verify liveness";

                    // Reset view
                    video.style.display = "block";
                    capturedImage.style.display = "none";
                }
            }, 1000);
        }

        window.captureImageAndSubmit = function (entryType) {
            if (!blinked || Date.now() > blinkValidUntil) {
                videoContainer.style.borderColor = "red";
                statusText.textContent = "Double blink required before submitting";
                Swal.fire({
                    title: "Liveness Check Failed",
                    text: "Please double blink to verify you're not using a static image.",
                    icon: "warning"
                });
                return;
            }

            blinked = false;
            clearInterval(blinkCountdownInterval);
            statusText.textContent = "";
            videoContainer.style.borderColor = "transparent";

            EntryTypeInput.value = entryType;

            const imageData = capturedImage.src;

            Swal.fire({
                title: "Verifying Face...",
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => Swal.showLoading()
            });

            fetch("/AS/Geo/AttendanceData", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Type: entryType, ImageData: imageData })
            })
                .then(res => res.json())
                .then(data => {
                    const now = new Date().toLocaleString();
                    if (data.success) {
                        successSound.play();
                        triggerHapticFeedback("success");
                        Swal.fire({
                            title: "Face Matched!",
                            text: `Attendance Recorded.\nDate & Time: ${now}`,
                            icon: "success",
                            timer: 3000,
                            showConfirmButton: false
                        }).then(() => location.reload());
                    } else {
                        errorSound.play();
                        triggerHapticFeedback("error");
                        Swal.fire({
                            title: "Face Not Recognized.",
                            text: `Click the button again to retry.\nDate & Time: ${now}`,
                            icon: "error"
                        });
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    triggerHapticFeedback("error");
                    Swal.fire("Error!", "An error occurred while processing your request.", "error");
                });
        };

        function triggerHapticFeedback(type) {
            if ("vibrate" in navigator) {
                navigator.vibrate(type === "success" ? 100 : [200, 100, 200]);
            }
        }
    });
</script>





this is my frontend side , in this i want to make changes that after the blink video capture the image of user in place of video after that when the user clicks the PunchIn button it matches with the image

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <div id="videoContainer" style="display: inline-block; border: 4px solid transparent; border-radius: 8px; transition: border-color 0.3s ease;">
            <video id="video" width="320" height="240" autoplay muted playsinline></video>
        </div>
        <canvas id="canvas" style="display:none;"></canvas>
        <p id="statusText" style="font-weight: bold; margin-top: 10px; color: #444;"></p>
    </div>

    <input type="hidden" name="Type" id="EntryType" />

    <div class="mt-5 form-group">
        <div class="col d-flex justify-content-center mb-4">
            @if (ViewBag.InOut == "I")
            {
                <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">Punch In</button>
            }
        </div>
        <div class="col d-flex justify-content-center">
            @if (ViewBag.InOut == "O")
            {
                <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">Punch Out</button>
            }
        </div>
    </div>
</form>

<script>
    window.addEventListener("DOMContentLoaded", async () => {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");
        const EntryTypeInput = document.getElementById("EntryType");
        const successSound = document.getElementById("successSound");
        const errorSound = document.getElementById("errorSound");
        const statusText = document.getElementById("statusText");
        const videoContainer = document.getElementById("videoContainer");

        let blinkCount = 0;
        let blinked = false;
        let blinkValidUntil = 0;
        let blinkCountdownInterval;

        const EAR_THRESHOLD = 0.26;
        const BLINK_GAP = 300;
        const DOUBLE_BLINK_WINDOW = 1500;
        const ALLOW_SUBMIT_DURATION = 10000;

        let lastEARBelowThresholdTime = 0;
        let firstBlinkTime = 0;

        const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

        await Promise.all([
            faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
            faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi')
        ]);
        console.log("Models loaded");
        startVideo();

        function startVideo() {
            navigator.mediaDevices.getUserMedia({
                video: { facingMode: "user", width: { ideal: 640 }, height: { ideal: 480 } }
            })
                .then(stream => {
                    video.srcObject = stream;
                    video.play();
                    video.addEventListener("loadeddata", () => {
                        const checkReady = setInterval(() => {
                            if (video.videoWidth > 0 && video.videoHeight > 0) {
                                clearInterval(checkReady);
                                detectBlink();
                            }
                        }, 100);
                    });
                })
                .catch(console.error);
        }

        function getEAR(eye) {
            const a = distance(eye[1], eye[5]);
            const b = distance(eye[2], eye[4]);
            const c = distance(eye[0], eye[3]);
            return (a + b) / (2.0 * c);
        }

        function distance(p1, p2) {
            return Math.hypot(p1.x - p2.x, p1.y - p2.y);
        }

        async function detectBlink() {
            const now = Date.now();

            if (blinked && now < blinkValidUntil) {
                requestAnimationFrame(detectBlink);
                return;
            }

            const detection = await faceapi.detectSingleFace(video, detectorOptions).withFaceLandmarks();

            if (detection) {
                const leftEye = detection.landmarks.getLeftEye();
                const rightEye = detection.landmarks.getRightEye();
                const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

                if (avgEAR < EAR_THRESHOLD) {
                    if (now - lastEARBelowThresholdTime > BLINK_GAP) {
                        blinkCount++;

                        if (blinkCount === 1) {
                            firstBlinkTime = now;
                        }

                        if (blinkCount === 2 && now - firstBlinkTime <= DOUBLE_BLINK_WINDOW) {
                            blinked = true;
                            blinkValidUntil = now + ALLOW_SUBMIT_DURATION;
                            blinkCount = 0;
                            showGreenBorder();
                            startCountdown();
                        } else if (blinkCount > 2 || now - firstBlinkTime > DOUBLE_BLINK_WINDOW) {
                            blinkCount = 0;
                        }

                        lastEARBelowThresholdTime = now;
                    }
                }

                if (!blinked) {
                    statusText.textContent = "Please double blink to verify liveness";
                    videoContainer.style.borderColor = "red";
                }
            } else {
                statusText.textContent = "No face detected";
                videoContainer.style.borderColor = "gray";
                blinked = false;
                blinkCount = 0;
            }

            requestAnimationFrame(detectBlink);
        }

        function showGreenBorder() {
            videoContainer.style.borderColor = "limegreen";
        }

        function startCountdown() {
            let remaining = ALLOW_SUBMIT_DURATION / 1000;
            statusText.textContent = `Double blink detected! You can proceed. (${remaining}s)`;

            clearInterval(blinkCountdownInterval);
            blinkCountdownInterval = setInterval(() => {
                remaining--;
                if (remaining > 0) {
                    statusText.textContent = `You can proceed. (${remaining}s)`;
                } else {
                    clearInterval(blinkCountdownInterval);
                    blinked = false;
                    videoContainer.style.borderColor = "red";
                    statusText.textContent = "Please double blink to verify liveness";
                }
            }, 1000);
        }

        window.captureImageAndSubmit = function (entryType) {
            if (!blinked || Date.now() > blinkValidUntil) {
                videoContainer.style.borderColor = "red";
                statusText.textContent = "Double blink required before submitting";
                Swal.fire({
                    title: "Liveness Check Failed",
                    text: "Please double blink to verify you're not using a static image.",
                    icon: "warning"
                });
                return;
            }

            blinked = false;
            clearInterval(blinkCountdownInterval);
            statusText.textContent = "";
            videoContainer.style.borderColor = "transparent";

            EntryTypeInput.value = entryType;

            const context = canvas.getContext("2d");
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            const imageData = canvas.toDataURL("image/jpeg");

            Swal.fire({
                title: "Verifying Face...",
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => Swal.showLoading()
            });

            fetch("/AS/Geo/AttendanceData", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Type: entryType, ImageData: imageData })
            })
                .then(res => res.json())
                .then(data => {
                    const now = new Date().toLocaleString();
                    if (data.success) {
                        successSound.play();
                        triggerHapticFeedback("success");
                        Swal.fire({
                            title: "Face Matched!",
                            text: `Attendance Recorded.\nDate & Time: ${now}`,
                            icon: "success",
                            timer: 3000,
                            showConfirmButton: false
                        }).then(() => location.reload());
                    } else {
                        errorSound.play();
                        triggerHapticFeedback("error");
                        Swal.fire({
                            title: "Face Not Recognized.",
                            text: `Click the button again to retry.\nDate & Time: ${now}`,
                            icon: "error"
                        });
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    triggerHapticFeedback("error");
                    Swal.fire("Error!", "An error occurred while processing your request.", "error");
                });
        };

        function triggerHapticFeedback(type) {
            if ("vibrate" in navigator) {
                navigator.vibrate(type === "success" ? 100 : [200, 100, 200]);
            }
        }
    });
</script
