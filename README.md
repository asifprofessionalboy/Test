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
