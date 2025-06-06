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
