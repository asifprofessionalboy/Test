<form id="deleteForm" method="post" action="/Technical/DeleteDocument">
    <input type="hidden" id="EditId" name="id" value="@Model.Id" />
    <input type="submit" value="Delete" class="btn" id="DeleteBtn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;"/>
</form>




<script>
document.addEventListener("DOMContentLoaded", function () {
    var successMessage = '@TempData["Success"]';
    if (successMessage) {
        Swal.fire({
            title: successMessage,
            icon: "success",
            timer: 3000
        });
    }
});
</script>

<script>
document.addEventListener("DOMContentLoaded", function () {
    var deleteButton = document.getElementById("DeleteBtn");

    deleteButton.addEventListener("click", function (event) {
        event.preventDefault(); // Prevent direct form submission

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to recover this document!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                document.getElementById("deleteForm").submit();
            }
        });
    });

    // Show success alert after reload
    var successMessage = '@TempData["Success"]';
    if (successMessage) {
        Swal.fire({
            title: successMessage,
            icon: "success",
            timer: 3000
        });
    }
});
</script>


this is my controller method for Delete and Modify
		[Authorize(Policy = "CanModify")]
		[HttpPost]
		public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string action, string RefNo = "")
		{
			
			
				if (ModelState.IsValid)
				{
					var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
					if (existingTechnicalService != null)
					{
						
						if (technicalService.Attach != null && technicalService.Attach.Any())
						{
							var uploadPath = configuration["FileUpload:Path"];
							var newAttachments = new List<string>();

							foreach (var file in technicalService.Attach)
							{
								if (file.Length > 0)
								{
									var uniqueId = Guid.NewGuid().ToString();
									var currentDateTime = DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss");
									var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
									var fileExtension = Path.GetExtension(file.FileName);
									var formattedFileName = $"{uniqueId}_{currentDateTime}_{originalFileName}{fileExtension}";
									var fullPath = Path.Combine(uploadPath, formattedFileName);

									using (var stream = new FileStream(fullPath, FileMode.Create))
									{
										await file.CopyToAsync(stream);
									}

									newAttachments.Add(formattedFileName);
								}
							}

							// Replace old attachments with new ones
							existingTechnicalService.Attachment = string.Join(",", newAttachments);
						}

						// Preserve existing attachments if no new file is uploaded
						if (technicalService.Attach == null || !technicalService.Attach.Any())
						{
							technicalService.Attachment = existingTechnicalService.Attachment;
						}

						var CreatedBy = HttpContext.Session.GetString("Session");
						existingTechnicalService.CreatedBy = CreatedBy;
						existingTechnicalService.CreatedOn = DateTime.Now;

						// Update other fields
						existingTechnicalService.RefNo = RefNo;
						existingTechnicalService.FinYear = technicalService.FinYear;
						existingTechnicalService.Month = technicalService.Month;
						existingTechnicalService.Department = technicalService.Department;
						existingTechnicalService.Subject = technicalService.Subject;

						await context.SaveChangesAsync();
						TempData["Message"] = "Document updated successfully!";
					}
				}
			

			return RedirectToAction("EditDocument", "Technical");
		}

[Authorize(Policy = "CanDelete")]
[HttpPost]
public IActionResult DeleteDocument(Guid id)
{
	var Data = context.AppTechnicalServices.FirstOrDefault(c => c.Id == id);
	if (Data != null)
	{
		context.AppTechnicalServices.Remove(Data);
		context.SaveChanges();
		TempData["Message"] = "Customer deleted successfully!";
	}
	
	return RedirectToAction("EditDocument");
}

, these are my two buttons

<input type="submit" value="Save" class="btn" id="SubmitBtn" style="border-radius:7px" />
<input type="submit" value="Delete" class="btn" id="DeleteBtn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;"/>


and these are my js , in this i want that show alerts when successfully data is stored not before 

document.addEventListener("DOMContentLoaded", function () {
	var deleteButton = document.getElementById("DeleteBtn");

	deleteButton.addEventListener("click", function () {
		var editId = document.getElementById("EditId").value;
		if (editId) {
			if (confirm("Are you sure you want to delete this Subject?")) {
				fetch(`/Technical/DeleteDocument/${editId}`, {
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

document.addEventListener("DOMContentLoaded", function () {
	var submitBtn = document.getElementById("SubmitBtn");

	submitBtn.addEventListener("click", function (event) {
		var editId = document.getElementById("EditId").value;
		if (editId) {
			event.preventDefault();

			var isValid = true;
			var elements = this.querySelectorAll('input, select, textarea');

			elements.forEach(function (element) {
				if (element.id === 'ApprovalFile' || element.id === 'fileInput' || element.id === 'dropdown-template' || element.id === 'status' || element.id === 'remarks' || element.id === 'StatusField' || element.id === 'Parameterid' || element.id === 'Paracode' || element.id === 'created' || element.id === 'ScoreId' || element.id === 'scorecode' || element.id === 'actionType' || element.id === 'daretotry' || element.id === 'daretotry-dropdown') {
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
					title: "Data Saved Successfully",
					icon: "success",
					timer: 3000
				}).then(() => {
					this.submit(); // Submit the form for saving
				});
			}
		}
	});
});
