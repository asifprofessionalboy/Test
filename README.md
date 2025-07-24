public partial class Compliance_Mis_Report : Classes.basePage
{
    App_Compliance_MIS_DS dsRecords = new App_Compliance_MIS_DS();
    DataSet dsDDL = new DataSet();

    // 🔸 Declare objMIS globally (class level)
    BL_Compliance_MIS objMIS = new BL_Compliance_MIS();

    protected override void SetBaseControls()
    {
        base.SetBaseControls();
        PageRecordsDataSet = dsRecords;
        PageDDLDataset = dsDDL;

        // Already declared above, no need to reassign objMIS here
        BLObject = objMIS; // Optional if you're using this somewhere
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string vendorCode = Session["UserName"].ToString(); 
            DataSet ds = objMIS.Get_Mis_Data(vendorCode); 

            if (ds != null && ds.Tables.Count > 0)
            {
                Mis_Records.DataSource = ds;
                Mis_Records.DataBind();
            }
        }
    }
}
