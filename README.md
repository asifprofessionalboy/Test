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

protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        string today = DateTime.Now.ToString("yyyy/MM/dd");
        fromdate.Text = today;
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
    ReportDataSource rds = new ReportDataSource("DataSet1", data); // "DataSet1" must match your RDLC dataset name
    ReportViewer1.LocalReport.DataSources.Add(rds);
    ReportViewer1.LocalReport.Refresh();

    ReportViewer1.Visible = true;
}



this is my code 
 protected void Page_Load(object sender, EventArgs e)
        {
            

                if (!IsPostBack)
                {
                    string today = DateTime.Now.ToString("yyyy/MM/dd");
                    fromdate.Text = today;
                ReportViewer1.Visible = false;

                BindDepartmentDropdown();

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
                LoadReport(from, to, dept, type, attempt);
            }
        }
