this is my face recognition logic , it works perfectly on day light but in night some times it is not catching perfectly   
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


          using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 98))
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
