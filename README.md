.

public class StickyHeaderGridView : GridView
{
    protected override void Render(HtmlTextWriter writer)
    {
        if (this.Rows.Count > 0)
        {
            writer.Write("<thead>");

            // Render custom group header
            GridViewRow customHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
            customHeader.CssClass = "groupHeaderRow";

            customHeader.Cells.Add(CreateHeaderCell("SlNo", 1));
            customHeader.Cells.Add(CreateHeaderCell("Vendor Details", 2));
            customHeader.Cells.Add(CreateHeaderCell("WorkOrder Details", 4));
            customHeader.Cells.Add(CreateHeaderCell("Department Details", 2));
            customHeader.Cells.Add(CreateHeaderCell("Location", 1));
            customHeader.Cells.Add(CreateHeaderCell("Responsible", 1));
            customHeader.Cells.Add(CreateHeaderCell("Male", 6));
            customHeader.Cells.Add(CreateHeaderCell("Female", 6));
            customHeader.Cells.Add(CreateHeaderCell("UnSkilled", 2));
            customHeader.Cells.Add(CreateHeaderCell("Semi Skilled", 2));
            customHeader.Cells.Add(CreateHeaderCell("Skilled", 2));
            customHeader.Cells.Add(CreateHeaderCell("Highly Skilled", 2));
            customHeader.Cells.Add(CreateHeaderCell("Others", 2));
            customHeader.Cells.Add(CreateHeaderCell("Total", 2));
            customHeader.Cells.Add(CreateHeaderCell("Wages", 7));
            customHeader.Cells.Add(CreateHeaderCell("PF/ESI", 5));
            customHeader.Cells.Add(CreateHeaderCell("Nil & Recognised", 5));

            customHeader.RenderControl(writer);

            // Render the built-in header
            this.HeaderRow.RenderControl(writer);

            writer.Write("</thead>");
        }

        // Render body normally
        base.Render(writer);
    }

    private TableHeaderCell CreateHeaderCell(string text, int colspan)
    {
        return new TableHeaderCell
        {
            Text = text,
            ColumnSpan = colspan,
            HorizontalAlign = HorizontalAlign.Center,
            CssClass = "groupHeader"
        };
    }
}

.groupHeaderRow th {
    position: sticky;
    top: 0;
    background-color: #dceefb;
    z-index: 3;
}

.grid th {
    position: sticky;
    top: 40px; /* Adjust depending on the first row height */
    background-color: #4A90E2;
    color: white;
    z-index: 2;
}
