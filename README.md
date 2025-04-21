when verifying Otp getting null of model.pno , storedOtp

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
