      }

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
