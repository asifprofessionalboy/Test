private bool VerifyFace(Bitmap captured, Bitmap stored)
{
    try
    {
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert images to grayscale
        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // Load Haarcascade for face detection
        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Cascades", "haarcascade_frontalface_default.xml");
        var faceCascade = new CascadeClassifier(cascadePath);

        // Detect faces
        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        // Extract faces and resize
        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);
        CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
        CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

        // Train face recognizer
        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
        {
            // Convert to grayscale for better recognition
            CvInvoke.EqualizeHist(capturedFace, capturedFace);
            CvInvoke.EqualizeHist(storedFace, storedFace);

            // Train with stored image
            VectorOfMat trainingImages = new VectorOfMat();
            trainingImages.Push(storedFace);
            VectorOfInt labels = new VectorOfInt(new int[] { 1 });

            faceRecognizer.Train(trainingImages, labels);

            // Predict using captured image
            var result = faceRecognizer.Predict(capturedFace);
            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            return result.Label == 1 && result.Distance < 70;  // Adjust distance threshold
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
        // Convert Bitmap to Mat
        Mat matCaptured = BitmapToMat(captured);
        Mat matStored = BitmapToMat(stored);

        // Convert to grayscale
        CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

        // Load pre-trained face detector dynamically
        string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Cascades", "haarcascade_frontalface_default.xml");
        if (!System.IO.File.Exists(cascadePath))
        {
            Console.WriteLine("Haarcascade XML file not found: " + cascadePath);
            return false;
        }

        var faceCascade = new CascadeClassifier(cascadePath);

        // Detect faces in both images
        Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
        Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);

        if (capturedFaces.Length == 0 || storedFaces.Length == 0)
        {
            Console.WriteLine("No face detected in one or both images.");
            return false;
        }

        // Crop and resize faces
        Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
        Mat storedFace = new Mat(matStored, storedFaces[0]);
        CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
        CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

        // Train the LBPH recognizer
        using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
        {
            faceRecognizer.Train(new List<Mat> { storedFace }, new List<int> { 1 });

            var result = faceRecognizer.Predict(capturedFace);
            Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

            return result.Label == 1 && result.Distance < 80; // Adjust threshold if needed
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error in face verification: " + ex.Message);
        return false;
    }
}




this is i am storing first in Table and comparing with this image
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadImage(string Pno, string Name, string photoData)
{
    if (!string.IsNullOrEmpty(photoData))
    {
       
        byte[] imageBytes = Convert.FromBase64String(photoData.Split(',')[1]);

        var person = new AppPerson
        {
            Pno = Pno, 
            Name = Name,
            Image = imageBytes 
        };

        context.AppPeople.Add(person);
        await context.SaveChangesAsync();
        return RedirectToAction("GeoFencing");
    }

    return View();
}

and it is 
 private bool VerifyFace(Bitmap captured, Bitmap stored)
 {
     try
     {
        
         Mat matCaptured = BitmapToMat(captured);
         Mat matStored = BitmapToMat(stored);

       
         CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
         CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

       
         var faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

        
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
             VectorOfMat trainingImages = new VectorOfMat();
             trainingImages.Push(matStored);

             VectorOfInt labels = new VectorOfInt(new int[] { 1 });

             faceRecognizer.Train(trainingImages, labels);


             var result = faceRecognizer.Predict(capturedFace);

             Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

             return result.Label == 1 && result.Distance < 80; 
         }
     }
     catch (Exception ex)
     {
         Console.WriteLine("Error in face verification: " + ex.Message);
         return false;
     }
 }

and it is saying face doesnot match , please provide better testing. i am testing in Mobile phone . in my pc there is no camera that is why i want phone testing that what the problem is 
