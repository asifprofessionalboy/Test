<div class="card rounded-9">
    <div class="card-header text-center" style="background-color: #bbb8bf;color: #000000;font-weight:bold;">
        Capture Photo
    </div>
    <div class="col-md-12">
        <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px;">
            <div class="row">
                <form asp-action="UploadImage" method="post">
                    <div class="form-group row">
                        <div class="col-sm-1">
                            <label>Pno</label>
                        </div>
                        <div class="col-sm-3">
                            <input id="Pno" name="Pno" class="form-control" required />
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
                            <video id="video" width="200" height="150" autoplay></video>
                            <canvas id="canvas" style="display:none;"></canvas>

                            <!-- Preview of Captured Image -->
                            <img id="previewImage" src="" alt="Captured Image" style="width: 200px; display: none; border: 2px solid black; margin-top: 5px;" />

                            <!-- Buttons to Capture and Retake Photo -->
                            <button type="button" id="captureBtn" class="btn btn-primary">Capture</button>
                            <button type="button" id="retakeBtn" class="btn btn-danger" style="display: none;">Retake</button>

                            <!-- Hidden input to store captured photo -->
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
    // Start the camera when the page loads
    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            document.getElementById("video").srcObject = stream;
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

    document.getElementById("captureBtn").addEventListener("click", function () {
        let video = document.getElementById("video");
        let canvas = document.getElementById("canvas");
        let context = canvas.getContext("2d");

        // Capture the image from the video feed
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        // Convert the captured image to Base64
        let imageData = canvas.toDataURL("image/png");
        document.getElementById("previewImage").src = imageData;
        document.getElementById("previewImage").style.display = "block";
        document.getElementById("photoData").value = imageData;

        // Hide video and capture button, Show Retake button
        video.style.display = "none";
        document.getElementById("captureBtn").style.display = "none";
        document.getElementById("retakeBtn").style.display = "inline-block";
        document.getElementById("submitBtn").disabled = false; // Enable submit button
    });

    // Retake Photo Functionality
    document.getElementById("retakeBtn").addEventListener("click", function () {
        let video = document.getElementById("video");

        // Show video and capture button again
        video.style.display = "block";
        document.getElementById("captureBtn").style.display = "inline-block";
        document.getElementById("retakeBtn").style.display = "none";
        document.getElementById("previewImage").style.display = "none";
        document.getElementById("submitBtn").disabled = true; // Disable submit button
    });
</script>

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadImage(string Pno, string Name, string photoData)
{
    if (!string.IsNullOrEmpty(photoData))
    {
        // Convert Base64 to byte array
        byte[] imageBytes = Convert.FromBase64String(photoData.Split(',')[1]);

        var person = new Person
        {
            Pno = Guid.Parse(Pno), // Convert string Pno to GUID
            Name = Name,
            Photo = imageBytes // Store Image as Byte Array
        };

        context.Persons.Add(person);
        await context.SaveChangesAsync();
        return RedirectToAction("SuccessPage");
    }
    
    return View();
}



this is used for storing users image and pno and name , i want in this not attachment, i want to use camera to capture the photo then it stored
<div class="card rounded-9">
    <div class="card-header text-center" style="background-color: #bbb8bf;color: #000000;font-weight:bold;">
        Upload Image
    </div>
    <div class="col-md-12">
        <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px;">
            <div class="row">
<form asp-action="UploadImage" method="post" enctype="multipart/form-data">
    <div class="form-group row">
        <div class="col-sm-1">
                            <label asp-for="Pno">Pno</label>
        </div>
                        <div class="col-sm-3">
                            <input asp-for="Pno" class="form-control" />
                        </div>
                        <div class="col-sm-1">
                            <label asp-for="Name">Name</label>
                        </div>
                        <div class="col-sm-3">
                            <input asp-for="Name" class="form-control" required />
                        </div>
                        <div class="col-sm-1">
                            <label>Upload Photo</label>
                        </div>
                        <div class="col-sm-3">
                            <input type="file" id="photoInput" name="photoFile" class="form-control" accept="image/*" required />
                        </div>
       
       
    </div>

  

    <div class="form-group">
        <img id="previewImage" src="" alt="Image Preview" style="width: 200px; display: none;" />
    </div>

    <button type="submit" class="btn btn-primary">Save Details</button>
</form>
</div>
</fieldset>
</div>
</div>
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadImage(AppPerson person, IFormFile photoFile)
{
    if (ModelState.IsValid)
    {
        if (photoFile != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await photoFile.CopyToAsync(memoryStream);
                person.Image = memoryStream.ToArray(); // Convert Image to Byte Array
            }
        }

        context.AppPeople.Add(person);
        await context.SaveChangesAsync();
        return RedirectToAction("");
    }
    return View(person);
}

<script>
    document.getElementById("photoInput").addEventListener("change", function (event) {
        let reader = new FileReader();
        reader.onload = function () {
            let img = document.getElementById("previewImage");
            img.src = reader.result;
            img.style.display = "block";
        };
        reader.readAsDataURL(event.target.files[0]);
    });
</script>
