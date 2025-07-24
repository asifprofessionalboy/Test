<asp:GridView ID="Mis_Records" runat="server" AutoGenerateColumns="False" OnRowCreated="Mis_Records_RowCreated" CssClass="table">
    <Columns>
        <asp:BoundField HeaderText="Vendor Code" DataField="V_Code" />
        <asp:BoundField HeaderText="Vendor Name" DataField="V_NAME" />
        <!-- Add all required BoundFields here -->
        <asp:BoundField HeaderText="Male Workers" DataField="MALE_NO_OF_MALE_WORKERS" />
        <asp:BoundField HeaderText="Male SC/ST" DataField="MALE_NOS_OF_SC_ST_WORKERS" />
        <asp:BoundField HeaderText="Male OBC" DataField="MALE_NOS_OF_OBC_WORKERS" />
        <asp:BoundField HeaderText="Male Mandays" DataField="MALE_MANDAYS" />
        <asp:BoundField HeaderText="Male SC/ST Mandays" DataField="MALE_MANDAYS_SC_ST" />
        <asp:BoundField HeaderText="Male OBC Mandays" DataField="MALE_MANDAYS_OBC" />

        <asp:BoundField HeaderText="Female Workers" DataField="FEMALE_NO_OF_FEMALE_WORKERS" />
        <asp:BoundField HeaderText="Female SC/ST" DataField="FEMALE_NOS_OF_SC_ST_WORKERS" />
        <asp:BoundField HeaderText="Female OBC" DataField="FEMALE_NOS_OF_OBC_WORKERS" />
        <asp:BoundField HeaderText="Female Mandays" DataField="FEMALE_MANDAYS" />
        <asp:BoundField HeaderText="Female SC/ST Mandays" DataField="FEMALE_MANDAYS_SC_ST" />
        <asp:BoundField HeaderText="Female OBC Mandays" DataField="FEMALE_MANDAYS_OBC" />

        <asp:BoundField HeaderText="Unskilled" DataField="UNSKILLED_NOS_OF_WORKERS" />
        <asp:BoundField HeaderText="Semi-Skilled" DataField="SEMISKILLED_NOS_OF_WORKERS" />
        <asp:BoundField HeaderText="Skilled" DataField="SKILLED_NOS_OF_WORKERS" />
        <asp:BoundField HeaderText="Highly Skilled" DataField="HIGHLYSKILLED_NOS_OF_WORKERS" />
        <!-- And so on -->
    </Columns>



    protected void Mis_Records_RowCreated(object sender, GridViewRowEventArgs e)
{
    if (e.Row.RowType == DataControlRowType.Header)
    {
        GridViewRow headerRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

        // Fixed info columns
        headerRow.Cells.Add(CreateHeaderCell("Vendor Info", 3)); // e.g. VendorCode, VendorName, WorkOrder

        // Dates
        headerRow.Cells.Add(CreateHeaderCell("Dates", 2)); // from_date, to_date

        // Dept & Location
        headerRow.Cells.Add(CreateHeaderCell("Department Info", 3)); // dept code, name, location

        // Responsible
        headerRow.Cells.Add(CreateHeaderCell("Responsible", 1));

        // Male Columns
        headerRow.Cells.Add(CreateHeaderCell("Male", 6));

        // Female Columns
        headerRow.Cells.Add(CreateHeaderCell("Female", 6));

        // Skills
        headerRow.Cells.Add(CreateHeaderCell("Skill Category", 6));

        // Add to GridView
        Mis_Records.Controls[0].Controls.AddAt(0, headerRow);
    }
}

private TableHeaderCell CreateHeaderCell(string text, int colSpan)
{
    TableHeaderCell cell = new TableHeaderCell
    {
        Text = text,
        ColumnSpan = colSpan,
        HorizontalAlign = HorizontalAlign.Center,
        CssClass = "groupHeader"
    };
    return cell;




    .groupHeader {
    background-color: #dceefb;
    color: #000;
    font-weight: bold;
    text-align: center;
    border: 1px solid #ccc;
}
}
</asp:GridView>
