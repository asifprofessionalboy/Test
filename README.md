i have this controller logic for delete and Update 
[Authorize(Policy = "CanModify")]
[HttpPost]
public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string action, string RefNo = "")
{
	if (action == "Delete")
	{
		var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
		if (existingTechnicalService != null)
		{
			context.AppTechnicalServices.Remove(existingTechnicalService);
			await context.SaveChangesAsync();
			TempData["Message"] = "Document deleted successfully!";
			return RedirectToAction("EditDocument", "Technical");
		}
	}
	else if (action == "Save")
	{
		if (ModelState.IsValid)
		{
			var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
			if (existingTechnicalService != null)
			{
				// If new files are uploaded, replace previous attachments
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
	}

	return RedirectToAction("EditDocument", "Technical");
}
i have these two buttons in same view 

<div class="text-center">
				<input type="hidden" name="action" id="actionField" />

				<input type="submit" value="Save" class="btn" style="border-radius:7px" onclick="setAction('Save')" />
				<input type="submit" value="Delete" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" onclick="setAction('Delete')" />

</div>

and this is my js

	document.getElementById('form2').addEventListener('submit', function (event) {
		event.preventDefault();

		var isValid = true;
		var elements = this.querySelectorAll('input, select, textarea');
		var actionType = document.getElementById('actionField').value; // Get action type

		elements.forEach(function (element) {
			if ([
				'ApprovalFile', 'Id', 'fileInput', 'dropdown-template', 'status', 'remarks',
				'StatusField', 'Parameterid', 'Paracode', 'created', 'ScoreId', 'scorecode',
				'actionType', 'daretotry', 'daretotry-dropdown'
			].includes(element.id)) {
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
			if (actionType === "Delete") {
				// Show confirmation popup for delete
				Swal.fire({
					title: "Are you sure?",
					text: "You won't be able to revert this!",
					icon: "warning",
					showCancelButton: true,
					confirmButtonColor: "#d33",
					cancelButtonColor: "#3085d6",
					confirmButtonText: "Yes, delete it!"
				}).then((result) => {
					if (result.isConfirmed) {
						Swal.fire({
							title: "Deleted!",
							text: "Data Deleted Successfully",
							icon: "success",
							timer: 3000
						}).then(() => {
							event.target.submit(); // Submit the form after confirmation
						});
					}
				});
			} else {
				// Success message for saving data
				Swal.fire({
					title: "Data Saved Successfully",
					icon: "success",
					timer: 3000
				}).then(() => {
					event.target.submit(); // Submit the form for saving
				});
			}
		}
	});

	// Function to set the action type dynamically
	function setAction(actionValue) {
		document.getElementById('actionField').value = actionValue;
	}
in this i want that sweetalert success message shows after data is stored . and i want in these two buttons to include Authorize policy of Delete and Modify. because of two buttons i am understanding what can i do

