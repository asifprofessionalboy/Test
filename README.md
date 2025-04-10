[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadImage(string Pno, string Name, string photoData)
{
    if (!string.IsNullOrEmpty(photoData) && !string.IsNullOrEmpty(Pno) && !string.IsNullOrEmpty(Name))
    {
        try
        {
            // Convert base64 to byte array (remove the 'data:image/jpeg;base64,' part if present)
            byte[] imageBytes = Convert.FromBase64String(photoData.Split(',')[1]);

            // Define folder path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");

            // Create directory if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Create file name and path
            string fileName = $"{Pno}-{Name}.jpg";
            string filePath = Path.Combine(folderPath, fileName);

            // Save image file to wwwroot/Images
            System.IO.File.WriteAllBytes(filePath, imageBytes);

            // Save person info in the database
            var person = new AppPerson
            {
                Pno = Pno,
                Name = Name,
                Image = fileName
            };

            context.AppPeople.Add(person);
            await context.SaveChangesAsync();

            // Return success response for AJAX
            return Ok(new { success = true, message = "Image uploaded and data saved successfully." });
        }
        catch (Exception ex)
        {
            // Log exception if needed
            return StatusCode(500, new { success = false, message = "Error saving image: " + ex.Message });
        }
    }

    return BadRequest(new { success = false, message = "Missing required fields!" });
}

document.getElementById('form2').addEventListener('submit', function (event) {
    event.preventDefault();

    var isValid = true;
    var form = this;
    var elements = form.querySelectorAll('input, select, textarea');

    elements.forEach(function (element) {
        if (['ApprovalFile'].includes(element.id)) {
            return;
        }

        if (element.value.trim() === '') {
            isValid = false;
            element.classList.add('is-invalid');
        } else {
            element.classList.remove('is-invalid');
        }
    });

    if (isValid) {
        // Show loading
        Swal.fire({
            title: "Uploading...",
            text: "Please wait while your image is being uploaded.",
            didOpen: () => {
                Swal.showLoading();
            },
            allowOutsideClick: false,
            allowEscapeKey: false
        });

        // Prepare form data
        const formData = new FormData(form);

        fetch(form.action, {
            method: 'POST',
            body: formData
        })
        .then(response => {
            if (response.redirected) {
                window.location.href = response.url; // handle redirect if needed
            } else if (response.ok) {
                Swal.fire({
                    title: "Success!",
                    text: "Data Saved Successfully",
                    icon: "success",
                    confirmButtonText: "OK"
                });
            } else {
                throw new Error("Upload failed.");
            }
        })
        .catch(error => {
            Swal.fire("Error", "There was an error uploading the image: " + error.message, "error");
        });
    }
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

             return RedirectToAction("UploadImage","User");
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

and this is my js

  document.getElementById('form2').addEventListener('submit', function (event) {
      event.preventDefault();


      var isValid = true;
      var elements = this.querySelectorAll('input, select, textarea');

     

      elements.forEach(function (element) {
         
          if (['ApprovalFile'].includes(element.id)) {
              return;
          }

       
          if (element.value.trim() === '') {
              isValid = false;
              element.classList.add('is-invalid');
          } else {
              element.classList.remove('is-invalid');
          }
      });




     
      if (isValid) {
          Swal.fire({
              title: "Success!",
              text: "Data Saved Successfully",
              icon: "success",
              confirmButtonText: "OK"
          }).then((result) => {
              if (result.isConfirmed) {
                  document.getElementById('form2').submit();
              }
          });
      }
  });

i want that when data is stored success after that success alert shows until process it shows loading 
