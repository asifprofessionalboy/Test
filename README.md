private DataTable GetAttendanceData(string psrNo)
{
    string query = @"
        SELECT PDE_PUNCHDATE,
            MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END) AS PunchInTime,
            MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END) AS PunchOutTime
        FROM vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS  
        WHERE PDE_PSRNO = @PsrNo
        GROUP BY PDE_PUNCHDATE 
        ORDER BY PDE_PUNCHDATE DESC";

    DataTable dt = new DataTable();

    using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
    {
        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@PsrNo", psrNo);  // Dynamic User ID

            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                sda.Fill(dt);
            }
        }
    }

    return dt;
}

public IActionResult AttendanceReport(string url)
{
    // Retrieve UserId (PsrNo) from cookies
    string psrNo = HttpContext.Request.Cookies["Session"]; 

    if (string.IsNullOrEmpty(psrNo))
    {
        return RedirectToAction("Login", "User"); // Redirect if not logged in
    }

    // If URL is missing, construct Attendance Report URL with PsrNo
    if (string.IsNullOrEmpty(url))
    {
        url = $"https://servicesdev.juscoltd.com/AttendanceReport/Webform1.aspx?pno={psrNo}";
    }
    else
    {
        url += $"?pno={psrNo}";
    }

    ViewBag.ReportUrl = url; // Pass URL to View
    return View();
}

function redirectToIframePage() {
    window.location.href = "/GeoFencing/Geo/AttendanceReport"; 
}
<div class="container">
    <iframe src="@ViewBag.ReportUrl" width="100%" height="600px" frameborder="0" class="report"></iframe>
</div>



