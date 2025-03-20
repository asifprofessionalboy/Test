using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;

class Program
{
    static void Main()
    {
        string storedImagePath = "wwwroot/images/stored.jpg";
        string capturedImagePath = "wwwroot/images/captured.jpg";

        bool isFaceMatched = CompareFaces(storedImagePath, capturedImagePath);

        if (isFaceMatched)
        {
            Console.WriteLine("Face Matched!");
        }
        else
        {
            Console.WriteLine("Face Does Not Match!");
        }
    }

    static bool CompareFaces(string storedImagePath, string capturedImagePath)
    {
        try
        {
            Mat storedImage = CvInvoke.Imread(storedImagePath, ImreadModes.Grayscale);
            Mat capturedImage = CvInvoke.Imread(capturedImagePath, ImreadModes.Grayscale);

            if (storedImage.IsEmpty || capturedImage.IsEmpty)
            {
                Console.WriteLine("Error: One or both images are empty!");
                return false;
            }

            CascadeClassifier faceCascade = new CascadeClassifier("wwwroot/Cascades/haarcascade_frontalface_default.xml");

            Rectangle[] storedFaces = faceCascade.DetectMultiScale(storedImage, 1.1, 5);
            Rectangle[] capturedFaces = faceCascade.DetectMultiScale(capturedImage, 1.1, 5);

            if (storedFaces.Length == 0 || capturedFaces.Length == 0)
            {
                Console.WriteLine("No face detected in one or both images.");
                return false;
            }

            Mat storedFace = new Mat(storedImage, storedFaces[0]);
            Mat capturedFace = new Mat(capturedImage, capturedFaces[0]);

            CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));
            CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));

            LBPHFaceRecognizer recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
            VectorOfMat trainingImages = new VectorOfMat();
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            trainingImages.Push(storedFace);
            recognizer.Train(trainingImages, labels);

            var result = recognizer.Predict(capturedFace);

            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            return result.Label == 1 && result.Distance < 50; // Adjust threshold as needed
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in face comparison: " + ex.Message);
            return false;
        }
    }
}




[HttpPost]
public IActionResult AttendanceData([FromBody] AttendanceRequest model)
{
    if (string.IsNullOrEmpty(model.ImageData))
    {
        return Json(new { success = false, message = "Image data is missing!" });
    }

    try
    {
        var UserId = HttpContext.Request.Cookies["Session"];
        string Pno = UserId;

        byte[] imageBytes = Convert.FromBase64String(model.ImageData.Split(',')[1]);

        // Log received image size
        Console.WriteLine($"Received image size: {imageBytes.Length} bytes");

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

                // Log before face verification
                Console.WriteLine("Starting face verification...");

                bool isPartialMatch;
                bool isFaceMatched = VerifyFace(capturedImage, storedImage, out isPartialMatch);

                if (isFaceMatched)
                {
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

public bool VerifyFace(Bitmap capturedImage, Bitmap storedImage, out bool isPartialMatch)
{
    isPartialMatch = false;

    try
    {
        // Convert images to grayscale if necessary
        Bitmap capturedGray = ConvertToGrayscale(capturedImage);
        Bitmap storedGray = ConvertToGrayscale(storedImage);

        // Resize images to a standard size for better comparison
        Bitmap resizedCaptured = ResizeImage(capturedGray, 100, 100);
        Bitmap resizedStored = ResizeImage(storedGray, 100, 100);

        // Simple pixel comparison (replace with AI-based face detection if needed)
        int matchCount = 0;
        int totalPixels = resizedCaptured.Width * resizedCaptured.Height;

        for (int x = 0; x < resizedCaptured.Width; x++)
        {
            for (int y = 0; y < resizedCaptured.Height; y++)
            {
                Color color1 = resizedCaptured.GetPixel(x, y);
                Color color2 = resizedStored.GetPixel(x, y);

                if (Math.Abs(color1.R - color2.R) < 30 &&
                    Math.Abs(color1.G - color2.G) < 30 &&
                    Math.Abs(color1.B - color2.B) < 30)
                {
                    matchCount++;
                }
            }
        }

        double matchPercentage = (matchCount / (double)totalPixels) * 100;
        Console.WriteLine($"Face Match Percentage: {matchPercentage}%");

        if (matchPercentage > 80) // Adjust threshold as needed
        {
            return true;
        }
        else if (matchPercentage > 60)
        {
            isPartialMatch = true;
            return false;
        }
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Face comparison error: " + ex.Message);
        return false;
    }
}

function captureImageAndSubmit(entryType) {
    EntryTypeInput.value = entryType;

    const context = canvas.getContext("2d");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    
    // Flip image horizontally if necessary (check if it is needed)
    // context.translate(canvas.width, 0);
    // context.scale(-1, 1);
    
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


[HttpPost]
public IActionResult AttendanceData([FromBody] AttendanceRequest model)
{
    if (string.IsNullOrEmpty(model.ImageData))
    {
        return Json(new { success = false, message = "Image data is missing!" });
    }

    try
    {
        var UserId = HttpContext.Request.Cookies["Session"];
        string Pno = UserId;

        byte[] imageBytes = Convert.FromBase64String(model.ImageData.Split(',')[1]);

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

                    if (model.Type == "Punch In")
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

// Model to handle JSON request
public class AttendanceRequest
{
    public string Type { get; set; }
    public string ImageData { get; set; }
}


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

        
            context.setTransform(1, 0, 0, 1, 0, 0);

            const imageData = canvas.toDataURL("image/png");

       
            fetch("/GeoFencing/Geo/AttendanceData", {
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
this shows imagedata is missing is there any issue in this code?
