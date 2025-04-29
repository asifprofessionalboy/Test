WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p' 
        AND DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
            <= EOMONTH(GETDATE())
),
FilteredPunches AS (
    SELECT 
        t.TRBDGDA_BD_PNO, 
        t.TRBDGDA_BD_DATE, 
        CONVERT(TIME, DATEADD(MINUTE, t.TRBDGDA_BD_TIME, 0)) AS PunchTime,
        ROW_NUMBER() OVER (
            PARTITION BY t.TRBDGDA_BD_PNO, t.TRBDGDA_BD_DATE 
            ORDER BY t.TRBDGDA_BD_TIME
        ) AS rn
    FROM TSUISLRFIDDB.dbo.T_TRBDGDAT_EARS t
),
ValidPunches AS (
    SELECT 
        fp.*, 
        MIN(PunchTime) OVER (PARTITION BY TRBDGDA_BD_PNO, TRBDGDA_BD_DATE) AS FirstPunch, 
        DATEDIFF(MINUTE, 
            MIN(PunchTime) OVER (PARTITION BY TRBDGDA_BD_PNO, TRBDGDA_BD_DATE), 
            PunchTime) AS MinDiff 
    FROM FilteredPunches fp
),
FilteredValidPunches AS (
    SELECT * 
    FROM ValidPunches 
    WHERE MinDiff >= 5 OR rn = 1
),
AllPunchesCount AS (
    SELECT 
        TRBDGDA_BD_PNO, 
        TRBDGDA_BD_DATE, 
        COUNT(*) AS TotalPunches
    FROM TSUISLRFIDDB.dbo.T_TRBDGDAT_EARS
    WHERE TRBDGDA_BD_PNO = '155026'
    GROUP BY TRBDGDA_BD_PNO, TRBDGDA_BD_DATE
)
SELECT 
    FORMAT(ds.punchdate, 'dd-MM-yyyy') AS TRBDGDA_BD_DATE,
    ISNULL(MIN(fvp.PunchTime), '00:00:00') AS PunchInTime,
    ISNULL(
        CASE 
            WHEN COUNT(fvp.PunchTime) > 1 THEN MAX(fvp.PunchTime)
            ELSE NULL 
        END, 
        '00:00:00'
    ) AS PunchOutTime,
    ISNULL(apc.TotalPunches, 0) AS SumOfPunching
FROM dateseries ds
LEFT JOIN FilteredValidPunches fvp 
    ON ds.punchdate = fvp.TRBDGDA_BD_DATE 
    AND fvp.TRBDGDA_BD_PNO = '155026'
LEFT JOIN AllPunchesCount apc 
    ON ds.punchdate = apc.TRBDGDA_BD_DATE 
    AND apc.TRBDGDA_BD_PNO = '155026'
GROUP BY ds.punchdate, apc.TotalPunches
ORDER BY ds.punchdate ASC;






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
