@model YourNamespace.Models.Person

@{
    ViewData["Title"] = "Face Recognition";
}

<h2>Face Recognition Login</h2>

<form id="photoForm" asp-action="VerifyFace" method="post" enctype="multipart/form-data">
    <div class="form-group">
        <label>Pno</label>
        <input type="text" id="pno" name="Pno" class="form-control" required />
    </div>

    <div class="form-group">
        <video id="video" width="320" height="240" autoplay></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>

    <div class="form-group">
        <button type="button" id="captureBtn" class="btn btn-primary">Capture Photo</button>
    </div>

    <input type="hidden" id="photoInput" name="photoData" />

    <button type="submit" class="btn btn-success">Verify Face</button>
</form>

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const captureBtn = document.getElementById("captureBtn");
    const photoInput = document.getElementById("photoInput");

    // Access user webcam
    navigator.mediaDevices.getUserMedia({ video: true })
        .then(stream => { video.srcObject = stream; })
        .catch(err => { console.error("Camera Access Denied", err); });

    // Capture image from video
    captureBtn.addEventListener("click", () => {
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        photoInput.value = canvas.toDataURL("image/png"); // Convert to Base64
    });
</script>

public class PersonController : Controller
{
    private readonly ApplicationDbContext _context;

    public PersonController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: Verify Face
    [HttpPost]
    public async Task<IActionResult> VerifyFace(string Pno, string photoData)
    {
        if (string.IsNullOrEmpty(Pno) || string.IsNullOrEmpty(photoData))
        {
            return BadRequest("Invalid Data");
        }

        // Convert Base64 image to byte array
        byte[] uploadedImageBytes = ConvertBase64ToByteArray(photoData);

        // Retrieve stored user photo
        var person = await _context.Persons.FirstOrDefaultAsync(p => p.Pno.ToString() == Pno);
        if (person == null || person.Photo == null)
        {
            return NotFound("No image found for this Pno");
        }

        // Convert stored image to Mat (OpenCV format)
        Mat uploadedMat = ConvertByteArrayToMat(uploadedImageBytes);
        Mat storedMat = ConvertByteArrayToMat(person.Photo);

        // Perform face recognition comparison
        bool isMatch = CompareFaces(uploadedMat, storedMat);

        if (isMatch)
        {
            return Content("Face Matched! Access Granted.");
        }
        else
        {
            return Content("Face Did Not Match! Access Denied.");
        }
    }

    // Convert Base64 string to Byte Array
    private byte[] ConvertBase64ToByteArray(string base64String)
    {
        base64String = base64String.Replace("data:image/png;base64,", "");
        return Convert.FromBase64String(base64String);
    }

    // Convert Byte Array to OpenCV Mat
    private Mat ConvertByteArrayToMat(byte[] imageBytes)
    {
        Mat mat = new Mat();
        using (MemoryStream ms = new MemoryStream(imageBytes))
        {
            mat = CvInvoke.Imdecode(ms.ToArray(), Emgu.CV.CvEnum.ImreadModes.Color);
        }
        return mat;
    }

    // Face Recognition Using OpenCV
    private bool CompareFaces(Mat img1, Mat img2)
    {
        var faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

        // Detect faces in both images
        var faces1 = faceCascade.DetectMultiScale(img1, 1.1, 10);
        var faces2 = faceCascade.DetectMultiScale(img2, 1.1, 10);

        if (faces1.Length == 0 || faces2.Length == 0)
            return false; // No face detected

        // Extract face regions
        Mat faceRegion1 = new Mat(img1, faces1[0]);
        Mat faceRegion2 = new Mat(img2, faces2[0]);

        // Convert to grayscale for comparison
        CvInvoke.CvtColor(faceRegion1, faceRegion1, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(faceRegion2, faceRegion2, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // Resize both face images to the same size
        CvInvoke.Resize(faceRegion1, faceRegion1, new System.Drawing.Size(100, 100));
        CvInvoke.Resize(faceRegion2, faceRegion2, new System.Drawing.Size(100, 100));

        // Compare Histograms
        double similarity = CvInvoke.CompareHist(faceRegion1, faceRegion2, Emgu.CV.CvEnum.HistogramCompMethod.Correl);
        return similarity > 0.7; // Threshold for matching
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YourNamespace.Data;
using YourNamespace.Models;
