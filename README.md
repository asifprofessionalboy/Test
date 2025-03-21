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

        // Generate a unique file name for the captured image
        string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");

        // Save the base64 image to a file
        SaveBase64ImageToFile(model.ImageData, capturedImagePath);

        bool isFaceMatched = false;

        // Load images inside a using block to ensure proper disposal
        using (Bitmap capturedImage = new Bitmap(capturedImagePath))
        using (Bitmap storedImage = new Bitmap(storedImagePath))
        {
            isFaceMatched = VerifyFace(capturedImage, storedImage);
        }

        // Now the files are released, so we can safely delete the captured image
        System.IO.File.Delete(capturedImagePath);

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
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
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

        // Generate a unique file name for the captured image
        string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");

        // Save the base64 image to a file
        SaveBase64ImageToFile(model.ImageData, capturedImagePath);

        // Compare both images
        using (Bitmap capturedImage = new Bitmap(capturedImagePath))
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

                // Delete the captured image after comparison (optional)
                System.IO.File.Delete(capturedImagePath);

                return Json(new { success = true, message = "Attendance recorded successfully." });
            }
            else
            {
                // Delete the captured image after comparison (optional)
                System.IO.File.Delete(capturedImagePath);

                return Json(new { success = false, message = "Face does not match!" });
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

private void SaveBase64ImageToFile(string base64String, string filePath)
{
    try
    {
        byte[] imageBytes = Convert.FromBase64String(base64String.Split(',')[1]);
        System.IO.File.WriteAllBytes(filePath, imageBytes);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error saving Base64 image to file: " + ex.Message);
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
