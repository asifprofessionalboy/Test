..
protected void Mis_Records_RowCreated(object sender, GridViewRowEventArgs e)
{
    if (e.Row.RowType == DataControlRowType.Header)
    {
        GridViewRow headerRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

        headerRow.BackColor = System.Drawing.Color.LightGray;

        // Example: Merge headers like "Male", "Female", "Skill" categories
        headerRow.Cells.Add(MakeHeaderCell("Vendor Code", 1));
        headerRow.Cells.Add(MakeHeaderCell("Vendor Name", 1));
        headerRow.Cells.Add(MakeHeaderCell("Workorder No.", 1));
        headerRow.Cells.Add(MakeHeaderCell("Male", 6));     // Span 6 columns under Male
        headerRow.Cells.Add(MakeHeaderCell("Female", 6));   // Span 6 columns under Female
        headerRow.Cells.Add(MakeHeaderCell("Skill Category", 8));
        headerRow.Cells.Add(MakeHeaderCell("Total", 2));
        headerRow.Cells.Add(MakeHeaderCell("Description", 1));

        // Add the row before default header row
        ((GridView)sender).Controls[0].Controls.AddAt(0, headerRow);
    }
}

private TableHeaderCell MakeHeaderCell(string text, int columnSpan)
{
    TableHeaderCell cell = new TableHeaderCell();
    cell.Text = text;
    cell.ColumnSpan = columnSpan;
    cell.HorizontalAlign = HorizontalAlign.Center;
    cell.BackColor = System.Drawing.Color.LightBlue;
    cell.Font.Bold = true;
    return cell;
}
