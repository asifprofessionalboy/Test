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

        if (punchInButton) {
    punchInButton.style.display = "none";
}
if (punchOutButton) {
    punchOutButton.style.display = "none";
}

        const EAR_THRESHOLD = 0.27;
        const DOUBLE_BLINK_WINDOW = 2000;
        let blinkCount = 0;
        let eyeClosed = false;
        let blinkStartTime = null;
        
        await Promise.all([
            faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
            faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi'),
            faceapi.nets.faceRecognitionNet.loadFromUri('/AS/faceApi')
        ]);

        const safeUserName = userName.replace(/\s+/g, "%20");
        const descriptors = [
            await loadDescriptor(`/AS/Images/${userId}-Captured.jpg`),
            await loadDescriptor(`/AS/Images/${userId}-${safeUserName}.jpg`)
        ].filter(d => d !== null);

        const faceMatcher = new faceapi.FaceMatcher([
            new faceapi.LabeledFaceDescriptors(userId, descriptors)
        ], 0.35);

        startVideo();

        function startVideo() {
            navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
                .then(stream => {
                    video.srcObject = stream;
                    video.onloadeddata = () => requestAnimationFrame(detectBlink);
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

        function isFaceCentered(box, tolerance = 0.25) {
            const centerX = video.videoWidth / 2;
            const centerY = video.videoHeight / 2;
            const faceCenterX = box.x + box.width / 2;
            const faceCenterY = box.y + box.height / 2;
            return Math.abs(faceCenterX - centerX) / video.videoWidth < tolerance &&
                   Math.abs(faceCenterY - centerY) / video.videoHeight < tolerance;
        }

        function isHeadUpright(landmarks) {
            const nose = landmarks.getNose();
            const chin = landmarks.positions[8];
            const leftEye = landmarks.getLeftEye();
            const rightEye = landmarks.getRightEye();
            const eyeY = (leftEye[1].y + rightEye[1].y) / 2;
            const upperPart = nose[nose.length - 1].y - eyeY;
            const lowerPart = chin.y - nose[nose.length - 1].y;
            const ratio = upperPart / lowerPart;
            return ratio > 0.08 && ratio < 0.92;
        }

        function isFaceTooSmall(box) {
            return box.height / video.videoHeight < 0.35;
        }

        async function detectBlink() {
            const detection = await faceapi.detectSingleFace(video, new faceapi.TinyFaceDetectorOptions({ inputSize: 320 }))
                                           .withFaceLandmarks();

            if (!detection) {
                statusText.textContent = "No face detected";
                videoContainer.style.borderColor = "gray";
                resetBlink();
                return requestAnimationFrame(detectBlink);
            }

            const box = detection.detection.box;
            const landmarks = detection.landmarks;

            if (isFaceTooSmall(box)) {
                statusText.textContent = "Move closer to the camera";
                return requestAnimationFrame(detectBlink);
            }

            if (!isFaceCentered(box)) {
                statusText.textContent = "Align your face in center";
                return requestAnimationFrame(detectBlink);
            }

            if (!isHeadUpright(landmarks)) {
                statusText.textContent = "Keep your head upright";
                return requestAnimationFrame(detectBlink);
            }

            const leftEye = landmarks.getLeftEye();
            const rightEye = landmarks.getRightEye();
            const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

            if (avgEAR < EAR_THRESHOLD) {
                if (!eyeClosed) {
                    eyeClosed = true;
                    blinkCount++;

                    if (blinkCount === 1) blinkStartTime = Date.now();

                    if (blinkCount === 2 && Date.now() - blinkStartTime <= DOUBLE_BLINK_WINDOW) {
                        blinkCount = 0;
                        eyeClosed = false;
                        statusText.textContent = "";
                        showGreenBorder();
                        setTimeout(verifyAndCapture, 500); 
                        return;
                    }

                    if (blinkCount > 2 || Date.now() - blinkStartTime > DOUBLE_BLINK_WINDOW) {
                        resetBlink();
                    }
                }
            } else {
                eyeClosed = false;
            }

            if (blinkCount < 2) {
                statusText.textContent = "Please double blink";
                videoContainer.style.borderColor = "red";
            }

            requestAnimationFrame(detectBlink);
        }

        function resetBlink() {
            blinkCount = 0;
            eyeClosed = false;
        }

        function showGreenBorder() {
            videoContainer.style.borderColor = "green";
            setTimeout(() => videoContainer.style.borderColor = "gray", 5000);
        }
        let imageCaptured = null;

async function verifyAndCapture() {
    const canvasTemp = document.createElement("canvas");
    canvasTemp.width = video.videoWidth;
    canvasTemp.height = video.videoHeight;
    canvasTemp.getContext("2d").drawImage(video, 0, 0, canvasTemp.width, canvasTemp.height);

    const captured = await faceapi.detectSingleFace(canvasTemp, new faceapi.TinyFaceDetectorOptions({ inputSize: 320 }))
                                  .withFaceLandmarks()
                                  .withFaceDescriptor();

    if (!captured) {
        statusText.textContent = "Face not detected in captured image";
        return resetBlink();
    }

    const match = faceMatcher.findBestMatch(captured.descriptor);

    if (match.label === userId && match.distance < 0.35) {
        statusText.textContent = `${userName} matched ✅`;

       setTimeout(() => {
    const captureCanvas = document.createElement("canvas");
    captureCanvas.width  = video.videoWidth;
    captureCanvas.height = video.videoHeight;

   
    const ctx = captureCanvas.getContext("2d");
    ctx.translate(captureCanvas.width, 0);
    ctx.scale(-1, 1);              
    ctx.drawImage(video, 0, 0, captureCanvas.width, captureCanvas.height);

    imageCaptured        = captureCanvas.toDataURL("image/jpeg");
    capturedImage.src    = imageCaptured;
    capturedImage.style.display = "block";
    video.style.display  = "none";

    if (punchInButton)  punchInButton.style.display  = "inline-block";
    if (punchOutButton) punchOutButton.style.display = "inline-block";
}, 100);   

    } else {
        statusText.textContent = "Face not matched ❌";
        videoContainer.style.borderColor = "red";

        setTimeout(() => {
            resetBlink();
            videoContainer.style.borderColor = "gray";
            detectBlink();
        }, 5000);
    }
}

        async function loadDescriptor(imagePath) {
            try {
                const img = await faceapi.fetchImage(imagePath);
                const detection = await faceapi.detectSingleFace(img, new faceapi.TinyFaceDetectorOptions({ inputSize: 320 }))
                                               .withFaceLandmarks()
                                               .withFaceDescriptor();
                return detection ? detection.descriptor : null;
            } catch (err) {
                console.warn("Failed to load image:", imagePath);
                return null;
            }
        }

      
        window.captureImageAndSubmit = async function (entryType) {
            if (!imageCaptured) {
                alert("No captured image available.");
                return;
            }

            EntryTypeInput.value = entryType;
            capturedImage.src = imageCaptured;
            capturedImage.style.display = "block";
            video.style.display = "none";

            Swal.fire({
                title: "Please wait...",
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => Swal.showLoading()
            });

            fetch("/AS/Geo/AttendanceData", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Type: entryType, ImageData: imageCaptured })
            })
            .then(res => res.json())
            .then(data => {
                const now = new Date().toLocaleString();
                if (data.success) {
                    Swal.fire("Thank you!", `Attendance Recorded.\nDate & Time: ${now}`, "success").then(() => location.reload());
                } else {
                    Swal.fire("Face Recognized, But Error!", "Server rejected attendance.", "error");
                }
            })
            .catch(() => {
                Swal.fire("Error!", "Submission failed.", "error");
            });
        };
    });
</script>

please make changes to this code
