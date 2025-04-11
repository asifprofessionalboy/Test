using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

public class FaceRecognitionController : Controller
{
    private readonly string _cascadePath;
    private readonly string _faceModelPath;
    private readonly string _faceProtoPath;
    private readonly string _embedModelPath;

    public FaceRecognitionController()
    {
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Cascades");
        _faceModelPath = Path.Combine(basePath, "res10_300x300_ssd_iter_140000_fp16.caffemodel");
        _faceProtoPath = Path.Combine(basePath, "deploy.prototxt");
        _embedModelPath = Path.Combine(basePath, "nn4.small2.v1.t7");
    }

    [HttpPost]
    public IActionResult VerifyFaceFromImages(IFormFile image1, IFormFile image2)
    {
        if (image1 == null || image2 == null)
            return BadRequest("Both images are required.");

        Bitmap bmp1;
        Bitmap bmp2;

        using (var stream1 = image1.OpenReadStream())
        using (var stream2 = image2.OpenReadStream())
        {
            bmp1 = new Bitmap(stream1);
            bmp2 = new Bitmap(stream2);
        }

        bool result = VerifyFace(bmp1, bmp2);
        return Ok(result ? "Faces Match" : "Faces Do Not Match");
    }

    private bool VerifyFace(Bitmap captured, Bitmap stored)
    {
        try
        {
            Mat matCaptured = BitmapToMat(captured);
            Mat matStored = BitmapToMat(stored);

            Mat capturedFace = DetectFace(matCaptured);
            Mat storedFace = DetectFace(matStored);

            if (capturedFace == null || storedFace == null)
            {
                Console.WriteLine("Face not detected in one or both images.");
                return false;
            }

            CvInvoke.Resize(capturedFace, capturedFace, new Size(96, 96));
            CvInvoke.Resize(storedFace, storedFace, new Size(96, 96));

            float[] embedding1 = GetFaceEmbedding(capturedFace);
            float[] embedding2 = GetFaceEmbedding(storedFace);

            double distance = CalculateEuclideanDistance(embedding1, embedding2);
            Console.WriteLine($"Distance: {distance}");

            return distance < 0.45;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error verifying face: " + ex.Message);
            return false;
        }
    }

    private Mat DetectFace(Mat image)
    {
        var net = DnnInvoke.ReadNetFromCaffe(_faceProtoPath, _faceModelPath);
        if (net.Empty)
        {
            Console.WriteLine("Failed to load face detection model.");
            return null;
        }

        Mat blob = DnnInvoke.BlobFromImage(image, 1.0, new Size(300, 300), new MCvScalar(104, 177, 123), false, false);
        net.SetInput(blob);
        Mat detections = net.Forward();

        int h = image.Rows;
        int w = image.Cols;

        for (int i = 0; i < detections.SizeOfDimension[2]; i++)
        {
            float confidence = detections.GetData<float>(0, 0, i, 2);
            if (confidence > 0.85)
            {
                int x1 = (int)(detections.GetData<float>(0, 0, i, 3) * w);
                int y1 = (int)(detections.GetData<float>(0, 0, i, 4) * h);
                int x2 = (int)(detections.GetData<float>(0, 0, i, 5) * w);
                int y2 = (int)(detections.GetData<float>(0, 0, i, 6) * h);

                Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                return new Mat(image, faceRect);
            }
        }

        return null;
    }

    private float[] GetFaceEmbedding(Mat face)
    {
        var net = DnnInvoke.ReadNetFromTorch(_embedModelPath);
        if (net.Empty)
        {
            throw new Exception("Failed to load face embedding model.");
        }

        Mat blob = DnnInvoke.BlobFromImage(face, 1.0 / 255.0, new Size(96, 96), new MCvScalar(0, 0, 0), true, false);
        net.SetInput(blob);
        Mat embedding = net.Forward();

        float[] result = new float[embedding.SizeOfDimension[1]];
        embedding.CopyTo(result);
        return result;
    }

    private double CalculateEuclideanDistance(float[] emb1, float[] emb2)
    {
        double sum = 0;
        for (int i = 0; i < emb1.Length; i++)
        {
            double diff = emb1[i] - emb2[i];
            sum += diff * diff;
        }
        return Math.Sqrt(sum);
    }

    private Mat BitmapToMat(Bitmap bitmap)
    {
        return bitmap == null ? null : bitmap.ToImage<Bgr, byte>().Mat;
    }
}





this is my logic which i want to 

implement in my existing code which is using LBPHFaceRecognizer, i want to change that to this logic 

