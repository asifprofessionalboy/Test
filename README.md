[HttpPost]
public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string RefNo = "")
{
    if (ModelState.IsValid)
    {
        var existingtechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
        if (existingtechnicalService != null)
        {
            // If new files are uploaded, replace the previous attachments
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
                existingtechnicalService.Attachment = string.Join(",", newAttachments);
            }

            // Preserve existing attachments if no new file is uploaded
            if (technicalService.Attach == null || !technicalService.Attach.Any())
            {
                technicalService.Attachment = existingtechnicalService.Attachment;
            }

            var CreatedBy = HttpContext.Session.GetString("Session");
            existingtechnicalService.CreatedBy = CreatedBy;
            existingtechnicalService.CreatedOn = DateTime.Now;

            // Set only necessary values, avoiding overwriting Attachment
            existingtechnicalService.RefNo = RefNo;
            existingtechnicalService.SomeOtherProperty = technicalService.SomeOtherProperty; // Update other fields as needed

            await context.SaveChangesAsync();
            return RedirectToAction("EditDocument", "Technical");
        }
    }

    return View(technicalService);
}




[HttpPost]
public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string RefNo = "")
{
    if (ModelState.IsValid)
    {
        var existingtechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
        if (existingtechnicalService != null)
        {
            // If new files are uploaded, replace the previous attachments
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
                existingtechnicalService.Attachment = string.Join(",", newAttachments);
            }

            // Preserve existing attachments if no new file is uploaded
            technicalService.Attachment ??= existingtechnicalService.Attachment;

            var CreatedBy = HttpContext.Session.GetString("Session");
            existingtechnicalService.CreatedBy = CreatedBy;
            existingtechnicalService.CreatedOn = DateTime.Now;

            context.Entry(existingtechnicalService).CurrentValues.SetValues(technicalService);
            existingtechnicalService.RefNo = RefNo;

            await context.SaveChangesAsync();
            return RedirectToAction("EditDocument", "Technical");
        }
    }

    return View(technicalService);
}



this to shows attachment to my view 
				<div class="col-sm-1 align-items-center">
					<label asp-for="Attach" class="control-label">Attachment </label>
				</div>
				<div class="col-sm-3">
					@if (!string.IsNullOrEmpty(Model.Attachment))
					{
						<div>
							<ul>
								@foreach (var fileName in Model.Attachment.Split(','))
								{
									var cleanFileName = ExtractFileName(fileName);
									var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
									var isPdf = fileExtension == ".pdf";

									<li>
										<a href="@Url.Action("DownloadFile", new { fileName = fileName })"
										   target="_blank">
											@cleanFileName
										</a>
									</li>
								}
							</ul>
						</div>
					}
				</div>
				@functions {
				private string ExtractFileName(string fileNameWithPrefix)
				{

					var parts = fileNameWithPrefix.Split('_');
					if (parts.Length > 1)
					{
						return parts[parts.Length - 1];
					}
					return fileNameWithPrefix;
				}
}

this is to add new attachment , if the user wants to edit the attachment 
<div class="col-sm-1 align-items-center">
				<label asp-for="Attach" class="control-label">Attachment </label>
</div>
<div class="col-sm-3">
				<input asp-for="Attach" type="file" class="form-control form-control-sm" multiple id="fileInput" />
				<span asp-validation-for="Attach" class="text-danger"></span>

</div>


this is my controller post method 
[HttpPost]
public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string RefNo = "")
{
	if (ModelState.IsValid)
	{
		var existingtechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
		if (existingtechnicalService != null)
		{
			if (technicalService.Attach != null && technicalService.Attach.Any())
			{
				var uploadPath = configuration["FileUpload:Path"];
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

						technicalService.Attachment += $"{formattedFileName},";
					}
				}

				if (!string.IsNullOrEmpty(technicalService.Attachment))
				{
					technicalService.Attachment = technicalService.Attachment.TrimEnd(',');
				}
			}

           

            var CreatedBy = HttpContext.Session.GetString("Session");

			technicalService.CreatedBy = CreatedBy;
			technicalService.CreatedOn = DateTime.Now;




			context.Entry(existingtechnicalService).CurrentValues.SetValues(technicalService);
			technicalService.RefNo = RefNo;
			await context.SaveChangesAsync();
            return RedirectToAction("EditDocument", "Technical");
            
        }
		
	}

    return View(technicalService);


}
n this i want that if the user add new attachment then it replaces the previous attachment ,if he doesnot want to edit attachment , attachment will remain same , currently it stores both attachment like this 
https://localhost:7073/Technical/DownloadFile?fileName=098c5816-0c38-4da2-8bf8-8da5583ff2e9_03-02-2025_11-16-28_InnovationRPT%20(8).pdfd7b4b9b8-1e6b-4dc4-ae2f-842690958e65_05-02-2025_06-53-05_FNF_Final_Report%20(1).pdf
