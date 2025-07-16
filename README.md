// Step 0: Category Alias Dictionary (fix spelling/mismatch)
Dictionary<string, string> categoryAlias = new Dictionary<string, string>
{
    { "SEMISKILLED", "SEMI SKILLED" },
    { "SEMI-SKILLED", "SEMI SKILLED" },
    { "SEMI  SKILLED", "SEMI SKILLED" },
    { "SEMI_SKILLED", "SEMI SKILLED" },
    { "SKILLED", "SKILLED" },
    { "UNSKILLED", "UNSKILLED" },
    // Add more if needed
};

List<string> validWorkOrders = new List<string>();

foreach (var workOrder in workOrders)
{
    bool allCategoriesMet = true;

    // Step 1: Filter distinctWorker for this workOrder
    // Make sure distinctWorker is created properly for this specific workOrder
    // Example: var distinctWorker = workerTable.Select($"WorkOrderNo = '{workOrder}'").CopyToDataTable();

    var categoryCount = (from r in distinctWorker.AsEnumerable()
                         let rawCat = r["JobMainCategory"]?.ToString().Trim().ToUpper() ?? ""
                         let cat = categoryAlias.ContainsKey(rawCat) ? categoryAlias[rawCat] : rawCat
                         where !string.IsNullOrWhiteSpace(cat)
                         group r by cat into g
                         select new
                         {
                             Key = g.Key,
                             Count = g.Count()
                         }).ToDictionary(x => x.Key, x => x.Count);

    // Step 2: Filter Ds2 rows for this work order
    var requiredRows = Ds2.Tables[0].Select($"WorkOrderNo = '{workOrder}'");

    // Step 3: Compare each category's required vs actual count
    foreach (var row in requiredRows)
    {
        string rawEmpType = row["EMP_TYPE"]?.ToString().Trim().ToUpper() ?? "";
        string category = categoryAlias.ContainsKey(rawEmpType) ? categoryAlias[rawEmpType] : rawEmpType;

        int requiredCount = 0;
        if (row["Total"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Total"].ToString()))
        {
            int.TryParse(row["Total"].ToString(), out requiredCount);
        }

        if (requiredCount == 0)
            continue;

        int actualCount = categoryCount.ContainsKey(category) ? categoryCount[category] : 0;

        Console.WriteLine($"[WorkOrder: {workOrder}] Category: {category} | Required: {requiredCount}, Actual: {actualCount}");

        if (actualCount < requiredCount)
        {
            allCategoriesMet = false;
            break;
        }
    }

    // Step 4: Add to final list only if all categories matched
    if (allCategoriesMet)
    {
        validWorkOrders.Add(workOrder);
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

        const storedDescriptor = await loadStoredFaceDescriptor('/images/151514-captured.jpg');
        const faceMatcher = new faceapi.FaceMatcher([new faceapi.LabeledFaceDescriptors("151514", [storedDescriptor])]);

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

        function captureImage() {
            const context = canvas.getContext("2d");
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.translate(canvas.width, 0);
            context.scale(-1, 1);
            context.drawImage(video, 0, 0, canvas.width, canvas.height);
            capturedImage.src = canvas.toDataURL("image/jpeg");
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
            const img = await faceapi.fetchImage(imagePath);
            const detection = await faceapi
                .detectSingleFace(img, detectorOptions)
                .withFaceLandmarks()
                .withFaceDescriptor();
            return detection ? detection.descriptor : null;
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
            return {
                matched: bestMatch.label !== "unknown",
                label: bestMatch.label,
                distance: bestMatch.distance,
                message: bestMatch.toString()
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

    // Run face recognition preview
    const result = await recognizeFace();

    if (result.matched) {
        statusText.textContent = `✅ ${result.label} - Face Matched`;
        videoContainer.style.borderColor = "green";
        if (punchInButton) punchInButton.style.display = "inline-block";
        if (punchOutButton) punchOutButton.style.display = "inline-block";
    } else {
        statusText.textContent = `❌ Unknown - Face Not Recognized`;
        videoContainer.style.borderColor = "red";
        if (punchInButton) punchInButton.style.display = "none";
        if (punchOutButton) punchOutButton.style.display = "none";
    }
}




this is my client side code for blink and other , i want this to add a face recognition of js 
<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="text-center camera">
        <div id="videoContainer" style="display: inline-block;width: 195px; border: 4px solid transparent; border-radius: 8px; transition: border-color 0.3s ease;">
            <video id="video" width="185" height="240" autoplay muted playsinline></video>
            <img id="capturedImage" style="display:none; width: 186px; height: 240px; border-radius: 8px;" />
        </div>
        <canvas id="canvas" style="display:none;"></canvas>
        <p id="statusText" style="font-weight: bold; margin-top: 10px; color: #444;"></p>
    </div>

    <input type="hidden" name="Type" id="EntryType" />

    <div class="mt-5 form-group">
        <div class="col d-flex justify-content-center mb-4">
            @if (ViewBag.InOut == "I")
            {
                <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">Punch In</button>
            }
        </div>
        <div class="col d-flex justify-content-center">
            @if (ViewBag.InOut == "O")
            {
                <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">Punch Out</button>
            }
        </div>
    </div>
</form>


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

        let eyeClosed = false;
        let blinkStartTime = 0;
        let blinkCount = 0;
        let blinked = false;
        let blinkValidUntil = 0;
        let blinkCountdownInterval;

        const EAR_THRESHOLD = 0.27;
        const BLINK_GAP = 300;
        const DOUBLE_BLINK_WINDOW = 2000;
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
                    statusText.textContent = "Keep your head straight";
                    videoContainer.style.borderColor = "orange";
                    blinked = false;
                    blinkCount = 0;
                    requestAnimationFrame(detectBlink);
                    return;
                }

                const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

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
                            setTimeout(captureImage, 500); 
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

                if (!blinked) {
                    statusText.textContent = "Please double blink";
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


i have image name 151514-captured.jpg and i want to compare with capture image, i have these models also ssd_mobilenetv1_model-shard1,ssd_mobilenetv1_model-shard2,ssd_mobilenetv1_model-weights_manifest.json
