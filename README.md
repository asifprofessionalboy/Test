
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

    <div class="col-md-12">
        <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px;">
            <div class="row">
                <form asp-action="UploadImage" method="post">
                    <div class="form-group row">
                        <div class="col-sm-1">
                            <label>Pno</label>
                        </div>
                        <div class="col-sm-3">
                            <input id="Pno" name="Pno" class="form-control" type="number" oninput="javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);" maxlength="6" autocomplete="off" required />
                        </div>
                        <div class="col-sm-1">
                            <label>Name</label>
                        </div>
                        <div class="col-sm-3">
                            <input id="Name" name="Name" class="form-control" required />
                        </div>
                        <div class="col-sm-1">
                            <label>Capture Photo</label>
                        </div>
                        <div class="col-sm-3">
                            <video id="video" width="320" height="240" autoplay playsinline></video>
                            <canvas id="canvas" style="display:none;"></canvas>

                          
                            <img id="previewImage" src="" alt="Captured Image" style="width: 200px; display: none; border: 2px solid black; margin-top: 5px;" />

                           
                            <button type="button" id="captureBtn" class="btn btn-primary">Capture</button>
                            <button type="button" id="retakeBtn" class="btn btn-danger" style="display: none;">Retake</button>

                           
                            <input type="hidden" id="photoData" name="photoData" />
                        </div>
                    </div>

                    <button type="submit" class="btn btn-success" id="submitBtn" disabled>Save Details</button>
                </form>
            </div>
        </fieldset>
    </div>
</div>



<script>
   
    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            let video = document.querySelector("video");
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

   


    document.getElementById("captureBtn").addEventListener("click", function () {
        let video = document.getElementById("video");
        let canvas = document.getElementById("canvas");
        let context = canvas.getContext("2d");

       
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        context.translate(canvas.width, 0);
        context.scale(-1, 1);
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        context.setTransform(1, 0, 0, 1, 0, 0);

       
        let imageData = canvas.toDataURL("image/png");
        document.getElementById("previewImage").src = imageData;
        document.getElementById("previewImage").style.display = "block";
        document.getElementById("photoData").value = imageData;

        
        video.style.display = "none";
        document.getElementById("captureBtn").style.display = "none";
        document.getElementById("retakeBtn").style.display = "inline-block";
        document.getElementById("submitBtn").disabled = false; 
    });


based on this logic i want my above code that is storing image of user 
 public IActionResult FaceRecognisation()
 
 {
      string storedImagePath = "wwwroot/Images/stored.jpg";
     string capturedImagePath = "wwwroot/Images/Captured.jpg";

     bool isFaceMatched = CompareFaces(storedImagePath, capturedImagePath);

     if (isFaceMatched)
     {
         Console.WriteLine("Face Matched!");
     }
     else
     {
         Console.WriteLine("Face Does Not Match!");
     }

     return View(); 
 }


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
    
    document.getElementById("retakeBtn").addEventListener("click", function () {
        let video = document.getElementById("video");

        
        video.style.display = "block";
        document.getElementById("captureBtn").style.display = "inline-block";
        document.getElementById("retakeBtn").style.display = "none";
        document.getElementById("previewImage").style.display = "none";
        document.getElementById("submitBtn").disabled = true; 
    });
</script>
