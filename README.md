i have this js 	
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
			let message = actionType === "Delete" ? "Data Deleted Successfully" : "Data Saved Successfully";
			let iconColor = actionType === "Delete" ? "#f34848" : "#28a745";

			Swal.fire({
				title: message,
				width: 600,
				padding: "3em",
				color: iconColor,
				background: "#fff",
				backdrop: `rgba(0,0,123,0.4)`,
				timer: 3000
			}).then(() => {
				this.submit();
			});
		}
	});

	function setAction(actionValue) {
		document.getElementById('actionField').value = actionValue;
	}
this is my method 

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

in this for delete i want a confirm message, when i click on confirm then data is deleted shows 


