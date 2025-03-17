[Authorize]
[HttpPost]
public async Task<IActionResult> EditDocument(AppTechnicalService technicalService, string action, string RefNo = "")
{
    var userIdString = HttpContext.Session.GetString("ID");
    if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
    {
        return RedirectToAction("AccessDenied");
    }

    string formName = "EditDocument"; 
    var form = await context.AppFormDetails
        .Where(f => f.FormName == formName)
        .Select(f => f.Id)
        .FirstOrDefaultAsync();

    if (form == default)
    {
        return RedirectToAction("AccessDenied");
    }

    bool canModify = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowModify == true);

    bool canDelete = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowDelete == true);

    if (action == "Delete")
    {
        if (!canDelete)
        {
            return RedirectToAction("AccessDenied");
        }

        var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
        if (existingTechnicalService != null)
        {
            context.AppTechnicalServices.Remove(existingTechnicalService);
            await context.SaveChangesAsync();
            TempData["Message"] = "Document deleted successfully!";
        }
    }
    else if (action == "Save")
    {
        if (!canModify)
        {
            return RedirectToAction("AccessDenied");
        }

        if (ModelState.IsValid)
        {
            var existingTechnicalService = await context.AppTechnicalServices.FindAsync(technicalService.Id);
            if (existingTechnicalService != null)
            {
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

@inject IAuthorizationService AuthorizationService

<div class="text-center">
    <input type="hidden" name="action" id="actionField" />

    @if (ViewBag.CanModify == true)
    {
        <input type="submit" value="Save" class="btn" style="border-radius:7px" onclick="setAction('Save')" />
    }

    @if (ViewBag.CanDelete == true)
    {
        <input type="submit" value="Delete" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" onclick="setAction('Delete')" />
    }
</div>
[Authorize]
public async Task<IActionResult> EditDocument(Guid id)
{
    var userIdString = HttpContext.Session.GetString("ID");
    if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
    {
        return RedirectToAction("AccessDenied");
    }

    string formName = "EditDocument"; // Use the actual action name
    var form = await context.AppFormDetails
        .Where(f => f.FormName == formName)
        .Select(f => f.Id)
        .FirstOrDefaultAsync();

    if (form == default)
    {
        return RedirectToAction("AccessDenied");
    }

    bool canModify = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowModify == true);

    bool canDelete = await context.AppUserFormPermissions
        .Where(p => p.UserId == userId && p.FormId == form)
        .AnyAsync(p => p.AllowDelete == true);

    ViewBag.CanModify = canModify;
    ViewBag.CanDelete = canDelete;

    var technicalService = await context.AppTechnicalServices.FindAsync(id);
    if (technicalService == null)
    {
        return NotFound();
    }

    return View(technicalService);
}
 
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

<script>
    document.addEventListener("DOMContentLoaded", function () {
        var message = "@TempData["Message"]";
        if (message) {
            Swal.fire({
                title: message,
                icon: "success",
                timer: 3000
            });
        }
    });

    function setAction(actionValue) {
        document.getElementById('actionField').value = actionValue;
    }
</script>
 
 
 
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
