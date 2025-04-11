i have this view , i want to fetch Image according to Pno and Name , i have folder wwwroot/Images that contains images of every user like this  151514-Shashi Kumar.jpg if the user visit on ImageViewer then it shows his image
public IActionResult ImageViewer()
 {
     var session = HttpContext.Request.Cookies["Session"];
     var userName = HttpContext.Request.Cookies["UserName"];


     return View();
 }

