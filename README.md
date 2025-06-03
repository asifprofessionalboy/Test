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
