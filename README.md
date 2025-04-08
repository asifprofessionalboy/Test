private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
        if (!File.Exists(cascadePath))
        {
            Console.WriteLine("Haarcascade not found!");
            return false;
        }

        var faceCascade = new CascadeClassifier(cascadePath);

        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5, Size.Empty, new Size(80, 80));
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5, Size.Empty, new Size(80, 80));

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);

        // Resize to larger size
        Size targetSize = new Size(150, 150);
        CvInvoke.Resize(capturedFace, capturedFace, targetSize);
        CvInvoke.Resize(storedFace, storedFace, targetSize);

        // Convert to grayscale
        CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // CLAHE enhancement
        using (CLAHE clahe = new CLAHE(2.0, new Size(8, 8)))
        {
            clahe.Apply(capturedFace, capturedFace);
            clahe.Apply(storedFace, storedFace);
        }

        // Slight blur to reduce noise
        CvInvoke.GaussianBlur(capturedFace, capturedFace, new Size(3, 3), 0);
        CvInvoke.GaussianBlur(storedFace, storedFace, new Size(3, 3), 0);

        // Train recognizer
        using (var recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 50)) // Tighter threshold
        {
            var trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            var labels = new VectorOfInt(new int[] { 1 });

            recognizer.Train(trainingImages, labels);
            var result = recognizer.Predict(capturedFace);

            Console.WriteLine($"Label: {result.Label}, Distance: {result.Distance}");

            // Histogram comparison as a second filter
            double histSimilarity = CompareHistograms(capturedFace, storedFace);

            Console.WriteLine($"Histogram similarity: {histSimilarity}");

            return result.Label == 1 && result.Distance <= 50 && histSimilarity > 0.6;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
        return false;
    }
}
private double CompareHistograms(Mat img1, Mat img2)
{
    DenseHistogram hist1 = new DenseHistogram(256, new RangeF(0, 256));
    DenseHistogram hist2 = new DenseHistogram(256, new RangeF(0, 256));

    hist1.Calculate(new Image<Gray, byte>[] { img1.ToImage<Gray, byte>() }, false, null);
    hist2.Calculate(new Image<Gray, byte>[] { img2.ToImage<Gray, byte>() }, false, null);

    CvInvoke.Normalize(hist1, hist1, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);
    CvInvoke.Normalize(hist2, hist2, 0, 1, Emgu.CV.CvEnum.NormType.MinMax);

    return CvInvoke.CompareHist(hist1, hist2, Emgu.CV.CvEnum.HistogramCompMethod.Correl);
}

 
 
  private bool VerifyFace(Bitmap captured, Bitmap stored)
 {
     try
     {
         // Convert bitmaps to Mat
         Mat matCaptured = BitmapToMat(captured);
         Mat matStored = BitmapToMat(stored);

         // Load the Haarcascade
         string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
         if (!System.IO.File.Exists(cascadePath))
         {
             Console.WriteLine("Error: Haarcascade file not found!");
             return false;
         }

         CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);

         // Detect faces in color image (better performance and accuracy)
         Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5, Size.Empty, new Size(80, 80));
         Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5, Size.Empty, new Size(80, 80));

         if (capturedFaces.Length == 0 || storedFaces.Length == 0)
         {
             Console.WriteLine("No face detected in one or both images.");
             return false;
         }

         // Focus only on the first detected faces
         Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
         Mat storedFace = new Mat(matStored, storedFaces[0]);

         // Convert to grayscale
         CvInvoke.CvtColor(capturedFace, capturedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
         CvInvoke.CvtColor(storedFace, storedFace, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

         // Enhance contrast
         CvInvoke.EqualizeHist(capturedFace, capturedFace);
         CvInvoke.EqualizeHist(storedFace, storedFace);

         // Resize faces for uniformity
         CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
         CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

         // Use tighter parameters for better accuracy
         using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 60)) // Lower distance threshold
         {
             var trainingImages = new VectorOfMat();
             trainingImages.Push(storedFace);
             var labels = new VectorOfInt(new int[] { 1 });

             faceRecognizer.Train(trainingImages, labels);

             var result = faceRecognizer.Predict(capturedFace);
             Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

             return result.Label == 1 && result.Distance <= 60; // Lower threshold for better match quality
         }
     }
     catch (Exception ex)
     {
         Console.WriteLine("Error in face verification: " + ex.Message);
         return false;
     }
 }

 it gives me more false result, it matches with every user give me more better or any other process to use
