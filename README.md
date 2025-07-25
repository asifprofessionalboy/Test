protected void btnSearch_Click(object sender, EventArgs e)
{
    string stryear = string.Join(",", Year.Items.Cast<ListItem>()
                                      .Where(i => i.Selected)
                                      .Select(i => i.Value));

    string strmnth = string.Join(",", Month.Items.Cast<ListItem>()
                                      .Where(i => i.Selected)
                                      .Select(i => i.Value));

    if (!string.IsNullOrEmpty(stryear) && !string.IsNullOrEmpty(strmnth))
    {
        BL_Compliance_MIS blobj = new BL_Compliance_MIS();

        string Locationvalue = string.Empty;
        string Dpartmentvalue = string.Empty;

        // Build location filter string
        foreach (ListItem item in Location.Items)
        {
            if (item.Selected)
                Locationvalue += $"'{item.Value}',";
        }
        Locationvalue = Locationvalue.TrimEnd(',');

        // Build department filter string
        foreach (ListItem item in Department.Items)
        {
            if (item.Selected)
                Dpartmentvalue += $"'{item.Value}',";
        }
        Dpartmentvalue = Dpartmentvalue.TrimEnd(',');

        string[] yearArray = stryear.Split(',');
        string[] monthArray = strmnth.Split(',');

        // Initialize one DataTable to collect all results
        DataTable resultTable = new DataTable();

        // Get your actual connection string name from Web.config
        string connStr = ConfigurationManager.ConnectionStrings["clmsdbConnectionString"].ConnectionString;

        foreach (string year in yearArray)
        {
            foreach (string month in monthArray)
            {
                int intMonth = Convert.ToInt32(month);
                int intYear = Convert.ToInt32(year);

                int nextMonth = (intMonth == 12) ? 1 : intMonth + 1;
                int nextYear = (intMonth == 12) ? intYear + 1 : intYear;

                string monthStr = intMonth.ToString("D2");
                string startDate = $"{intYear}-{monthStr}-01";
                string endDate = new DateTime(nextYear, nextMonth, 1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss");

                string locationFilter = !string.IsNullOrEmpty(Locationvalue)
                                        ? $" AND WOR.LOC_OF_WORK IN ({Locationvalue}) "
                                        : "";

                string departmentFilter = !string.IsNullOrEmpty(Dpartmentvalue)
                                        ? $" AND tab.DepartmentCode IN ({Dpartmentvalue}) "
                                        : "";

                // ⚠️ Replace below with your actual working query, injecting dynamic variables
                string strQuery = $@"SELECT TOP 100 *
                                     FROM YourTableName
                                     WHERE SomeDate BETWEEN '{startDate}' AND '{endDate}'
                                     {locationFilter}
                                     {departmentFilter}";

                // Execute query and append results to resultTable
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(strQuery, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            continue;

                        DataTable temp = new DataTable();
                        temp.Load(reader);
                        if (resultTable.Columns.Count == 0)
                            resultTable = temp.Clone(); // Create schema on first iteration

                        foreach (DataRow row in temp.Rows)
                        {
                            resultTable.ImportRow(row);
                        }
                    }
                }
            }
        }

        // Bind the final result to GridView
        if (resultTable.Rows.Count > 0)
        {
            Mis_Records.DataSource = resultTable;
            Mis_Records.DataBind();
        }
        else
        {
            lblError.Text = "No records found for selected filters.";
            Mis_Records.DataSource = null;
            Mis_Records.DataBind();
        }
    }
    else
    {
        lblError.Text = "Please select at least one Year and one Month.";
    }
}
