..
DataTable finalTable = null;

foreach (string year in yearArray)
{
    foreach (string month in monthArray)
    {
        DataSet ds_L1 = blobj.Get_Mis_Data(...);
        if (ds_L1.Tables.Count > 0 && ds_L1.Tables[0].Rows.Count > 0)
        {
            if (finalTable == null)
                finalTable = ds_L1.Tables[0].Clone(); // Clone structure

            foreach (DataRow row in ds_L1.Tables[0].Rows)
                finalTable.ImportRow(row);
        }
    }
}

if (finalTable != null && finalTable.Rows.Count > 0)
{
    Mis_Records.DataSource = finalTable;
    Mis_Records.DataBind();
}
else
{
    MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Warning, "No Data Found !!!");
}
