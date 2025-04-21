[HttpPost]
public IActionResult ResendOtp()
{
    var userId = HttpContext.Request.Cookies["Session"];
    if (string.IsNullOrEmpty(userId))
        return Json(new { success = false, message = "User session not found!" });

    string otp = new Random().Next(100000, 999999).ToString();
    UserOtpMap[userId] = otp;

    SendSmsToUser(userId, otp);

    return Json(new { success = true, message = "OTP resent successfully." });
}
<div class="modal fade" id="otpModal" tabindex="-1" aria-labelledby="otpModalLabel" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="otpModalLabel">Enter OTP</h5>
      </div>
      <div class="modal-body">
        <input type="text" class="form-control" id="otpInput" placeholder="Enter OTP">
        <p class="mt-2" id="timer"></p>
        <button class="btn btn-link p-0 mt-2" id="resendBtn" onclick="resendOtp()" style="display: none;">Resend OTP</button>
      </div>
      <div class="modal-footer">
        <button class="btn btn-primary" onclick="submitOtp()">Verify</button>
      </div>
    </div>
  </div>
</div>

function startOTPTimer() {
    let timeLeft = 120;
    const timerLabel = document.getElementById("timer");
    const resendBtn = document.getElementById("resendBtn");
    resendBtn.style.display = "none";

    otpInterval = setInterval(() => {
        let mins = Math.floor(timeLeft / 60);
        let secs = timeLeft % 60;
        timerLabel.innerText = `OTP expires in ${mins}:${secs.toString().padStart(2, '0')}`;
        timeLeft--;

        if (timeLeft < 0) {
            clearInterval(otpInterval);
            timerLabel.innerText = "OTP expired.";
            resendBtn.style.display = "block";
        }
    }, 1000);
}

function resendOtp() {
    fetch("/Geo/ResendOtp", {
        method: "POST"
    })
    .then(res => res.json())
    .then(data => {
        if (data.success) {
            Swal.fire({
                title: "OTP Resent!",
                icon: "info",
                timer: 2000,
                showConfirmButton: false
            });
            clearOtpModal();
            startOTPTimer(); // Restart timer
        } else {
            Swal.fire("Error", data.message || "Could not resend OTP", "error");
        }
    });
}


when verifying Otp getting null of model.pno , storedOtp

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
                //string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");

                string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"159445-Adwine Keshav Jha.jpg");
                string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"159445-Adwine Keshav Jha.jpg");

                if (!System.IO.File.Exists(storedImagePath) && !System.IO.File.Exists(lastCapturedPath))
                {
                    return Json(new { success = false, message = "No reference image found to verify face!" });
                }

                string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"155478-Pramod Kumar Bhanbheru.jpg");
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

                if (isFaceMatched)
                {
                    FaceRetryCount[Pno] = 0; 
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (model.Type == "Punch In")
                    {
                        string newCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");
                        SaveBase64ImageToFile(model.ImageData, newCapturedPath);

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
                    if (!FaceRetryCount.ContainsKey(Pno))
                        FaceRetryCount[Pno] = 1;
                    else
                        FaceRetryCount[Pno]++;

                    if (FaceRetryCount[Pno] >= 3)
                    {
                        FaceRetryCount[Pno] = 0;
                        string otp = new Random().Next(100000, 999999).ToString();
                        UserOtpMap[Pno] = otp;

                        SendSmsToUser(UserId, otp); 

                        return Json(new { success = false, otpRequired = true, message = "Face not matched. OTP has been sent." });
                    }

                    return Json(new { success = false, message = "Face does not match!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

       

        private void SendSmsToUser(string userContact, string otp)
        {
            try
            {
                string connectionString = GetRFIDConnectionString();

                string query = @"
        SELECT Phone
        FROM UserLoginDB.dbo.App_EmployeeMaster
        WHERE Pno = @pno ";

                string phoneNumber = "";

                using (var connection = new SqlConnection(connectionString))
                {
                    phoneNumber = connection.QuerySingleOrDefault<string>(query, new { pno = userContact });
                }

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    Console.WriteLine("Phone number not found for user.");
                    return;
                }


                string message = $"Your OTP for attendance is {otp}.valid for 2 min. -Tata Steel UISL (JUSCO)";
                string smsUrl = $"https://enterprise.smsgupshup.com/GatewayAPI/rest?method=SendMessage&send_to={phoneNumber}&msg={Uri.EscapeDataString(message)}&msg_type=TEXT&userid=2000060285&auth_scheme=plain&password=jusco&v=1.1&format=text";

                WebRequest request = WebRequest.Create(smsUrl);
                request.Proxy = WebRequest.DefaultWebProxy;
                request.UseDefaultCredentials = true;
                request.Proxy.Credentials = new NetworkCredential("###", "###");

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine("SMS Sent: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending SMS: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult VerifyOtp([FromBody] OtpRequest model)
        {
            if (UserOtpMap.TryGetValue(model.Pno, out string storedOtp) && model.Otp == storedOtp)
            {
                UserOtpMap.Remove(model.Pno);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid or expired OTP." });
        }
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
                Swal.close(); // Close loading

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

                } else if (data.otpRequired) {
                    errorSound.play();
                    triggerHapticFeedback("error");

                    disablePunchButtons();
                    startOTPTimer();

                    const otpModal = new bootstrap.Modal(document.getElementById('otpModal'));
                    otpModal.show();
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


    function disablePunchButtons() {
        document.getElementById("PunchIn")?.setAttribute("disabled", true);
        document.getElementById("PunchOut")?.setAttribute("disabled", true);
    }

    function enablePunchButtons() {
        document.getElementById("PunchIn")?.removeAttribute("disabled");
        document.getElementById("PunchOut")?.removeAttribute("disabled");
    }

    let otpInterval;
    function startOTPTimer() {
        let timeLeft = 120;
        const timerLabel = document.getElementById("timer");

        otpInterval = setInterval(() => {
            let mins = Math.floor(timeLeft / 60);
            let secs = timeLeft % 60;
            timerLabel.innerText = `OTP expires in ${mins}:${secs.toString().padStart(2, '0')}`;
            timeLeft--;

            if (timeLeft < 0) {
                clearInterval(otpInterval);
                timerLabel.innerText = "OTP expired. Please try again.";
                disablePunchButtons();
            }
        }, 1000);
    }

    function submitOtp() {
        const otp = document.getElementById("otpInput").value;

        fetch("/Geo/VerifyOtp", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ otp })
        })
            .then(res => res.json())
            .then(result => {
                if (result.success) {
                    Swal.fire({
                        title: "OTP Verified!",
                        icon: "success",
                        timer: 2000,
                        showConfirmButton: false
                    });

                    const modal = bootstrap.Modal.getInstance(document.getElementById('otpModal'));
                    modal.hide();
                    clearOtpModal();
                    enablePunchButtons();
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Invalid OTP",
                        text: result.message || "Please try again."
                    });
                }
            });
    }

    function clearOtpModal() {
        document.getElementById("otpInput").value = "";
        document.getElementById("timer").innerText = "";
        clearInterval(otpInterval);
    }
</script>

and if time is finished then and otp is not verified then resend option of send sms again 
