this is logic 

  [HttpPost]
  public IActionResult AttendanceData(string EntryType, string ImageData)
  {
      try
      {
          var UserId = HttpContext.Request.Cookies["Session"];
          string Pno = UserId;

          // Convert Base64 Image Data to Byte Array
          byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]);
          using (var ms = new MemoryStream(imageBytes))
          {
              Bitmap capturedImage = new Bitmap(ms);

              // Retrieve the stored image from the database
              var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
              if (user == null || user.Image == null)
              {
                  return Json(new { success = false, message = "User Image Not Found!" });
              }

              // Convert stored image to Bitmap
              using (var storedStream = new MemoryStream(user.Image))
              {
                  Bitmap storedImage = new Bitmap(storedStream);

                  // Perform Face Recognition
                  bool isFaceMatched = VerifyFace(capturedImage, storedImage);

                  if (!isFaceMatched)
                  {
                      return Json(new { success = false, message = "Face does not match!" });
                  }

                  // If face matches, store attendance
                  string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                  string currentTime = DateTime.Now.ToString("HH:mm");

                  if (EntryType == "Punch In")
                  {
                      StoreData(currentDate, currentTime, null, Pno);
                  }
                  else
                  {
                      StoreData(currentDate, null, currentTime, Pno);
                  }

                  return Json(new { success = true, message = "Attendance Marked Successfully!" });
              }
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

          using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
          {
              
              faceRecognizer.Train(new List<Mat> { matStored }, new List<int> { 1 });

              
              var result = faceRecognizer.Predict(matCaptured);

             
              return result.Label == 1 && result.Distance < 80; 
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
      Mat mat = new Mat();
      using (MemoryStream ms = new MemoryStream())
      {
          bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
          mat = CvInvoke.Imdecode(ms.ToArray(), Emgu.CV.CvEnum.ImreadModes.Color);
      }
      return mat;
  }

please provide success logic and proper logic 
