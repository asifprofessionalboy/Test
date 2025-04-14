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

        if (confidence > 0.90) // Stricter confidence
        {
            int x1 = (int)(detectionData[i * 7 + 3] * image.Width);
            int y1 = (int)(detectionData[i * 7 + 4] * image.Height);
            int x2 = (int)(detectionData[i * 7 + 5] * image.Width);
            int y2 = (int)(detectionData[i * 7 + 6] * image.Height);

            Rectangle faceRect = new Rectangle(x1, y1, x2 - x1, y2 - y1);

            if (faceRect.Width > 50 && faceRect.Height > 50) // avoid too small detections
                return new Mat(image, faceRect);
        }
    }

    return null;
}

private float[] NormalizeEmbedding(float[] vector)
{
    double norm = Math.Sqrt(vector.Sum(x => x * x));
    return vector.Select(v => (float)(v / norm)).ToArray();
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

    float[] embedding = output.GetData().Cast<float>().ToArray();
    return NormalizeEmbedding(embedding);
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

make it lil bit strict, sometimes it matches with same user with everyone and some times not
