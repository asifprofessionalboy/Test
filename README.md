for filteration i want Pno	
<div class="form-group col-md-4 mb-1">
                                 <label for="Pno" class="m-0 mr-2 p-0 col-form-label-sm col-sm-1 font-weight-bold fs-6">P.No.:</label>
                                 <asp:TextBox ID="Pno" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoComplete="off"></asp:TextBox>
                                   
                             </div>

 private void LoadReport(string date, string Department)
        {
            DataTable inoData = GetGeoData(date, Department);

            ReportViewer1.LocalReport.ReportPath = Server.MapPath("FaceRecognition_Report.rdlc");
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("DataSet1", inoData);
            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.Refresh();
        }


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
