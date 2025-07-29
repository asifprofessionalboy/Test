..
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Data;

protected void btnExport_Click(object sender, EventArgs e)
{
    if (ViewState["MISData"] != null)
    {
        DataTable dt = (DataTable)ViewState["MISData"];

        using (ExcelPackage pck = new ExcelPackage())
        {
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Compliance Report");

            // Load the datatable into the sheet, starting from cell A1. Print column names on row 1
            ws.Cells["A1"].LoadFromDataTable(dt, true);

            // Optional: Auto-fit columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Output to browser
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=ComplianceReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.Flush();
            Response.End();
        }
    }
}
