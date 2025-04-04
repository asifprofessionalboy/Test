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

// 🔹 NEW Improved Face Matching Function
private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        Mat capturedFace = DetectFace(matCaptured);
        Mat storedFace = DetectFace(matStored);

        if (capturedFace == null || storedFace == null)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        float[] capturedEmbedding = GetFaceEmbedding(capturedFace);
        float[] storedEmbedding = GetFaceEmbedding(storedFace);

        double distance = CalculateEuclideanDistance(capturedEmbedding, storedEmbedding);
        Console.WriteLine($"Euclidean Distance: {distance}");

        return distance < 0.6; // ✅ If distance < 0.6, consider it the same person
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}

// 🔹 Detect Face Using Deep Learning Model (Better than Haarcascade)
private Mat DetectFace(Mat image)
{
    string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/res10_300x300_ssd_iter_140000_fp16.caffemodel");
    string protoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/deploy.prototxt");

    if (!System.IO.File.Exists(modelPath) || !System.IO.File.Exists(protoPath))
    {
        Console.WriteLine("Error: Face detection model files not found!");
        return null;
    }

    Net faceNet = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);
    Mat blob = DnnInvoke.BlobFromImage(image, 1.0, new Size(300, 300), new MCvScalar(104, 177, 123));

    faceNet.SetInput(blob);
    Mat detections = faceNet.Forward();

    for (int i = 0; i < detections.SizeOfDimension[2]; i++)
    {
        float confidence = detections.GetData(new[] { 0, 0, i, 2 })[0];

        if (confidence > 0.5) // If confidence > 50%
        {
            int x1 = (int)(detections.GetData(new[] { 0, 0, i, 3 })[0] * image.Width);
            int y1 = (int)(detections.GetData(new[] { 0, 0, i, 4 })[0] * image.Height);
            int x2 = (int)(detections.GetData(new[] { 0, 0, i, 5 })[0] * image.Width);
            int y2 = (int)(detections.GetData(new[] { 0, 0, i, 6 })[0] * image.Height);

            Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            return new Mat(image, faceRect);
        }
    }

    return null;
}

// 🔹 Get Face Embeddings (Feature Extraction)
private float[] GetFaceEmbedding(Mat face)
{
    string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/openface.nn4.small2.v1.t7");
    if (!System.IO.File.Exists(modelPath))
    {
        Console.WriteLine("Error: Face recognition model file not found!");
        return null;
    }

    Net faceRecognizer = DnnInvoke.ReadNetFromTorch(modelPath);
    Mat blob = DnnInvoke.BlobFromImage(face, 1.0 / 255, new Size(96, 96), new MCvScalar(0, 0, 0), true, false);
    faceRecognizer.SetInput(blob);
    Mat output = faceRecognizer.Forward();

    return output.GetData().Cast<float>().ToArray();
}

// 🔹 Calculate Euclidean Distance Between Two Face Embeddings
private double CalculateEuclideanDistance(float[] vec1, float[] vec2)
{
    if (vec1.Length != vec2.Length) return double.MaxValue;

    double sum = 0;
    for (int i = 0; i < vec1.Length; i++)
    {
        sum += Math.Pow(vec1[i] - vec2[i], 2);
    }

    return Math.Sqrt(sum);
}

// 🔹 Convert Bitmap to Mat
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

             return result.Label == 1 && result.Distance <= 100;
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
