public IActionResult ImageViewer()
{
    var pno = HttpContext.Request.Cookies["Session"];
    var userName = HttpContext.Request.Cookies["UserName"];

    if (string.IsNullOrEmpty(pno) || string.IsNullOrEmpty(userName))
    {
        return RedirectToAction("Login"); // or handle unauthenticated access
    }

    var fileName = $"{pno}-{userName}.jpg";
    var imagePath = $"/Images/{fileName}";

    ViewBag.ImagePath = imagePath;

    return View();
}

@{
    var imagePath = ViewBag.ImagePath as string;
}

@if (!string.IsNullOrEmpty(imagePath))
{
    <div class="text-center">
        <img src="@imagePath" alt="User Image" style="max-width: 300px;" />
    </div>
}
else
{
    <p>No image available.</p>
}



i have this view , i want to fetch Image according to Pno and Name , i have folder wwwroot/Images that contains images of every user like this  151514-Shashi Kumar.jpg if the user visit on ImageViewer then it shows his image
public IActionResult ImageViewer()
 {
     var session = HttpContext.Request.Cookies["Session"];
     var userName = HttpContext.Request.Cookies["UserName"];


     return View();
 }

