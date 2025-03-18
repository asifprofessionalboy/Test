<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");

    // Start Camera
    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

    function setEntryType(entryType) {
        // Capture Image
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        // Convert Image to Base64
        let imageData = canvas.toDataURL("image/png");

        // Send Data to Controller for Face Verification
        $.ajax({
            url: "/Geo/AttendanceData",
            type: "POST",
            data: { EntryType: entryType, ImageData: imageData },
            success: function (response) {
                if (response.success) {
                    alert("✅ " + response.message);
                } else {
                    alert("❌ Face does not match! Please try again.");
                }
            },
            error: function () {
                alert("❌ Error processing the request.");
            }
        });

        // Prevent default form submission
        event.preventDefault();
    }
</script>

using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Models;

public class GeoController : Controller
{
    private readonly YourDbContext context;

    public GeoController(YourDbContext _context)
    {
        context = _context;
    }

    [HttpPost]
    public IActionResult AttendanceData(string EntryType, string ImageData)
    {
        try
        {
            var UserId = HttpContext.Request.Cookies["Session"];
            string Pno = UserId;

            // Convert Base64 Image Data to Byte Array
            byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]);
            using (var ms = new MemoryStream(imageBytes))
            {
                Bitmap capturedImage = new Bitmap(ms);

                // Retrieve the stored image from the database
                var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
                if (user == null || user.Image == null)
                {
                    return Json(new { success = false, message = "User Image Not Found!" });
                }

                // Convert stored image to Bitmap
                using (var storedStream = new MemoryStream(user.Image))
                {
                    Bitmap storedImage = new Bitmap(storedStream);

                    // Perform Face Recognition
                    bool isFaceMatched = VerifyFace(capturedImage, storedImage);

                    if (!isFaceMatched)
                    {
                        return Json(new { success = false, message = "Face does not match!" });
                    }

                    // If face matches, store attendance
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (EntryType == "Punch In")
                    {
                        StoreData(currentDate, currentTime, null, Pno);
                    }
                    else
                    {
                        StoreData(currentDate, null, currentTime, Pno);
                    }

                    return Json(new { success = true, message = "Attendance Marked Successfully!" });
                }
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private bool VerifyFace(Bitmap captured, Bitmap stored)
    {
        try
        {
            using (var faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity)) // You can also use LBPHFaceRecognizer
            {
                // Convert both images to grayscale
                var grayCaptured = new Image<Gray, byte>(captured);
                var grayStored = new Image<Gray, byte>(stored);

                // Train the recognizer with the stored face
                faceRecognizer.Train(new Image<Gray, byte>[] { grayStored }, new int[] { 1 });

                // Predict the captured face
                var result = faceRecognizer.Predict(grayCaptured);

                // If the distance score is low, it's a match
                return result.Label == 1 && result.Distance < 5000; // Lower distance means better match
            }
        }
        catch
        {
            return false;
        }
    }
}


this is my controller method 
 [HttpPost]
 public IActionResult AttendanceData(string EntryType)
 {
     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];

         string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
         string currentTime = DateTime.Now.ToString("HH:mm");
         string Pno = UserId;

         if (EntryType == "Punch In")
         {
             StoreData(currentDate, currentTime, null, Pno);
         }
         else
         {
             StoreData(currentDate, null, currentTime, Pno);
         }

         return Json(new { success = true, message = "Data Saved Successfully" });
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }

and this is my form 

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post" enctype="multipart/form-data">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>
    <input type="hidden" name="EntryType" id="EntryType" />

    <div class="row mt-5 form-group" style="margin-top:50%;">
        <div class="col d-flex justify-content-center ">
            <button type="submit" class="Btn form-group" id="PunchIn" onclick="setEntryType('Punch In')">
                Punch In
            </button>
        </div>

        <div class="col d-flex justify-content-center form-group">
            <button type="submit" class="Btn2 form-group" id="PunchOut" onclick="setEntryType('Punch Out')">
                Punch Out
            </button>
        </div>
    </div>
</form>
this is my js 

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const captureBtn = document.getElementById("captureBtn");
    const photoInput = document.getElementById("photoInput");


    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            let video = document.querySelector("video");
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

         

    captureBtn.addEventListener("click", () => {
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        photoInput.value = canvas.toDataURL("image/png"); 
    });
</script>

in this i want that when user clicks on punchIn or Punchout then it capture the Image of the user and Verify using OpenCV in my controller method , if it matches then it going inside EntryType Punch In or Punch Out otherwise shows an alert Face doesnot matches
