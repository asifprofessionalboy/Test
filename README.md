<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");

    let previousFrame = null;
    let motionDetected = false;

    // Start video stream
    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
            video.addEventListener('play', () => {
                requestAnimationFrame(detectMotion);
            });
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

    function detectMotion() {
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const currentFrame = context.getImageData(0, 0, canvas.width, canvas.height);

        if (previousFrame) {
            let diff = 0;
            const threshold = 30; // pixel difference threshold
            const motionThreshold = 15000; // number of differing pixels to consider as motion

            for (let i = 0; i < currentFrame.data.length; i += 4) {
                const r = Math.abs(currentFrame.data[i] - previousFrame.data[i]);
                const g = Math.abs(currentFrame.data[i + 1] - previousFrame.data[i + 1]);
                const b = Math.abs(currentFrame.data[i + 2] - previousFrame.data[i + 2]);

                if (r > threshold || g > threshold || b > threshold) {
                    diff++;
                }
            }

            motionDetected = diff > motionThreshold;
        }

        previousFrame = currentFrame;
        requestAnimationFrame(detectMotion);
    }

    function captureImageAndSubmit(entryType) {
        if (!motionDetected) {
            Swal.fire({
                title: "No Motion Detected",
                text: "Please do not use an image. Move slightly to verify liveness.",
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
            var now = new Date();
            var formattedDateTime = now.toLocaleString();

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
    }

    function triggerHapticFeedback(type) {
        if ("vibrate" in navigator) {
            if (type === "success") {
                navigator.vibrate(100);
            } else if (type === "error") {
                navigator.vibrate([200, 100, 200]);
            }
        }
    }
</script>







this is my js in this i want to check that there is motion in video or not if there is no motion detect then pupup a msg and also dont give option to capture the image that please dont use image , first verify properly then capture

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");

    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            let video = document.getElementById("video");
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

 
    function captureImageAndSubmit(entryType) {
        EntryTypeInput.value = entryType;

        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageData = canvas.toDataURL("image/jpeg"); // Save as JPG

        
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
                if (data.success) {
                    var now = new Date();
                    var formattedDateTime = now.toLocaleString();
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
                    var now = new Date();
                    var formattedDateTime = now.toLocaleString();
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
            
    }

    function triggerHapticFeedback(type) {
        if ("vibrate" in navigator) {
            if (type === "success") {
                navigator.vibrate(100); 
            } else if (type === "error") {
                navigator.vibrate([200, 100, 200]); 
            }
        }
    }
</script>
