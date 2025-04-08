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
