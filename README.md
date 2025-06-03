function startVideo() {
    navigator.mediaDevices.getUserMedia({
        video: {
            facingMode: "user",
            width: { ideal: 640 },
            height: { ideal: 480 }
        }
    })
    .then(stream => {
        video.srcObject = stream;
        video.play();

        video.addEventListener("loadeddata", () => {
            console.log("Camera video is ready. Starting face detection...");

            const checkReady = setInterval(() => {
                if (video.videoWidth > 0 && video.videoHeight > 0) {
                    clearInterval(checkReady);
                    detectBlink();
                }
            }, 100);
        });
    })
    .catch(err => {
        console.error("Camera error:", err);
    });
}




<div class="form-group text-center">
    <div id="videoContainer" style="display: inline-block; border: 4px solid transparent; border-radius: 8px; transition: border-color 0.3s ease;">
        <video id="video" width="320" height="240" autoplay muted playsinline></video>
    </div>
    <canvas id="canvas" style="display:none;"></canvas>
    <p id="statusText" style="font-weight: bold; margin-top: 10px; color: #444;"></p>
</div>

<script>
    window.addEventListener("DOMContentLoaded", async () => {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");
        const EntryTypeInput = document.getElementById("EntryType");
        const successSound = document.getElementById("successSound");
        const errorSound = document.getElementById("errorSound");
        const statusText = document.getElementById("statusText");
        const videoContainer = document.getElementById("videoContainer");

        let blinked = false;
        let lastBlinkTime = 0;
        const BLINK_INTERVAL = 3000;
        const EAR_THRESHOLD = 0.30;
        const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

        try {
            await Promise.all([
                faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
                faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi')
            ]);
            console.log("FaceAPI models loaded");
            startVideo();
        } catch (e) {
            console.error("Failed to load face-api models:", e);
        }

        function startVideo() {
            navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
                .then(stream => {
                    video.srcObject = stream;
                    video.play();
                    video.addEventListener("play", detectBlink);
                })
                .catch(err => {
                    console.error("Camera error:", err);
                });
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
            const detection = await faceapi
                .detectSingleFace(video, detectorOptions)
                .withFaceLandmarks();

            if (detection) {
                const box = detection.detection.box;
                const faceWidth = box.width;

                if (faceWidth < 90) {
                    statusText.textContent = "Move closer to the camera";
                    videoContainer.style.borderColor = "orange";
                    blinked = false;
                } else {
                    const leftEye = detection.landmarks.getLeftEye();
                    const rightEye = detection.landmarks.getRightEye();
                    const leftEAR = getEAR(leftEye);
                    const rightEAR = getEAR(rightEye);
                    const avgEAR = (leftEAR + rightEAR) / 2.0;

                    if (avgEAR < EAR_THRESHOLD && Date.now() - lastBlinkTime > BLINK_INTERVAL) {
                        blinked = true;
                        lastBlinkTime = Date.now();
                        console.log("Blink detected");

                        videoContainer.style.borderColor = "limegreen";
                        statusText.textContent = "Blink detected! You can now proceed.";
                        setTimeout(() => videoContainer.style.borderColor = "transparent", 1500);
                    } else if (!blinked) {
                        statusText.textContent = "Please blink to verify liveness";
                        videoContainer.style.borderColor = "red";
                    }
                }
            } else {
                statusText.textContent = "No face detected";
                videoContainer.style.borderColor = "gray";
                blinked = false;
            }

            requestAnimationFrame(detectBlink);
        }

        window.captureImageAndSubmit = function (entryType) {
            if (!blinked) {
                videoContainer.style.borderColor = "red";
                statusText.textContent = "Blink required before submitting";
                Swal.fire({
                    title: "Liveness Check Failed",
                    text: "Please blink to verify you're not using a static image.",
                    icon: "warning"
                });
                return;
            }

            blinked = false; // Reset after submission
            statusText.textContent = "";

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
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            fetch("/AS/Geo/AttendanceData", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    Type: entryType,
                    ImageData: imageData
                })
            })
                .then(response => response.json())
                .then(data => {
                    const now = new Date();
                    const formattedDateTime = now.toLocaleString();

                    if (data.success) {
                        successSound.play();
                        triggerHapticFeedback("success");
                        Swal.fire({
                            title: "Face Matched!",
                            text: "Attendance Recorded.\nDate & Time: " + formattedDateTime,
                            icon: "success",
                            timer: 3000,
                            showConfirmButton: false
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        errorSound.play();
                        triggerHapticFeedback("error");
                        Swal.fire({
                            title: "Face Not Recognized.",
                            text: "Click the button again to retry.\nDate & Time: " + formattedDateTime,
                            icon: "error",
                            confirmButtonText: "Retry"
                        });
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    triggerHapticFeedback("error");
                    Swal.fire({
                        title: "Error!",
                        text: "An error occurred while processing your request.",
                        icon: "error"
                    });
                });
        };

        function triggerHapticFeedback(type) {
            if ("vibrate" in navigator) {
                if (type === "success") {
                    navigator.vibrate(100);
                } else if (type === "error") {
                    navigator.vibrate([200, 100, 200]);
                }
            }
        }
    });
