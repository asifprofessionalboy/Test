private bool VerifyFace(Bitmap captured, Bitmap stored, out bool isPartialMatch)
{
    isPartialMatch = false;

    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Cascades", "haarcascade_frontalface_default.xml");
        var faceCascade = new CascadeClassifier(cascadePath);

        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
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
            trainingImages.Push(storedFace);
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);

            var result = faceRecognizer.Predict(capturedFace);
            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            // Strong Match (Exact Face Match)
            if (result.Label == 1 && result.Distance < 50)
            {
                return true;
            }
            // Partial Match (50% Face Match)
            else if (result.Label == 1 && result.Distance >= 50 && result.Distance <= 80)
            {
                isPartialMatch = true;
                return false;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
    }

    return false;
}




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

    // Flip the image back to normal
    context.setTransform(1, 0, 0, 1, 0, 0);

    const imageData = canvas.toDataURL("image/png");

    // Send the image via AJAX
    fetch("/YourController/AttendanceData", {
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

 [HttpPost]
public IActionResult AttendanceData(string Type, string ImageData)
{
    if (string.IsNullOrEmpty(ImageData))
    {
        return Json(new { success = false, message = "Image data is missing!" });
    }

    try
    {
        var UserId = HttpContext.Request.Cookies["Session"];
        string Pno = UserId;

        byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]);

        using (var ms = new MemoryStream(imageBytes))
        {
            Bitmap capturedImage = new Bitmap(ms);

            var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
            if (user == null || user.Image == null)
            {
                return Json(new { success = false, message = "User Image Not Found!" });
            }

            using (var storedStream = new MemoryStream(user.Image))
            {
                Bitmap storedImage = new Bitmap(storedStream);

                bool isPartialMatch;
                bool isFaceMatched = VerifyFace(capturedImage, storedImage, out isPartialMatch);

                if (isFaceMatched)
                {
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (Type == "Punch In")
                    {
                        StoreData(currentDate, currentTime, null, Pno);
                    }
                    else
                    {
                        StoreData(currentDate, null, currentTime, Pno);
                    }

                    return Json(new { success = true, message = "Attendance Marked Successfully!" });
                }
                else if (isPartialMatch)
                {
                    return Json(new { success = false, message = "Face partially matched! Please try again." });
                }
                else
                {
                    return Json(new { success = false, message = "Face does not match!" });
                }
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

 
 
 const video = document.getElementById("video");
 const canvas = document.getElementById("canvas");
 const EntryTypeInput = document.getElementById("EntryType");
 const form = document.getElementById("form");


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

     context.translate(canvas.width, 0);
     context.scale(-1, 1);
     context.drawImage(video, 0, 0, canvas.width, canvas.height);
     context.setTransform(1, 0, 0, 1, 0, 0);

     const imageData = canvas.toDataURL("image/png");
     
    
     const imageInput = document.createElement("input");
     imageInput.type = "hidden";
     imageInput.name = "ImageData";
     imageInput.value = imageData;
     form.appendChild(imageInput);

    
     form.submit();
 }
