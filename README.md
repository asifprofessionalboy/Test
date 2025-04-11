public IActionResult ImageViewer()
{
    var pno = HttpContext.Request.Cookies["Session"];
    var userName = HttpContext.Request.Cookies["UserName"];

    if (string.IsNullOrEmpty(pno) || string.IsNullOrEmpty(userName))
    {
        return RedirectToAction("Login");
    }

    var fileName = $"{pno}-{userName}.jpg";
    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
    var imageFilePath = Path.Combine(folderPath, fileName);

    if (System.IO.File.Exists(imageFilePath))
    {
        // IMPORTANT: Include virtual directory in the image URL
        ViewBag.ImagePath = $"/TSUISLARS/Images/{fileName}";
    }
    else
    {
        ViewBag.ImagePath = null;
    }

    return View();
}

@if (!string.IsNullOrEmpty(ViewBag.ImagePath))
{
    <img src="@ViewBag.ImagePath" class="img-fluid rounded shadow" style="max-height: 400px;" />
}
else
{
    <div class="alert alert-warning mt-3">
        No image available for this user.
    </div>
}



@{
    ViewData["Title"] = "Image Viewer";
    var imagePath = ViewBag.ImagePath as string;
    var userName = Context.Request.Cookies["UserName"];
}

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-6 text-center">
            <div class="card shadow-lg border-0">
                <div class="card-body">
                    <h4 class="card-title mb-4">Welcome, @userName</h4>

                    @if (!string.IsNullOrEmpty(imagePath))
                    {
                        <img src="@imagePath" alt="User Image" class="img-fluid rounded shadow" style="max-height: 400px;" />
                    }
                    else
                    {
                        <div class="alert alert-warning mt-3">
                            No image available for this user.
                        </div>
                    }
                </div>
            </div>
        </div>
    </div>
</div>

using Microsoft.AspNetCore.Mvc;
using System.IO;

public class YourControllerName : Controller
{
    public IActionResult ImageViewer()
    {
        var pno = HttpContext.Request.Cookies["Session"];
        var userName = HttpContext.Request.Cookies["UserName"];

        if (string.IsNullOrEmpty(pno) || string.IsNullOrEmpty(userName))
        {
            return RedirectToAction("Login"); // Handle unauthenticated access
        }

        var fileName = $"{pno}-{userName}.jpg";
        var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imageFilePath = Path.Combine(wwwRootPath, "Images", fileName);

        if (System.IO.File.Exists(imageFilePath))
        {
            ViewBag.ImagePath = $"/Images/{fileName}";
        }
        else
        {
            ViewBag.ImagePath = null; // or use a placeholder like "/Images/default.jpg"
        }

        return View();
    }
}



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

