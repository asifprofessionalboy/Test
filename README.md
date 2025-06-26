<script defer src="/faceApi/face-api.min.js"></script>

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
        let blinkFrameCount = 0;
        let lastBlinkTime = 0;

        const EAR_THRESHOLD = 0.23;
        const BLINK_MIN_FRAMES = 2;
        const BLINK_MAX_FRAMES = 5;
        const BLINK_INTERVAL = 3000;
        const MIN_FACE_WIDTH = 100;

        const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

        try {
            await Promise.all([
                faceapi.nets.tinyFaceDetector.loadFromUri('/faceApi'),
                faceapi.nets.faceLandmark68Net.loadFromUri('/faceApi')
            ]);
            console.log("FaceAPI models loaded");
            startVideo();
        } catch (e) {
            console.error("Failed to load face-api models:", e);
        }

        function startVideo() {
            navigator.mediaDevices.getUserMedia({
                video: { facingMode: "user", width: { ideal: 640 }, height: { ideal: 480 } }
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

        function distance(p1, p2) {
            return Math.hypot(p1.x - p2.x, p1.y - p2.y);
        }

        function getEAR(eye) {
            const a = distance(eye[1], eye[5]);
            const b = distance(eye[2], eye[4]);
            const c = distance(eye[0], eye[3]);
            return (a + b) / (2.0 * c);
        }

        async function detectBlink() {
            const detection = await faceapi
                .detectSingleFace(video, detectorOptions)
                .withFaceLandmarks();

            if (detection) {
                const box = detection.detection.box;
                const faceWidth = box.width;

                if (faceWidth < MIN_FACE_WIDTH) {
                    statusText.textContent = "Move closer to the camera";
                    videoContainer.style.borderColor = "orange";
                    blinked = false;
                    blinkFrameCount = 0;
                } else {
                    const leftEye = detection.landmarks.getLeftEye();
                    const rightEye = detection.landmarks.getRightEye();
                    const leftEAR = getEAR(leftEye);
                    const rightEAR = getEAR(rightEye);
                    const avgEAR = (leftEAR + rightEAR) / 2.0;

                    if (avgEAR < EAR_THRESHOLD) {
                        blinkFrameCount++;
                    } else {
                        if (
                            blinkFrameCount >= BLINK_MIN_FRAMES &&
                            blinkFrameCount <= BLINK_MAX_FRAMES &&
                            Date.now() - lastBlinkTime > BLINK_INTERVAL
                        ) {
                            blinked = true;
                            lastBlinkTime = Date.now();
                            console.log("Blink detected");
                            videoContainer.style.borderColor = "limegreen";
                            statusText.textContent = "Blink detected! You can now proceed.";
                            setTimeout(() => videoContainer.style.borderColor = "transparent", 1500);
                        } else {
                            blinked = false;
                        }
                        blinkFrameCount = 0;
                    }

                    if (!blinked) {
                        statusText.textContent = "Please blink to verify liveness";
                        videoContainer.style.borderColor = "red";
                    }
                }
            } else {
                statusText.textContent = "No face detected";
                videoContainer.style.borderColor = "gray";
                blinked = false;
                blinkFrameCount = 0;
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

            blinked = false;
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
                headers: { "Content-Type": "application/json" },
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
                    successSound?.play();
                    triggerHapticFeedback("success");
                    Swal.fire({
                        title: "Face Matched!",
                        text: "Attendance Recorded.\nDate & Time: " + formattedDateTime,
                        icon: "success",
                        timer: 3000,
                        showConfirmButton: false
                    }).then(() => location.reload());
                } else {
                    errorSound?.play();
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





only the shard file is not loading it shows 404 
https://localhost:7153/faceApi/tiny_face_detector_model-shard1
https://localhost:7153/faceApi/face_landmark_68_model-shard1
in the same .json files are available and that is loaded correctly why these 2 files are not loading
app.UseStaticFiles(); // Keep this if it's already there

// Add this to allow .shard1 files
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream"
});



