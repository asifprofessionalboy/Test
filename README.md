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

        // Get Pno from cookies (ensure it's set in the session)
        const Pno = document.cookie.split('; ').find(row => row.startsWith('Session=')).split('=')[1];

        fetch("/Geo/AttendanceData", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Type: entryType,
                Pno: Pno, // Send Pno
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



using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using OpenCvSharp.Face;

[HttpPost]
public IActionResult AttendanceData([FromBody] AttendanceRequest model)
{
    try
    {
        if (string.IsNullOrEmpty(model.Pno) || string.IsNullOrEmpty(model.ImageData))
        {
            return Json(new { success = false, message = "Invalid data received!" });
        }

        string Pno = model.Pno;

        // **Retrieve stored image based on Pno**
        string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", $"{Pno}.jpg");
        if (!System.IO.File.Exists(storedImagePath))
        {
            return Json(new { success = false, message = "No registered face found for this user!" });
        }

        // **Convert the received Base64 image to JPG**
        byte[] imageBytes = Convert.FromBase64String(model.ImageData.Split(',')[1]);
        string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", $"{Pno}-Captured.jpg");
        System.IO.File.WriteAllBytes(capturedImagePath, imageBytes);

        // **Load the images using OpenCV**
        using (Mat storedImage = Cv2.ImRead(storedImagePath, ImreadModes.Grayscale))
        using (Mat capturedImage = Cv2.ImRead(capturedImagePath, ImreadModes.Grayscale))
        {
            // **Compare faces**
            bool isFaceMatched = VerifyFace(storedImage, capturedImage);

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

                return Json(new { success = true, message = "Face matched. Data saved successfully!" });
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


private bool VerifyFace(Mat capturedImage, Mat storedImage)
{
    try
    {
        // Convert images to grayscale
        Cv2.CvtColor(capturedImage, capturedImage, ColorConversionCodes.BGR2GRAY);
        Cv2.CvtColor(storedImage, storedImage, ColorConversionCodes.BGR2GRAY);

        // Resize images to the same size (e.g., 200x200)
        Cv2.Resize(capturedImage, capturedImage, new Size(200, 200));
        Cv2.Resize(storedImage, storedImage, new Size(200, 200));

        // Initialize LBPH face recognizer
        var faceRecognizer = LBPHFaceRecognizer.Create();
        faceRecognizer.Train(new Mat[] { storedImage }, new int[] { 1 });

        // Predict the face
        int label;
        double confidence;
        faceRecognizer.Predict(capturedImage, out label, out confidence);

        // If confidence is low, it means the face matches
        return confidence < 50; // Lower confidence = better match
    }
    catch
    {
        return false;
    }
}



now i have this model in this Image is stored as .jpg 
public partial class AppPerson
{
    public Guid Id { get; set; }
    public string? Pno { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
}

and this is my two buttons 

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>
    <input type="hidden" name="Type" id="EntryType" />

    <div class="row mt-5 form-group">
        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                Punch In
            </button>
        </div>

        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                Punch Out
            </button>
        </div>
    </div>
</form>
and this is my js to compare with stored image 
now i want that when it capture image it sends to controller and matches 
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

       const imageData = canvas.toDataURL("image/png");

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
               alert(data.message);
           })
           .catch(error => {
               console.error("Error:", error);
               alert("An error occurred while submitting the image.");
           });
   }

   </script>

this is my controller 
 [HttpPost]
 public IActionResult AttendanceData([FromBody] AttendanceRequest model)
 {
     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];
         string Pno = UserId;


         string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/StoredFace.jpg");
         string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Captured.jpg");

         if (!System.IO.File.Exists(storedImagePath) || !System.IO.File.Exists(capturedImagePath))
         {
             return Json(new { success = false, message = "One or both hardcoded images not found!" });
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

                 return Json(new { success = true, message = "Data Saved Successfully" });
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

captured image and stored image matches with Pno and then verify face
