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
