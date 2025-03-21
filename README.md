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

        // Load stored image from the directory
        string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-{Name}.jpg");
        if (!System.IO.File.Exists(storedImagePath))
        {
            return Json(new { success = false, message = "Stored image not found!" });
        }

        // Convert Base64 ImageData to Bitmap
        Bitmap capturedImage = ConvertBase64ToBitmap(model.ImageData);
        if (capturedImage == null)
        {
            return Json(new { success = false, message = "Invalid image format!" });
        }

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

// Convert Base64 image string to Bitmap
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



this is my Action. in this i dont want to save CapturedImage in Image folder. capture image only compares with Stored Image that is in wwwroot/Images that it.
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

         
         string capturedImagePath = SaveBase64Image(model.ImageData, Pno,Name);
         if (string.IsNullOrEmpty(capturedImagePath))
         {
             return Json(new { success = false, message = "Failed to save captured image!" });
         }

        
         using (Bitmap storedImage = new Bitmap(storedImagePath))
         using (Bitmap capturedImage = new Bitmap(capturedImagePath))
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

private string SaveBase64Image(string base64String, string Pno,string Name)
{
    try
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-{Name}.jpg");
        byte[] imageBytes = Convert.FromBase64String(base64String.Split(',')[1]);
        System.IO.File.WriteAllBytes(filePath, imageBytes);
        return filePath;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error saving image: " + ex.Message);
        return null;
    }
}
this is my js 
 <script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");

    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

    function captureImageAndSubmit(entryType) {
        EntryTypeInput.value = entryType;

        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageData = canvas.toDataURL("image/jpeg"); // Save as JPG

       

        fetch("/GFAS/Geo/AttendanceData", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Type: entryType,
                ImageData: imageData
            })
        })
        .then(response => response.json())
        .then(data => {
            alert(data.message);
        })
        .catch(error => {
            console.error("Error:", error);
            alert("An error occurred while submitting the image.");
        });
    }
</script>
