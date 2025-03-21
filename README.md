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

        // **Hardcoded Image Path**
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", "151514-Shashi Kumar.jpg");
        if (!System.IO.File.Exists(imagePath))
        {
            return Json(new { success = false, message = "Stored image not found!" });
        }

        // **Use the same image for both captured and stored**
        using (Bitmap capturedImage = new Bitmap(imagePath))
        using (Bitmap storedImage = new Bitmap(imagePath))
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



i have this image

151514-Shashi Kumar.jpg 
i want same image hardcoded for captured and stored 

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
                {
                    Console.WriteLine("Error: Haarcascade file not found!");
                    return false;
                }

                CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
                Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 3, Size.Empty, new Size(30, 30));
                Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 3, Size.Empty, new Size(30, 30));

                if (capturedFaces.Length == 0)
                {
                    return false;  // Alert: "No face detected in captured image"
                }

                if (storedFaces.Length == 0)
                {
                    return false;  // Alert: "No face detected in stored image"
                }



                Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
                Mat storedFace = new Mat(matStored, storedFaces[0]);

               
                CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
                CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

                
                using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
                {
                    CvInvoke.EqualizeHist(capturedFace, capturedFace);
                    CvInvoke.EqualizeHist(storedFace, storedFace);

                    VectorOfMat trainingImages = new VectorOfMat();
trainingImages.Push(storedFace);  // Add more images here
VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                    faceRecognizer.Train(trainingImages, labels);
                    var result = faceRecognizer.Predict(capturedFace);

                    Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                    return result.Label == 1 && result.Distance < 120;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in face verification: " + ex.Message);
                return false;
            }
        }
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
