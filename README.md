this is my face recognition attendance system logic 
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

             return result.Label == 1 && result.Distance <= 95;
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


in this it sometimes give false cases like it matches with same user with other person , i want to make it more accurate and more reliable and also matches with only with the same person with stored images
