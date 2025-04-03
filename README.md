document.addEventListener("DOMContentLoaded", function () {
    const pnoInput = document.getElementById("Pno");
    const nameInput = document.getElementById("Name");
    const photoDataInput = document.getElementById("photoData");
    const captureBtn = document.getElementById("captureBtn");
    const retakeBtn = document.getElementById("retakeBtn");
    const submitBtn = document.getElementById("submitBtn");
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const previewImage = document.getElementById("previewImage");

    // Disable submit button initially
    submitBtn.disabled = true;

    // **🔹 Validate form fields**
    function validateForm() {
        if (pnoInput.value.trim() !== "" && nameInput.value.trim() !== "" && photoDataInput.value.trim() !== "") {
            submitBtn.disabled = false;  // Enable the submit button
        } else {
            submitBtn.disabled = true;   // Keep it disabled if any field is empty
        }
    }

    // **🔹 Capture Image Logic**
    captureBtn.addEventListener("click", function () {
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        // Convert image to Base64
        const imageData = canvas.toDataURL("image/jpeg");

        // Show preview image
        previewImage.src = imageData;
        previewImage.style.display = "block";

        // Store the image data
        photoDataInput.value = imageData;

        // Hide capture button, show retake button
        captureBtn.style.display = "none";
        retakeBtn.style.display = "inline-block";

        // Validate form after capturing
        validateForm();
    });

    // **🔹 Retake Photo Logic**
    retakeBtn.addEventListener("click", function () {
        previewImage.style.display = "none";  // Hide preview image
        photoDataInput.value = "";            // Clear hidden input
        captureBtn.style.display = "inline-block"; // Show capture button
        retakeBtn.style.display = "none";     // Hide retake button
        validateForm();
    });

    // **🔹 Validate on input change**
    pnoInput.addEventListener("input", validateForm);
    nameInput.addEventListener("input", validateForm);

    // **🔹 SweetAlert on successful submission**
    document.querySelector("form").addEventListener("submit", function (event) {
        event.preventDefault(); // Prevent default form submission

        Swal.fire({
            title: "Saving Details...",
            text: "Please wait...",
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        // Simulate form submission delay
        setTimeout(() => {
            Swal.fire({
                title: "Success!",
                text: "Details saved successfully.",
                icon: "success",
                timer: 3000,
                showConfirmButton: false
            }).then(() => {
                this.submit(); // Submit the form after SweetAlert
            });
        }, 2000);
    });
});

 
 
 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> UploadImage(string Pno, string Name, string photoData)
 {
     if (!string.IsNullOrEmpty(photoData) && !string.IsNullOrEmpty(Pno) && !string.IsNullOrEmpty(Name))
     {
         try
         {
            
             byte[] imageBytes = Convert.FromBase64String(photoData.Split(',')[1]);

            
             string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");

          
             if (!Directory.Exists(folderPath))
             {
                 Directory.CreateDirectory(folderPath);
             }

           
             string fileName = $"{Pno}-{Name}.jpg";
             string filePath = Path.Combine(folderPath, fileName);

            
             System.IO.File.WriteAllBytes(filePath, imageBytes);

            
             var person = new AppPerson
             {
                 Pno = Pno,
                 Name = Name,
                 Image = $"{fileName}" 
             };

             context.AppPeople.Add(person);
             await context.SaveChangesAsync();

             return RedirectToAction("UploadImage");
         }
         catch (Exception ex)
         {
             ModelState.AddModelError("", "Error saving image: " + ex.Message);
         }
     }
     else
     {
         ModelState.AddModelError("", "Missing required fields!");
     }

     return View();
 }

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
                            <input id="Pno" name="Pno" class="form-control" type="number" oninput="javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);" maxlength="6" autocomplete="off" required />
                        </div>
                        <div class="col-sm-1">
                            <label>Name</label>
                        </div>
                        <div class="col-sm-3">
                            <input id="Name" name="Name" class="form-control" required readonly/>
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

i want js validation for required fields and after success i want a sweetalert alert after success
