private void LoadReport()
        {
            
            DataTable inoData = GetAttendanceData();

          
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("Attendance_Report.rdlc");

           
            ReportViewer1.LocalReport.DataSources.Clear();

         
            ReportDataSource rds = new ReportDataSource("DataSet1", inoData);

          
            ReportViewer1.LocalReport.DataSources.Add(rds);

         
            ReportViewer1.LocalReport.Refresh();
        }

        in this i am calling GetAttendanceData(); and getting this error 
        There is no argument given that corresponds to the required formal parameter 'psrNo' of 'WebForm1.GetAttendanceData(string)'	

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                LoadReport();
            }
        }
