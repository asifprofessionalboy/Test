in this i want that if face matched then the captured image will stored on my images folder in wwwroot when PunchIn 

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
