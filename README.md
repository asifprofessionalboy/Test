this is my controller method 


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

                
                string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Temp-{DateTime.Now.Ticks}.jpg");
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

       
       

        fetch("/TSUISLARS/Geo/AttendanceData", {
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

i am getting this error 
An error occurred while processing your request.
