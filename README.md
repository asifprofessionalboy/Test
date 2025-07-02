<script>
window.addEventListener("DOMContentLoaded", async () => {
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");
    const statusText = document.getElementById("statusText");
    const videoContainer = document.getElementById("videoContainer");

    let blinkCount = 0, blinked = false, blinkValidUntil = 0, blinkCountdownInterval;
    const EAR_THRESHOLD = 0.27, BLINK_GAP = 300, DOUBLE_BLINK_WINDOW = 1500, ALLOW_SUBMIT_DURATION = 5000;
    let lastEARBelowThresholdTime = 0, firstBlinkTime = 0;

    const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

    await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
        faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi'),
        faceapi.nets.faceRecognitionNet.loadFromUri('/AS/faceApi'),
        faceapi.nets.ssdMobilenetv1.loadFromUri('/AS/faceApi') // for face descriptor extraction
    ]);
    console.log("Models loaded");

    const referenceDescriptors = [];
    await loadReferenceImage(); // Load stored face

    startVideo();

    function startVideo() {
        navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
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

    async function loadReferenceImage() {
        const img = await faceapi.fetchImage('/images/151514-Captured.jpg');
        const detection = await faceapi.detectSingleFace(img).withFaceLandmarks().withFaceDescriptor();
        if (detection) {
            referenceDescriptors.push(new faceapi.LabeledFaceDescriptors('151514', [detection.descriptor]));
        } else {
            console.error("Reference face not detected.");
            Swal.fire("Error", "Reference image not valid or face not found.", "error");
        }
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

            if (avgEAR < EAR_THRESHOLD && now - lastEARBelowThresholdTime > BLINK_GAP) {
                blinkCount++;
                if (blinkCount === 1) firstBlinkTime = now;

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

    window.captureImageAndSubmit = async function (entryType) {
        if (!blinked || Date.now() > blinkValidUntil) {
            videoContainer.style.borderColor = "red";
            statusText.textContent = "Double blink required before submitting";
            Swal.fire("Liveness Check Failed", "Please double blink to verify you're not using a static image.", "warning");
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

        // Face recognition from canvas
        const liveDetection = await faceapi.detectSingleFace(canvas).withFaceLandmarks().withFaceDescriptor();

        if (!liveDetection) {
            errorSound.play();
            Swal.fire("No Face Detected", "Please make sure your face is visible and well-lit.", "error");
            return;
        }

        const faceMatcher = new faceapi.FaceMatcher(referenceDescriptors, 0.6); // threshold
        const match = faceMatcher.findBestMatch(liveDetection.descriptor);

        if (match.label !== '151514') {
            errorSound.play();
            Swal.fire("Face Not Matched", "Face does not match reference image.", "error");
            return;
        }

        // Proceed to backend after successful match
        const imageData = canvas.toDataURL("image/jpeg");
        Swal.fire({ title: "Verifying Face...", allowOutsideClick: false, showConfirmButton: false, didOpen: () => Swal.showLoading() });

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
                Swal.fire({ title: "Face Matched!", text: `Attendance Recorded.\nDate & Time: ${now}`, icon: "success", timer: 3000, showConfirmButton: false })
                    .then(() => location.reload());
            } else {
                errorSound.play();
                triggerHapticFeedback("error");
                Swal.fire({ title: "Face Not Recognized", text: "Click the button again to retry.", icon: "error" });
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

ssd_mobilenetv1_model-weights_manifest.json


i have this code 
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

    const EAR_THRESHOLD = 0.27;
    const BLINK_GAP = 300; 
    const DOUBLE_BLINK_WINDOW = 1500; 
    const ALLOW_SUBMIT_DURATION = 5000; 

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
</script>

this is my one image file name 151514-Captured.jpg , i want to test this one image now
