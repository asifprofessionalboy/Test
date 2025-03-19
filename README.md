<!DOCTYPE html>
<html lang="en">
<head>
    <title>Face Recognition Attendance</title>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>
<body>

    <h2>Face Recognition Attendance</h2>

    <video id="video" width="320" height="240" autoplay></video>
    <canvas id="canvas" width="320" height="240" style="display: none;"></canvas>
    
    <button id="capture">Capture</button>
    <button id="punchIn">Punch In</button>
    <button id="punchOut">Punch Out</button>

    <p id="status"></p>

    <script>
        $(document).ready(function () {
            // Access the user's camera
            navigator.mediaDevices.getUserMedia({ video: true })
                .then(function (stream) {
                    document.getElementById("video").srcObject = stream;
                })
                .catch(function (err) {
                    console.log("Error accessing camera: " + err);
                });

            // Capture Image from Video
            $("#capture").click(function () {
                let video = document.getElementById("video");
                let canvas = document.getElementById("canvas");
                let context = canvas.getContext("2d");

                context.drawImage(video, 0, 0, 320, 240);
                let imageData = canvas.toDataURL("image/jpeg");

                sessionStorage.setItem("capturedImage", imageData); // Store captured image
                alert("Image captured successfully!");
            });

            // Handle Punch In
            $("#punchIn").click(function () {
                let imageData = sessionStorage.getItem("capturedImage");
                if (!imageData) {
                    alert("Please capture an image first!");
                    return;
                }
                sendImageToServer(imageData, "Punch In");
            });

            // Handle Punch Out
            $("#punchOut").click(function () {
                let imageData = sessionStorage.getItem("capturedImage");
                if (!imageData) {
                    alert("Please capture an image first!");
                    return;
                }
                sendImageToServer(imageData, "Punch Out");
            });

            // Function to Send Image to Backend
            function sendImageToServer(imageData, entryType) {
                $.ajax({
                    type: "POST",
                    url: "/Home/AttendanceData",
                    data: { EntryType: entryType, ImageData: imageData },
                    success: function (response) {
                        $("#status").text(response.message);
                        if (response.success) {
                            $("#status").css("color", "green");
                        } else {
                            $("#status").css("color", "red");
                        }
                    },
                    error: function () {
                        $("#status").text("Error processing request.");
                        $("#status").css("color", "red");
                    }
                });
            }
        });
    </script>

</body>
</html>

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
        using (var capturedImage = new Bitmap(ms))
        {
            var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
            if (user == null || user.FaceEncoding == null)
            {
                return Json(new { success = false, message = "User not found or no face stored!" });
            }

            // Perform Face Recognition
            bool isFaceMatched = VerifyFace(capturedImage, user.FaceEncoding);

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
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

using System.Drawing.Imaging;
using System.IO;
using FaceRecognitionDotNet;

private byte[] GetFaceEncoding(Bitmap image)
{
    string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "models");

    // Resize image before processing
    Bitmap resized = new Bitmap(image, new Size(150, 150));

    using (var faceRecognition = FaceRecognition.Create(modelPath))
    {
        var faceImage = FaceRecognition.LoadImage(resized);
        var encodings = faceRecognition.FaceEncodings(faceImage).FirstOrDefault();

        return encodings?.GetRawEncoding();  // Returns null if no face is detected
    }
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadImage(string Pno, string Name, string photoData)
{
    if (!string.IsNullOrEmpty(photoData))
    {
        byte[] imageBytes = Convert.FromBase64String(photoData.Split(',')[1]);

        using (var ms = new MemoryStream(imageBytes))
        using (var bitmap = new Bitmap(ms))
        {
            byte[] faceEncoding = GetFaceEncoding(bitmap);

            if (faceEncoding == null)
            {
                return Json(new { success = false, message = "No face detected!" });
            }

            var person = new AppPerson
            {
                Pno = Pno,
                Name = Name,
                FaceEncoding = faceEncoding
            };

            context.AppPeople.Add(person);
            await context.SaveChangesAsync();
            return Json(new { success = true, message = "Face stored successfully!" });
        }
    }

    return Json(new { success = false, message = "Invalid image data!" });
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
        using (var capturedImage = new Bitmap(ms))
        {
            var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
            if (user == null || user.FaceEncoding == null)
            {
                return Json(new { success = false, message = "User not found or no face stored!" });
            }

            // Perform Face Recognition
            bool isFaceMatched = VerifyFace(capturedImage, user.FaceEncoding);

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
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}
using FaceRecognitionDotNet;

private byte[] GetFaceEncoding(Bitmap image)
{
    string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "models");

    using (var faceRecognition = FaceRecognition.Create(modelPath))
    {
        var faceImage = FaceRecognition.LoadImage(image);
        var encodings = faceRecognition.FaceEncodings(faceImage).FirstOrDefault();

        return encodings?.GetRawEncoding();  // Returns null if no face is detected
    }
}
private bool VerifyFace(Bitmap captured, byte[] storedEncoding)
{
    string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "models");

    using (var faceRecognition = FaceRecognition.Create(modelPath))
    {
        var faceImage = FaceRecognition.LoadImage(captured);
        var capturedEncoding = faceRecognition.FaceEncodings(faceImage).FirstOrDefault();

        if (capturedEncoding == null)
            return false;  // No face detected

        // Compare faces with a threshold (0.6 is a good value)
        return FaceRecognition.CompareFaceEncodings(storedEncoding, capturedEncoding.GetRawEncoding(), tolerance: 0.6);
    }
}


