<!-- Include SweetAlert2 for UI alerts -->
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

<!-- face-api.js -->
<script defer src="https://cdn.jsdelivr.net/npm/face-api.js"></script>

<!-- Video and Canvas Elements -->
<video id="video" width="480" height="360" autoplay muted></video>
<canvas id="canvas" style="display:none;"></canvas>

<!-- Hidden Inputs -->
<input type="hidden" id="EntryType" />

<!-- Audio Feedback -->
<audio id="successSound" src="/success.mp3"></audio>
<audio id="errorSound" src="/error.mp3"></audio>

<!-- Buttons -->
<button onclick="captureImageAndSubmit('Punch In')">Punch In</button>
<button onclick="captureImageAndSubmit('Punch Out')">Punch Out</button>

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");

    let blinked = false;
    let lastBlinkTime = 0;
    const BLINK_INTERVAL = 3000; // 3 seconds between valid blinks
    const EAR_THRESHOLD = 0.23;

    // Load face-api models
    Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri('/models'),
        faceapi.nets.faceLandmark68Net.loadFromUri('/models')
    ]).then(startVideo);

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
            }
        }

        requestAnimationFrame(detectBlink);
    }

    function captureImageAndSubmit(entryType) {
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
https://github.com/justadudewhohacks/face-api.js-models



function detectMotion() {
    const context = canvas.getContext("2d");
    // Resize for faster processing and sensitivity boost
    const tempWidth = 160;  // Small resolution improves performance & sensitivity
    const tempHeight = 120;

    canvas.width = tempWidth;
    canvas.height = tempHeight;
    context.drawImage(video, 0, 0, tempWidth, tempHeight);

    const currentFrame = context.getImageData(0, 0, tempWidth, tempHeight);

    if (previousFrame) {
        let diff = 0;
        const threshold = 30;         // Color difference threshold
        const motionThreshold = 2000; // Lowered from 15000 to 2000 for better sensitivity

        for (let i = 0; i < currentFrame.data.length; i += 4) {
            const r = Math.abs(currentFrame.data[i] - previousFrame.data[i]);
            const g = Math.abs(currentFrame.data[i + 1] - previousFrame.data[i + 1]);
            const b = Math.abs(currentFrame.data[i + 2] - previousFrame.data[i + 2]);

            if (r > threshold || g > threshold || b > threshold) {
                diff++;
            }
        }

        motionDetected = diff > motionThreshold;

        // Optional: log for debugging
        // console.log("Pixel Diff:", diff, "Motion Detected:", motionDetected);
    }

    previousFrame = currentFrame;
    requestAnimationFrame(detectMotion);
}

<div id="motionStatus" style="font-weight: bold; color: red;">Motion: Not Detected</div>
document.getElementById("motionStatus").textContent = motionDetected ? "Motion: Detected" : "Motion: Not Detected";
document.getElementById("motionStatus").style.color = motionDetected ? "green" : "red";



in this js 
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

this is my controller code
 [HttpPost]
 public IActionResult AttendanceData([FromBody] AttendanceRequest model)
 {
     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];
         var UserName = HttpContext.Request.Cookies["UserName"];
         if (string.IsNullOrEmpty(UserId))
             return Json(new { success = false, message = "User session not found!" });

         string Pno = UserId;
         string Name = UserName;

         string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-{Name}.jpg");
         string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");

         //string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Shashi Kumar.jpg");
         //string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Shashi Kumar.jpg");

         if (!System.IO.File.Exists(storedImagePath) && !System.IO.File.Exists(lastCapturedPath))
         {
             return Json(new { success = false, message = "No reference image found to verify face!" });
         }

         string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");
         //string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"stored.jpg");
         SaveBase64ImageToFile(model.ImageData, tempCapturedPath);

         bool isFaceMatched = false;

         using (Bitmap tempCaptured = new Bitmap(tempCapturedPath))
         {
             if (System.IO.File.Exists(storedImagePath))
             {
                 using (Bitmap stored = new Bitmap(storedImagePath))
                 {
                     isFaceMatched = VerifyFace(tempCaptured, stored);
                 }
             }

             if (!isFaceMatched && System.IO.File.Exists(lastCapturedPath))
             {
                 using (Bitmap lastCaptured = new Bitmap(lastCapturedPath))
                 {
                     isFaceMatched = VerifyFace(tempCaptured, lastCaptured);
                 }
             }
         }

         System.IO.File.Delete(tempCapturedPath);

         string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
         string currentTime = DateTime.Now.ToString("HH:mm");


         DateTime today = DateTime.Today;

         var record = context.AppFaceVerificationDetails
             .FirstOrDefault(x => x.Pno == Pno && x.DateAndTime.Value.Date == today);

         if (record == null)
         {
             record = new AppFaceVerificationDetail
             {
                 Pno = Pno,
                 PunchInFailedCount = 0,
                 PunchOutFailedCount = 0,
                 PunchInSuccess = false,
                 PunchOutSuccess = false
             };
             context.AppFaceVerificationDetails.Add(record);
         }

         if (isFaceMatched)
         {
             if (model.Type == "Punch In")
             {
                 string newCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");
                 SaveBase64ImageToFile(model.ImageData, newCapturedPath);

                 StoreData(currentDate, currentTime, null, Pno);

                 record.PunchInSuccess = true;
             }
             else
             {
                 StoreData(currentDate, null, currentTime, Pno);

                 record.PunchOutSuccess = true;
             }

             context.SaveChanges();
             return Json(new { success = true, message = "Attendance recorded successfully." });
         }
         else
         {
             if (model.Type == "Punch In")
                 record.PunchInFailedCount = (record.PunchInFailedCount ?? 0) + 1;
             else
                 record.PunchOutFailedCount = (record.PunchOutFailedCount ?? 0) + 1;

             context.SaveChanges();
             return Json(new { success = false, message = "Face does not match!" });
         }

     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }


even i am moving and in motion it shows me no motion detect , why 
