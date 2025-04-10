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

        // This is the captured image that will be used for comparison — KEEP THIS
        string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");

        // Save captured image for comparison (this stays and is reused/updated)
        SaveBase64ImageToFile(model.ImageData, capturedImagePath);

        bool isFaceMatched = false;

        using (Bitmap capturedImage = new Bitmap(capturedImagePath))
        using (Bitmap storedImage = new Bitmap(storedImagePath))
        {
            isFaceMatched = VerifyFace(capturedImage, storedImage);
        }

        if (isFaceMatched)
        {
            string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
            string currentTime = DateTime.Now.ToString("HH:mm");

            // On Punch In, also save/update the captured image to a permanent file
            if (model.Type == "Punch In")
            {
                // Save/update the permanent PunchIn image
                string punchInImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-PunchIn.jpg");
                System.IO.File.Copy(capturedImagePath, punchInImagePath, true);

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


         //System.IO.File.Delete(capturedImagePath);

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


in this iwant only that temporary capturedImage that is for comparision don't delete it . stored that when PunchIn and again if the captured the image then update that image
