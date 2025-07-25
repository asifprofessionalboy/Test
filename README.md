protected void btnSearch_Click(object sender, EventArgs e)
{
    // Extract selected year(s) and month(s)
    string stryear = string.Join(",", Year.Items.Cast<ListItem>()
        .Where(i => i.Selected).Select(i => i.Value));

    string strmnth = string.Join(",", Month.Items.Cast<ListItem>()
        .Where(i => i.Selected).Select(i => i.Value));

    // Check that at least one year and one month is selected
    if (!string.IsNullOrEmpty(stryear) && !string.IsNullOrEmpty(strmnth))
    {
        // Initialize business logic object
        BL_Compliance_MIS blobj = new BL_Compliance_MIS();

        // Initialize location and department values
        string Locationvalue = string.Empty;
        string Dpartmentvalue = string.Empty;

        // Build comma-separated list of selected locations
        foreach (ListItem item in Location.Items)
        {
            if (item.Selected)
            {
                Locationvalue += $"'{item.Value}',";
            }
        }
        Locationvalue = Locationvalue.TrimEnd(',');

        // Build comma-separated list of selected departments
        foreach (ListItem item in Department.Items)
        {
            if (item.Selected)
            {
                Dpartmentvalue += $"'{item.Value}',";
            }
        }
        Dpartmentvalue = Dpartmentvalue.TrimEnd(',');

        // Convert selected values to arrays
        string[] yearArray = stryear.Split(',');
        string[] monthArray = strmnth.Split(',');

        // Loop over each selected year and month
        foreach (string year in yearArray)
        {
            foreach (string month in monthArray)
            {
                // (Place your corrected dynamic query generation code here)
            }
        }
    }
    else
    {
        // Handle invalid selection (optional)
        lblError.Text = "Please select at least one Year and one Month.";
    }
}
