using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class AttendanceRequest
{
    public string ImageData { get; set; }
    public string Type { get; set; }
}

public class OtpRequest
{
    public string Pno { get; set; }
    public string Otp { get; set; }
}

public class GeoController : Controller
{
    private static Dictionary<string, int> FaceRetryCount = new();
    private static Dictionary<string, string> UserOtpMap = new();

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

            string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");
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
                FaceRetryCount[Pno] = 0; // reset retry count
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

                    SendSmsToUser(UserName, otp); // You can modify this to use mobile/email

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

    private void SendSmsToUser(string userContact, string otp)
    {
        // Replace this with actual SMS gateway integration (Twilio, Msg91, etc.)
        Console.WriteLine($"Send OTP {otp} to user at {userContact}");
    }

    private bool VerifyFace(Bitmap captured, Bitmap stored)
    {
        try
        {
            Mat matCaptured = BitmapToMat(captured);
            Mat matStored = BitmapToMat(stored);

            CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

            string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
            if (!System.IO.File.Exists(cascadePath))
                return false;

            CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
            Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
            Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);

            if (capturedFaces.Length == 0 || storedFaces.Length == 0)
                return false;

            Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
            Mat storedFace = new Mat(matStored, storedFaces[0]);

            CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
            CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

            using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 75)) // reduced threshold for stricter match
            {
                CvInvoke.EqualizeHist(capturedFace, capturedFace);
                CvInvoke.EqualizeHist(storedFace, storedFace);

                var trainingImages = new VectorOfMat();
                trainingImages.Push(storedFace);
                var labels = new VectorOfInt(new int[] { 1 });

                faceRecognizer.Train(trainingImages, labels);
                var result = faceRecognizer.Predict(capturedFace);

                Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                return result.Label == 1 && result.Distance <= 75;
            }
        }
        catch
        {
            return false;
        }
    }

    private Mat BitmapToMat(Bitmap bitmap)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Bmp);
            byte[] imageData = ms.ToArray();

            Mat mat = new Mat();
            CvInvoke.Imdecode(new Emgu.CV.Util.VectorOfByte(imageData), Emgu.CV.CvEnum.ImreadModes.Color, mat);

            return mat;
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
                    bmp.Save(filePath, ImageFormat.Jpeg);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving Base64 image to file: " + ex.Message);
        }
    }

    private void StoreData(string date, string inTime, string outTime, string Pno)
    {
        // Store attendance logic here
        Console.WriteLine($"Stored Data for {Pno} - Date: {date}, In: {inTime}, Out: {outTime}");
    }
}




<!-- OTP Bootstrap Modal -->
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
      </div>

      <div class="modal-footer">
        <button type="button" class="btn btn-primary" onclick="submitOtp()">Submit OTP</button>
      </div>

    </div>
  </div>
</div>

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

    fetch("/AS/Geo/VerifyOtp", {
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

this is my frontend logic 

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>

    <input type="hidden" name="Type" id="EntryType" />

    <div class="mt-5 form-group">
        <div class="col d-flex justify-content-center mb-4">
            @if (ViewBag.InOut == "O" || string.IsNullOrEmpty(ViewBag.InOut))
            {
                <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                    Punch In
                </button>
            }
        </div>

        <div class="col d-flex justify-content-center">
            @if (ViewBag.InOut == "I")
            {
                <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                    Punch Out
                </button>
            }
        </div>

    </div>

  
</form>





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

       
       

        fetch("/AS/Geo/AttendanceData", {
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


in this i want to make changes to otp , when faceretry more than 3 times then punchin and out button disable and send otp and there is a dialog open to enter otp with timer of 2 mins and , please provide full logic to implement 
