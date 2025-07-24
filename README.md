protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        string vendorCode = Session["UserName"].ToString(); // Assuming VendorCode is stored here
        DataSet ds = objMIS.Get_Mis_Data(vendorCode); // Call your BL method

        if (ds != null && ds.Tables.Count > 0)
        {
            Mis_Records.DataSource = ds;         // Bind to your <cc1:DetailsContainer>
            Mis_Records.DataBind();
        }
    }
}
