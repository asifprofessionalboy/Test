private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
        if (!System.IO.File.Exists(cascadePath))
        {
            Console.WriteLine("Error: Haarcascade file not found!");
            return false;
        }

        var faceCascade = new CascadeClassifier(cascadePath);

        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 10, Size.Empty, new Size(100, 100));
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 10, Size.Empty, new Size(100, 100));

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one of the images.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);

        // Resize and preprocess
        Size targetSize = new Size(150, 150);
        CvInvoke.Resize(capturedFace, capturedFace, targetSize);
        CvInvoke.Resize(storedFace, storedFace, targetSize);

        CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        CvInvoke.EqualizeHist(capturedFace, capturedFace);
        CvInvoke.EqualizeHist(storedFace, storedFace);

        using (var recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 70))
        {
            var trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            var labels = new VectorOfInt(new int[] { 1 });

            recognizer.Train(trainingImages, labels);
            var result = recognizer.Predict(capturedFace);

            double histSimilarity = CompareHistograms(capturedFace, storedFace);

            Console.WriteLine($"Label: {result.Label}, Distance: {result.Distance}, Histogram: {histSimilarity}");

            return result.Label == 1 && result.Distance <= 70 && histSimilarity > 0.5;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}

public void TestFaceRecognition()
{
    string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/123-John.jpg");
    string testImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/123-Test.jpg");

    if (!File.Exists(storedImagePath) || !File.Exists(testImagePath))
    {
        Console.WriteLine("Test images not found.");
        return;
    }

    using (Bitmap stored = new Bitmap(storedImagePath))
    using (Bitmap test = new Bitmap(testImagePath))
    {
        bool result = VerifyFace(test, stored);
        Console.WriteLine("Face Match Result: " + (result ? "Matched" : "Not Matched"));
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
                string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");

                if (!System.IO.File.Exists(storedImagePath) && !System.IO.File.Exists(lastCapturedPath))
                {
                    return Json(new { success = false, message = "No reference image found to verify face!" });
                }

                
                string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Temp-{DateTime.Now.Ticks}.jpg");
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

                if (isFaceMatched)
                {
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (model.Type == "Punch In")
                    {
                        
                        string newCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");
                        SaveBase64ImageToFile(model.ImageData, newCapturedPath);

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




        private bool VerifyFace(Bitmap captured, Bitmap stored)
        {
            try
            {
                Mat matCaptured = BitmapToMat(captured);
                Mat matStored = BitmapToMat(stored);

                string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
                if (!System.IO.File.Exists(cascadePath))
                {
                    Console.WriteLine("Error: Haarcascade file not found!");
                    return false;
                }

                var faceCascade = new CascadeClassifier(cascadePath);

                Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5, Size.Empty, new Size(80, 80));
                Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5, Size.Empty, new Size(80, 80));

                if (capturedFaces.Length == 0 || storedFaces.Length == 0)
                {
                    Console.WriteLine("No face detected.");
                    return false;
                }

                Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
                Mat storedFace = new Mat(matStored, storedFaces[0]);

               
                Size targetSize = new Size(150, 150);
                CvInvoke.Resize(capturedFace, capturedFace, targetSize);
                CvInvoke.Resize(storedFace, storedFace, targetSize);

              
                CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

              
                CvInvoke.GaussianBlur(capturedFace, capturedFace, new Size(3, 3), 0);
                CvInvoke.GaussianBlur(storedFace, storedFace, new Size(3, 3), 0);

               
                using (var recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 60)) // threshold = 50
                {
                    var trainingImages = new VectorOfMat();
                    trainingImages.Push(storedFace);
                    var labels = new VectorOfInt(new int[] { 1 });

                    recognizer.Train(trainingImages, labels);
                    var result = recognizer.Predict(capturedFace);

                    Console.WriteLine($"Label: {result.Label}, Distance: {result.Distance}");

                   
                    double histSimilarity = CompareHistograms(capturedFace, storedFace);
                    Console.WriteLine($"Histogram similarity: {histSimilarity}");

                    return result.Label == 1 && result.Distance <= 60 && histSimilarity > 0.5;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in face verification: " + ex.Message);
                return false;
            }
        }

        private double CompareHistograms(Mat img1, Mat img2)
        {
            DenseHistogram hist1 = new DenseHistogram(256, new RangeF(0, 256));
            DenseHistogram hist2 = new DenseHistogram(256, new RangeF(0, 256));

            hist1.Calculate(new Image<Gray, byte>[] { img1.ToImage<Gray, byte>() }, false, null);
            hist2.Calculate(new Image<Gray, byte>[] { img2.ToImage<Gray, byte>() }, false, null);

            CvInvoke.Normalize(hist1, hist1, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);
            CvInvoke.Normalize(hist2, hist2, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);

            return CvInvoke.CompareHist(hist1, hist2, Emgu.CV.CvEnum.HistogramCompMethod.Correl);
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

        this is not working, when matching the face it shows everytime face doesnot match, please check this out and also give a hardcoded testing for images
