    [HttpPost]
    public IActionResult AttendanceData(string Type, string ImageData)
    {
        if (string.IsNullOrEmpty(ImageData))
        {
            return Json(new { success = false, message = "Image data is missing!" });
        }

        try
        {
            var UserId = HttpContext.Request.Cookies["Session"];
            string Pno = UserId;

           
            byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]);

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

                  
                    bool isFaceMatched = VerifyFace(capturedImage, storedImage);

                    if (!isFaceMatched)
                    {
                        return Json(new { success = false, message = "Face does not match!" });
                    }

                   
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (Type == "Punch In")
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

            
            string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Cascades", "haarcascade_frontalface_default.xml");
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

                return result.Label == 1 && result.Distance < 50;  
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
            CvInvoke.Imdecode(new VectorOfByte(imageData), Emgu.CV.CvEnum.ImreadModes.Grayscale, mat);

            return mat;
        }
    }

after implementing this, not working . it everytime shows face doesnot match. why. please find the problem and show as an alert. if 50% face is matching then shows matched 
