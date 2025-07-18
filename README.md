const punchInButton = document.getElementById("PunchIn");
const punchOutButton = document.getElementById("PunchOut");

// Always check before using
if (punchInButton) {
    punchInButton.style.display = "none";
}
if (punchOutButton) {
    punchOutButton.style.display = "none";
}



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
            // ✅ Capture image after 2 seconds
            const captureCanvas = document.createElement("canvas");
            captureCanvas.width = video.videoWidth;
            captureCanvas.height = video.videoHeight;
            captureCanvas.getContext("2d").drawImage(video, 0, 0, captureCanvas.width, captureCanvas.height);

            imageCaptured = captureCanvas.toDataURL("image/jpeg"); // Save base64 image
            capturedImage.src = imageCaptured;
            capturedImage.style.display = "block";
            video.style.display = "none";

            // Show Punch buttons
            if (punchInButton) punchInButton.style.display = "inline-block";
            if (punchOutButton) punchOutButton.style.display = "inline-block";

            statusText.textContent = "";
        }, 2000);

    } else {
        statusText.textContent = "Face not matched ❌";
        videoContainer.style.borderColor = "red";

        setTimeout(() => {
            resetBlink();
            videoContainer.style.borderColor = "gray";
            detectBlink();
        }, 10000);
    }
}






let imageCaptured = null; // Global captured image variable

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

        // ✅ Capture image after match
        imageCaptured = canvasTemp.toDataURL("image/jpeg");

        if (punchInButton) punchInButton.style.display = "inline-block";
        if (punchOutButton) punchOutButton.style.display = "inline-block";

        setTimeout(() => statusText.textContent = "", 2000);
    } else {
        statusText.textContent = "Face not matched ❌";
        videoContainer.style.borderColor = "red";
        setTimeout(() => {
            resetBlink();
            videoContainer.style.borderColor = "gray";
            detectBlink();
        }, 10000);
    }
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

        // Hide punch buttons initially
        punchInButton.style.display = "none";
        punchOutButton.style.display = "none";

        const EAR_THRESHOLD = 0.27;
        const DOUBLE_BLINK_WINDOW = 2000;
        let blinkCount = 0;
        let eyeClosed = false;
        let blinkStartTime = null;
        let imageCaptured = null;

        // Load models
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
                        setTimeout(verifyAndCapture, 2000); // wait 2 seconds
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

                // Show buttons
                punchInButton.style.display = "inline-block";
                punchOutButton.style.display = "inline-block";

                // Store image for submission
                imageCaptured = canvasTemp.toDataURL("image/jpeg");

                setTimeout(() => statusText.textContent = "", 2000);
            } else {
                statusText.textContent = "Face not matched ❌";
                videoContainer.style.borderColor = "red";
                setTimeout(() => {
                    resetBlink();
                    videoContainer.style.borderColor = "gray";
                    detectBlink();
                }, 10000);
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

        // Called when user clicks Punch In or Punch Out
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






this is my full js , i want to clear unnecessary code and if the blink detected and face matched then capture the image immediately after 2 sec and call the function captureImageandSubmit after button click
 <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">Punch In</button>
<button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">Punch Out</button>
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

        let blinked = false;
let blinkCount = 0;
let eyeClosed = false;
let blinkStartTime = null;
let blinkValidUntil = null;


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
                    statusText.textContent = ""; 
                    captureImage();
                    startCountdown(); 
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

        function resetBlink() {
    blinkCount = 0;
    eyeClosed = false;
    blinked = false;
    blinkStartTime = null;
    blinkValidUntil = null;
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
        resetAfterDelay();
        return;
    }

    const match = faceMatcher.findBestMatch(captured.descriptor);

    if (match.label === userId && match.distance < 0.35) {
        statusText.textContent = `${userName} matched ✅`;
        videoContainer.style.borderColor = "green";

        //captureImageAndShow("Punch In");
      
        if (punchInButton) punchInButton.style.display = "inline-block";
        if (punchOutButton) punchOutButton.style.display = "inline-block";

        setTimeout(() => {
            statusText.textContent = "";
            videoContainer.style.borderColor = "gray";
        }, 2000);
    } else {
        statusText.textContent = "Face not matched ❌";
        videoContainer.style.borderColor = "red";

        
        setTimeout(() => {
            resetBlink();
            statusText.textContent = "Please double blink";
            videoContainer.style.borderColor = "red";
            detectBlink(); 
        }, 10000);
    }
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

 function captureImageAndShow(entryType) {
        const canvas = document.createElement("canvas");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        const ctx = canvas.getContext("2d");
         ctx.translate = (canvas.width,0);
    ctx.scale = (-1,1);
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageDataUrl = canvas.toDataURL("image/jpeg");

        video.style.display = "none";
        capturedImage.src = imageDataUrl;
        capturedImage.style.display = "block";

        window.captureImageAndSubmit(entryType, imageDataUrl);
    }

    function startRetryCountdown() {
   
    setTimeout(() => {
        capturedImage.style.display = "none";
        video.style.display = "block";
        detectionActive = true;
    }, 10000);
}



        window.captureImageAndSubmit = async function (entryType) {
          
            const video = document.getElementById("video");
    const canvas = document.createElement("canvas");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    const ctx = canvas.getContext("2d");
    ctx.translate = (canvas.width,0);
    ctx.scale = (-1,1);
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

    const imageDataUrl = canvas.toDataURL("image/jpeg");

    
    video.style.display = "none";
    const img = document.getElementById("capturedImage");
    img.src = imageDataUrl;
    img.style.display = "block";


            EntryTypeInput.value = entryType;
            const imageData = capturedImage.src;

            Swal.fire({
                title: "Please wait...",
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => Swal.showLoading()
            });

            
            fetch("/AS/Geo/AttendanceData", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Type: entryType, ImageData: imageData})
            })
                .then(res => res.json())
                .then(data => {
                    const now = new Date().toLocaleString();
                    if (data.success) {
                        Swal.fire({
                            title: "Thank you!",
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
    function captureFrameAndSend(isMatched) {
    const video = document.getElementById("video");
    const canvas = document.createElement("canvas");

    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext("2d");
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

    const imageData = canvas.toDataURL("image/jpeg");

    
    document.getElementById("video").style.display = "none";
    const img = document.getElementById("capturedImage");
    img.src = imageData;
    img.style.display = "block";

    
    sendCapturedImage(imageData, isMatched);
}

</script>
