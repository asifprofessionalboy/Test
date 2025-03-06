this is my report side 
 protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                LoadReport();
            }
        }

        private void LoadReport()
        {
            
            DataTable inoData = GetAttendanceData();

          
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("Attendance_Report.rdlc");

           
            ReportViewer1.LocalReport.DataSources.Clear();

         
            ReportDataSource rds = new ReportDataSource("DataSet1", inoData);

          
            ReportViewer1.LocalReport.DataSources.Add(rds);

         
            ReportViewer1.LocalReport.Refresh();
        }

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
        } ,

        this is in another app view side 
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
 <div class="container">
    <iframe src="@ViewBag.ReportUrl" width="200%" height="600px" frameborder="0" class="report"></iframe>
</div>
 function redirectToIframePage() {
     window.location.href = "/GeoFencing/Geo/AttendanceReport";
 }
 <div class="text-center">
    <button onclick="redirectToIframePage()" class="btn btn-primary mt-2">Check your Attendance</button>
</div>
i want only that why i am getting error on Load Report 
Severity	Code	Description	Project	File	Line	Suppression State
Error	CS7036	There is no argument given that corresponds to the required formal parameter 'psrNo' of 'WebForm1.GetAttendanceData(string)'	AspNet-Rdlc	C:\Users\AEUPC9300H\Desktop\GeoFencingReport\AspNet-Rdlc\AspNet-Rdlc\WebForm1.aspx.cs	27	Active
