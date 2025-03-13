[Authorize(Policy = "CanRead")]
public IActionResult TechnicalService()
{
    var viewModel = new AppTechnicalService
    {
        Attach = new List<IFormFile>(),
    };
    
    ViewBag.Department = GetDepartmentDD();
    ViewBag.Month = GetMonthDD();
    ViewBag.Subjects = GetSubjectDD();

    return View(viewModel);
}

[Authorize(Policy = "CanWrite")]
[HttpPost]
public async Task<IActionResult> TechnicalService(AppTechnicalService service)
{
    if (!ModelState.IsValid)
    {
        return View(service);
    }

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

        service.Attachment = service.Attachment.TrimEnd(',');
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

 
 @inject Microsoft.AspNetCore.Authorization.IAuthorizationService AuthorizationService

@{
    var canWrite = (await AuthorizationService.AuthorizeAsync(User, "CanWrite")).Succeeded;
}

<form asp-action="TechnicalService" method="post" enctype="multipart/form-data">
    @* Form fields here *@

    @if (canWrite)
    {
        <button type="submit" class="btn btn-primary">Submit</button>
    }
    else
    {
        <p class="text-danger">You only have read access. You cannot submit this form.</p>
    }
</form>

 
 protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
 {
     var httpContext = _httpContextAccessor.HttpContext;

     if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
     {
         return;
     }

     var userIdString = httpContext.Session.GetString("ID"); 
     if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
     {
         return;
     }

   
     string? formName = httpContext.GetRouteData()?.Values["action"]?.ToString();

     if (string.IsNullOrEmpty(formName))
     {
         return;
     }

   
     var form = await _context.AppFormDetails
         .Where(f => f.FormName == formName)
         .Select(f => new { f.Id })
         .FirstOrDefaultAsync();

     if (form == null)
     {
         return;
     }

     Guid formId = form.Id;

    
     var hasPermission = await _context.AppUserFormPermissions
         .Where(p => p.UserId == userId && p.FormId == formId)
         .AnyAsync(p =>
             (requirement.Permission == "AllowWrite" && p.AllowWrite == true) ||
             (requirement.Permission == "AllowRead" && p.AllowRead == true) ||
             (requirement.Permission == "AllowDelete" && p.AllowDelete == true) ||
             (requirement.Permission == "AllowModify" && p.AllowModify == true)
         );

     if (hasPermission)
     {
         context.Succeed(requirement);
     }
 }
