protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        Date.Text = today;
        PunchType.SelectedValue = "I"; // Default to PunchIn
        LoadReport(today, "I");
    }
}




protected void SubmitBtn_Click(object sender, EventArgs e)
{
    string selectedDate = Date.Text.Trim(); // Already in yyyy-MM-dd format
    string punchType = PunchType.SelectedValue;

    if (!string.IsNullOrEmpty(selectedDate) && !string.IsNullOrEmpty(punchType))
    {
        LoadReport(selectedDate, punchType);
    }
    else
    {
        // Optionally display a validation message
    }
}

private DataTable GetStudentData(string date, string punchType)
{
    string query = "SELECT DISTINCT PDE_PUNCHDATE, PDE_PUNCHTIME, PDE_PSRNO " +
                   "FROM T_TRPUNCHDATA_EARS " +
                   "WHERE PDE_PUNCHDATE = @Date AND PDE_SUBAREA = 'JUSC12' AND PDE_INOUT = @PunchType " +
                   "ORDER BY PDE_PUNCHTIME DESC";

    DataTable dt = new DataTable();

    using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
    {
        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@PunchType", punchType);

            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                sda.Fill(dt);
            }
        }
    }

    return dt;
}


private void LoadReport(string date, string punchType)
{
    DataTable inoData = GetStudentData(date, punchType);

    ReportViewer1.LocalReport.ReportPath = Server.MapPath("AttendaceDetails.rdlc");
    ReportViewer1.LocalReport.DataSources.Clear();
    ReportDataSource rds = new ReportDataSource("DataSet1", inoData);
    ReportViewer1.LocalReport.DataSources.Add(rds);
    ReportViewer1.LocalReport.Refresh();
}



this is my aspx page

 <div class="form-inline row">
                             <div class="form-group col-md-4 mb-1">
                                 <label for="Date" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">Date:</label>
                                 <asp:TextBox ID="Date" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoComplete="off" ToolTip="dd/MM/yyyy"></asp:TextBox>
                                    <ask:CalendarExtender ID="CalendarExtender2" runat="server" Enabled="True"  Format="dd/MM/yyyy" PopupPosition="TopRight" TargetControlID="Date" TodaysDateFormat="dd/MM/yyyy" ></ask:CalendarExtender>  
                             </div>

                              <div class="form-group col-md-4 mb-1">
                                 <label for="PunchType" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">Date:</label>
                                <asp:DropDownList ID="PunchType" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoPostBack="false">
        <asp:ListItem Text="-- Select --" Value="" />
        <asp:ListItem Text="PunchIn" Value="I" />
        <asp:ListItem Text="PunchOut" Value="O" />
    </asp:DropDownList>

                             </div>

                             <div class="form-group col-md-4 mb-1">
                                 <asp:Button ID="SubmitBtn" runat="server" Text="Submit" CssClass="btn btn-sm btn-info" OnClick="SubmitBtn_Click" ValidationGroup="search"/>
                                 </div>
                            </div>


this is my cs code
 protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                LoadReport();
            }
        }

        private void LoadReport()
        {
          
            DataTable inoData = GetStudentData();

           
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("AttendaceDetails.rdlc");

           
            ReportViewer1.LocalReport.DataSources.Clear();

           
            ReportDataSource rds = new ReportDataSource("DataSet1", inoData);

           
            ReportViewer1.LocalReport.DataSources.Add(rds);

           
            ReportViewer1.LocalReport.Refresh();
        }

        private DataTable GetStudentData()
        {
           
            string query = "select distinct PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_PSRNO from T_TRPUNCHDATA_EARS " +
                "where PDE_PUNCHDATE = '2025-04-24' and PDE_SUBAREA = 'JUSC12' and PDE_INOUT = 'I'order by  PDE_PUNCHTIME desc";

           
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

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {

        }

on button click i want to show details of daily report of Attendance , there is dropdown PunchIn and PunchOut set I and O and Date to retrieve details
