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
