public DataSet Get_Mis_Data_Filtered(string year, string month, string departmentCode = "", string locationCode = "")
{
    StringBuilder query = new StringBuilder();

    query.Append(@"
        WITH RankedAttendance AS (
            SELECT
                AD.VendorCode,
                AD.WorkOrderNo,
                EM.Sex,
                EM.Social_Category,
                AD.WorkManCategory,
                EM.AadharCard,
                CAST(AD.Present AS INT) AS Present,
                EM.CreatedOn,
                ROW_NUMBER() OVER (
                    PARTITION BY AD.VendorCode, AD.WorkOrderNo, EM.Sex, EM.Social_Category, AD.WorkManCategory
                    ORDER BY EM.CreatedOn DESC
                ) AS rn
            FROM App_AttendanceDetails AD
            INNER JOIN App_EmployeeMaster EM
                ON EM.AadharCard = AD.AadharNo
                AND EM.VendorCode = AD.VendorCode
                AND EM.WorkManSlNo = AD.WorkManSl
            WHERE MONTH(AD.Dates) = @Month AND YEAR(AD.Dates) = @Year
        ),

        AttendanceAgg AS (
            SELECT
                VendorCode,
                WorkOrderNo,
                Sex,
                Social_Category,
                WorkManCategory,
                COUNT(DISTINCT AadharCard) AS TotalWorkers,
                SUM(Present) AS TotalMandays
            FROM RankedAttendance
            WHERE rn = 1
            GROUP BY VendorCode, WorkOrderNo, Sex, Social_Category, WorkManCategory
        ),

        ContractorContact AS (
            SELECT
                CREATEDBY AS VendorCode,
                STRING_AGG(NAME + '-' + CONTACT_NO, ', ') AS RESPONSIBLE_PERSON
            FROM App_Vendor_Representative
            GROUP BY CREATEDBY
        ),

        WorkOrders AS (
            SELECT DISTINCT
                V_CODE AS vendorcode,
                WO_NO AS workorder,
                CONVERT(varchar, START_DATE, 103) AS from_date,
                CONVERT(varchar, END_DATE, 103) AS to_date,
                DEPT_CODE AS DepartmentCode,
                TXZ01 AS Description
            FROM App_Vendorwodetails
            WHERE MONTH(START_DATE) <= @Month AND MONTH(END_DATE) >= @Month AND YEAR(START_DATE) <= @Year AND YEAR(END_DATE) >= @Year
        ),

        WageAgg AS (
            SELECT
                VendorCode,
                WorkOrderNo,
                ROUND(ISNULL(AVG(CAST(NULLIF(ISNULL(CAST(BasicWages AS FLOAT), 0) + ISNULL(CAST(DAWages AS FLOAT), 0), 0) AS FLOAT)), 0), 2) AS BasicA,
                ROUND(ISNULL(AVG(CAST(NULLIF(ISNULL(CAST(OtherAllow AS FLOAT), 0), 0) AS FLOAT)), 0), 2) AS Allowance,
                ROUND(ISNULL(AVG(CAST(NULLIF(ISNULL(CAST(TotalWages AS FLOAT), 0), 0) AS FLOAT)), 0), 2) AS GrossWages,
                ROUND(ISNULL(AVG(CAST(NULLIF(ISNULL(CAST(PfAmt AS FLOAT), 0), 0) AS FLOAT)), 0), 2) AS PF_DEDUCTION,
                ROUND(ISNULL(AVG(CAST(NULLIF(ISNULL(CAST(EsiAmt AS FLOAT), 0), 0) AS FLOAT)), 0), 2) AS ESI_DEDUCTION,
                ROUND(ISNULL(AVG(CAST(NULLIF(ISNULL(CAST(OtherDeduAmt AS FLOAT), 0), 0) AS FLOAT)), 0), 2) AS Other_DEDUCTION,
                ROUND(ISNULL(AVG(CAST(NULLIF(ISNULL(CAST(NetWagesAmt AS FLOAT), 0), 0) AS FLOAT)), 0), 2) AS NET_WAGES_AMOUNT
            FROM App_WagesDetailsJharkhand
            WHERE MonthWage = @Month AND YearWage = @Year
            GROUP BY VendorCode, WorkOrderNo
        )

        SELECT
            ROW_NUMBER() OVER (ORDER BY mis.vendorcode) AS SlNo,
            RIGHT('0' + CAST(@Month AS VARCHAR), 2) AS ProcessMonth,
            @Year AS ProcessYear,
            mis.vendorcode,
            VM.V_NAME,
            mis.workorder,
            mis.from_date,
            mis.to_date,
            ISNULL(DM.DepartmentCode, 'Work Order Not Registered') AS DepartmentCode,
            ISNULL(DM.DepartmentName, 'Work Order Not Registered') AS DepartmentName,
            ISNULL(LM.Location, 'Work Order Not Registered') AS Location,
            ISNULL(CC.RESPONSIBLE_PERSON, 'Vendor Registration Not Done') AS RESPONSIBLE_PERSON_OF_THE_CONTRACTOR,
            mis.Description AS NatureOfWork,

            SUM(CASE WHEN AA.Sex = 'M' THEN AA.TotalWorkers ELSE 0 END) AS MALE_NO_OF_MALE_WORKERS,
            SUM(CASE WHEN AA.Sex = 'F' THEN AA.TotalWorkers ELSE 0 END) AS FEMALE_NO_OF_FEMALE_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Unskilled' THEN AA.TotalWorkers ELSE 0 END) AS UNSKILLED_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Semi Skilled' THEN AA.TotalWorkers ELSE 0 END) AS SEMISKILLED_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Skilled' THEN AA.TotalWorkers ELSE 0 END) AS SKILLED_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Highly Skilled' THEN AA.TotalWorkers ELSE 0 END) AS HIGHLYSKILLED_NOS_OF_WORKERS,

            ISNULL(WA.BasicA, 0) AS BasicDA,
            ISNULL(WA.Allowance, 0) AS Allowance,
            ISNULL(WA.GrossWages, 0) AS GrossWages,
            ISNULL(WA.PF_DEDUCTION, 0) AS PF_DEDUCTION,
            ISNULL(WA.ESI_DEDUCTION, 0) AS ESI_DEDUCTION,
            ISNULL(WA.Other_DEDUCTION, 0) AS Other_DEDUCTION,
            ISNULL(WA.NET_WAGES_AMOUNT, 0) AS NET_WAGES_AMOUNT

        FROM WorkOrders mis
        LEFT JOIN App_VendorMaster VM ON VM.V_CODE = mis.vendorcode
        LEFT JOIN App_DepartmentMaster DM ON DM.DepartmentCode = mis.DepartmentCode
        LEFT JOIN App_WorkOrder_Reg WOR ON WOR.WO_NO = mis.workorder
        LEFT JOIN App_LocationMaster LM ON LM.LocationCode = WOR.LOC_OF_WORK
        LEFT JOIN ContractorContact CC ON CC.VendorCode = mis.vendorcode
        LEFT JOIN AttendanceAgg AA ON AA.VendorCode = mis.vendorcode AND AA.WorkOrderNo = mis.workorder
        LEFT JOIN WageAgg WA ON WA.VendorCode = mis.vendorcode AND WA.WorkOrderNo = mis.workorder

        WHERE 1 = 1 ");

    if (!string.IsNullOrEmpty(departmentCode))
        query.Append(" AND DM.DepartmentCode = @DepartmentCode ");

    if (!string.IsNullOrEmpty(locationCode))
        query.Append(" AND LM.LocationCode = @LocationCode ");

    query.Append(" GROUP BY mis.vendorcode, VM.V_NAME, mis.workorder, mis.from_date, mis.to_date, DM.DepartmentCode, DM.DepartmentName, LM.Location, CC.RESPONSIBLE_PERSON, mis.Description, WA.BasicA, WA.Allowance, WA.GrossWages, WA.PF_DEDUCTION, WA.ESI_DEDUCTION, WA.Other_DEDUCTION, WA.NET_WAGES_AMOUNT ORDER BY mis.vendorcode;");

    Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@Year", year },
        { "@Month", month }
    };

    if (!string.IsNullOrEmpty(departmentCode))
        parameters.Add("@DepartmentCode", departmentCode);

    if (!string.IsNullOrEmpty(locationCode))
        parameters.Add("@LocationCode", locationCode);

    DataHelper dh = new DataHelper();
    return dh.GetDataset(query.ToString(), "DataSet", parameters);
}
