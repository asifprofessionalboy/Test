private void SaveBase64ImageToFile(string base64String, string filePath)
{
    try
    {
        if (string.IsNullOrWhiteSpace(base64String) || !base64String.Contains(","))
            throw new ArgumentException("Invalid base64 string");

        string base64Data = base64String.Split(',')[1];

        if (string.IsNullOrWhiteSpace(base64Data))
            throw new ArgumentException("Base64 image data is empty");

        byte[] imageBytes = Convert.FromBase64String(base64Data);

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
        throw; // Optional: rethrow if you want to see it in the calling method
    }
}
Console.WriteLine("Base64 image starts with: " + base64String.Substring(0, Math.Min(50, base64String.Length)));

console.log("ImageData length:", imageData.length);
console.log("ImageData preview:", imageData.substring(0, 100));



this is my js
<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");

    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            let video = document.getElementById("video");
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

        
        Swal.fire({
            title: "Verifying Face...",
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

       
       

        fetch("/Geo/AttendanceData", {
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
                if (data.success) {
                    var now = new Date();
                    var formattedDateTime = now.toLocaleString();
                    successSound.play();
                    triggerHapticFeedback("success");

                    Swal.fire({
                        title: "Face Matched!",
                        text: "Attendance Recorded.\nDate & Time: " + formattedDateTime,
                        icon: "success",
                        timer: 3000,
                        showConfirmButton: false
                    }).then(() => {
                        location.reload();  
                    }); 

                } else {
                    errorSound.play();
                    triggerHapticFeedback("error");
                    var now = new Date();
                    var formattedDateTime = now.toLocaleString();
                    Swal.fire({
                        title: "Face Not Recognized.",
                        text: "Click the button again to retry.\nDate & Time: " + formattedDateTime,
                        icon: "error",
                        confirmButtonText: "Retry"
                    });
                }
            })
            .catch(error => {
                console.error("Error:", error);
                triggerHapticFeedback("error");

                Swal.fire({
                    title: "Error!",
                    text: "An error occurred while processing your request.",
                    icon: "error"
                });
            });
            
    }

    function triggerHapticFeedback(type) {
        if ("vibrate" in navigator) {
            if (type === "success") {
                navigator.vibrate(100); 
            } else if (type === "error") {
                navigator.vibrate([200, 100, 200]); 
            }
        }
    }
</script>

this is my controller side 
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

         string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"151514-Captured.jpg");
        
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

base64String in this parameter i am getting this data:, and getting this error 
Parameter is not valid.
