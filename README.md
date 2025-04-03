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