</script>




not working the liveness check when i show photos of a static image it shows green and also submit the capture image
<script>
    window.addEventListener("DOMContentLoaded", async () => {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");
        const EntryTypeInput = document.getElementById("EntryType");
        const successSound = document.getElementById("successSound");
        const errorSound = document.getElementById("errorSound");

        let blinked = false;
        let lastBlinkTime = 0;
        const BLINK_INTERVAL = 3000;
        const EAR_THRESHOLD = 0.30;

        try {
           
            await Promise.all([
                faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
                faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi')
            ]);
            console.log("FaceAPI models loaded");

            startVideo();
        } catch (e) {
            console.error("Failed to load face-api models:", e);
        }

        function startVideo() {
            navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
                .then(stream => {
                    video.srcObject = stream;
                    video.play();
                    video.addEventListener("play", () => {
                        detectBlink();
                    });
                })
                .catch(err => {
                    console.error("Camera error:", err);
                });
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
            const detection = await faceapi
                .detectSingleFace(video, new faceapi.TinyFaceDetectorOptions())
                .withFaceLandmarks();

            if (detection) {
                const leftEye = detection.landmarks.getLeftEye();
                const rightEye = detection.landmarks.getRightEye();

                const leftEAR = getEAR(leftEye);
                const rightEAR = getEAR(rightEye);
                const avgEAR = (leftEAR + rightEAR) / 2.0;

                const now = Date.now();
                if (avgEAR < EAR_THRESHOLD && now - lastBlinkTime > BLINK_INTERVAL) {
                    blinked = true;
                    lastBlinkTime = now;
                    console.log("Blink detected!");

                    const videoContainer = document.getElementById("videoContainer");
                    videoContainer.style.borderColor = "limegreen";

                    setTimeout(() => {
                        videoContainer.style.borderColor = "transparent";
                    }, 1500); // green border lasts 1.5 seconds
                }
            }

            requestAnimationFrame(detectBlink);
        }

        window.captureImageAndSubmit = function (entryType) {
            if (!blinked) {
                Swal.fire({
                    title: "Liveness Check Failed",
                    text: "Please blink to verify you're not using a static image.",
                    icon: "warning"
                });
                return;
            }

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
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            fetch("/AS/Geo/AttendanceData", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    Type: entryType,
                    ImageData: imageData
                })
            })
                .then(response => response.json())
                .then(data => {
                    const now = new Date();
                    const formattedDateTime = now.toLocaleString();

                    if (data.success) {
                        successSound.play();
                        triggerHapticFeedback("success");

                        Swal.fire({
                            title: "Face Matched!",
                            text: "Attendance Recorded.\nDate & Time: " + formattedDateTime,
                            icon: "success",
                            timer: 3000,
                            showConfirmButton: false
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        errorSound.play();
                        triggerHapticFeedback("error");

                        Swal.fire({
                            title: "Face Not Recognized.",
                            text: "Click the button again to retry.\nDate & Time: " + formattedDateTime,
                            icon: "error",
                            confirmButtonText: "Retry"
                        });
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    triggerHapticFeedback("error");

                    Swal.fire({
                        title: "Error!",
                        text: "An error occurred while processing your request.",
                        icon: "error"
                    });
                });
        };

        function triggerHapticFeedback(type) {
            if ("vibrate" in navigator) {
                if (type === "success") {
                    navigator.vibrate(100);
                } else if (type === "error") {
                    navigator.vibrate([200, 100, 200]);
                }
            }
        }
    });
</script>
