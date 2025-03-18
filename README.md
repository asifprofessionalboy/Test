this is my view 
<form asp-action="SubjectMaster" id="form" asp-controller="Master" method="post">
    <input type="hidden" asp-for="Id" id="SubjectId" name="Id" />

    <div class="row">
                <div class="col-sm-1 d-flex align-items-center">
                    <label for="Subject" class="control-label">Subject</label>
                </div>

                <div class="col-sm-3">
                    <input asp-for="Subject" class="form-control form-control-sm" id="Subject" type="text" autocomplete="off">
                </div>
                <div class="col-sm-1 d-flex align-items-center">
                    <label for="CreatedBy" class="control-label">Created By</label>
                </div>

                <div class="col-sm-3">
                    <input asp-for="CreatedBy" class="form-control form-control-sm" id="CreatedBy" value="@ViewBag.user" type="text" readonly>
                </div>
        <div class="col-sm-1 d-flex align-items-center">
                    <label for="CreatedOn" class="control-label">CreatedOn</label>
                </div>

                <div class="col-sm-3">
                    <input asp-for="CreatedOn" class="form-control form-control-sm" id="CreatedOn" value="@ViewBag.CreatedOn" type="text" readonly>
                </div>
       </div>

    <div class="mt-4 text-center">
        <button class="btn btn-primary" id="submitButton" type="submit">Submit</button>

        <button class="btn btn-danger" id="deleteButton" style="display: none;">Delete</button>

        

    </div>
</form>

this is my js

   document.addEventListener("DOMContentLoaded", function () {
       var newButton = document.getElementById("newButton"); // Ensure this button exists
       var subjectMaster = document.getElementById("SubjectMaster");
       var refNoLinks = document.querySelectorAll(".refNoLink");
       var deleteButton = document.getElementById("deleteButton");

       if (newButton) {
           newButton.addEventListener("click", function () {
               subjectMaster.style.display = "block";
               document.getElementById("Subject").value = "";
              
               document.getElementById("SubjectId").value = ""; // Reset ID
               deleteButton.style.display = "none";
           });
       }

       refNoLinks.forEach(link => {
           link.addEventListener("click", function (event) {
               event.preventDefault();
               subjectMaster.style.display = "block";

               document.getElementById("Subject").value = this.getAttribute("data-subject");
               document.getElementById("CreatedBy").value = this.getAttribute("data-createdBy");
               document.getElementById("CreatedOn").value = this.getAttribute("data-createdOn");
               document.getElementById("SubjectId").value = this.getAttribute("data-id"); // Set ID

               deleteButton.style.display = "inline-block";
           });
       });

       deleteButton.addEventListener("click", function () {
           var subjectId = document.getElementById("SubjectId").value;
           if (subjectId) {
               if (confirm("Are you sure you want to delete this Subject?")) {
                   fetch(`/Master/DeleteSubject/${subjectId}`, {
                       method: "POST"
                   }).then(response => {
                       if (response.ok) {
                           
                           location.reload();
                       } else {
                           alert("Error deleting transaction.");
                       }
                   });
               }
           }
       });
   });

this is my controller logic
[HttpPost]
public async Task<IActionResult> SubjectMaster(AppSubjectMaster model)
{

    var User = HttpContext.Session.GetString("Session");
    if (model == null)
    {
        return BadRequest("Invalid data.");
    }

    if (model.Id == Guid.Empty) 
    {
        model.CreatedBy = User;
        model.CreatedOn = DateTime.Now;
        context.AppSubjectMasters.Add(model);
    }
    else  
    {
        var existingRecord = await context.AppSubjectMasters.FindAsync(model.Id);
        if (existingRecord != null)
        {
            
            existingRecord.Subject = model.Subject;
            existingRecord.CreatedBy = User;
            existingRecord.CreatedOn = DateTime.Now;
            context.AppSubjectMasters.Update(existingRecord);
        }
        else
        {
            return NotFound("Record not found.");
        }
    }

    await context.SaveChangesAsync();
    return RedirectToAction("SubjectMaster");
}

i want same logic for two buttons one is for delete another for update and make authorization policy
