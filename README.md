i have this method in this 

 
[HttpPost]
 public IActionResult AttendanceData([FromBody] AttendanceRequest model)
 {
     if (string.IsNullOrEmpty(model.ImageData))
     {
         return Json(new { success = false, message = "Image data is missing!" });
     }

     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];
         string Pno = UserId;

         byte[] imageBytes = Convert.FromBase64String(model.ImageData.Split(',')[1]);

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

                 bool isPartialMatch;
                 bool isFaceMatched = VerifyFace(capturedImage, storedImage, out isPartialMatch);

                 if (isFaceMatched)
                 {
                     string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                     string currentTime = DateTime.Now.ToString("HH:mm");

                     if (model.Type == "Punch In")
                     {
                         StoreData(currentDate, currentTime, null, Pno);
                     }
                     else
                     {
                         StoreData(currentDate, null, currentTime, Pno);
                     }

                     return Json(new { success = true, message = "Attendance Marked Successfully!" });
                 }
                 else
                 {
                     return Json(new { success = false, message = "Face does not match!" });
                 }
             }
         }
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }

i want two images as hardcode for debuggingg
 string storedImagePath = "wwwroot/Images/stored.jpg";
string capturedImagePath = "wwwroot/Images/Captured.jpg";
