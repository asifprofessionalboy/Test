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
        Bitmap capturedImage = ConvertBase64ToBitmap(model.ImageData);
        if (capturedImage == null)
        {
            return Json(new { success = false, message = "Invalid image format!" });
        }


        //using (Bitmap capturedImage = new Bitmap(imagePath2))
        using (Bitmap storedImage = new Bitmap(storedImagePath))
        {
            bool isFaceMatched = VerifyFace(capturedImage, storedImage);
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

                return Json(new { success = true, message = "Attendance recorded successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Face does not match!" });
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}


private Bitmap ConvertBase64ToBitmap(string base64String)
{
    try
    {
        byte[] imageBytes = Convert.FromBase64String(base64String.Split(',')[1]);
        using (MemoryStream ms = new MemoryStream(imageBytes))
        {
            return new Bitmap(ms);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error converting Base64 to Bitmap: " + ex.Message);
        return null;
    }
}


in this method i dont want ConvertBase64ToBitmap i want same like this //using (Bitmap capturedImage = new Bitmap(imagePath2)) for model.ImageData . just like hardcoded i have two jpg image that is matching. in this i want same capture image is also jpg and stored image then compare 
