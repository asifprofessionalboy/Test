this is my query in my report 
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

this is my view method in my another Application ,

 public IActionResult AttendanceReport()
 {
     if (HttpContext.Session.GetString("Session") != null)
     {

     }
     else
     {
         return RedirectToAction("Login", "User");
     }
     return View();
 }
and i am calling the report using Iframe , i want to make this dynamic using session, which of the user login in query it sets the pno
 where PDE_PSRNO = '151514'


 <iframe src="@iframeUrl" width="100%" height="600px" frameborder="0"></iframe>
