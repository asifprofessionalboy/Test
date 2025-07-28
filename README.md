if (ds_L1 != null && ds_L1.Tables.Count > 0 && ds_L1.Tables[0].Rows.Count > 0)
{
    PageRecordDataSet.Merge(ds_L1);

    // Debug
    Response.Write("Rows in dataset: " + PageRecordDataSet.Tables[0].Rows.Count);

    Mis_Records.DataSource = PageRecordDataSet.Tables[0];
    Mis_Records.DataBind();

    // If inside UpdatePanel
    UpdatePanel1.Update();
}
else
{
    Mis_Records.DataSource = null;
    Mis_Records.DataBind();
    MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Warning, "No Data Found !!!");
}
