protected void btnExport_Click(object sender, EventArgs e)
{
    // Optional: restore saved data (or bind again)
    if (ViewState["MISData"] != null)
    {
        Mis_Grid.DataSource = (DataTable)ViewState["MISData"];
        Mis_Grid.DataBind();
    }

    Response.Clear();
    Response.Buffer = true;
    Response.AddHeader("content-disposition", "attachment;filename=ComplianceReport.xls");
    Response.Charset = "";
    Response.ContentType = "application/vnd.ms-excel";

    using (StringWriter sw = new StringWriter())
    {
        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
        {
            Mis_Grid.RenderControl(hw);
            string style = "<style> .textmode { mso-number-format:\\@; } </style>";
            Response.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
    }
}

// Required for exporting server controls
public override void VerifyRenderingInServerForm(Control control)
{
    // Do nothing – required for GridView export
}
