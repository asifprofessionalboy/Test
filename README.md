<asp:Button ID="btnExport" runat="server" Text="Export to Excel"
    OnClick="btnExport_Click" CssClass="btn btn-success mt-2" />


protected void btnExport_Click(object sender, EventArgs e)
{
    // Clear the response
    Response.Clear();
    Response.Buffer = true;
    Response.AddHeader("content-disposition", "attachment;filename=ComplianceReport.xls");
    Response.Charset = "";
    Response.ContentType = "application/vnd.ms-excel";

    using (StringWriter sw = new StringWriter())
    {
        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
        {
            // To Export all pages, disable paging temporarily
            Mis_Grid.AllowPaging = false;

            // Re-bind data
            BindGrid(); // You must have your data-binding method here

            // Hide unwanted columns if needed
            foreach (GridViewRow row in Mis_Grid.Rows)
            {
                row.Attributes.Add("class", "textmode");
            }

            Mis_Grid.RenderControl(hw);

            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
    }
}

// Required override
public override void VerifyRenderingInServerForm(Control control)
{
    // This confirms that the control can be rendered
}
