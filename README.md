i have these three different datasets function. these functions fetches data from . i want to show output at once 
public DataSet Leave_details(string WorkOrder, string VendorCode)
        {
}

public DataSet Bonus_details(string WorkOrder, string VendorCode)
        {
}

public DataTable RR_Alert_latest(string WorkOrderNo, string VendorCode)
        {
 return dt;

}
public static List<Dictionary<string, object>> ConvertDataTableToDictionaryList(DataTable dt)
    {
        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                dict[col.ColumnName] = row[col];
            }
            list.Add(dict);
        }
        return list;
    }
}
 this is my controller 

 [HttpGet("Submit")]
        public IActionResult RR_Alert_Latest(string WorkOrderNo, string VendorCode)
        {

            try
            {
                var data = compliance.RR_Alert_latest(WorkOrderNo, VendorCode);
                
                return Ok(data);    
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error Occured while Getting RR_ALert_Latest");
                return StatusCode(500, ex.Message);
            }
        }
