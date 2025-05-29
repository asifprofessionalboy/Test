private bool VerifyFaceWithDnn(Bitmap captured, Bitmap stored, out string message)
{
    message = null;

    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Load DNN model
        string protoPath = Path.Combine("wwwroot/Cascades", "deploy.prototxt");
        string modelPath = Path.Combine("wwwroot/Cascades", "res10_300x300_ssd_iter_140000_fp16.caffemodel");

        if (!System.IO.File.Exists(protoPath) || !System.IO.File.Exists(modelPath))
        {
            message = "Face detection model files are missing.";
            return false;
        }

        var net = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);

        // Detect faces
        Rectangle? faceRectCaptured = DetectFaceWithDnn(matCaptured, net);
        Rectangle? faceRectStored = DetectFaceWithDnn(matStored, net);

        if (faceRectCaptured == null || faceRectStored == null)
        {
            message = "Face not detected. Ensure good lighting and visibility.";
            return false;
        }

        // Expand rectangles slightly to include full face
        faceRectCaptured = ExpandRect(faceRectCaptured.Value, matCaptured.Width, matCaptured.Height, 10);
        faceRectStored = ExpandRect(faceRectStored.Value, matStored.Width, matStored.Height, 10);

        // Crop faces
        Mat capturedFace = new Mat(matCaptured, faceRectCaptured.Value);
        Mat storedFace = new Mat(matStored, faceRectStored.Value);

        // Resize to standard LBPH size
        CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
        CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

        // Convert to grayscale
        CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // Reduce noise
        CvInvoke.GaussianBlur(capturedFace, capturedFace, new Size(3, 3), 0);
        CvInvoke.GaussianBlur(storedFace, storedFace, new Size(3, 3), 0);

        // Normalize histograms
        CvInvoke.EqualizeHist(capturedFace, capturedFace);
        CvInvoke.EqualizeHist(storedFace, storedFace);

        // LBPH recognition
        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 99))
        {
            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);

            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);
            var result = faceRecognizer.Predict(capturedFace);

            double distance = result.Distance;
            int predictedLabel = result.Label;

            if (predictedLabel == 1 && distance <= 95) // Loosened threshold
            {
                if (distance <= 70)
                    message = $"Face matched with **high confidence**. Distance: {distance:F2}";
                else
                    message = $"Face matched with **medium confidence**. Distance: {distance:F2}";

                return true;
            }
            else
            {
                message = $"Face did **not** match. Distance: {distance:F2}. Try retaking the photo with better lighting.";
                return false;
            }
        }
    }
    catch (Exception ex)
    {
        message = $"Error in DNN verification: {ex.Message}";
        return false;
    }
}

private Rectangle? DetectFaceWithDnn(Mat image, Net net)
{
    Size inputSize = new Size(300, 300);
    double scale = 1.0;
    MCvScalar mean = new MCvScalar(104, 177, 123);

    using (var blob = DnnInvoke.BlobFromImage(image, scale, inputSize, mean, false, false))
    {
        net.SetInput(blob);
        using (var detection = net.Forward())
        {
            float[,,,] faces = (float[,,,])detection.GetData();

            for (int i = 0; i < faces.GetLength(2); i++)
            {
                float confidence = faces[0, 0, i, 2];
                if (confidence > 0.5)
                {
                    int x1 = (int)(faces[0, 0, i, 3] * image.Cols);
                    int y1 = (int)(faces[0, 0, i, 4] * image.Rows);
                    int x2 = (int)(faces[0, 0, i, 5] * image.Cols);
                    int y2 = (int)(faces[0, 0, i, 6] * image.Rows);

                    return new Rectangle(x1, y1, x2 - x1, y2 - y1);
                }
            }
        }
    }
    return null;
}

private Rectangle ExpandRect(Rectangle rect, int imageWidth, int imageHeight, int padding)
{
    int x = Math.Max(0, rect.X - padding);
    int y = Math.Max(0, rect.Y - padding);
    int width = Math.Min(imageWidth - x, rect.Width + 2 * padding);
    int height = Math.Min(imageHeight - y, rect.Height + 2 * padding);
    return new Rectangle(x, y, width, height);
}




this is my logic 

   private bool VerifyFaceWithDnn(Bitmap captured, Bitmap stored, out string errorMessage)
   {
       errorMessage = null;

       try
       {
           Mat matCaptured = BitmapToMat(captured);
           Mat matStored = BitmapToMat(stored);

           string protoPath = Path.Combine("wwwroot/Cascades", "deploy.prototxt");
           string modelPath = Path.Combine("wwwroot/Cascades", "res10_300x300_ssd_iter_140000_fp16.caffemodel");
          
               if (!System.IO.File.Exists(protoPath) || !System.IO.File.Exists(modelPath))
               {
                   errorMessage = "Face detection model files missing!";
                   return false;
               }

               var net = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);

               Rectangle? faceRectCaptured = DetectFaceWithDnn(matCaptured, net);
               Rectangle? faceRectStored = DetectFaceWithDnn(matStored, net);

               if (faceRectCaptured == null || faceRectStored == null)
               {
                   errorMessage = "Face not detected. Ensure lighting is adequate.";
                   return false;
               }

               Mat capturedFace = new Mat(matCaptured, faceRectCaptured.Value);
               Mat storedFace = new Mat(matStored, faceRectStored.Value);

               CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
               CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

               CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
               CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

               CvInvoke.EqualizeHist(capturedFace, capturedFace);
               CvInvoke.EqualizeHist(storedFace, storedFace);

               using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 99))
               {
                   VectorOfMat trainingImages = new VectorOfMat();
                   trainingImages.Push(storedFace);
                   VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                   faceRecognizer.Train(trainingImages, labels);
                   var result = faceRecognizer.Predict(capturedFace);

                   if (result.Label == 1)
                   {
                       if (result.Distance <= 85)
                           return true;
                   }

                   errorMessage = $"Face did not match. Distance: {result.Distance:F2}";
                   return false;
               }
           
       }
       catch (Exception ex)
       {
           errorMessage = "Error in DNN verification: " + ex.Message;
           return false;
       }
   }

   private Rectangle? DetectFaceWithDnn(Mat image, Net net)
   {
       Size size = new Size(300, 300);
       double scale = 1.0;
       MCvScalar mean = new MCvScalar(104, 177, 123);

       using (var blob = DnnInvoke.BlobFromImage(image, scale, size, mean, false, false))
       {
           net.SetInput(blob);
           using (var detection = net.Forward())
           {
               var data = detection.GetData();
               float[,,,] faces = (float[,,,])data;

               for (int i = 0; i < faces.GetLength(2); i++)
               {
                   float confidence = faces[0, 0, i, 2];

                   if (confidence > 0.5)
                   {
                       int x1 = (int)(faces[0, 0, i, 3] * image.Cols);
                       int y1 = (int)(faces[0, 0, i, 4] * image.Rows);
                       int x2 = (int)(faces[0, 0, i, 5] * image.Cols);
                       int y2 = (int)(faces[0, 0, i, 6] * image.Rows);

                       return new Rectangle(x1, y1, x2 - x1, y2 - y1);
                   }
               }
           }
       }
       return null;
   }
sometimes it matches and sometimes not i want less strict not so much strict and false positive less and matches with User with less strictness
