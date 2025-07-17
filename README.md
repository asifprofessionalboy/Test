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

        // ✅ SHORT-CIRCUIT: If client already verified the face, skip server-side face match
        if (model.IsFaceMatched)
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

        // 🔁 fallback to original server-side face verification (if needed)
        // [Your existing logic remains here]

        return Json(new { success = false, message = "Face verification failed." }); // fallback
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}






this is my controller code ,in this when button is click this method is called and verify the face but now i want that from client side of the js if face is matched then i dont want to verify face automatically it true in IsFaceMatched
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

        

         if (!System.IO.File.Exists(storedImagePath) && !System.IO.File.Exists(lastCapturedPath))
         {
             return Json(new { success = false, message = "No reference image found to verify face!" });
         }

        
         string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");

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
and this is my client side code
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
        }, 2000); 
    } else {
        statusText.textContent = "Face not matched ❌";
        videoContainer.style.borderColor = "red";
        setTimeout(() => {
            statusText.textContent = "";
            videoContainer.style.borderColor = "gray";
        }, 2000);
    }
}


        function startCountdown() {
    blinkValidUntil = Date.now() + 10000; 
    setTimeout(() => {
        statusText.textContent = "Please double blink";
        videoContainer.style.borderColor = "red";
        resetBlink();
        detectBlink(); 
    }, 10000);
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

