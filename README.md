  [HttpGet]
  public IActionResult TestFaceVerification()
  {
      try
      {

          string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/151514-Shashi Kumar.jpg");
          string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/159445-Adwine Keshav Jha.jpg");


          if (!System.IO.File.Exists(storedImagePath) || !System.IO.File.Exists(capturedImagePath))
          {
              return Json(new { success = false, message = "One or both test images not found!" });
          }

          bool isFaceMatched;


          using (Bitmap storedImage = new Bitmap(storedImagePath))
          using (Bitmap capturedImage = new Bitmap(capturedImagePath))
          {
              isFaceMatched = VerifyFace(capturedImage, storedImage);
          }

          if (isFaceMatched)
          {
              return Json(new { success = true, message = "Faces match successfully!" });
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

        return distance < 0.8; 
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}


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

    Array detectionArray = detections.GetData(); 
    float[] detectionData = detectionArray.Cast<float>().ToArray(); 

    int numDetections = detections.SizeOfDimension[2];

    for (int i = 0; i < numDetections; i++)
    {
        float confidence = detectionData[i * 7 + 2]; 

        if (confidence > 0.5) 
        {
            int x1 = (int)(detectionData[i * 7 + 3] * image.Width);
            int y1 = (int)(detectionData[i * 7 + 4] * image.Height);
            int x2 = (int)(detectionData[i * 7 + 5] * image.Width);
            int y2 = (int)(detectionData[i * 7 + 6] * image.Height);

            Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            return new Mat(image, faceRect);
        }
    }

    return null;
}


private float[] GetFaceEmbedding(Mat face)
{
    string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/nn4.small2.v1.t7");
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

in this code when i use TestFaceVerification() it works fine but when i use this method AttendanceData() then it works for only one Pno that is 159445-Adwine Keshav Jha. why? is there any issue in post method of this it gets the live picture of user and then compare faces.





private static DlibDotNet.Array2D<DlibDotNet.RgbPixel> BitmapToDlibImage(Bitmap bmp)
{
    var array = new DlibDotNet.Array2D<DlibDotNet.RgbPixel>(bmp.Height, bmp.Width);
    for (int y = 0; y < bmp.Height; y++)
    {
        for (int x = 0; x < bmp.Width; x++)
        {
            var color = bmp.GetPixel(x, y);
            array[y, x] = new DlibDotNet.RgbPixel
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
        }
    }
    return array;
}



Argument 1: cannot convert from 'uint' to 'DlibDotNet.MatrixTemplateSizeParameter'

private static DlibDotNet.Matrix<DlibDotNet.RgbPixel> BitmapToDlibImage(Bitmap bmp)
{
    var mat = new DlibDotNet.Matrix<DlibDotNet.RgbPixel>((uint)bmp.Height, (uint)bmp.Width);
    for (int y = 0; y < bmp.Height; y++)
    {
        for (int x = 0; x < bmp.Width; x++)
        {
            var color = bmp.GetPixel(x, y);
            mat[y, x] = new DlibDotNet.RgbPixel { Blue = color.B, Green = color.G, Red = color.R };
        }
    }
    return mat;
}



this is my full code 
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

     private bool VerifyFace(Bitmap capturedBitmap, Bitmap storedBitmap)
     {
         try
         {
             string shapePredictorPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Dlib/shape_predictor_5_face_landmarks.dat");
             string faceRecognitionModelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Dlib/dlib_face_recognition_resnet_model_v1.dat");

             using (var shapePredictor = ShapePredictor.Deserialize(shapePredictorPath))
             using (var faceRecognizer = DlibDotNet.Dnn.LossMetric.Deserialize(faceRecognitionModelPath))
             using (var detector = Dlib.GetFrontalFaceDetector())
             {
                 // Convert Bitmap to Dlib image
                 using (var capturedImg = BitmapToDlibImage(capturedBitmap))
                 using (var storedImg = BitmapToDlibImage(storedBitmap))
                 {
                     var capturedFaces = detector.Operator(capturedImg);
                     var storedFaces = detector.Operator(storedImg);

                     if (capturedFaces.Length == 0 || storedFaces.Length == 0)
                     {
                         Console.WriteLine("No face detected in one or both images.");
                         return false;
                     }

                     var capturedShape = shapePredictor.Detect(capturedImg, capturedFaces[0]);
                     var storedShape = shapePredictor.Detect(storedImg, storedFaces[0]);

                     var capturedFaceChip = Dlib.ExtractImageChip<RgbPixel>(capturedImg, Dlib.GetFaceChipDetails(capturedShape, 150, 0.25));
                     var storedFaceChip = Dlib.ExtractImageChip<RgbPixel>(storedImg, Dlib.GetFaceChipDetails(storedShape, 150, 0.25));

                     var capturedDescriptor = faceRecognizer.Operator(capturedFaceChip)[0];
                     var storedDescriptor = faceRecognizer.Operator(storedFaceChip)[0];

                     // Calculate Euclidean distance
                     double distance = Dlib.Length(capturedDescriptor - storedDescriptor);

                     Console.WriteLine($"Face distance: {distance}");

                     return distance < 0.6; // Typical threshold for good match
                 }
             }
         }
         catch (Exception ex)
         {
             Console.WriteLine("Error in Dlib face verification: " + ex.Message);
             return false;
         }
     }

     private static Matrix<RgbPixel> BitmapToDlibImage(Bitmap bmp)
     {
         var mat = new Matrix<RgbPixel>((uint)bmp.Height, (uint)bmp.Width);
         for (int y = 0; y < bmp.Height; y++)
         {
             for (int x = 0; x < bmp.Width; x++)
             {
                 var color = bmp.GetPixel(x, y);
                 mat[y, x] = new RgbPixel { Blue = color.B, Green = color.G, Red = color.R };
             }
         }
         return mat;
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

getting this error 'Matrix<>' is an ambiguous reference between 'Emgu.CV.Matrix<TDepth>' and 'DlibDotNet.Matrix<TElement>'
