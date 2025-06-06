<asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoPostBack="false">
    <asp:ListItem Text="-- Select Type --" Value="" />
    <asp:ListItem Text="PUNCH IN" Value="PUNCH IN" />
    <asp:ListItem Text="PUNCH OUT" Value="PUNCH OUT" />   
</asp:DropDownList>

 private DataTable GetGeoData(string fromDate, string toDate, string department, string type, string attemptRange)
{
    bool isPunchIn = type == "PUNCH IN";
    bool isPunchOut = type == "PUNCH OUT";
    bool isBoth = string.IsNullOrEmpty(type); // If no type selected

    // Select different columns based on type
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

    // Handle attempt filter
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


private void LoadReport(string fromDate, string toDate, string department, string type, string attemptRange)
{
    DataTable data = GetGeoData(fromDate, toDate, department, type, attemptRange);

    ReportViewer1.LocalReport.ReportPath = Server.MapPath("FaceRecognition_Report.rdlc");
    ReportViewer1.LocalReport.DataSources.Clear();
    ReportDataSource rds = new ReportDataSource("DataSet1", data);
    ReportViewer1.LocalReport.DataSources.Add(rds);
    ReportViewer1.LocalReport.Refresh();
}

 protected void SubmitBtn_Click(object sender, EventArgs e)
{
    string from = fromdate.Text.Trim();
    string to = todate.Text.Trim();
    string dept = DeptDropdown.SelectedValue;
    string type = DropDownList1.SelectedValue; // May be empty
    string attempt = DropDownList2.SelectedValue;

    if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
    {
        LoadReport(from, to, dept, type, attempt);
    }
}

 
 private DataTable GetGeoData(string date, string department)
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
                    cmd.Parameters.AddWithValue("@DateCondition", date);

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

 private void LoadReport(string date, string department)
        {
            DataTable inoData = GetGeoData(date, department);

            ReportViewer1.LocalReport.ReportPath = Server.MapPath("FaceRecognition_Report.rdlc");
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("DataSet1", inoData);
            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.Refresh();
        }

this is my aspx 
 <form id="form1" runat="server">
        <div class="ml-3 mr-3">

            <fieldset  class="" style="border: 1px solid #bfbebe; padding: 5px 20px 5px 20px; border-radius: 6px">
                 <legend style="width: auto; border: 0; font-size: 14px; margin: 0px 6px 0px 6px; padding: 0px 5px 0px 5px; color: #0000FF"><b>Search</b></legend>
                         <div class="form-inline row">
                             <div class="form-group col-md-3 mb-1">
                                 <label for="From Date" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">From Date:</label>
                                 <asp:TextBox ID="fromdate" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoComplete="off" ToolTip="yyyy/MM/dd"></asp:TextBox>
                                    <ask:CalendarExtender ID="CalendarExtender2" runat="server" Enabled="True"  Format="yyyy/MM/dd" PopupPosition="TopRight" TargetControlID="fromdate" TodaysDateFormat="yyyy/MM/dd" ></ask:CalendarExtender>  
                             </div>

                             <div class="form-group col-md-3 mb-1">
                                 <label for="To Date" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">To Date:</label>
                                 <asp:TextBox ID="todate" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoComplete="off" ToolTip="yyyy/MM/dd"></asp:TextBox>
                                    <ask:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True"  Format="yyyy/MM/dd" PopupPosition="TopRight" TargetControlID="todate" TodaysDateFormat="yyyy/MM/dd" ></ask:CalendarExtender>  
                             </div>
                             <div class="form-group col-md-3 mb-1">
    <label for="Attempt" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">Attempt:</label>
    <asp:DropDownList ID="DropDownList2" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoPostBack="false">
        <asp:ListItem Text="0-1" Value="0-1" />
        <asp:ListItem Text="2-5" Value="2-5" />
        <asp:ListItem Text="6-10" Value="6-10" />
        <asp:ListItem Text="10-above" Value="10-above" />
    </asp:DropDownList>
</div>
                <div class="form-group col-md-3 mb-1">
    <label for="Type" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">Type:</label>
    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoPostBack="false">
        <asp:ListItem Text="PUNCH IN" Value="PUNCH IN" />
        <asp:ListItem Text="PUNCH OUT" Value="PUNCH OUT" />   
    </asp:DropDownList>
</div>             
                                 <div class="form-group col-md-3 mb-1">
    <label for="Department" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">Department:</label>
    <asp:DropDownList ID="DeptDropdown" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoPostBack="false">
        <asp:ListItem Text="-- Select Department --" Value="" />
    </asp:DropDownList>
                                     </div>
                                     


                            
                 
                             </div>
                 <div class="form-group col-md-2 mb-1">
                                 <asp:Button ID="SubmitBtn" runat="server" Text="Submit" CssClass="btn btn-sm btn-info" OnClick="SubmitBtn_Click" ValidationGroup="search"/>
                                 </div>
                   
                </fieldset>


i want filter using these textboxes or dropdown , like from date, to date , Type like PunchIn and PunchOut , attempt . i have these 2 query for punchIn and PunchOut , if punchIn click then show PunchIn_failedCount and if PunchOut click shows based on PunchOut_Failed Count
select DE.Pno, Emp.DepartmentName, DE.DateAndTime, 
                            DE.PunchIn_FailedCount
                     from App_FaceVerification_Details As DE 
                     INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
                     ON DE.Pno COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT 
                     where CAST(DateAndTime as Date) >= '2025-06-04' and CAST(DateAndTime as Date)<='2025-06-06'
                     and PunchIn_FailedCount>= 1 and PunchIn_FailedCount<= 2
                     order by DateAndTime


                     select DE.Pno, Emp.DepartmentName, DE.DateAndTime, 
                            DE.PunchOut_FailedCount
                     from App_FaceVerification_Details As DE 
                     INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
                     ON DE.Pno COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT 
                     where CAST(DateAndTime as Date) >= '2025-06-04' and CAST(DateAndTime as Date)<='2025-06-06'
                     and PunchIn_FailedCount>= 1 and PunchIn_FailedCount<= 2
                     order by DateAndTime
