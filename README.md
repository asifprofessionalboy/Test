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
        string Locationvalue = string.Join(",", Location.Items.Cast<ListItem>()
                                     .Where(i => i.Selected)
                                     .Select(i => $"'{i.Value}'"));

        string Departmentvalue = string.Join(",", Department.Items.Cast<ListItem>()
                                     .Where(i => i.Selected)
                                     .Select(i => $"'{i.Value}'"));

        string month = strmnth;
        string year = stryear;

        BL_Compliance_MIS blobj = new BL_Compliance_MIS();
        DataSet ds = blobj.Get_Mis_Data(month, year, Locationvalue, Departmentvalue); // Your method

        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            Mis_Records.DataSource = ds.Tables[0];
            Mis_Records.DataBind();
        }
        else
        {
            MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Warning, "No Data Found !!!");
        }
    }
}
