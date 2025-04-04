using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

public class FaceRecognition
{
    private readonly string modelPath;
    private readonly string protoPath;
    private readonly string cascadePath;

    public FaceRecognition()
    {
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades");
        modelPath = Path.Combine(basePath, "res10_300x300_ssd_iter_140000_fp16.caffemodel");
        protoPath = Path.Combine(basePath, "deploy.prototxt");
        cascadePath = Path.Combine(basePath, "haarcascade_frontalface_default.xml");

        if (!File.Exists(modelPath) || !File.Exists(protoPath) || !File.Exists(cascadePath))
        {
            throw new FileNotFoundException("One or more model files not found.");
        }
    }

    public bool VerifyFace(Bitmap captured, Bitmap stored)
    {
        try
        {
            Mat matCaptured = BitmapToMat(captured);
            Mat matStored = BitmapToMat(stored);

            // Convert to grayscale
            CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

            // Apply Face Detection using DNN Model
            Mat capturedFace = DetectFace(matCaptured);
            Mat storedFace = DetectFace(matStored);

            if (capturedFace == null || storedFace == null)
            {
                Console.WriteLine("No face detected in one or both images.");
                return false;
            }

            // Extract Face Features (128-D Vectors)
            float[] capturedEmbedding = GetFaceEmbedding(capturedFace);
            float[] storedEmbedding = GetFaceEmbedding(storedFace);

            // Compute Euclidean Distance
            double distance = CalculateEuclideanDistance(capturedEmbedding, storedEmbedding);
            Console.WriteLine($"Euclidean Distance: {distance}");

            // If distance is less than 0.6, consider it a match
            return distance < 0.6;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in face verification: " + ex.Message);
            return false;
        }
    }

    private Mat DetectFace(Mat image)
    {
        using (CascadeClassifier faceCascade = new CascadeClassifier(cascadePath))
        {
            Rectangle[] faces = faceCascade.DetectMultiScale(image, 1.1, 5);

            if (faces.Length == 0)
                return null;

            return new Mat(image, faces[0]); // Crop the detected face
        }
    }

    private float[] GetFaceEmbedding(Mat face)
    {
        Net faceNet = DnnInvoke.ReadNetFromCaffe(protoPath, modelPath);
        Mat blob = DnnInvoke.BlobFromImage(face, 1.0, new Size(300, 300), new MCvScalar(104, 177, 123));

        faceNet.SetInput(blob);
        Mat output = faceNet.Forward();

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
        Mat mat = new Mat();
        using (Image<Bgr, byte> image = bitmap.ToImage<Bgr, byte>())
        {
            mat = image.Mat;
        }
        return mat;
    }
}




private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert to grayscale
        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // Apply Bilateral Filter (preserves edges)
        CvInvoke.BilateralFilter(matCaptured, matCaptured, 5, 75, 75);
        CvInvoke.BilateralFilter(matStored, matStored, 5, 75, 75);

        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
        if (!System.IO.File.Exists(cascadePath))
        {
            Console.WriteLine("Error: Haarcascade file not found!");
            return false;
        }

        CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 4); // Keeping minNeighbors at 4
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 4);

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);

        // Resize slightly larger for better detection
        CvInvoke.Resize(capturedFace, capturedFace, new Size(110, 110));
        CvInvoke.Resize(storedFace, storedFace, new Size(110, 110));

        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100)) // Keep threshold at 100
        {
            CvInvoke.EqualizeHist(capturedFace, capturedFace);
            CvInvoke.EqualizeHist(storedFace, storedFace);

            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);
            var result = faceRecognizer.Predict(capturedFace);

            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            // Set an optimal distance threshold (95-105)
            return result.Label == 1 && result.Distance <= 100;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}




private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert to grayscale
        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // Apply Gaussian Blur to reduce noise
        CvInvoke.GaussianBlur(matCaptured, matCaptured, new Size(3, 3), 0);
        CvInvoke.GaussianBlur(matStored, matStored, new Size(3, 3), 0);

        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
        if (!System.IO.File.Exists(cascadePath))
        {
            Console.WriteLine("Error: Haarcascade file not found!");
            return false;
        }

        CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 3); // Lowered minNeighbors to detect smaller faces
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 3);

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);

        // Increase the resizing size for better matching
        CvInvoke.Resize(capturedFace, capturedFace, new Size(120, 120));
        CvInvoke.Resize(storedFace, storedFace, new Size(120, 120));

        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 120)) // Increased threshold
        {
            CvInvoke.EqualizeHist(capturedFace, capturedFace);
            CvInvoke.EqualizeHist(storedFace, storedFace);

            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);
            var result = faceRecognizer.Predict(capturedFace);

            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            // Increase the matching distance to allow more variations
            return result.Label == 1 && result.Distance <= 120;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
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

             return result.Label == 1 && result.Distance <= 98;
         }
     }
     catch (Exception ex)
     {
         Console.WriteLine("Error in face verification: " + ex.Message);
         return false;
     }
 }
