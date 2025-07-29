  protected void Mis_Records_RowCreated(object sender,
                                        GridViewRowEventArgs e)
  {
      if (e.Row.RowType == DataControlRowType.Header)
      {
          GridViewRow headerRow = new GridViewRow(0, 0, DataControlRowType.Header,
                                                  DataControlRowState.Insert);

          headerRow.Cells.Add(CreateHeaderCell("SlNo", 1));
          //headerRow.Cells.Add(CreateHeaderCell("Process Month/Year", 2));

          headerRow.Cells.Add(CreateHeaderCell("Vendor Details", 2));

          headerRow.Cells.Add(CreateHeaderCell("WorkOrder Details", 4));

          headerRow.Cells.Add(CreateHeaderCell("Department Details", 2));
          headerRow.Cells.Add(CreateHeaderCell("Location", 1));

          headerRow.Cells.Add(CreateHeaderCell("Responsible", 1));

          headerRow.Cells.Add(CreateHeaderCell("Male", 6));

          headerRow.Cells.Add(CreateHeaderCell("Female", 6));

          headerRow.Cells.Add(CreateHeaderCell("UnSkilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Semi Skilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Skilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Highly Skilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Others", 2));
          headerRow.Cells.Add(CreateHeaderCell("Total", 2));
          headerRow.Cells.Add(CreateHeaderCell("Wages", 7));
          headerRow.Cells.Add(CreateHeaderCell("PF/ESI", 5));
          headerRow.Cells.Add(CreateHeaderCell("Nil & Recognised", 5));

          headerRow.CssClass = "sticky-header";
        

          Mis_Grid.Controls[0].Controls.AddAt(0, headerRow);
      }
  }

  private TableHeaderCell CreateHeaderCell(string text, int colSpan)
  {
      TableHeaderCell cell =
          new TableHeaderCell
          {
              Text = text,
              ColumnSpan = colSpan,
              HorizontalAlign = HorizontalAlign.Center,
              CssClass = "groupHeader"
          };
      return cell;
  }

        <style>
        .groupHeader {
            background-color: #dceefb;
            color: #000;
            font-weight: bold;
            text-align: center;
            border: 1px solid #ccc;
        }


         .grid {
        border-collapse: collapse;
        width: 100%;
        font-size: 13px;
        table-layout: auto;
    }

    .grid th, .grid td {
        padding: 6px 10px;
        text-align: center;
        border: 1px solid #ccc;
        vertical-align: middle;
        white-space: nowrap;
    }

    .grid th {
        background-color: #4A90E2;
        color: white;
        position: sticky;
        top: 0;
        z-index: 2;
    }

    .grid tr:nth-child(even) {
        background-color: #f9f9f9;
    }

    .grid tr:hover {
        background-color: #2783ef;
    }

    .scrollable-grid {
        height: 450px;
        overflow: auto;
        border: 1px solid #ccc;
        padding: 5px;
    }

    



        .sticky-header {
    position: sticky;
    top: 0;
    z-index: 3;
    background-color: #dceefb;
}
