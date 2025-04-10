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
