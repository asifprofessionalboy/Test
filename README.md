now i  have to store no. of failed count , previous i am storing like this in my backend when i am using face recognition in my backend now i am using client side face matching. now i want to store failed count of punchIn and PunchOut and store like this in my table 

this is the code 
//DateTime today = DateTime.Today;

             //var record = context.AppFaceVerificationDetails
             //    .FirstOrDefault(x => x.Pno == Pno && x.DateAndTime.Value.Date == today);

             //if (record == null)
             //{
             //    record = new AppFaceVerificationDetail
             //    {
             //        Pno = Pno,
             //        PunchInFailedCount = 0,
             //        PunchOutFailedCount = 0,
             //        PunchInSuccess = false,
             //        PunchOutSuccess = false
             //    };
             //    context.AppFaceVerificationDetails.Add(record);
             //}

and this is full code and you already know my full js

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

         bool isFaceMatched = true;


         string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
         string currentTime = DateTime.Now.ToString("HH:mm");

        
             //DateTime today = DateTime.Today;

             //var record = context.AppFaceVerificationDetails
             //    .FirstOrDefault(x => x.Pno == Pno && x.DateAndTime.Value.Date == today);

             //if (record == null)
             //{
             //    record = new AppFaceVerificationDetail
             //    {
             //        Pno = Pno,
             //        PunchInFailedCount = 0,
             //        PunchOutFailedCount = 0,
             //        PunchInSuccess = false,
             //        PunchOutSuccess = false
             //    };
             //    context.AppFaceVerificationDetails.Add(record);
             //}
        
         if (isFaceMatched)
         {
             if (model.Type == "Punch In")
             {
                 string newCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");
                 SaveBase64ImageToFile(model.ImageData, newCapturedPath);

                 StoreData(currentDate, currentTime, null, Pno);
                 //record.PunchInSuccess = true;
             }
             else
             {
                 StoreData(currentDate, null, currentTime, Pno);
                 //record.PunchOutSuccess = true;
             }

             context.SaveChanges();
             return Json(new { success = true, message = "Attendance recorded successfully." });
         }

         

         return Json(new { success = false, message = "Face verification failed." }); 
     
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }
