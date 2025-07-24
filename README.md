<asp:TemplateField HeaderText="Sl. No.">
    <ItemTemplate>
        <asp:Label ID="lblSlNo" runat="server" />
    </ItemTemplate>
</asp:TemplateField>


protected void Mis_Records_RowDataBound(object sender, GridViewRowEventArgs e)
{
    if (e.Row.RowType == DataControlRowType.DataRow)
    {
        Label lblSlNo = (Label)e.Row.FindControl("lblSlNo");
        if (lblSlNo != null)
        {
            lblSlNo.Text = (e.Row.RowIndex + 1).ToString();
        }
    }
}
