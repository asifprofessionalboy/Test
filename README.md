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
            return Json(new { success = false, message = "No reference image found to verify face!" });

        string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");

        SaveBase64ImageToFile(model.ImageData, tempCapturedPath);

        string errorMessage;
        bool isFaceMatched = false;

        using (Bitmap tempCaptured = new Bitmap(tempCapturedPath))
        {
            if (System.IO.File.Exists(storedImagePath))
            {
                using (Bitmap stored = new Bitmap(storedImagePath))
                {
                    isFaceMatched = VerifyFaceWithDnn(tempCaptured, stored, out errorMessage);
                }
            }

            if (!isFaceMatched && System.IO.File.Exists(lastCapturedPath))
            {
                using (Bitmap lastCaptured = new Bitmap(lastCapturedPath))
                {
                    isFaceMatched = VerifyFaceWithDnn(tempCaptured, lastCaptured, out errorMessage);
                }
            }
        }

        System.IO.File.Delete(tempCapturedPath);

        string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
        string currentTime = DateTime.Now.ToString("HH:mm");

        DateTime today = DateTime.Today;

        var record = context.AppFaceVerificationDetails.FirstOrDefault(x => x.Pno == Pno && x.DateAndTime.Value.Date == today);
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
            return Json(new { success = true, message = "Face matched. Attendance recorded." });
        }
        else
        {
            if (model.Type == "Punch In")
                record.PunchInFailedCount++;
            else
                record.PunchOutFailedCount++;

            context.SaveChanges();
            return Json(new { success = false, message = errorMessage ?? "Face did not match." });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Server error: " + ex.Message });
    }
}
private bool VerifyFaceWithDnn(Bitmap captured, Bitmap stored, out string errorMessage)
{
    errorMessage = null;

    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        string protoPath = Path.Combine("wwwroot/DnnModels", "deploy.prototxt");
        string modelPath = Path.Combine("wwwroot/DnnModels", "res10_300x300_ssd_iter_140000_fp16.caffemodel");

        if (!File.Exists(protoPath) || !File.Exists(modelPath))
        {
            errorMessage = "Face detection model files missing!";
            return false;
        }

        var net = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);

        Rectangle? faceRectCaptured = DetectFaceWithDnn(matCaptured, net);
        Rectangle? faceRectStored = DetectFaceWithDnn(matStored, net);

        if (faceRectCaptured == null || faceRectStored == null)
        {
            errorMessage = "Face not detected. Ensure lighting is adequate.";
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, faceRectCaptured.Value);
        Mat storedFace = new Mat(matStored, faceRectStored.Value);

        CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
        CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

        CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        CvInvoke.EqualizeHist(capturedFace, capturedFace);
        CvInvoke.EqualizeHist(storedFace, storedFace);

        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 99))
        {
            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);
            var result = faceRecognizer.Predict(capturedFace);

            if (result.Label == 1)
            {
                if (result.Distance <= 50)
                    return true;
                else if (result.Distance <= 70)
                {
                    errorMessage = "Face matched with low confidence. Please try again in better lighting.";
                    return false;
                }
            }

            errorMessage = $"Face did not match. Distance: {result.Distance:F2}";
            return false;
        }
    }
    catch (Exception ex)
    {
        errorMessage = "Error in DNN verification: " + ex.Message;
        return false;
    }
}

private Rectangle? DetectFaceWithDnn(Mat image, Net net)
{
    Size size = new Size(300, 300);
    double scale = 1.0;
    MCvScalar mean = new MCvScalar(104, 177, 123);

    using (var blob = DnnInvoke.BlobFromImage(image, scale, size, mean, false, false))
    {
        net.SetInput(blob);
        using (var detection = net.Forward())
        {
            var data = detection.GetData();
            float[,,,] faces = (float[,,,])data;

            for (int i = 0; i < faces.GetLength(2); i++)
            {
                float confidence = faces[0, 0, i, 2];

                if (confidence > 0.5)
                {
                    int x1 = (int)(faces[0, 0, i, 3] * image.Cols);
                    int y1 = (int)(faces[0, 0, i, 4] * image.Rows);
                    int x2 = (int)(faces[0, 0, i, 5] * image.Cols);
                    int y2 = (int)(faces[0, 0, i, 6] * image.Rows);

                    return new Rectangle(x1, y1, x2 - x1, y2 - y1);
                }
            }
        }
    }
    return null;
}



