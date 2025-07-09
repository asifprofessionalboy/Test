let eyeClosed = false;
let blinkStartTime = 0;

if (avgEAR < EAR_THRESHOLD) {
    if (!eyeClosed) {
        eyeClosed = true;
        blinkCount++;

        if (blinkCount === 1) {
            blinkStartTime = now;
        }

        if (blinkCount === 2 && now - blinkStartTime <= DOUBLE_BLINK_WINDOW) {
            blinked = true;
            blinkValidUntil = now + ALLOW_SUBMIT_DURATION;
            blinkCount = 0;
            eyeClosed = false;

            showGreenBorder();
            setTimeout(captureImage, 500); // delay after blink
            startCountdown();
        }

        if (blinkCount > 2 || now - blinkStartTime > DOUBLE_BLINK_WINDOW) {
            blinkCount = 0;
            eyeClosed = false;
        }

        lastEARBelowThresholdTime = now;
    }
} else {
    eyeClosed = false;
}


<script>
    window.addEventListener("DOMContentLoaded", async () => {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");
        const capturedImage = document.getElementById("capturedImage");
        const EntryTypeInput = document.getElementById("EntryType");
        const statusText = document.getElementById("statusText");
        const videoContainer = document.getElementById("videoContainer");
        const punchInButton = document.getElementById("PunchIn");
        const punchOutButton = document.getElementById("PunchOut");

        if (punchInButton) punchInButton.style.display = "none";
        if (punchOutButton) punchOutButton.style.display = "none";

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

        function getFaceAngleDegrees(leftEye, rightEye) {
            const dx = rightEye[0].x - leftEye[0].x;
            const dy = rightEye[0].y - leftEye[0].y;
            return Math.atan2(dy, dx) * (180 / Math.PI);
        }

        function isFaceCentered(box, tolerance = 0.25) {
            const frameCenterX = video.videoWidth / 2;
            const frameCenterY = video.videoHeight / 2;

            const faceCenterX = box.x + box.width / 2;
            const faceCenterY = box.y + box.height / 2;

            const offsetX = Math.abs(faceCenterX - frameCenterX) / video.videoWidth;
            const offsetY = Math.abs(faceCenterY - frameCenterY) / video.videoHeight;

            return offsetX < tolerance && offsetY < tolerance;
        }

        function isHeadUpright(landmarks, maxTilt = 0.08) {
            const nose = landmarks.getNose();
            const chin = landmarks.positions[8];
            const leftEye = landmarks.getLeftEye();
            const rightEye = landmarks.getRightEye();

            const eyeAvgY = (leftEye[1].y + rightEye[1].y) / 2;
            const noseBaseY = nose[nose.length - 1].y;
            const chinY = chin.y;

            const upperPart = noseBaseY - eyeAvgY;
            const lowerPart = chinY - noseBaseY;

            const ratio = upperPart / lowerPart;
            return ratio > maxTilt && ratio < (1 - maxTilt); // typically between 0.2 and 0.8
        }

        function isFaceTooSmall(box, minHeightRatio = 0.2) {
            return (box.height / video.videoHeight) < minHeightRatio;
        }

        async function detectBlink() {
            const now = Date.now();

            if (blinked && now < blinkValidUntil) {
                requestAnimationFrame(detectBlink);
                return;
            }

            const detection = await faceapi.detectSingleFace(video, detectorOptions).withFaceLandmarks();

            if (detection) {
                const box = detection.detection.box;

                if (isFaceTooSmall(box)) {
                    statusText.textContent = "Move closer to the camera";
                    videoContainer.style.borderColor = "orange";
                    blinked = false;
                    blinkCount = 0;
                    requestAnimationFrame(detectBlink);
                    return;
                }

                if (!isFaceCentered(box)) {
                    statusText.textContent = "Align your face in center of camera";
                    videoContainer.style.borderColor = "orange";
                    blinked = false;
                    blinkCount = 0;
                    requestAnimationFrame(detectBlink);
                    return;
                }

                const landmarks = detection.landmarks;
                const leftEye = landmarks.getLeftEye();
                const rightEye = landmarks.getRightEye();

                const angle = getFaceAngleDegrees(leftEye, rightEye);
                if (Math.abs(angle) > 10) {
                    statusText.textContent = "Keep your head straight (no side tilt)";
                    videoContainer.style.borderColor = "orange";
                    blinked = false;
                    blinkCount = 0;
                    requestAnimationFrame(detectBlink);
                    return;
                }

                if (!isHeadUpright(landmarks)) {
                    statusText.textContent = "Keep your head straight (no up/down tilt)";
                    videoContainer.style.borderColor = "orange";
                    blinked = false;
                    blinkCount = 0;
                    requestAnimationFrame(detectBlink);
                    return;
                }

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
                            setTimeout(captureImage, 500); // allow eyes to reopen
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

            context.translate(canvas.width, 0);
            context.scale(-1, 1); // un-mirror
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            const imageData = canvas.toDataURL("image/jpeg");
            capturedImage.src = imageData;
            capturedImage.style.display = "block";
            video.style.display = "none";

            if (punchInButton) punchInButton.style.display = "inline-block";
            if (punchOutButton) punchOutButton.style.display = "inline-block";
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

                    video.style.display = "block";
                    capturedImage.style.display = "none";
                    if (punchInButton) punchInButton.style.display = "none";
                    if (punchOutButton) punchOutButton.style.display = "none";
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
                    Swal.fire({
                        title: "Face Matched!",
                        text: `Attendance Recorded.\nDate & Time: ${now}`,
                        icon: "success",
                        timer: 3000,
                        showConfirmButton: false
                    }).then(() => location.reload());
                } else {
                    Swal.fire({
                        title: "Face Not Recognized.",
                        text: `Click the button again to retry.\nDate & Time: ${now}`,
                        icon: "error"
                    }).then(() => {
                        video.style.display = "block";
                        capturedImage.style.display = "none";
                        if (punchInButton) punchInButton.style.display = "none";
                        if (punchOutButton) punchOutButton.style.display = "none";
                    });
                }
            })
            .catch(error => {
                console.error("Error:", error);
                Swal.fire("Error!", "An error occurred while processing your request.", "error");
            });
        };
    });
