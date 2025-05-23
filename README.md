 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> EmployeePositionMaster([FromBody] AppEmpPosition appPosition, [FromQuery] string actionType)
 {
    

     if (string.IsNullOrEmpty(actionType))
     {
         return BadRequest("No action specified.");
     }

     var existingParameter = await context.AppEmpPositions.FindAsync(appPosition.Id);

     var UserId = HttpContext.Request.Cookies["Session"];

     if (actionType == "Submit")
     {
         if (!ModelState.IsValid)
         {
             return BadRequest(ModelState);
         }

         var duplicate = await context.AppEmpPositions
               .Where(x => x.Position == appPosition.Position&&x.Pno==appPosition.Pno)
               .Where(x => x.Id != appPosition.Id) // Exclude self during update
               .FirstOrDefaultAsync();

         if (duplicate != null)
         {
             //TempData["DuplicateMsg"] = "Duplicate position exists for the same worksite!";
             return Ok("Duplicate position exists for the same Personal No!");
         }

         if (existingParameter != null)
         {
             context.Entry(existingParameter).CurrentValues.SetValues(appPosition);
             await context.SaveChangesAsync();
             return Ok("Updated");
         }
         else
         {
             await context.AppEmpPositions.AddAsync(appPosition);
             await context.SaveChangesAsync();
             return Ok("Created");
         }
     }
     else if (actionType == "Delete")
     {
         if (existingParameter != null)
         {
             context.AppEmpPositions.Remove(existingParameter);
             await context.SaveChangesAsync();
             return Ok("Deleted");
         }
     }

     return BadRequest("Invalid action.");
 }


  $('#submitButton').click(function (e) {
      e.preventDefault();

      // Validate form fields
      if (!validateForm()) {
          Swal.fire({
              title: 'Validation Error',
              text: 'Please fill in all required fields.',
              icon: 'warning',
              confirmButtonColor: '#3085d6'
          });
          return;
      }

      const id = $('#LocationId').val();
      const pno = $('#Pno').val().trim();
      const position = $('#Position').val().trim();

      $.ajax({
          url: '@Url.Action("EmployeePositionMaster", "Master")' + '?actionType=Submit',
          type: 'POST',
          contentType: 'application/json',
          headers: {
              'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
          },
          data: JSON.stringify({
              Id: id,
              Pno: pno,
              Position: position
          }),
          success: function (response) {
              Swal.fire({
                  title: 'Success!',
                  text: 'Position saved successfully.',
                  icon: 'success',
                  confirmButtonColor: '#3085d6'
              }).then(() => {
                  $('#formContainer').hide();
                  location.reload();
              });
          },
          error: function (xhr) {
              Swal.fire(
                  'Error!',
                  'An error occurred while saving the position.',
                  'error'
              );
              console.error(xhr.responseText);
          }
      });
  });

after duplicate it shows me position Saved Successfully it catches the duplicate value but because of this jquery i am getting this issue
