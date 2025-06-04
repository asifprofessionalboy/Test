private bool VerifyFace(Bitmap capturedBmp, Bitmap storedBmp)
{
    try
    {
        // Load DNN model
        string protoPath = Path.Combine("wwwroot/Cascades", "deploy.prototxt");
        string modelPath = Path.Combine("wwwroot/Cascades", "res10_300x300_ssd_iter_140000.caffemodel");

        var net = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);

        // Convert to Mat
        Mat capturedMat = BitmapToMat(capturedBmp);
        Mat storedMat = BitmapToMat(storedBmp);

        Rectangle? capturedFace = DetectFaceDnn(net, capturedMat);
        Rectangle? storedFace = DetectFaceDnn(net, storedMat);

        if (capturedFace == null || storedFace == null)
        {
            Console.WriteLine("Face not detected in one or both images.");
            return false;
        }

        // Crop face regions
        Mat capturedFaceMat = new Mat(capturedMat, capturedFace.Value);
        Mat storedFaceMat = new Mat(storedMat, storedFace.Value);

        CvInvoke.Resize(capturedFaceMat, capturedFaceMat, new Size(150, 150));
        CvInvoke.Resize(storedFaceMat, storedFaceMat, new Size(150, 150));

        // Use histogram comparison or template match (simplified logic)
        double similarity = CompareImagesHistogram(capturedFaceMat, storedFaceMat);

        Console.WriteLine("Similarity score: " + similarity);

        return similarity >= 0.8; // Adjust threshold as needed
    }
    catch (Exception ex)
    {
        Console.WriteLine("DNN face verification error: " + ex.Message);
        return false;
    }
}
private Rectangle? DetectFaceDnn(Net net, Mat image)
{
    Size inputSize = new Size(300, 300);
    double scaleFactor = 1.0;
    MCvScalar meanVal = new MCvScalar(104, 177, 123);

    using var blob = DnnInvoke.BlobFromImage(image, scaleFactor, inputSize, meanVal, false, false);
    net.SetInput(blob);
    using var detections = net.Forward();

    float[,,,] data = detections.GetData(true) as float[,,,];
    if (data == null) return null;

    for (int i = 0; i < data.GetLength(2); i++)
    {
        float confidence = data[0, 0, i, 2];
        if (confidence > 0.6f)
        {
            int x1 = (int)(data[0, 0, i, 3] * image.Cols);
            int y1 = (int)(data[0, 0, i, 4] * image.Rows);
            int x2 = (int)(data[0, 0, i, 5] * image.Cols);
            int y2 = (int)(data[0, 0, i, 6] * image.Rows);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }

    return null;
}

private double CompareImagesHistogram(Mat img1, Mat img2)
{
    Mat hsv1 = new Mat();
    Mat hsv2 = new Mat();

    CvInvoke.CvtColor(img1, hsv1, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);
    CvInvoke.CvtColor(img2, hsv2, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

    var histSize = new int[] { 50, 60 };
    var ranges = new RangeF[] { new RangeF(0, 180), new RangeF(0, 256) };
    int[] channels = { 0, 1 };

    var hist1 = new Mat();
    var hist2 = new Mat();

    CvInvoke.CalcHist(new Mat[] { hsv1 }, channels, null, hist1, histSize, ranges, true);
    CvInvoke.CalcHist(new Mat[] { hsv2 }, channels, null, hist2, histSize, ranges, true);

    CvInvoke.Normalize(hist1, hist1, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);
    CvInvoke.Normalize(hist2, hist2, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);

    return CvInvoke.CompareHist(hist1, hist2, Emgu.CV.CvEnum.HistogramCompMethod.Correl);
}



private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Load DNN face detector
        string protoPath = Path.Combine("wwwroot/Cascades", "deploy.prototxt.txt");
        string modelPath = Path.Combine("wwwroot/Cascades", "res10_300x300_ssd_iter_140000_fp16.caffemodel");

        var net = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);

        Rectangle? capturedFaceRect = DetectFaceWithDnn(net, matCaptured);
        Rectangle? storedFaceRect = DetectFaceWithDnn(net, matStored);

        if (capturedFaceRect == null || storedFaceRect == null)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaceRect.Value);
        Mat storedFace = new Mat(matStored, storedFaceRect.Value);

        CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
        CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
        {
            CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

            CvInvoke.EqualizeHist(capturedFace, capturedFace);
            CvInvoke.EqualizeHist(storedFace, storedFace);

            var trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            var labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);

            var result = faceRecognizer.Predict(capturedFace);

            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            return result.Label == 1 && result.Distance <= 98; // You can tune the threshold
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in DNN-based face verification: " + ex.Message);
        return false;
    }
}
private Rectangle? DetectFaceWithDnn(Net net, Mat image)
{
    try
    {
        Size size = new Size(300, 300);
        Mat blob = DnnInvoke.BlobFromImage(image, 1.0, size, new MCvScalar(104, 177, 123), false, false);
        net.SetInput(blob);
        Mat detections = net.Forward();

        float confidenceThreshold = 0.5f;

        for (int i = 0; i < detections.SizeOfDimension[2]; i++)
        {
            float confidence = detections.GetData(new int[] { 0, 0, i, 2 }) is float conf ? conf : 0;
            if (confidence > confidenceThreshold)
            {
                int x1 = (int)(detections.GetData(new int[] { 0, 0, i, 3 }) is float fx1 ? fx1 * image.Cols : 0);
                int y1 = (int)(detections.GetData(new int[] { 0, 0, i, 4 }) is float fy1 ? fy1 * image.Rows : 0);
                int x2 = (int)(detections.GetData(new int[] { 0, 0, i, 5 }) is float fx2 ? fx2 * image.Cols : 0);
                int y2 = (int)(detections.GetData(new int[] { 0, 0, i, 6 }) is float fy2 ? fy2 * image.Rows : 0);

                Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                return faceRect;
            }
        }

        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine("DNN detection error: " + ex.Message);
        return null;
    }
}




this is my current face Recognition logic , but it gets the false cases and when i low the threshold it is not matching with same user and if i increase the threshold it matches with everyone , is there anything better to do in this logic please    
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

                    return result.Label == 1 && result.Distance <= 98;
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
