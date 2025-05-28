private DataTable GetGeoData(string condition, string department)
{
    string query = @"select DE.Pno, Emp.DepartmentName, DE.DateAndTime, 
                            DE.PunchIn_FailedCount, DE.PunchOut_FailedCount 
                     from App_FaceVerification_Details As DE 
                     INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
                     ON DE.Pno COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT 
                     where CAST(DateAndTime as Date) = @DateCondition";

    if (!string.IsNullOrEmpty(department))
    {
        query += " AND Emp.DepartmentName = @Department";
    }

    query += " Order By Emp.DepartmentName";

    DataTable dt = new DataTable();
    using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
    {
        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@DateCondition", condition);

            if (!string.IsNullOrEmpty(department))
            {
                cmd.Parameters.AddWithValue("@Department", department);
            }

            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                sda.Fill(dt);
            }
        }
    }

    return dt;
}
protected void SubmitBtn_Click(object sender, EventArgs e)
{
    string selectedDate = Date.Text.Trim();
    string Department = DeptDropdown.Text.Trim();

    ReportViewer1.Visible = true;

    if (!string.IsNullOrEmpty(selectedDate))
    {
        LoadReport(selectedDate, Department);
    }
}

  
  
   protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string selectedDate = Date.Text.Trim();
            string Department = DeptDropdown.Text.Trim();

            ReportViewer1.Visible = true;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                LoadReport(selectedDate, Department);
            }

        }
  private DataTable GetGeoData(string condition, string department)
        {
            string query = @"select DE.Pno,Emp.DepartmentName,DE.DateAndTime,DE.PunchIn_FailedCount,DE.PunchOut_FailedCount from App_FaceVerification_Details As DE 
INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
ON DE.Pno COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT where 
Emp.DepartmentName = '"+department+"' And CAST(DateAndTime as Date) = '"+ condition + "' Order By Emp.DepartmentName";

           
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

  private void LoadReport(string date, string Department)
        {
            DataTable inoData = GetGeoData(date, Department);

            ReportViewer1.LocalReport.ReportPath = Server.MapPath("FaceRecognition_Report.rdlc");
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("DataSet1", inoData);
            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.Refresh();
        }

i want if deparment is not selected then value shows based on the Date 
