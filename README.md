this is my method but when i debug on this line ,CascadeClassifier faceCascade = new CascadeClassifier(cascadePath); it shows the same error on catch 
OpenCV: Input file is invalid. File path is exists but why it showing this?
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

         string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "D:/Irshad_Project/GFAS/GFAS/wwwroot/Cascades/haarcascade_frontalface_default.xml");
         Console.WriteLine($"Cascade Path: {cascadePath}");

         if (!System.IO.File.Exists(cascadePath))
         {
             Console.WriteLine("Error: Haarcascade file not found!");
             return false;
         }

         CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);

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
