[HttpPost]
public IActionResult AttendanceData([FromBody] AttendanceRequest model)
{
    if (string.IsNullOrEmpty(model.ImageData))
    {
        return Json(new { success = false, message = "Image data is missing!" });
    }

    try
    {
        var UserId = HttpContext.Request.Cookies["Session"];
        string Pno = UserId;

        // Convert Base64 captured image to byte array
        byte[] imageBytes = Convert.FromBase64String(model.ImageData.Split(',')[1]);

        using (var ms = new MemoryStream(imageBytes))
        {
            Bitmap capturedImage = new Bitmap(ms);

            // Fetch stored image from database (binary)
            var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
            if (user == null || user.Image == null)
            {
                return Json(new { success = false, message = "User Image Not Found!" });
            }

            // Convert binary image data to JPG format
            string tempFilePath = Path.Combine(Path.GetTempPath(), "storedImage.jpg");
            System.IO.File.WriteAllBytes(tempFilePath, user.Image);
            Bitmap storedImage = new Bitmap(tempFilePath);

            bool isPartialMatch;
            bool isFaceMatched = VerifyFace(capturedImage, storedImage, out isPartialMatch);

            if (isFaceMatched)
            {
                string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                string currentTime = DateTime.Now.ToString("HH:mm");

                if (model.Type == "Punch In")
                {
                    StoreData(currentDate, currentTime, null, Pno);
                }
                else
                {
                    StoreData(currentDate, null, currentTime, Pno);
                }

                return Json(new { success = true, message = "Attendance Marked Successfully!" });
            }
            else if (isPartialMatch)
            {
                return Json(new { success = false, message = "Face partially matched! Please try again." });
            }
            else
            {
                return Json(new { success = false, message = "Face does not match!" });
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

// Face Verification Logic
private bool VerifyFace(Bitmap captured, Bitmap stored, out bool isPartialMatch)
{
    isPartialMatch = false;

    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert images to grayscale
        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "D:/Irshad_Project/GFAS/GFAS/wwwroot/Cascades/haarcascade_frontalface_default.xml");
        Console.WriteLine($"Cascade Path: {cascadePath}");

        if (!System.IO.File.Exists(cascadePath))
        {
            Console.WriteLine("Error: Haarcascade file not found!");
            return false;
        }
        var faceCascade = new CascadeClassifier(cascadePath);

        // Detect faces in images
        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);

        // Resize faces for better recognition
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

            // Strong Match (Exact Face Match)
            if (result.Label == 1 && result.Distance < 50)
            {
                return true;
            }
            // Partial Match (50% Face Match)
            else if (result.Label == 1 && result.Distance >= 50 && result.Distance <= 80)
            {
                isPartialMatch = true;
                return false;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
    }

    return false;
}

// Convert Bitmap to Mat for EmguCV
private Mat BitmapToMat(Bitmap bitmap)
{
    Mat mat = new Mat();
    using (MemoryStream ms = new MemoryStream())
    {
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        byte[] byteArray = ms.ToArray();
        mat = CvInvoke.Imdecode(byteArray, ImreadModes.Color);
    }
    return mat;
}




yes it works after replacing Xml file now i have this method . i want the same logic 
 [HttpPost]
 public IActionResult AttendanceData([FromBody] AttendanceRequest model)
 {
     if (string.IsNullOrEmpty(model.ImageData))
     {
         return Json(new { success = false, message = "Image data is missing!" });
     }

     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];
         string Pno = UserId;

         byte[] imageBytes = Convert.FromBase64String(model.ImageData.Split(',')[1]);

         using (var ms = new MemoryStream(imageBytes))
         {
             Bitmap capturedImage = new Bitmap(ms);

             var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
             if (user == null || user.Image == null)
             {
                 return Json(new { success = false, message = "User Image Not Found!" });
             }

             using (var storedStream = new MemoryStream(user.Image))
             {
                 Bitmap storedImage = new Bitmap(storedStream);

                 bool isPartialMatch;
                 bool isFaceMatched = VerifyFace(capturedImage, storedImage, out isPartialMatch);

                 if (isFaceMatched)
                 {
                     string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                     string currentTime = DateTime.Now.ToString("HH:mm");

                     if (model.Type == "Punch In")
                     {
                         StoreData(currentDate, currentTime, null, Pno);
                     }
                     else
                     {
                         StoreData(currentDate, null, currentTime, Pno);
                     }

                     return Json(new { success = true, message = "Attendance Marked Successfully!" });
                 }
                 else if (isPartialMatch)
                 {
                     return Json(new { success = false, message = "Face partially matched! Please try again." });
                 }
                 else
                 {
                     return Json(new { success = false, message = "Face does not match!" });
                 }
             }
         }
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }
 private bool VerifyFace(Bitmap captured, Bitmap stored, out bool isPartialMatch)
 {
     isPartialMatch = false;

     try
     {
         Mat matCaptured = BitmapToMat(captured);
         Mat matStored = BitmapToMat(stored);

         CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
         CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

         string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "D:/Irshad_Project/GFAS/GFAS/wwwroot/Cascades/haarcascade_frontalface_default.xml");
         Console.WriteLine($"Cascade Path: {cascadePath}");

         if (!System.IO.File.Exists(cascadePath))
         {
             Console.WriteLine("Error: Haarcascade file not found!");
             return false;
         }
         var faceCascade = new CascadeClassifier(cascadePath);

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

             // Strong Match (Exact Face Match)
             if (result.Label == 1 && result.Distance < 50)
             {
                 return true;
             }
             // Partial Match (50% Face Match)
             else if (result.Label == 1 && result.Distance >= 50 && result.Distance <= 80)
             {
                 isPartialMatch = true;
                 return false;
             }
         }
     }
     catch (Exception ex)
     {
         Console.WriteLine("Error in face verification: " + ex.Message);
     }

     return false;
 }



in this image is coming from table and another one is captured but in binary,is it works for binary? previous i have compare with Jpg that is works. if image file i want jpg then please convert 
