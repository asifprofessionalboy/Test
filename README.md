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
         
          ViewBag.ImagePath = $"/Images/{fileName}";
      }
      else
      {
          ViewBag.ImagePath = null;
      }

      return View();
  }

this is my view side 
<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-4 text-center">
            <div class="card shadow-lg border-0">
                <div class="card-body">
                    <h4 class="card-title mb-4">Base Image</h4>

                    @if (!string.IsNullOrEmpty(ViewBag.ImagePath))
                    {
                        <img src="@ViewBag.ImagePath" class="img-fluid rounded shadow" style="max-height: 200px;" />
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
        <div class="col-md-4 text-center">
            <div class="card shadow-lg border-0">
                <div class="card-body">
                    <h4 class="card-title mb-4">Current Captured Image</h4>

                    @if (!string.IsNullOrEmpty(ViewBag.ImagePath))
                    {
                        <img src="@ViewBag.ImagePath" class="img-fluid rounded shadow" style="max-height: 200px;" />
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
        <div class="col-md-4 text-center">
            <div class="card shadow-lg border-0">
                <div class="card-body">
                    <h4 class="card-title mb-4">Attendance Location</h4>
                    <p>Corporate Office</p>
                </div>
            </div>
        </div>
    </div>
</div>

and this is my query to fetch user's location 
SELECT ps.Worksite FROM TSUISLRFIDDB.DBO.App_Position_Worksite AS ps 
                     INNER JOIN TSUISLRFIDDB.DBO.App_Emp_position AS es ON es.position = ps.position 
                     WHERE es.Pno = @UserId


in this i want that base image logic will be same it fetches like 15151-shashikumar.jpg now i want current captured image as like 151514-Captured.jpg and set Attendance location using the query
