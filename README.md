this is my view 
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

store as a jpg image on wwwroot/Images and image name will be like this if the pno is 151514 and name is Irshad on textboxes then save as 151514-Irshad.Jpg