</script>





if (detection) {
    const box = detection.detection.box;

    if (!isFaceCentered(box)) {
        statusText.textContent = "Align your face in center of camera";
        videoContainer.style.borderColor = "orange";
        blinked = false;
        blinkCount = 0;
        requestAnimationFrame(detectBlink);
        return;
    }

    const leftEye = detection.landmarks.getLeftEye();
    const rightEye = detection.landmarks.getRightEye();
    const angle = getFaceAngleDegrees(leftEye, rightEye);
    if (Math.abs(angle) > 10) {
        statusText.textContent = "Please keep your head straight (no side tilt)";
        videoContainer.style.borderColor = "orange";
        blinked = false;
        blinkCount = 0;
        requestAnimationFrame(detectBlink);
        return;
    }

    if (!isHeadUpright(detection.landmarks)) {
        statusText.textContent = "Please keep your head straight (no up/down tilt)";
        videoContainer.style.borderColor = "orange";
        blinked = false;
        blinkCount = 0;
        requestAnimationFrame(detectBlink);
        return;
    }

    // Proceed with EAR calculation and blink logic...
}

function isHeadUpright(landmarks, maxTilt = 0.08) {
    const nose = landmarks.getNose();      // 0 to 8 (nose bridge down to base)
    const chin = landmarks.positions[8];   // Tip of the chin
    const leftEye = landmarks.getLeftEye();
    const rightEye = landmarks.getRightEye();

    const eyeAvgY = (leftEye[1].y + rightEye[1].y) / 2;
    const noseBaseY = nose[nose.length - 1].y;
    const chinY = chin.y;

    const upperPart = noseBaseY - eyeAvgY;   // distance between eyes and nose base
    const lowerPart = chinY - noseBaseY;     // distance between nose base and chin

    const ratio = upperPart / lowerPart;

    // Upright face should have upperPart ~30-50% of lowerPart (ratio ~0.25 to 0.6)
    return ratio > maxTilt && ratio < (1 - maxTilt); // i.e., between ~0.1 and 0.9
}




