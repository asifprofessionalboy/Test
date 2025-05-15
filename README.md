private readonly YourDbContext _context; // Injected via constructor

public YourController(YourDbContext context)
{
    _context = context;
}

public IActionResult YourAction()
{
    var allowedPnos = _context.AppPermissionMaster
                         .Select(x => x.Pno)
                         .ToList();

    var userId = Request.Cookies["Session"];

    if (!allowedPnos.Contains(userId))
    {
        return RedirectToAction("Login", "User");
    }

    // Proceed if allowed
    return View();
}

ViewBag.AllowedPnos = allowedPnos;
return View();
@{
    var userId = HttpContextAccessor.HttpContext.Request.Cookies["Session"] ?? "N/A";
    var allowedPnos = ViewBag.AllowedPnos as List<string>;
}

@if (allowedPnos != null && allowedPnos.Contains(userId))
{
    <p>Welcome, authorized user!</p>
}
p


i have this model 
 public partial class AppPermissionMaster
 {
     public Guid Id { get; set; }
     public string? Pno { get; set; }
 }

and this is my code 

if (UserId != "151515" && UserId!="151514" && UserId != "155478" && UserId != "159566")
{
    return RedirectToAction("Login", "User");
}


i have also this 

@inject Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor

@{
    var userId = HttpContextAccessor.HttpContext.Request.Cookies["Session"] ?? "N/A";
    var UserName = HttpContextAccessor.HttpContext.Request.Cookies["UserName"] ?? "Guest";
}


    @if (userId == "151514" || userId == "155478" || userId == "151515"|| userId == "159566")
{

}


i want to fetch Pno from AppPermissionMaster and in place of hard code i want that all the pno that is stored in Pno give permission to those
