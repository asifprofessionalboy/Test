if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
{
    Mis_Records.DataSource = ds.Tables[0];
    Mis_Records.DataBind();
}
else
{
    Mis_Records.DataSource = null;
    Mis_Records.DataBind();
}