this is my main      

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


                string capturedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured-{DateTime.Now.Ticks}.jpg");


                SaveBase64ImageToFile(model.ImageData, capturedImagePath);

                bool isFaceMatched = false;


                using (Bitmap capturedImage = new Bitmap(capturedImagePath))
                using (Bitmap storedImage = new Bitmap(storedImagePath))
                {
                    isFaceMatched = VerifyFace(capturedImage, storedImage);
                }


                System.IO.File.Delete(capturedImagePath);

                if (isFaceMatched)
                {
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (model.Type == "Punch In")
                    {
                        StoreData(currentDate, currentTime, null, Pno, model.ImageData);
                    }
                    else
                    {
                        StoreData(currentDate, null, currentTime, Pno, model.ImageData);
                    }

                    return Json(new { success = true, message = "Attendance recorded successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Face does not match!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private Mat DetectFace(Mat image)
        {
            string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/res10_300x300_ssd_iter_140000_fp16.caffemodel");
            string protoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/deploy.prototxt");

            if (!System.IO.File.Exists(modelPath) || !System.IO.File.Exists(protoPath))
            {
                Console.WriteLine("Error: Face detection model files not found!");
                return null;
            }

            Net faceNet = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);
            Mat blob = DnnInvoke.BlobFromImage(image, 1.0, new Size(300, 300), new MCvScalar(104, 177, 123));
            faceNet.SetInput(blob);
            Mat detections = faceNet.Forward();

            Array detectionArray = detections.GetData();
            float[] detectionData = detectionArray.Cast<float>().ToArray();
            int numDetections = detections.SizeOfDimension[2];

            for (int i = 0; i < numDetections; i++)
            {
                float confidence = detectionData[i * 7 + 2];

                if (confidence > 0.85) // More strict confidence level
                {
                    int x1 = (int)(detectionData[i * 7 + 3] * image.Width);
                    int y1 = (int)(detectionData[i * 7 + 4] * image.Height);
                    int x2 = (int)(detectionData[i * 7 + 5] * image.Width);
                    int y2 = (int)(detectionData[i * 7 + 6] * image.Height);

                    Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                    return new Mat(image, faceRect);
                }
            }

            return null;
        }

        private bool VerifyFace(Bitmap captured, Bitmap stored)
        {
            try
            {
                Mat matCaptured = BitmapToMat(captured);
                Mat matStored = BitmapToMat(stored);

                Mat capturedFace = DetectFace(matCaptured);
                Mat storedFace = DetectFace(matStored);

                if (capturedFace == null || storedFace == null)
                {
                    Console.WriteLine("No face detected in one or both images.");
                    return false;
                }

                // Resize both face regions to 96x96
                CvInvoke.Resize(capturedFace, capturedFace, new Size(96, 96));
                CvInvoke.Resize(storedFace, storedFace, new Size(96, 96));

                float[] capturedEmbedding = GetFaceEmbedding(capturedFace);
                float[] storedEmbedding = GetFaceEmbedding(storedFace);

                double distance = CalculateEuclideanDistance(capturedEmbedding, storedEmbedding);
                Console.WriteLine($"[FaceMatch] Euclidean Distance: {distance}");

                return distance < 0.45; // Stricter threshold to avoid false matches
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in face verification: " + ex.Message);
                return false;
            }
        }


        private float[] GetFaceEmbedding(Mat face)
        {
            string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/nn4.small2.v1.t7");
            if (!System.IO.File.Exists(modelPath))
            {
                Console.WriteLine("Error: Face recognition model file not found!");
                return null;
            }

            Net faceRecognizer = DnnInvoke.ReadNetFromTorch(modelPath);
            Mat blob = DnnInvoke.BlobFromImage(face, 1.0 / 255, new Size(96, 96), new MCvScalar(0, 0, 0), true, false);
            faceRecognizer.SetInput(blob);
            Mat output = faceRecognizer.Forward();

            return output.GetData().Cast<float>().ToArray();
        }


        private double CalculateEuclideanDistance(float[] vec1, float[] vec2)
        {
            if (vec1.Length != vec2.Length) return double.MaxValue;

            double sum = 0;
            for (int i = 0; i < vec1.Length; i++)
            {
                sum += Math.Pow(vec1[i] - vec2[i], 2);
            }

            return Math.Sqrt(sum);
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

        private void SaveBase64ImageToFile(string base64String, string filePath)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String.Split(',')[1]);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    using (Bitmap bmp = new Bitmap(ms))
                    {
                        bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving Base64 image to file: " + ex.Message);
            }
        }

and this is old code 
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

                
                string tempCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Temp-{DateTime.Now.Ticks}.jpg");
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
                    string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string currentTime = DateTime.Now.ToString("HH:mm");

                    if (model.Type == "Punch In")
                    {
                        
                        string newCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");
                        SaveBase64ImageToFile(model.ImageData, newCapturedPath);

                        StoreData(currentDate, currentTime, null, Pno, model.ImageData);
                    }
                    else
                    {
                        StoreData(currentDate, null, currentTime, Pno, model.ImageData);
                    }

                    return Json(new { success = true, message = "Attendance recorded successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Face does not match!" });
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


                CvInvoke.Resize(capturedFace, capturedFace, new Size(96, 96));
                CvInvoke.Resize(storedFace, storedFace, new Size(96, 96));


                using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 95))
                {
                    CvInvoke.EqualizeHist(capturedFace, capturedFace);
                    CvInvoke.EqualizeHist(storedFace, storedFace);

                    VectorOfMat trainingImages = new VectorOfMat();
                    trainingImages.Push(storedFace);
                    VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                    faceRecognizer.Train(trainingImages, labels);
                    var result = faceRecognizer.Predict(capturedFace);

                    Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                    return result.Label == 1 && result.Distance <= 95;
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

                Mat mat = new Mat();
                CvInvoke.Imdecode(new VectorOfByte(imageData), ImreadModes.Color, mat);

                if (mat.IsEmpty)
                {
                    Console.WriteLine("Error: Image conversion failed!");
                }

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
                        bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving Base64 image to file: " + ex.Message);
            }
        }

