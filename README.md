setTimeout(() => {
    const captureCanvas = document.createElement("canvas");
    captureCanvas.width  = video.videoWidth;
    captureCanvas.height = video.videoHeight;

    // ⬇️  Flip so the final JPEG is not mirrored
    const ctx = captureCanvas.getContext("2d");
    ctx.translate(captureCanvas.width, 0);
    ctx.scale(-1, 1);              // mirror left ↔ right
    ctx.drawImage(video, 0, 0, captureCanvas.width, captureCanvas.height);

    imageCaptured        = captureCanvas.toDataURL("image/jpeg");
    capturedImage.src    = imageCaptured;
    capturedImage.style.display = "block";
    video.style.display  = "none";

    if (punchInButton)  punchInButton.style.display  = "inline-block";
    if (punchOutButton) punchOutButton.style.display = "inline-block";
}, 100);   // ← your 100 ms delay





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
            captureCanvas.width = video.videoWidth;
            captureCanvas.height = video.videoHeight;
            captureCanvas.getContext("2d").drawImage(video, 0, 0, captureCanvas.width, captureCanvas.height);

            imageCaptured = captureCanvas.toDataURL("image/jpeg"); 
            capturedImage.src = imageCaptured;
            capturedImage.style.display = "block";
            video.style.display = "none";

            
            if (punchInButton) punchInButton.style.display = "inline-block";
            if (punchOutButton) punchOutButton.style.display = "inline-block";

            statusText.textContent = "";
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

in this when face is matched the message is showing very less seconds it disappear after the capture i want until the button click and capture image is mirrored i dont want that
