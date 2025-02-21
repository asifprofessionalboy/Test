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

		// Save data first
		context.AppTechnicalServices.Add(appTechnicalService);
		await context.SaveChangesAsync();

		// Call SubmitNotification **correctly**
		await SubmitNotification();

		return RedirectToAction("TechnicalService", "Technical");
	}

	return View(service);
}

// Make SubmitNotification a private method
private async Task SubmitNotification()
{
	var savedService = await context.AppTechnicalServices
			.OrderByDescending(x => x.Id)
			.FirstOrDefaultAsync();

	if (savedService == null || string.IsNullOrEmpty(savedService.RefNo))
	{
		throw new Exception("Error: RefNo is NULL after saving.");
	}

	var allUsers = await context.AppLogins.Select(x => x.UserId).ToListAsync();

	var notifications = allUsers.Select(userId => new AppNotification
	{
		RefNo = savedService.RefNo,
		Pno = userId,
		Subject = savedService.Subject,
		IsViewed = false
	}).ToList();

	await context.AppNotifications.AddRangeAsync(notifications);
	await context.SaveChangesAsync();
}




i want this type of approach for this logic , is this good?
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


		SubmitNotification();

		return RedirectToAction("TechnicalService", "Technical");
	}

	return View(service);
}


[HttpPost]

public async Task<IActionResult> SubmitNotification()
{
	var savedService = await context.AppTechnicalServices
			.OrderByDescending(x => x.Id)
			.FirstOrDefaultAsync();

	if (savedService == null || string.IsNullOrEmpty(savedService.RefNo))
	{
		throw new Exception("Error: RefNo is NULL after saving.");
	}


	var allUsers = await context.AppLogins.Select(x => x.UserId).ToListAsync();

	var notifications = allUsers.Select(userId => new AppNotification
	{
		RefNo = savedService.RefNo,
		Pno = userId,
		Subject = savedService.Subject,
		IsViewed = false
	}).ToList();

	await context.AppNotifications.AddRangeAsync(notifications);
	await context.SaveChangesAsync();

	return RedirectToAction("TechnicalServices");
} 
