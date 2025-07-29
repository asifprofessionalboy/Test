protected void btnExport_Click(object sender, EventArgs e)
{
    if (ViewState["MISData"] != null)
    {
        Mis_Grid.DataSource = (DataTable)ViewState["MISData"];
        Mis_Grid.DataBind();

        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=ComplianceReport.xls");
        Response.Charset = "";
        Response.ContentType = "application/vnd.ms-excel";

        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                // Optional: style numbers to treat as text
                string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                Response.Write(style);

                Mis_Grid.RenderControl(hw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }
}
