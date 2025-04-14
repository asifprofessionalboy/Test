private double CalculateCosineSimilarity(float[] vec1, float[] vec2)
{
    if (vec1.Length != vec2.Length) return -1;

    double dotProduct = 0.0;
    double magnitude1 = 0.0;
    double magnitude2 = 0.0;

    for (int i = 0; i < vec1.Length; i++)
    {
        dotProduct += vec1[i] * vec2[i];
        magnitude1 += Math.Pow(vec1[i], 2);
        magnitude2 += Math.Pow(vec2[i], 2);
    }

    magnitude1 = Math.Sqrt(magnitude1);
    magnitude2 = Math.Sqrt(magnitude2);

    if (magnitude1 == 0 || magnitude2 == 0)
        return -1;

    return dotProduct / (magnitude1 * magnitude2); // returns value between -1 to 1
}



double similarity = CalculateCosineSimilarity(capturedEmbedding, storedEmbedding);
Console.WriteLine($"[FaceMatch] Cosine Similarity: {similarity}");
return similarity > 0.55; // You can tune this threshold (0.5–0.6 is typically strict)

this is my face recognition code 

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


                CvInvoke.Resize(capturedFace, capturedFace, new Size(96, 96));
                CvInvoke.Resize(storedFace, storedFace, new Size(96, 96));

                float[] capturedEmbedding = GetFaceEmbedding(capturedFace);
                float[] storedEmbedding = GetFaceEmbedding(storedFace);

                double distance = CalculateEuclideanDistance(capturedEmbedding, storedEmbedding);
                Console.WriteLine($"[FaceMatch] Euclidean Distance: {distance}");

                return distance < 0.45;
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



some times it matches with everyone and some times not , make it more strict and also matches with same person that is stored as a jpg
