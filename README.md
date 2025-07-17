let blinked = false;
let blinkCount = 0;
let eyeClosed = false;
let blinkStartTime = null;
let blinkValidUntil = null;

function resetBlink() {
    blinkCount = 0;
    eyeClosed = false;
    blinked = false;
    blinkStartTime = null;
    blinkValidUntil = null;
}

function startCountdown() {
    blinkValidUntil = Date.now() + 10000; // 10 seconds window
    setTimeout(() => {
        statusText.textContent = "Please double blink";
        videoContainer.style.borderColor = "red";
        resetBlink();
        detectBlink(); // restart detection after 10s
    }, 10000);
}

function showGreenBorder() {
    videoContainer.style.borderColor = "green";
    setTimeout(() => {
        videoContainer.style.borderColor = "gray";
    }, 5000);
}

async function captureImage() {
    const canvas = faceapi.createCanvasFromMedia(video);
    const context = canvas.getContext('2d');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    const captured = await faceapi.detectSingleFace(canvas, detectorOptions).withFaceLandmarks().withFaceDescriptor();

    if (!captured) {
        alert("Face not detected in captured image");
        return;
    }

    const stored = await loadStoredFaceDescriptor(`/AS/Images/${userId}-${userName}.jpg`);

    if (!stored) {
        alert("Stored image not found or face not detected in it");
        return;
    }

    const faceMatcher = new faceapi.FaceMatcher([new faceapi.LabeledFaceDescriptors(userId, [stored])], 0.35);
    const match = faceMatcher.findBestMatch(captured.descriptor);

    if (match.label === userId) {
        statusText.textContent = `${userName} matched ✅`;
        videoContainer.style.borderColor = "green";
        setTimeout(() => {
            statusText.textContent = "";
            videoContainer.style.borderColor = "gray";
        }, 2000); // brief display
    } else {
        statusText.textContent = "Face not matched ❌";
        videoContainer.style.borderColor = "red";
        setTimeout(() => {
            statusText.textContent = "";
            videoContainer.style.borderColor = "gray";
        }, 2000);
    }
}

async function detectBlink() {
    const detection = await faceapi.detectSingleFace(video, detectorOptions).withFaceLandmarks();

    if (detection) {
        const box = detection.detection.box;

        if (isFaceTooSmall(box)) {
            statusText.textContent = "Move closer to the camera";
            videoContainer.style.borderColor = "orange";
            resetBlink();
            requestAnimationFrame(detectBlink);
            return;
        }

        if (!isFaceCentered(box)) {
            statusText.textContent = "Align your face in center";
            videoContainer.style.borderColor = "orange";
            resetBlink();
            requestAnimationFrame(detectBlink);
            return;
        }

        const landmarks = detection.landmarks;
        const leftEye = landmarks.getLeftEye();
        const rightEye = landmarks.getRightEye();
        const angle = getFaceAngleDegrees(leftEye, rightEye);

        if (Math.abs(angle) > 10) {
            statusText.textContent = "Keep your head straight";
            videoContainer.style.borderColor = "orange";
            resetBlink();
            requestAnimationFrame(detectBlink);
            return;
        }

        if (!isHeadUpright(landmarks)) {
            statusText.textContent = "Keep your head upright";
            videoContainer.style.borderColor = "orange";
            resetBlink();
            requestAnimationFrame(detectBlink);
            return;
        }

        const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

        if (avgEAR < EAR_THRESHOLD) {
            if (!eyeClosed) {
                eyeClosed = true;
                blinkCount++;

                if (blinkCount === 1) blinkStartTime = Date.now();

                if (blinkCount === 2 && Date.now() - blinkStartTime <= DOUBLE_BLINK_WINDOW) {
                    blinked = true;
                    blinkCount = 0;
                    eyeClosed = false;

                    showGreenBorder();
                    statusText.textContent = ""; // hide status
                    captureImage();
                    startCountdown(); // 10 sec lock
                    return;
                }

                if (blinkCount > 2 || Date.now() - blinkStartTime > DOUBLE_BLINK_WINDOW) {
                    resetBlink();
                }
            }
        } else {
            eyeClosed = false;
        }

        if (!blinked) {
            statusText.textContent = "Please double blink";
            videoContainer.style.borderColor = "red";
        }
    } else {
        statusText.textContent = "No face detected";
        videoContainer.style.borderColor = "gray";
        resetBlink();
    }

    requestAnimationFrame(detectBlink);
}





