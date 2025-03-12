using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect to login if not authenticated
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect if access is denied
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanWrite", policy => policy.Requirements.Add(new PermissionRequirement("AllowWrite")));
});

// Register custom permission handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor(); // Required for accessing session

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly AppDbContext _context;  
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
        {
            return;
        }

        var userIdString = httpContext.Session.GetString("Session"); // Get logged-in UserId
        if (string.IsNullOrEmpty(userIdString))
        {
            return;
        }

        Guid userId = Guid.Parse(userIdString); // Convert UserId to GUID

        // Get the current FormId (you need to pass this from the controller)
        var formIdString = httpContext.Items["FormId"]?.ToString();
        if (string.IsNullOrEmpty(formIdString))
        {
            return;
        }

        Guid formId = Guid.Parse(formIdString); // Convert FormId to GUID

        // Check if user has required permission
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
}

[Authorize(Policy = "CanWrite")]
public IActionResult TechnicalService()
{
    var formId = new Guid("d48679a0-f8ba-4658-bb9e-c564e95da013"); // Set the correct FormId for this page

    HttpContext.Items["FormId"] = formId; // Pass the FormId to the middleware

    var viewModel = new AppTechnicalService
    {
        Attach = new List<IFormFile>(),
    };

    ViewBag.Department = GetDepartmentDD();
    ViewBag.Month = GetMonthDD();
    ViewBag.Subjects = GetSubjectDD();

    return View(viewModel);
}



i have this model for Permission of the user 
public partial class AppUserFormPermission
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FormId { get; set; }
    public bool AllowRead { get; set; }
    public bool AllowWrite { get; set; }
    public bool? AllowDelete { get; set; }
    public bool? AllowAll { get; set; }
    public bool? AllowModify { get; set; }
    public bool DownTime { get; set; }
}

this is the data in my table 
dffd1b83-9359-4f69-883b-a23247084ed3	15fae783-d72d-468b-b75c-e388dc0f9b7e	d48679a0-f8ba-4658-bb9e-c564e95da013	True	True	True	True	True	False

every form has there FormId , and UserId of the , check if the user has the permission of the page , if the permission then he can open the page but if not then shows access denied