function getFaceAngleDegrees(leftEye, rightEye) {
    const dx = rightEye[0].x - leftEye[0].x;
    const dy = rightEye[0].y - leftEye[0].y;
    return Math.atan2(dy, dx) * (180 / Math.PI);
}

const leftEye = detection.landmarks.getLeftEye();
const rightEye = detection.landmarks.getRightEye();

const faceAngle = getFaceAngleDegrees(leftEye, rightEye);
if (Math.abs(faceAngle) > 10) {
    statusText.textContent = "Please keep your head upright and straight";
    videoContainer.style.borderColor = "orange";
    blinked = false;
    blinkCount = 0;
    requestAnimationFrame(detectBlink);
    return;
}




function isFaceCentered(box, tolerance = 0.3) {
    const frameCenterX = video.videoWidth / 2;
    const frameCenterY = video.videoHeight / 2;

    const faceCenterX = box.x + box.width / 2;
    const faceCenterY = box.y + box.height / 2;

    const offsetX = Math.abs(faceCenterX - frameCenterX) / video.videoWidth;
    const offsetY = Math.abs(faceCenterY - frameCenterY) / video.videoHeight;

    return offsetX < tolerance && offsetY < tolerance;
}






mif (detection) {
    const box = detection.detection.box;

    if (!isFaceCentered(box)) {
        statusText.textContent = "Align your face in center of camera";
        videoContainer.style.borderColor = "orange";
        blinked = false;
        blinkCount = 0;
        requestAnimationFrame(detectBlink);
        return;
    }

    const leftEye = detection.landmarks.getLeftEye();
    const rightEye = detection.landmarks.getRightEye();
    const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

    // ... rest of your blink logic
}



this is my js 
<script>
    window.addEventListener("DOMContentLoaded", async () => {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");
        const capturedImage = document.getElementById("capturedImage");
        const EntryTypeInput = document.getElementById("EntryType");
        const statusText = document.getElementById("statusText");
        const videoContainer = document.getElementById("videoContainer");
        const punchInButton = document.getElementById("PunchIn");
        const punchOutButton = document.getElementById("PunchOut");

        
        if (punchInButton) punchInButton.style.display = "none";
        if (punchOutButton) punchOutButton.style.display = "none";

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
                            setTimeout(captureImage, 500); 
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

           
            context.translate(canvas.width, 0);
            context.scale(-1, 1);

            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            const imageData = canvas.toDataURL("image/jpeg");
            capturedImage.src = imageData;
            capturedImage.style.display = "block";
            video.style.display = "none";

            
            if (punchInButton) punchInButton.style.display = "inline-block";
            if (punchOutButton) punchOutButton.style.display = "inline-block";
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

                   
                    video.style.display = "block";
                    capturedImage.style.display = "none";

                    if (punchInButton) punchInButton.style.display = "none";
                    if (punchOutButton) punchOutButton.style.display = "none";
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
                        
                        triggerHapticFeedback("success");
                        Swal.fire({
                            title: "Face Matched!",
                            text: `Attendance Recorded.\nDate & Time: ${now}`,
                            icon: "success",
                            timer: 3000,
                            showConfirmButton: false
                        }).then(() => location.reload());
                    } else {
                      
                        triggerHapticFeedback("error");
                        Swal.fire({
                            title: "Face Not Recognized.",
                            text: `Click the button again to retry.\nDate & Time: ${now}`,
                            icon: "error"
                        }).then(() => {
                            blinked = false;
                            clearInterval(blinkCountdownInterval);
                            statusText.textContent = "";
                            videoContainer.style.borderColor = "transparent";
                            video.style.display = "block";
                            capturedImage.style.display = "none";
                            if (punchInButton) punchInButton.style.display = "none";
                            if (punchOutButton) punchOutButton.style.display = "none";
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


i am facing an issue that if the user place the mobile from bottom or upper it detected the blink ,i want if face is center of the camera then it detect the blink