this is my full js 
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

        const EAR_THRESHOLD = 0.27;
        const DOUBLE_BLINK_WINDOW = 2000;
        const ALLOW_SUBMIT_DURATION = 10000;

        let eyeClosed = false;
        let blinkCount = 0;
        let blinked = false;
        let blinkStartTime = 0;
        let blinkValidUntil = 0;
        let blinkCountdownInterval;

        const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

        await Promise.all([
            faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
            faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi'),
            faceapi.nets.faceRecognitionNet.loadFromUri('/AS/faceApi')
        ]);

        console.log("Models loaded");
       const safeUserName = userName.replace(/\s+/g, "%20"); 

       console.log("Safe user name:"+safeUserName);

       const descriptors = [
    await loadStoredFaceDescriptor(`/AS/Images/${userId}-Captured.jpg`),
    await loadStoredFaceDescriptor(`/AS/Images/${userId}-${safeUserName}.jpg`)
].filter(d => d !== null);

const faceMatcher = new faceapi.FaceMatcher([
    new faceapi.LabeledFaceDescriptors(userId, descriptors)
], 0.35); 


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
            const centerX = video.videoWidth / 2;
            const centerY = video.videoHeight / 2;
            const faceCenterX = box.x + box.width / 2;
            const faceCenterY = box.y + box.height / 2;
            const offsetX = Math.abs(faceCenterX - centerX) / video.videoWidth;
            const offsetY = Math.abs(faceCenterY - centerY) / video.videoHeight;
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
            return ratio > maxTilt && ratio < (1 - maxTilt);
        }

        function isFaceTooSmall(box, minHeightRatio = 0.35) {
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
                    resetBlink();
                    requestAnimationFrame(detectBlink);
                    return;
                }

                if (!isFaceCentered(box)) {
                    statusText.textContent = "Align your face in center";
                    videoContainer.style.borderColor = "orange";
                    resetBlink();
                    requestAnimationFrame(detectBlink);
                    return;
                }

                const landmarks = detection.landmarks;
                const leftEye = landmarks.getLeftEye();
                const rightEye = landmarks.getRightEye();
                const angle = getFaceAngleDegrees(leftEye, rightEye);

                if (Math.abs(angle) > 10) {
                    statusText.textContent = "Keep your head straight (no tilt)";
                    videoContainer.style.borderColor = "orange";
                    resetBlink();
                    requestAnimationFrame(detectBlink);
                    return;
                }

                if (!isHeadUpright(landmarks)) {
                    statusText.textContent = "Keep your head upright";
                    videoContainer.style.borderColor = "orange";
                    resetBlink();
                    requestAnimationFrame(detectBlink);
                    return;
                }

                const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

                if (avgEAR < EAR_THRESHOLD) {
                    if (!eyeClosed) {
                        eyeClosed = true;
                        blinkCount++;

                        if (blinkCount === 1) blinkStartTime = now;

                        if (blinkCount === 2 && now - blinkStartTime <= DOUBLE_BLINK_WINDOW) {
                            blinked = true;
                            blinkValidUntil = now + ALLOW_SUBMIT_DURATION;
                            blinkCount = 0;
                            eyeClosed = false;

                            showGreenBorder();
                            setTimeout(captureImage, 500);
                            startCountdown();
                        }

                        if (blinkCount > 2 || now - blinkStartTime > DOUBLE_BLINK_WINDOW) {
                            resetBlink();
                        }
                    }
                } else {
                    eyeClosed = false;
                }

                if (!blinked) {
                    statusText.textContent = "Please double blink";
                    videoContainer.style.borderColor = "red";
                }
            } else {
                statusText.textContent = "No face detected";
                videoContainer.style.borderColor = "gray";
                resetBlink();
            }

            requestAnimationFrame(detectBlink);
        }

        function resetBlink() {
            blinkCount = 0;
            eyeClosed = false;
            blinked = false;
        }

        function showGreenBorder() {
            videoContainer.style.borderColor = "limegreen";
        }

       async function captureImage() {
    const context = canvas.getContext("2d");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    context.translate(canvas.width, 0);
    context.scale(-1, 1);
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    capturedImage.src = canvas.toDataURL("image/jpeg");
    capturedImage.style.display = "block";
    video.style.display = "none";

    const result = await recognizeFace();

    if (result.matched) {
        statusText.textContent = `✅ ${result.label} - Face Matched (Distance: ${result.distance.toFixed(2)})`;
        videoContainer.style.borderColor = "green";

        if (punchInButton) punchInButton.style.display = "inline-block";
        if (punchOutButton) punchOutButton.style.display = "inline-block";

        
        setTimeout(() => {
            statusText.textContent = "";
        }, 2500);
    } else {
        statusText.textContent = `❌ Unknown - Face Not Recognized (Distance: ${result.distance.toFixed(2)})`;
        videoContainer.style.borderColor = "red";

        if (punchInButton) punchInButton.style.display = "none";
        if (punchOutButton) punchOutButton.style.display = "none";

     
        setTimeout(() => {
            statusText.textContent = "";
            video.style.display = "block";
            capturedImage.style.display = "none";
        }, 2500);
    }
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
                    resetBlink();
                    videoContainer.style.borderColor = "red";
                    statusText.textContent = "Please double blink again";
                    video.style.display = "block";
                    capturedImage.style.display = "none";
                    if (punchInButton) punchInButton.style.display = "none";
                    if (punchOutButton) punchOutButton.style.display = "none";
                }
            }, 1000);
        }

        async function loadStoredFaceDescriptor(imagePath) {
    try {
        const img = await faceapi.fetchImage(imagePath);
        const detection = await faceapi
            .detectSingleFace(img, detectorOptions)
            .withFaceLandmarks()
            .withFaceDescriptor();

        return detection ? detection.descriptor : null;
    } catch (err) {
        console.warn("Failed to load image: " + imagePath, err);
        return null;
    }
}



        async function recognizeFace() {
    const detection = await faceapi
        .detectSingleFace(canvas, detectorOptions)
        .withFaceLandmarks()
        .withFaceDescriptor();

    if (!detection) {
        return { matched: false, message: "Face not detected in captured image." };
    }

    const bestMatch = faceMatcher.findBestMatch(detection.descriptor);

    if (bestMatch.distance > 0.4 || bestMatch.label === "unknown") {
        return {
            matched: false,
            label: "unknown",
            distance: bestMatch.distance,
            message: "Not matched or distance too high"
        };
    }

    return {
        matched: true,
        label: bestMatch.label,
        distance: bestMatch.distance,
        message: `Matched with ${bestMatch.label}`
    };
}

        window.captureImageAndSubmit = async function (entryType) {
            if (!blinked || Date.now() > blinkValidUntil) {
                videoContainer.style.borderColor = "red";
                statusText.textContent = "Double blink required before submitting";
                Swal.fire({
                    title: "Liveness Check Failed",
                    text: "Please double blink first.",
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

            const result = await recognizeFace();

            if (!result.matched) {
                Swal.fire({
                    title: "Face Not Recognized",
                    text: result.message,
                    icon: "error"
                });
                return;
            }

            // Face matched, send to backend
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
                            title: "Face Recognized, But Error!",
                            text: `Server didn't accept attendance.\nDate & Time: ${now}`,
                            icon: "error"
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



i this when image is not found it shows face matched with Distance 0.00 , i just want that if image is not found or blank it shows a alert that Based image and current image is not available and sometimes face matcher is not calling , only blink works , please provide a good and provide proper js to handle this otherwise it works fine and good 