[HttpGet("Submit")]
public IActionResult GetAllDetails(string WorkOrderNo, string VendorCode)
{
    try
    {
        // Fetch the datasets
        var leaveDetails = compliance.Leave_details(WorkOrderNo, VendorCode);
        var bonusDetails = compliance.Bonus_details(WorkOrderNo, VendorCode);
        var rrAlertLatest = compliance.RR_Alert_latest(WorkOrderNo, VendorCode);

        // Convert DataSets to List of Dictionaries
        var leaveDetailsList = new List<object>();
        var bonusDetailsList = new List<object>();

        if (leaveDetails != null && leaveDetails.Tables.Count > 0)
        {
            foreach (DataTable table in leaveDetails.Tables)
            {
                leaveDetailsList.Add(ConvertDataTableToDictionaryList(table));
            }
        }

        if (bonusDetails != null && bonusDetails.Tables.Count > 0)
        {
            foreach (DataTable table in bonusDetails.Tables)
            {
                bonusDetailsList.Add(ConvertDataTableToDictionaryList(table));
            }
        }

        // Convert DataTable to List of Dictionary
        var rrAlertList = ConvertDataTableToDictionaryList(rrAlertLatest);

        // Combine all data in a single response object
        var response = new
        {
            LeaveDetails = leaveDetailsList,
            BonusDetails = bonusDetailsList,
            RRAlertLatest = rrAlertList
        };

        return Ok(response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error Occurred while fetching details");
        return StatusCode(500, ex.Message);
    }
}




i have these three different datasets function. these functions fetches data from . i want to show output at once 
public DataSet Leave_details(string WorkOrder, string VendorCode)
        {
}

public DataSet Bonus_details(string WorkOrder, string VendorCode)
        {
}

public DataTable RR_Alert_latest(string WorkOrderNo, string VendorCode)
        {
 return dt;

}
public static List<Dictionary<string, object>> ConvertDataTableToDictionaryList(DataTable dt)
    {
        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                dict[col.ColumnName] = row[col];
            }
            list.Add(dict);
        }
        return list;
    }
}
 this is my controller 

 [HttpGet("Submit")]
        public IActionResult RR_Alert_Latest(string WorkOrderNo, string VendorCode)
        {

            try
            {
                var data = compliance.RR_Alert_latest(WorkOrderNo, VendorCode);
                
                return Ok(data);    
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error Occured while Getting RR_ALert_Latest");
                return StatusCode(500, ex.Message);
            }
        }
