this is my query in report 
 private DataTable GetAttendanceData()
        {
            string query = @"
       select PDE_PUNCHDATE,
min(case when PDE_INOUT like '%I%' then PDE_PUNCHTIME end ) as punchintime,
max(case when PDE_INOUT like '%O%' then PDE_PUNCHTIME end) as Punchouttime
from vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS  where PDE_PSRNO = '151514'
group by PDE_PUNCHDATE order by PDE_PUNCHDATE DESC
    ";

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

this is my method in another application, in this i want psr no using Cookies 
var UserId = HttpContext.Request.Cookies["Session"];

 public IActionResult AttendanceReport(string url)
 {
     if (string.IsNullOrEmpty(url))
     {
         return RedirectToAction("Login", "User"); 
     }

     ViewBag.ReportUrl = url; 
     return View();
 }

function redirectToIframePage() {
    var iframeUrl = encodeURIComponent("https://servicesdev.juscoltd.com/AttendanceReport/Webform1.aspx");
    window.location.href = "/GeoFencing/Geo/AttendanceReport?url=" + iframeUrl;
}

<div class="text-center">
    <button onclick="redirectToIframePage()" class="btn btn-primary mt-2">Check your Attendance</button>
</div>
