using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class GeoController : Controller
{
    [HttpPost]
    public IActionResult AttendanceData([FromBody] AttendanceRequest model)
    {
        try
        {
            // Get User ID from Session Cookie
            var UserId = HttpContext.Request.Cookies["Session"];
            if (string.IsNullOrEmpty(UserId))
                return Json(new { success = false, message = "User session not found!" });

            string Pno = UserId;

            // Fetch Stored Image Path dynamically
            string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}.jpg");
            if (!System.IO.File.Exists(storedImagePath))
            {
                return Json(new { success = false, message = "Stored image not found!" });
            }

            // Convert Base64 ImageData to .jpg and Save
            string capturedImagePath = SaveBase64Image(model.ImageData, Pno);
            if (string.IsNullOrEmpty(capturedImagePath))
            {
                return Json(new { success = false, message = "Failed to save captured image!" });
            }

            // Perform Face Verification
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

    private string SaveBase64Image(string base64String, string Pno)
    {
        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}_captured.jpg");
            byte[] imageBytes = Convert.FromBase64String(base64String.Split(',')[1]);
            System.IO.File.WriteAllBytes(filePath, imageBytes);
            return filePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving image: " + ex.Message);
            return null;
        }
    }

    private bool VerifyFace(Bitmap captured, Bitmap stored)
    {
        try
        {
            Mat matCaptured = BitmapToMat(captured);
            Mat matStored = BitmapToMat(stored);

            // Convert to Grayscale
            CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

            // Load Haarcascade for face detection
            string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
            if (!System.IO.File.Exists(cascadePath))
            {
                Console.WriteLine("Error: Haarcascade file not found!");
                return false;
            }

            CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
            Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
            Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);

            if (capturedFaces.Length == 0 || storedFaces.Length == 0)
            {
                Console.WriteLine("No face detected in one or both images.");
                return false;
            }

            // Crop faces
            Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
            Mat storedFace = new Mat(matStored, storedFaces[0]);

            // Resize faces
            CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
            CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

            // Initialize Face Recognizer
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

                return result.Label == 1 && result.Distance < 50;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in face verification: " + ex.Message);
            return false;
        }
    }

    private Mat BitmapToMat(Bitmap bitmap)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageData = ms.ToArray();

            Mat mat =




make full changes on this code 

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

public class AttendanceRequest
{
    public string Type { get; set; }
    public string ImageData { get; set; }
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

           
            return result.Label == 1 && result.Distance < 50;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
    }

    return false;
}

       
private Mat BitmapToMat(Bitmap bitmap)
{
    using (MemoryStream ms = new MemoryStream())
    {
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        byte[] imageData = ms.ToArray();

        Mat mat = new Mat();
        CvInvoke.Imdecode(new VectorOfByte(imageData), ImreadModes.Color, mat);

        if (mat.IsEmpty)
        {
            Console.WriteLine("Error: Image conversion failed!");
        }

        return mat;
    }
}
