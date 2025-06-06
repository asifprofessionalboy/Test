       protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string from = DateTime.Now.ToString("yyyy/MM/dd");
                fromdate.Text = from;
                string to = DateTime.Now.ToString("yyyy/MM/dd");
                todate.Text = to;
                ReportViewer1.Visible = false;
                BindDepartmentDropdown();
            }
            else if (ViewState["from"] != null && ViewState["to"] != null)
            {
                string from = ViewState["from"].ToString();
                string to = ViewState["to"].ToString();
                string dept = ViewState["dept"]?.ToString();
                string type = ViewState["type"]?.ToString();
                string attempt = ViewState["attempt"]?.ToString();

                LoadReport(from, to, dept, type, attempt);
            }
        }

        private void LoadReport(string from, string to, string dept, string type, string attempt)
        {
            DataTable data = GetGeoData(from, to, dept, type, attempt);

            ReportViewer1.LocalReport.ReportPath = Server.MapPath("FaceRecognition_Report.rdlc");
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("DataSet1", data); 
            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.Refresh();

            ReportViewer1.Visible = true;
        }




        private void BindDepartmentDropdown()
        {
            string query = "SELECT DISTINCT DepartmentName FROM UserLoginDB.dbo.App_EmployeeMaster  where DepartmentName is not Null order by DepartmentName";
            DataTable dt = GetData(query);
            DeptDropdown.DataSource = dt;
            DeptDropdown.DataTextField = "DepartmentName";
            DeptDropdown.DataValueField = "DepartmentName";
            DeptDropdown.DataBind();
            DeptDropdown.Items.Insert(0, new ListItem("-- Select Department --", ""));
        }


        private DataTable GetGeoData(string fromDate, string toDate, string department, string type, string attemptRange)
        {
            bool isPunchIn = type == "PUNCH IN";
            bool isPunchOut = type == "PUNCH OUT";
            bool isBoth = string.IsNullOrEmpty(type); 

           
            string selectClause = isBoth
                ? "DE.Pno, Emp.DepartmentName, DE.DateAndTime, DE.PunchIn_FailedCount, DE.PunchOut_FailedCount"
                : isPunchIn
                    ? "DE.Pno, Emp.DepartmentName, DE.DateAndTime, DE.PunchIn_FailedCount"
                    : "DE.Pno, Emp.DepartmentName, DE.DateAndTime, DE.PunchOut_FailedCount";

            string query = $@"
        SELECT {selectClause}
        FROM App_FaceVerification_Details AS DE
        INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
            ON DE.Pno COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
        WHERE CAST(DE.DateAndTime AS DATE) BETWEEN @FromDate AND @ToDate
    ";

            if (!string.IsNullOrEmpty(department))
            {
                query += " AND Emp.DepartmentName = @Department";
            }

           
            int minAttempt = 0, maxAttempt = 0;
            if (!string.IsNullOrEmpty(attemptRange) && attemptRange.Contains("-"))
            {
                var parts = attemptRange.Split('-');
                minAttempt = int.Parse(parts[0]);
                maxAttempt = parts[1].ToLower() == "above" ? int.MaxValue : int.Parse(parts[1]);

                if (isPunchIn)
                    query += " AND DE.PunchIn_FailedCount BETWEEN @MinAttempt AND @MaxAttempt";
                else if (isPunchOut)
                    query += " AND DE.PunchOut_FailedCount BETWEEN @MinAttempt AND @MaxAttempt";
                else
                    query += " AND (DE.PunchIn_FailedCount BETWEEN @MinAttempt AND @MaxAttempt OR DE.PunchOut_FailedCount BETWEEN @MinAttempt AND @MaxAttempt)";
            }

            query += " ORDER BY DE.DateAndTime";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    if (!string.IsNullOrEmpty(department))
                        cmd.Parameters.AddWithValue("@Department", department);

                    if (maxAttempt > 0)
                    {
                        cmd.Parameters.AddWithValue("@MinAttempt", minAttempt);
                        cmd.Parameters.AddWithValue("@MaxAttempt", maxAttempt);
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable GetData(string query)
        {

            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.Connection = con;
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }



        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string from = fromdate.Text.Trim();
            string to = todate.Text.Trim();
            string dept = DeptDropdown.SelectedValue;
            string type = DropDownList1.SelectedValue;
            string attempt = DropDownList2.SelectedValue;

            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                // Store values in ViewState for postback
                ViewState["from"] = from;
                ViewState["to"] = to;
                ViewState["dept"] = dept;
                ViewState["type"] = type;
                ViewState["attempt"] = attempt;

                LoadReport(from, to, dept, type, attempt);
            }
        }


it reloads again and again , i think it goes to infinite loop or something 