private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        CvInvoke.CvtColor(matCaptured, matCaptured, ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, ColorConversion.Bgr2Gray);

        string protoPath = Path.Combine(Directory.GetCurrentDirectory(), "models", "deploy.prototxt");
        string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "models", "res10_300x300_ssd_iter_140000.caffemodel");

        if (!File.Exists(protoPath) || !File.Exists(modelPath))
        {
            Console.WriteLine("Error: DNN model files not found!");
            return false;
        }

        var net = CvInvoke.ReadNetFromCaffe(protoPath, modelPath);

        Rectangle[] DetectFaces(Mat image)
        {
            var blob = CvInvoke.DnnBlobFromImage(image, 1.0, new Size(300, 300), new MCvScalar(104, 177, 123));
            net.SetInput(blob);
            var detection = net.Forward();

            List<Rectangle> faces = new List<Rectangle>();
            for (int i = 0; i < detection.SizeOfDimension[2]; i++)
            {
                float confidence = detection.GetData(new int[] { 0, 0, i, 2 });
                if (confidence > 0.7) // Confidence threshold
                {
                    int x1 = (int)(detection.GetData(new int[] { 0, 0, i, 3 }) * image.Cols);
                    int y1 = (int)(detection.GetData(new int[] { 0, 0, i, 4 }) * image.Rows);
                    int x2 = (int)(detection.GetData(new int[] { 0, 0, i, 5 }) * image.Cols);
                    int y2 = (int)(detection.GetData(new int[] { 0, 0, i, 6 }) * image.Rows);
                    faces.Add(new Rectangle(x1, y1, x2 - x1, y2 - y1));
                }
            }
            return faces.ToArray();
        }

        Rectangle[] capturedFaces = DetectFaces(matCaptured);
        Rectangle[] storedFaces = DetectFaces(matStored);

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);

        CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
        CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 98))
        {
            CvInvoke.EqualizeHist(capturedFace, capturedFace);
            CvInvoke.EqualizeHist(storedFace, storedFace);

            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);
            var result = faceRecognizer.Predict(capturedFace);

            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            return result.Label == 1 && result.Distance <= 98;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}


private bool IsLowLight(Mat image)
{
    Mat gray = new Mat();
    CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
    double meanBrightness = CvInvoke.Mean(gray).V0;
    return meanBrightness < 50; // Threshold for low light
}




this is my full code of controller 
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




        private bool VerifyFace(Bitmap captured, Bitmap stored)
        {
            try
            {
                Mat matCaptured = BitmapToMat(captured);
                Mat matStored = BitmapToMat(stored);


                CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);


                string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
                if (!System.IO.File.Exists(cascadePath))
                {
                    Console.WriteLine("Error: Haarcascade file not found!");
                    return false;
                }

                CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
                Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
                Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);


                if (capturedFaces.Length == 0 || storedFaces.Length == 0)
                {
                    Console.WriteLine("No face detected in one or both images.");
                    return false;
                }




                Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
                Mat storedFace = new Mat(matStored, storedFaces[0]);


                CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
                CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));


                using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 99))
                {
                    CvInvoke.EqualizeHist(capturedFace, capturedFace);
                    CvInvoke.EqualizeHist(storedFace, storedFace);

                    VectorOfMat trainingImages = new VectorOfMat();
                    trainingImages.Push(storedFace);
                    VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                    faceRecognizer.Train(trainingImages, labels);
                    var result = faceRecognizer.Predict(capturedFace);

                    Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                    return result.Label == 1 && result.Distance <= 99;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in face verification: " + ex.Message);
                return false;
            }
        }
       private Mat BitmapToMat(Bitmap bitmap)
       {
           using (MemoryStream ms = new MemoryStream())
           {
               bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
               byte[] imageData = ms.ToArray();

               Mat mat = new Mat();
               CvInvoke.Imdecode(new VectorOfByte(imageData), ImreadModes.Color, mat);

               if (mat.IsEmpty)
               {
                   Console.WriteLine("Error: Image conversion failed!");
               }

               return mat;
           }
       }
       private void SaveBase64ImageToFile(string base64String, string filePath)
       {
           try
           {
               byte[] imageBytes = Convert.FromBase64String(base64String.Split(',')[1]);
               using (MemoryStream ms = new MemoryStream(imageBytes))
               {
                   using (Bitmap bmp = new Bitmap(ms))
                   {
                       bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                   }
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine("Error saving Base64 image to file: " + ex.Message);
           }
       }
this is my js
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

       
       

        fetch("/TSUISLARS/Geo/AttendanceData", {
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

i want full code based on this logic to Implement DNN
