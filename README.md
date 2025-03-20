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
        string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/StoredFace.jpg");
        string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Captured.jpg");

        if (!System.IO.File.Exists(storedImagePath) || !System.IO.File.Exists(capturedImagePath))
        {
            return Json(new { success = false, message = "One or both hardcoded images not found!" });
        }

        // Load images properly
        using (Bitmap storedImage = new Bitmap(storedImagePath))
        using (Bitmap capturedImage = new Bitmap(capturedImagePath))
        {
            bool isFaceMatched = VerifyFace(capturedImage, storedImage);

            if (isFaceMatched)
            {
                return Json(new { success = true, message = "Face Matched! Debugging successful." });
            }
            else
            {
                return Json(new { success = false, message = "Face does not match in hardcoded test!" });
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

public class AttendanceRequest
{
    public string Type { get; set; }
    public string ImageData { get; set; }
}

// ✅ Improved Face Verification Logic
private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert to grayscale
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

        // Resize images for face recognition
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

            // ✅ Strong match
            return result.Label == 1 && result.Distance < 50;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
    }

    return false;
}

// ✅ Fixed Bitmap to Mat Conversion
private Mat BitmapToMat(Bitmap bitmap)
{
    using (MemoryStream ms = new MemoryStream())
    {
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        byte[] imageData = ms.ToArray();

        Mat mat = CvInvoke.Imdecode(new VectorOfByte(imageData), ImreadModes.Color);
        if (mat.Empty)
        {
            Console.WriteLine("Error: Image conversion failed!");
        }
        return mat;
    }
}




this is my full code make update to this 

 [HttpPost]
 public IActionResult AttendanceData([FromBody] AttendanceRequest model)
 {
     try
     {
        
         string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/StoredFace.jpg");
         string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Captured.jpg");

         if (!System.IO.File.Exists(storedImagePath) || !System.IO.File.Exists(capturedImagePath))
         {
             return Json(new { success = false, message = "One or both hardcoded images not found!" });
         }

        
         Bitmap storedImage = new Bitmap(storedImagePath);
         Bitmap capturedImage = new Bitmap(capturedImagePath);

       
         bool isFaceMatched = VerifyFace(capturedImage, storedImage);

         if (isFaceMatched)
         {
             return Json(new { success = true, message = "Face Matched! Debugging successful." });
         }
         else
         {
             return Json(new { success = false, message = "Face does not match in hardcoded test!" });
         }
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }


 public class AttendanceRequest
 {
     public string Type { get; set; }
     public string ImageData { get; set; }
 }



 private bool VerifyFace(Bitmap captured, Bitmap stored)
 {
     

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
             
         }
     }
     catch (Exception ex)
     {
         Console.WriteLine("Error in face verification: " + ex.Message);
     }

     return false;
 }



 private Mat BitmapToMat(Bitmap bitmap)
 {
    
     using (MemoryStream ms = new MemoryStream())
     {
         bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
         byte[] imageData = ms.ToArray();

        
         Mat mat = new Mat();
         CvInvoke.Imdecode(new VectorOfByte(imageData), Emgu.CV.CvEnum.ImreadModes.Grayscale, mat);

         return mat;
     }
 }
