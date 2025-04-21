<div class="modal fade" id="otpModal" tabindex="-1" aria-labelledby="otpModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">

            <div class="modal-header">
                <h5 class="modal-title" id="otpModalLabel">OTP Verification</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" onclick="clearOtpModal()"></button>
            </div>

            <div class="modal-body">
                <p>An OTP has been sent to your registered mobile number. Please enter it below:</p>
                <input type="text" id="otpInput" maxlength="6" class="form-control" placeholder="Enter 6-digit OTP" />
                <div id="timer" class="mt-2 text-danger fw-bold text-center"></div>
                <button class="btn btn-link p-0 mt-2" id="resendBtn" onclick="resendOtp()" style="display: none;">Resend OTP</button>

            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="submitOtp()">Submit OTP</button>
            </div>

        </div>
    </div>
</div>

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
                Swal.close(); 

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


    function clearOtpModal() {
        document.getElementById("otpInput").value = "";
        document.getElementById("timer").innerText = "";
        clearInterval(otpInterval);
    }

    

    let otpInterval;
    function startOTPTimer() {
        let timeLeft = 60;
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
                        title: "OTP Verified!Attendance Recorded Successfully",
                        icon: "success",
                        timer: 2000,
                        showConfirmButton: false
                    });

                    const modal = bootstrap.Modal.getInstance(document.getElementById('otpModal'));
                    modal.hide();
                    clearOtpModal();
                   
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Invalid OTP",
                        text: result.message || "Please try again."
                    });
                }
            });
    }


    function resendOtp() {
        fetch("/Geo/ResendOtp", {
            method: "POST"
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    alert("Ohh yes");
                    Swal.fire({
                        title: "OTP Resent!",
                        icon: "info",
                        timer: 1000,
                        showConfirmButton: false
                    });
                    clearOtpModal();
                    startOTPTimer(); // Restart timer
                } else {
                    Swal.fire("Error", data.message || "Could not resend OTP", "error");
                }
            });
    }
</script>

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



in this when i resend the otp modal is not opening for again entering otp and  it shows 
 This page isn’t working
If the problem continues, contact the site owner.
HTTP ERROR 415
