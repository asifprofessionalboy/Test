protected void btnSearch_Click(object sender, EventArgs e)
{
    string selectedMonths = GetSelectedItems(Month);
    string selectedYears = GetSelectedItems(Year);
    string selectedDepartments = GetSelectedItems(Department);
    string selectedLocations = GetSelectedItems(Location);

    // Example: "2025,2024"
    // Pass to SQL: IN ('2025','2024')

    BL_Compliance_MIS objMIS = new BL_Compliance_MIS();

    DataSet ds = objMIS.Get_Mis_Data_Filtered(selectedMonths, selectedYears, selectedDepartments, selectedLocations);

    if (ds != null && ds.Tables.Count > 0)
    {
        Mis_Records.DataSource = ds;
        Mis_Records.DataBind();
    }
}


private string GetSelectedItems(CheckBoxList chkList)
{
    List<string> selected = new List<string>();
    foreach (ListItem item in chkList.Items)
    {
        if (item.Selected)
        {
            selected.Add($"'{item.Value}'");
        }
    }
    return string.Join(",", selected);
}


public DataSet Get_Mis_Data_Filtered(string months, string years, string departments, string locations)
{
    StringBuilder query = new StringBuilder();

    query.Append("SELECT ... FROM YourViewOrTable WHERE 1=1");

    if (!string.IsNullOrEmpty(months))
        query.Append(" AND ProcessMonth IN (" + months + ")");

    if (!string.IsNullOrEmpty(years))
        query.Append(" AND ProcessYear IN (" + years + ")");

    if (!string.IsNullOrEmpty(departments))
        query.Append(" AND DepartmentCode IN (" + departments + ")");

    if (!string.IsNullOrEmpty(locations))
        query.Append(" AND LocationCode IN (" + locations + ")");

    return Get_Data(query.ToString());
}
