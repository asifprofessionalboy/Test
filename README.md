protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
{
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
    {
        return;
    }

    var userIdString = httpContext.Session.GetString("Session"); // Fetch UserId from Session
    if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
    {
        return;
    }

    // Extract form name dynamically from the request path (controller name)
    string? formName = httpContext.GetRouteData()?.Values["controller"]?.ToString();

    if (string.IsNullOrEmpty(formName))
    {
        return;
    }

    // Fetch FormId from AppFormDetails based on FormName
    var form = await _context.AppFormDetails
        .Where(f => f.FormName == formName)
        .Select(f => new { f.Id })
        .FirstOrDefaultAsync();

    if (form == null)
    {
        return;
    }

    Guid formId = form.Id;

    // Check if the user has permission for this form
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



this is my model for FormDetails
public partial class AppFormDetail
{
    public Guid Id { get; set; }
    public string? FormName { get; set; }
    public string? Description { get; set; }
}

and this is my permission handler , in this formid hard coded but i want to fetch from AppFormDetails and Compare with AppUserFormPermission

  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
  {
      var httpContext = _httpContextAccessor.HttpContext;

      if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
      {
          return;
      }

      var userIdString = httpContext.Session.GetString("ID"); 
      if (string.IsNullOrEmpty(userIdString))
      {
          return;
      }

      Guid userId = Guid.Parse(userIdString); 

     
      var formIdString = "d48679a0-f8ba-4658-bb9e-c564e95da013";
      if (string.IsNullOrEmpty(formIdString))
      {
          return;
      }

      Guid formId = Guid.Parse(formIdString); 

     
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
