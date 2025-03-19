leave everything i have this two buttons 
<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>
    <input type="hidden" name="Type" id="EntryType" />

    <div class="row mt-5 form-group">
        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                Punch In
            </button>
        </div>

        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                Punch Out
            </button>
        </div>
    </div>
</form>

this is my js for VideoCamera to click picture 
<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const form = document.getElementById("form");

    // Start Camera
    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

    function captureImageAndSubmit(entryType) {
        // Set Entry Type (Punch In / Punch Out)
        EntryTypeInput.value = entryType;

        // Capture Image
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        // Convert Image to Base64
        const imageData = canvas.toDataURL("image/png");
        
        // Append Image Data to Form
        const imageInput = document.createElement("input");
        imageInput.type = "hidden";
        imageInput.name = "ImageData";
        imageInput.value = imageData;
        form.appendChild(imageInput);

        // Submit Form
        form.submit();
    }
</script>

this is my controller method when click on PunchIn and punchOut then this method is call
 [HttpPost]
 public IActionResult AttendanceData(string Type, string ImageData)
 {
     if (string.IsNullOrEmpty(ImageData))
     {
         return Json(new { success = false, message = "Image data is missing!" });
     }

     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];
         string Pno = UserId;

        
         byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]);

         using (var ms = new MemoryStream(imageBytes))
         {
             Bitmap capturedImage = new Bitmap(ms);

             var user = context.AppPeople.FirstOrDefault(x => x.Pno == Pno);
             if (user == null || user.Image == null)
             {
                 return Json(new { success = false, message = "User Image Not Found!" });
             }

             using (var storedStream = new MemoryStream(user.Image))
             {
                 Bitmap storedImage = new Bitmap(storedStream);

               
                 bool isFaceMatched = VerifyFace(capturedImage, storedImage);

                 if (!isFaceMatched)
                 {
                     return Json(new { success = false, message = "Face does not match!" });
                 }

                
                 string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                 string currentTime = DateTime.Now.ToString("HH:mm");

                 if (Type == "Punch In")
                 {
                     StoreData(currentDate, currentTime, null, Pno);
                 }
                 else
                 {
                     StoreData(currentDate, null, currentTime, Pno);
                 }

                 return Json(new { success = true, message = "Attendance Marked Successfully!" });
             }
         }
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }

i want in this , that it captures the Image when i click on PunchIn or punchOut and if Face matches then  it executes the StoreData function otherwise show Face is not matching 
i want to use FaceRecognition in place of this 
i have this model where Image is store and compare with this public partial class AppPerson
{
    public Guid Id { get; set; }
    public string? Pno { get; set; }
    public string? Name { get; set; }
    public byte[]? Image { get; set; }
}

if u want any changes for this, please provide and please give me good code for accurate face matching 
