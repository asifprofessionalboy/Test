  public IActionResult ImageViewer()
  {
      var pno = HttpContext.Request.Cookies["Session"];
      var userName = HttpContext.Request.Cookies["UserName"];

      if (string.IsNullOrEmpty(pno) || string.IsNullOrEmpty(userName))
      {
          return RedirectToAction("Login","User");
      }

      var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

     
      var baseImageFile = $"{pno}-{userName}.jpg";
      var baseImagePath = Path.Combine(folderPath, baseImageFile);
      ViewBag.BaseImagePath = System.IO.File.Exists(baseImagePath) ? $"/TSUISLARS/Images/{baseImageFile}" : null;

      
      var capturedImageFile = $"{pno}-Captured.jpg";
      var capturedImagePath = Path.Combine(folderPath, capturedImageFile);
      ViewBag.CapturedImagePath = System.IO.File.Exists(capturedImagePath) ? $"/TSUISLARS/Images/{capturedImageFile}" : null;

      
      string attendanceLocation = "N/A";
      string connectionString = GetRFIDConnectionString();

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
                        <img src="@ViewBag.BaseImagePath" class="img-fluid rounded shadow" style="max-height: 170px;" />
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
                        <img src="@ViewBag.CapturedImagePath" class="img-fluid rounded shadow" style="max-height: 170px;" />
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

image is showing after late, it captures but showing late why please solve it
