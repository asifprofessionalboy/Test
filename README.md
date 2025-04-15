this is my cookies authentication 

builder.Services.AddAuthentication("Cookies")
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/User/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated)
    {
        var userId = context.Request.Cookies["Session"];
        var UserName = context.Request.Cookies["UserName"];
        var Pno = context.Request.Cookies["UserSession"];
        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Pno))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,UserName),
                new Claim("Pno",Pno),
                new Claim("Session",userId)
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

        }
    }
        await next();
});

 [HttpPost]
 public async Task<IActionResult> Login(AppLogin login)
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
             HttpContext.Session.SetString("UserSession",login.UserId);

             //store cookies

             var cookieOptions = new CookieOptions
             {
                 Expires = DateTimeOffset.Now.AddDays(1),
                 HttpOnly = false,
                 Secure = true,
                 IsEssential = true
             };

             Response.Cookies.Append("UserSession", login.UserId, cookieOptions);
             Response.Cookies.Append("Session", UserLoginData?.Pno ?? "N/A", cookieOptions);
             Response.Cookies.Append("UserName", UserLoginData?.Ename ?? "Guest", cookieOptions);






            
                 return RedirectToAction("GeoFencing", "Geo");
             
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

is this the logic is creating problem to for not creating my MAUI url , it redirects to http?
