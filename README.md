i have these 2 methods, One is for view and other is for Button submit. i want that if the user has permission for CanWrite then submit otherwise shows AccessDenied. if read then he can only view the Viewer side

[Authorize(Policy = "CanWrite")]
public IActionResult TechnicalService()
{

    var viewModel = new AppTechnicalService
		{
			Attach = new List<IFormFile>(),

		};
		var Dept = GetDepartmentDD();
        ViewBag.Department = Dept;

		var Month = GetMonthDD();
		ViewBag.Month = Month;

        var subjectDDs = GetSubjectDD();
        ViewBag.Subjects = subjectDDs;


        return View(viewModel);

}
[Authorize(Policy = "CanWrite")]
[HttpPost]
public async Task<IActionResult> TechnicalService(AppTechnicalService service)
{
	if (ModelState.IsValid)
	{
		if (service.Attach != null && service.Attach.Any())
		{
			var uploadPath = configuration["FileUpload:Path"];
			foreach (var file in service.Attach)
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

					service.Attachment += $"{formattedFileName},";
				}
			}

			if (!string.IsNullOrEmpty(service.Attachment))
			{
				service.Attachment = service.Attachment.TrimEnd(',');
			}
		}

		var User = HttpContext.Session.GetString("Session");

		var appTechnicalService = new AppTechnicalService
		{
			Department = service.Department,
			Subject = service.Subject,
			FinYear = service.FinYear,
			CreatedBy = User,
			Attachment = service.Attachment,
			Month = service.Month
		};

		
		context.AppTechnicalServices.Add(appTechnicalService);
		await context.SaveChangesAsync();
		await context.Entry(appTechnicalService).ReloadAsync();

		
		await SubmitNotification();

		return RedirectToAction("TechnicalService", "Technical");
	}

	return View(service);
}
