public DataSet Get_Mis_Data_Filtered(string vendorCode = "", string departmentCode = "", string workOrder = "")
{
    StringBuilder query = new StringBuilder();

    query.Append(@"
        -- full CTEs here: RankedAttendance, AttendanceAgg, ContractorContact, etc.
        WITH RankedAttendance AS (
            -- your entire CTE logic
        ),
        ...
        SELECT
            ROW_NUMBER() OVER (ORDER BY mis.vendorcode) AS SlNo,
            ...
        FROM WorkOrders mis
        LEFT JOIN ...
        WHERE 1=1
    ");

    if (!string.IsNullOrEmpty(vendorCode))
        query.Append(" AND mis.vendorcode = @VendorCode");

    if (!string.IsNullOrEmpty(departmentCode))
        query.Append(" AND DM.DepartmentCode = @DepartmentCode");

    if (!string.IsNullOrEmpty(workOrder))
        query.Append(" AND mis.workorder = @WorkOrder");

    Dictionary<string, object> param = new Dictionary<string, object>();
    if (!string.IsNullOrEmpty(vendorCode)) param.Add("@VendorCode", vendorCode);
    if (!string.IsNullOrEmpty(departmentCode)) param.Add("@DepartmentCode", departmentCode);
    if (!string.IsNullOrEmpty(workOrder)) param.Add("@WorkOrder", workOrder);

    DataHelper dh = new DataHelper();
    return dh.GetDataset(query.ToString(), "DataSet", param);
}
