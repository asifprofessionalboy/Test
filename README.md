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

        using (var db = new YourDbContext())
        {
            DateTime today = DateTime.Today;

            var record = db.AppFaceVerificationDetails
                .FirstOrDefault(x => x.Pno == Pno && x.DateAndTime.Value.Date == today);

            if (record == null)
            {
                record = new AppFaceVerificationDetail
                {
                    Id = Guid.NewGuid(),
                    Pno = Pno,
                    DateAndTime = DateTime.Now,
                    PunchInFailedCount = 0,
                    PunchOutFailedCount = 0,
                    PunchInSuccess = false,
                    PunchOutSuccess = false
                };
                db.AppFaceVerificationDetails.Add(record);
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

                db.SaveChanges();
                return Json(new { success = true, message = "Attendance recorded successfully." });
            }
            else
            {
                if (model.Type == "Punch In")
                    record.PunchInFailedCount = (record.PunchInFailedCount ?? 0) + 1;
                else
                    record.PunchOutFailedCount = (record.PunchOutFailedCount ?? 0) + 1;

                db.SaveChanges();
                return Json(new { success = false, message = "Face does not match!" });
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}





i have this two method 


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

                if (isFaceMatched)
                {
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (model.Type == "Punch In")
                    {
                        
                        string newCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");
                        SaveBase64ImageToFile(model.ImageData, newCapturedPath);

                        StoreData(currentDate, currentTime, null, Pno);
                    }
                    else
                    {
                        StoreData(currentDate, null, currentTime, Pno);
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


                using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 98))
                {
                    CvInvoke.EqualizeHist(capturedFace, capturedFace);
                    CvInvoke.EqualizeHist(storedFace, storedFace);

                    VectorOfMat trainingImages = new VectorOfMat();
                    trainingImages.Push(storedFace);
                    VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                    faceRecognizer.Train(trainingImages, labels);
                    var result = faceRecognizer.Predict(capturedFace);

                    Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                    return result.Label == 1 && result.Distance <= 98;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in face verification: " + ex.Message);
                return false;
            }
        }


and this is my model to count details 
public partial class AppFaceVerificationDetail
{
    public Guid Id { get; set; }
    public string? Pno { get; set; }
    public DateTime? DateAndTime { get; set; }
    public int? PunchInFailedCount { get; set; }
    public bool? PunchInSuccess { get; set; }
    public int? PunchOutFailedCount { get; set; }
    public bool? PunchOutSuccess { get; set; }
}

i want that if face not recognized then count no. of failed attempt and if success then store true on PunchIn success and Punchout success 
