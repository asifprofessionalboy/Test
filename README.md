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
