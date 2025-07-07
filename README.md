now i am finally get the model ,modelnew2_onnx.onnx.
now i have this current code please make changes that is necessary

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

               //string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Shashi Kumar.jpg");
               //string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Shashi Kumar.jpg");

               if (!System.IO.File.Exists(storedImagePath) && !System.IO.File.Exists(lastCapturedPath))
               {
                   return Json(new { success = false, message = "No reference image found to verify face!" });
               }

               //string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"159445-Adwine Keshav Jha.jpg");
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
         var face1 = ExtractFaceEmbedding(captured);
         var face2 = ExtractFaceEmbedding(stored);

         if (face1 == null || face2 == null) return false;

         double distance = CalculateEuclideanDistance(face1, face2);

         Console.WriteLine($"Face distance: {distance}");

         return distance < 0.8;
     }
     catch
     {
         return false;
     }
 }

 private float[] ExtractFaceEmbedding(Bitmap image)
 {
     string protoPath = Path.Combine("wwwroot", "Cascades", "deploy.prototxt");
     string modelPath = Path.Combine("wwwroot", "Cascades", "res10_300x300_ssd_iter_140000_fp16.caffemodel");
     string faceNetPath = Path.Combine("wwwroot", "Cascades", "facenet.onnx");

     var mat = BitmapToMat(image);
     var net = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);
     var blob = DnnInvoke.BlobFromImage(mat, 1.0, new Size(300, 300), new MCvScalar(104, 177, 123));
     net.SetInput(blob);

     Mat detection = net.Forward();
     float[,,,] data = (float[,,,])detection.GetData();

     for (int i = 0; i < data.GetLength(2); i++)
     {
         float confidence = data[0, 0, i, 2];

         if (confidence > 0.6f)
         {
             int x1 = (int)(data[0, 0, i, 3] * mat.Cols);
             int y1 = (int)(data[0, 0, i, 4] * mat.Rows);
             int x2 = (int)(data[0, 0, i, 5] * mat.Cols);
             int y2 = (int)(data[0, 0, i, 6] * mat.Rows);

             Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
             Mat face = new Mat(mat, faceRect);
             CvInvoke.Resize(face, face, new Size(368, 368));
             return RunFaceNetEmbedding(face, faceNetPath);
         }
     }

     return null;
 }
 public static Bitmap MatToBitmap(Mat mat)
 {
     using (VectorOfByte vb = new VectorOfByte())
     {
         CvInvoke.Imencode(".bmp", mat, vb);
         using (MemoryStream ms = new MemoryStream(vb.ToArray()))
         {
             return new Bitmap(ms);
         }
     }
 }
 private float[] RunFaceNetEmbedding(Mat faceMat, string modelPath)
 {
     Bitmap resizedBmp = MatToBitmap(faceMat);
     var input = new DenseTensor<float>(new[] { 1, 3, 368, 368 });

     for (int y = 0; y < 368; y++)
     {
         for (int x = 0; x < 368; x++)
         {
             var pixel = resizedBmp.GetPixel(x, y);
             input[0, 0, y, x] = (pixel.R / 127.5f) - 1.0f;
             input[0, 1, y, x] = (pixel.G / 127.5f) - 1.0f;
             input[0, 2, y, x] = (pixel.B / 127.5f) - 1.0f;
         }
     }

     var inputs = new List<NamedOnnxValue>
 {
     NamedOnnxValue.CreateFromTensor("input", input)
 };

     using var session = new InferenceSession(modelPath);
     using var results = session.Run(inputs);

     return results.First().AsEnumerable<float>().ToArray();
 }

 private double CalculateEuclideanDistance(float[] emb1, float[] emb2)
 {
     double sum = 0;
     for (int i = 0; i < emb1.Length; i++)
         sum += Math.Pow(emb1[i] - emb2[i], 2);
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
