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
        if (!System.IO.File.Exists(storedImagePath))
        {
            return Json(new { success = false, message = "Stored image not found!" });
        }

        string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");

        SaveBase64ImageToFile(model.ImageData, capturedImagePath);

        bool isFaceMatched = false;

        using (Bitmap capturedImage = new Bitmap(capturedImagePath))
        using (Bitmap storedImage = new Bitmap(storedImagePath))
        {
            isFaceMatched = VerifyFace(capturedImage, storedImage);
        }

        System.IO.File.Delete(capturedImagePath);

        if (isFaceMatched)
        {
            string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
            string currentTime = DateTime.Now.ToString("HH:mm");

            if (model.Type == "Punch In")
            {
                StoreData(currentDate, currentTime, null, Pno, model.ImageData);
            }
            else
            {
                StoreData(currentDate, null, currentTime, Pno, model.ImageData);
            }

            return Json(new { success = true, message = "Attendance recorded successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Face does not match!" });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

// ✅ Preprocess Image for Better Accuracy
private Mat PreprocessImage(Mat image)
{
    CvInvoke.CvtColor(image, image, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
    
    // ✅ Apply Gaussian Blur to Reduce Noise
    CvInvoke.GaussianBlur(image, image, new Size(3, 3), 0);

    // ✅ Apply CLAHE for Adaptive Contrast Adjustment
    Mat equalizedImage = new Mat();
    CvInvoke.EqualizeHist(image, equalizedImage);

    return equalizedImage;
}

private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // ✅ Preprocess images to normalize lighting
        matCaptured = PreprocessImage(matCaptured);
        matStored = PreprocessImage(matStored);

        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
        if (!System.IO.File.Exists(cascadePath))
        {
            Console.WriteLine("Error: Haarcascade file not found!");
            return false;
        }

        CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 6, new Size(30, 30));
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 6, new Size(30, 30));

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);

        CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
        CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

        using (var faceRecognizer = new LBPHFaceRecognizer(2, 16, 8, 8, 90))  // ✅ Fine-tuned LBPH parameters
        {
            CvInvoke.EqualizeHist(capturedFace, capturedFace);
            CvInvoke.EqualizeHist(storedFace, storedFace);

            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);
            var result = faceRecognizer.Predict(capturedFace);

            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            return result.Label == 1 && result.Distance < 90;
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
                if (!System.IO.File.Exists(storedImagePath))
                {
                    return Json(new { success = false, message = "Stored image not found!" });
                }


                string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");


                SaveBase64ImageToFile(model.ImageData, capturedImagePath);

                bool isFaceMatched = false;


                using (Bitmap capturedImage = new Bitmap(capturedImagePath))
                using (Bitmap storedImage = new Bitmap(storedImagePath))
                {
                    isFaceMatched = VerifyFace(capturedImage, storedImage);
                }


                System.IO.File.Delete(capturedImagePath);

                if (isFaceMatched)
                {
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (model.Type == "Punch In")
                    {
                        StoreData(currentDate, currentTime, null, Pno, model.ImageData);
                    }
                    else
                    {
                        StoreData(currentDate, null, currentTime, Pno, model.ImageData);
                    }

                    return Json(new { success = true, message = "Attendance recorded successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Face does not match!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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


                using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
                {
                    CvInvoke.EqualizeHist(capturedFace, capturedFace);
                    CvInvoke.EqualizeHist(storedFace, storedFace);

                    VectorOfMat trainingImages = new VectorOfMat();
                    trainingImages.Push(storedFace);
                    VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                    faceRecognizer.Train(trainingImages, labels);
                    var result = faceRecognizer.Predict(capturedFace);

                    Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                    return result.Label == 1 && result.Distance < 100;
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


make this more good for based on lighting or other things. sometimes it matched sometimes not . if i go to brighter light then matched but sometimes not 
