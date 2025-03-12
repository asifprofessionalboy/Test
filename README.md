using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect to Login if unauthorized
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect if access denied
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session timeout
        options.SlidingExpiration = true; // Renew session on activity
    });

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanRead", policy => policy.Requirements.Add(new PermissionRequirement("Form-Read")));
    options.AddPolicy("CanWrite", policy => policy.Requirements.Add(new PermissionRequirement("Form-Write")));
    options.AddPolicy("CanDelete", policy => policy.Requirements.Add(new PermissionRequirement("Form-Delete")));
    options.AddPolicy("CanModify", policy => policy.Requirements.Add(new PermissionRequirement("Form-Modify")));
});

// Register authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// Add controllers with default authorization
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy)); // Ensure all controllers require authentication
});

var app = builder.Build();

// Middleware setup
app.UseAuthentication(); // Ensure authentication is enabled
app.UseAuthorization(); // Ensure authorization policies are enforced

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();




[HttpPost]
public async Task<IActionResult> Login(AppLogin login, string returnUrl = null)
{
    if (!string.IsNullOrEmpty(login.UserId) && string.IsNullOrEmpty(login.Password))
    {
        ViewBag.FailedMsg = "Login Failed: Password is required";
        return View(login);
    }

    var user = await context.AppLogins
        .Where(x => x.UserId == login.UserId)
        .FirstOrDefaultAsync();

    if (user != null)
    {
        bool isPasswordValid = hash_Password.VerifyPassword(login.Password, user.Password, user.PasswordSalt);

        if (isPasswordValid)
        {
            var UserLoginData = await context1.AppEmployeeMasters
                .Where(x => x.Pno == login.UserId)
                .FirstOrDefaultAsync();

            string userName = UserLoginData?.Ename ?? "Guest";

            // Fetch user permissions from the database
            var userPermissions = await context.AppUserFormPermissions
                .Where(x => x.UserId == user.UserId) // Ensure it matches logged-in user
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Pno", UserLoginData?.Pno ?? "N/A")
            };

            // Add permissions to claims
            foreach (var permission in userPermissions)
            {
                claims.Add(new Claim($"Form-{permission.FormId}-Read", permission.AllowRead.ToString()));
                claims.Add(new Claim($"Form-{permission.FormId}-Write", permission.AllowWrite.ToString()));
                claims.Add(new Claim($"Form-{permission.FormId}-Delete", permission.AllowDelete?.ToString() ?? "false"));
                claims.Add(new Claim($"Form-{permission.FormId}-Modify", permission.AllowModify?.ToString() ?? "false"));
                claims.Add(new Claim($"Form-{permission.FormId}-All", permission.AllowAll?.ToString() ?? "false"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            HttpContext.Session.SetString("Session", UserLoginData?.Pno ?? "N/A");
            HttpContext.Session.SetString("UserName", userName);
            HttpContext.Session.SetString("UserSession", login.UserId);

            return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Homepage", "Technical");
        }
        else
        {
            ViewBag.FailedMsg = "Login Failed: Incorrect password";
        }
    }
    else
    {
        ViewBag.FailedMsg = "Login Failed: User not found";
    }

    return View(login);
}

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
 
 public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == requirement.Permission && c.Value == "True"))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanRead", policy => policy.Requirements.Add(new PermissionRequirement("Form-Read")));
    options.AddPolicy("CanWrite", policy => policy.Requirements.Add(new PermissionRequirement("Form-Write")));
    options.AddPolicy("CanDelete", policy => policy.Requirements.Add(new PermissionRequirement("Form-Delete")));
    options.AddPolicy("CanModify", policy => policy.Requirements.Add(new PermissionRequirement("Form-Modify")));
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

[Authorize(Policy = "CanRead")]
public IActionResult Read()
{
    return View();
}

[Authorize(Policy = "CanWrite")]
public IActionResult Create()
{
    return View();
}

[Authorize(Policy = "CanDelete")]
public IActionResult Delete()
{
    return View();
}

[Authorize(Policy = "CanModify")]
public IActionResult Update()
{
    return View();
}

@if (User.HasClaim(c => c.Type == "Form-Read" && c.Value == "True"))
{
    <a href="/Controller/Read" class="btn btn-primary">View</a>
}

@if (User.HasClaim(c => c.Type == "Form-Write" && c.Value == "True"))
{
    <a href="/Controller/Create" class="btn btn-success">Create</a>
}

@if (User.HasClaim(c => c.Type == "Form-Modify" && c.Value == "True"))
{
    <a href="/Controller/Update" class="btn btn-warning">Edit</a>
}

@if (User.HasClaim(c => c.Type == "Form-Delete" && c.Value == "True"))
{
    <a href="/Controller/Delete" class="btn btn-danger">Delete</a>
}

 
 public partial class AppUserFormPermissionViewModel
 {
     public Guid UserId { get; set; }

     public List<AppUserFormPermission> FormPermissions { get; set; }
 }

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

this is my login logic 


        [HttpPost]
        public async Task<IActionResult> Login(AppLogin login, string returnUrl = null)
        {

            if (!string.IsNullOrEmpty(login.UserId) && string.IsNullOrEmpty(login.Password))
            {
                ViewBag.FailedMsg = "Login Failed: Password is required";
                return View(login);
            }


            var user = await context.AppLogins
                .Where(x => x.UserId == login.UserId)
                .FirstOrDefaultAsync();

            if (user != null)
            {

                bool isPasswordValid = hash_Password.VerifyPassword(login.Password, user.Password, user.PasswordSalt);

                if (isPasswordValid)
                {
                    var UserLoginData = await context1.AppEmployeeMasters.
                        Where(x => x.Pno == login.UserId).FirstOrDefaultAsync();

                    string userName = UserLoginData?.Ename ?? "Guest";



                    HttpContext.Session.SetString("Session", UserLoginData?.Pno ?? "N/A");
                    HttpContext.Session.SetString("UserName", UserLoginData?.Ename ?? "Guest");
                    HttpContext.Session.SetString("UserSession", login.UserId);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Homepage", "Technical");
                    }
                }
                else
                {
                    ViewBag.FailedMsg = "Login Failed: Incorrect password";
                }
            }
            else
            {
                ViewBag.FailedMsg = "Login Failed: User not found";
            }

            return View(login);
        }

now how to do it 
