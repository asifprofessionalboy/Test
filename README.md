in this logic i am using a hardcoded image for testing , is this working or not ?
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

        //string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-{Name}.jpg");
        // string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");

         string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Shashi Kumar.jpg");
         string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Shashi Kumar.jpg");

         if (!System.IO.File.Exists(storedImagePath) && !System.IO.File.Exists(lastCapturedPath))
         {
             return Json(new { success = false, message = "No reference image found to verify face!" });
         }

         string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Shashi Kumar.jpg");
        
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

         //System.IO.File.Delete(tempCapturedPath);

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
