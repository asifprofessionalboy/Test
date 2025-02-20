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

        // Save the record to generate RefNo
        context.AppTechnicalServices.Add(appTechnicalService);
        await context.SaveChangesAsync();

        // Fetch the saved record to get the auto-generated RefNo
        var savedService = await context.AppTechnicalServices
            .Where(x => x.CreatedBy == CreatedOn && x.Subject == service.Subject)
            .OrderByDescending(x => x.Id) // Assuming Id is an auto-incremented field
            .FirstOrDefaultAsync();

        if (savedService == null)
        {
            return BadRequest("Error retrieving saved Technical Service record.");
        }

        // Now, use savedService.RefNo for notifications
        var allUsers = context.AppLogins.Select(x => x.UserId).ToList();

        var notifications = allUsers.Select(userId => new AppNotification
        {
            RefNo = savedService.RefNo, // Now RefNo has a value
            Pno = userId,
            Subject = service.Subject,
            IsViewed = false
        }).ToList();

        await context.AppNotifications.AddRangeAsync(notifications);
        await context.SaveChangesAsync();

        return RedirectToAction("TechnicalService", "Technical");
    }

    return View(service);
}





in this controller method 
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

         
          var allUsers = context.AppLogins.Select(x => x.UserId).ToList();

          var notifications = allUsers.Select(userId => new AppNotification
          {
             
              RefNo = service.RefNo, 
              Pno = userId,  
              Subject = service.Subject,
              IsViewed = false
          }).ToList();

          await context.AppNotifications.AddRangeAsync(notifications);
          await context.SaveChangesAsync();

          return RedirectToAction("TechnicalService", "Technical");
      }

      return View(service);
  }


I am getting RefNo null , RefNo is AutoGen Number when Data is Saved 

context.AppTechnicalServices.Add(appTechnicalService);
          await context.SaveChangesAsync();

