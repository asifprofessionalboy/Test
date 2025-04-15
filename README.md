List<string> locationList = new List<string>();

if (!string.IsNullOrEmpty(attendanceLocation))
{
    locationList = attendanceLocation
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Trim())
                    .Where(l => !string.IsNullOrEmpty(l))
                    .ToList();
}

ViewBag.AttendanceLocations = locationList;

@if (ViewBag.AttendanceLocations != null && ((List<string>)ViewBag.AttendanceLocations).Any())
{
    <ol class="text-start">
        @foreach (var location in (List<string>)ViewBag.AttendanceLocations)
        {
            <li>@location</li>
        }
    </ol>
}
else
{
    <p>No location available.</p>
}



public IActionResult ImageViewer()
{
    var pno = HttpContext.Request.Cookies["Session"];
    var userName = HttpContext.Request.Cookies["UserName"];

    if (string.IsNullOrEmpty(pno) || string.IsNullOrEmpty(userName))
    {
        return RedirectToAction("Login");
    }

    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

    // Base image: Pno-Username.jpg
    var baseImageFile = $"{pno}-{userName}.jpg";
    var baseImagePath = Path.Combine(folderPath, baseImageFile);
    ViewBag.BaseImagePath = System.IO.File.Exists(baseImagePath) ? $"/Images/{baseImageFile}" : null;

    // Captured image: Pno-Captured.jpg
    var capturedImageFile = $"{pno}-Captured.jpg";
    var capturedImagePath = Path.Combine(folderPath, capturedImageFile);
    ViewBag.CapturedImagePath = System.IO.File.Exists(capturedImagePath) ? $"/Images/{capturedImageFile}" : null;

    // Fetch worksite (attendance location)
    string attendanceLocation = "N/A";
    string connectionString = "YourConnectionStringHere";

    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        conn.Open();
        string query = @"
            SELECT ps.Worksite 
            FROM TSUISLRFIDDB.DBO.App_Position_Worksite AS ps
            INNER JOIN TSUISLRFIDDB.DBO.App_Emp_position AS es ON es.position = ps.position
            WHERE es.Pno = @UserId";

        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@UserId", pno);
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                attendanceLocation = result.ToString();
            }
        }
    }

    ViewBag.AttendanceLocation = attendanceLocation;

    return View();
}

<div class="container mt-5">
    <div class="row justify-content-center">
        <!-- Base Image -->
        <div class="col-md-4 text-center">
            <div class="card shadow-lg border-0">
                <div class="card-body">
                    <h4 class="card-title mb-4">Base Image</h4>

                    @if (!string.IsNullOrEmpty(ViewBag.BaseImagePath))
                    {
                        <img src="@ViewBag.BaseImagePath" class="img-fluid rounded shadow" style="max-height: 200px;" />
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

        <!-- Captured Image -->
        <div class="col-md-4 text-center">
            <div class="card shadow-lg border-0">
                <div class="card-body">
                    <h4 class="card-title mb-4">Current Captured Image</h4>

                    @if (!string.IsNullOrEmpty(ViewBag.CapturedImagePath))
                    {
                        <img src="@ViewBag.CapturedImagePath" class="img-fluid rounded shadow" style="max-height: 200px;" />
                    }
                    else
                    {
                        <div class="alert alert-warning mt-3">
                            No captured image available.
                        </div>
                    }
                </div>
            </div>
        </div>

        <!-- Attendance Location -->
        <div class="col-md-4 text-center">
            <div class="card shadow-lg border-0">
                <div class="card-body">
                    <h4 class="card-title mb-4">Attendance Location</h4>
                    <p>@ViewBag.AttendanceLocation</p>
                </div>
            </div>
        </div>
    </div>
</div>
  
  
  
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
