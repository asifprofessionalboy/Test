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

        var CreatedOn = HttpContext.Session.GetString("Session");

        var appTechnicalService = new AppTechnicalService
        {
            Department = service.Department,
            Subject = service.Subject,
            FinYear = service.FinYear,
            CreatedBy = CreatedOn,
            Attachment = service.Attachment,
            Month = service.Month
        };

        context.AppTechnicalServices.Add(appTechnicalService);
        await context.SaveChangesAsync();

        // **Fetching all UserIds from AppLogin**
        var allUsers = context.AppLogins.Select(x => x.UserId).ToList();

        // **Inserting notification for each user**
        var notifications = allUsers.Select(userId => new AppNotification
        {
            Id = Guid.NewGuid(),
            RefNo = userId,  // Using UserId as RefNo
            Pno = userId,  // Using UserId as Pno
            Subject = service.Subject,
            ChildSubject = "Technical Service Notification", // Define your child subject
            IsViewed = false // Default to false
        }).ToList();

        await context.AppNotifications.AddRangeAsync(notifications);
        await context.SaveChangesAsync();

        return RedirectToAction("TechnicalService", "Technical");
    }

    return View(service);
}



this is my controller method 
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
		var CreatedOn = HttpContext.Session.GetString("Session");

		var appTechnicalService = new AppTechnicalService
		{
			
			Department = service.Department,
			Subject = service.Subject,
			FinYear = service.FinYear,
			CreatedBy = CreatedOn,
			Attachment = service.Attachment,
			Month = service.Month

		};
		context.AppTechnicalServices.Add(appTechnicalService);
		await context.SaveChangesAsync();

		return RedirectToAction("TechnicalService", "Technical");
	}
	return View(service);
}

this is my login Model 
public partial class AppLogin
 {
     public Guid Id { get; set; }
     public string UserId { get; set; } = null!;
     public string Password { get; set; } = null!;
     }


and this is for my notification info 
 public partial class AppNotification
 {
     public Guid Id { get; set; }
     public string? RefNo { get; set; }
     public string? Pno { get; set; }
     public string? Subject { get; set; }
     public string? ChildSubject { get; set; }
     public bool? IsViewed { get; set; }
 }

i want that fetch all userId from AppLogin and Insert each row for AppNotification , if i have userId 151514 then insert a row for this user id , All values RefNo,Pno is userid, Subject Child Subject , IsViewed in from Technical Service Controller Method  
