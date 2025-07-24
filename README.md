<asp:BoundField DataField="V_Code" HeaderText="Vendor Code" />
<asp:BoundField DataField="V_NAME" HeaderText="Vendor Name" />
<asp:BoundField DataField="WO_NO" HeaderText="Workorder No." />
<asp:BoundField DataField="from_date" HeaderText="From Date" />
<asp:BoundField DataField="to_date" HeaderText="To Date" />
<asp:BoundField DataField="DepartmentCode" HeaderText="Department Code" />
<asp:BoundField DataField="DepartmentName" HeaderText="Department Name" />
<asp:BoundField DataField="Location" HeaderText="Location" />
<asp:BoundField DataField="RESPONSIBLE_PERSON_OF_THE_CONTRACTOR" HeaderText="Responsible Person" />

<asp:BoundField DataField="MALE_NO_OF_MALE_WORKERS" HeaderText="Male Workers" />
<asp:BoundField DataField="MALE_NOS_OF_SC_ST_WORKERS" HeaderText="Male SC/ST" />
<asp:BoundField DataField="MALE_NOS_OF_OBC_WORKERS" HeaderText="Male OBC" />
<asp:BoundField DataField="MALE_MANDAYS" HeaderText="Male Mandays" />
<asp:BoundField DataField="MALE_MANDAYS_SC_ST" HeaderText="Male SC/ST Mandays" />
<asp:BoundField DataField="MALE_MANDAYS_OBC" HeaderText="Male OBC Mandays" />

<asp:BoundField DataField="FEMALE_NO_OF_FEMALE_WORKERS" HeaderText="Female Workers" />
<asp:BoundField DataField="FEMALE_NOS_OF_SC_ST_WORKERS" HeaderText="Female SC/ST" />
<asp:BoundField DataField="FEMALE_NOS_OF_OBC_WORKERS" HeaderText="Female OBC" />
<asp:BoundField DataField="FEMALE_MANDAYS" HeaderText="Female Mandays" />
<asp:BoundField DataField="FEMALE_MANDAYS_SC_ST" HeaderText="Female SC/ST Mandays" />
<asp:BoundField DataField="FEMALE_MANDAYS_OBC" HeaderText="Female OBC Mandays" />

<asp:BoundField DataField="UNSKILLED_NOS_OF_WORKERS" HeaderText="Unskilled Workers" />
<asp:BoundField DataField="UNSKILLED_TOTAL_MANDAYS" HeaderText="Unskilled Mandays" />
<asp:BoundField DataField="SEMISKILLED_NOS_OF_WORKERS" HeaderText="Semi-Skilled Workers" />
<asp:BoundField DataField="SKILLED_NOS_OF_WORKERS" HeaderText="Skilled Workers" />
<asp:BoundField DataField="SKILLED_TOTAL_MANDAYS" HeaderText="Skilled Mandays" />
<asp:BoundField DataField="HIGHLYSKILLED_NOS_OF_WORKERS" HeaderText="Highly Skilled Workers" />
<asp:BoundField DataField="HIGHLYSKILLED_TOTAL_MANDAYS" HeaderText="Highly Skilled Mandays" />
<asp:BoundField DataField="Other_NOS_OF_WORKERS" HeaderText="Other Workers" />
<asp:BoundField DataField="Other_TOTAL_MANDAYS" HeaderText="Other Mandays" />

<asp:BoundField DataField="Total_NOS_OF_WORKERS" HeaderText="Total Workers" />
<asp:BoundField DataField="Total_Mandays" HeaderText="Total Mandays" />
<asp:BoundField DataField="Description" HeaderText="Description" />




protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        string vendorCode = Session["UserName"].ToString();

        string strQuery = @"
        WITH AttendanceAgg AS (
            SELECT
                AD.VendorCode,
                AD.WorkOrderNo,
                EM.Sex,
                EM.Social_Category,
                AD.WorkManCategory,
                COUNT(DISTINCT EM.AadharCard) AS TotalWorkers,
                SUM(CAST(AD.Present AS INT)) AS TotalMandays
            FROM App_AttendanceDetails AD
            INNER JOIN App_EmployeeMaster EM
                ON EM.AadharCard = AD.AadharNo
                AND EM.VendorCode = AD.VendorCode
                AND EM.WorkManSlNo = AD.WorkManSl
            WHERE AD.dates >= '2025-01-01' AND AD.dates < '2025-02-01'
            GROUP BY AD.VendorCode, AD.WorkOrderNo, EM.Sex, EM.Social_Category, AD.WorkManCategory
        ),
        ContractorContact AS (
            SELECT
                CREATEDBY AS VendorCode,
                STRING_AGG(NAME + '-' + CONTACT_NO, ', ') AS RESPONSIBLE_PERSON
            FROM App_Vendor_Representative
            GROUP BY CREATEDBY
        ),
        WorkOrders AS (
            SELECT
                V_CODE AS vendorcode,
                WO_NO AS workorder,
                CONVERT(varchar, START_DATE, 103) AS from_date,
                CONVERT(varchar, END_DATE, 103) AS to_date,
                DEPT_CODE AS DepartmentCode,
                TXZ01 AS Description
            FROM App_Vendorwodetails
            WHERE START_DATE < '2025-01-31' AND END_DATE > '2025-01-01'
        )

        SELECT
            '01' AS ProcessMonth,
            '2025' AS ProcessYear,
            mis.vendorcode AS V_Code,
            VM.V_NAME,
            mis.workorder AS WO_NO,
            mis.from_date,
            mis.to_date,
            ISNULL(DM.DepartmentCode, 'Work Order Not Registered') AS DepartmentCode,
            ISNULL(DM.DepartmentName, 'Work Order Not Registered') AS DepartmentName,
            ISNULL(LM.Location, 'Work Order Not Registered') AS Location,
            ISNULL(CC.RESPONSIBLE_PERSON, 'Vendor Registration Not Done') AS RESPONSIBLE_PERSON_OF_THE_CONTRACTOR,

            -- MALE
            SUM(CASE WHEN AA.Sex = 'M' THEN AA.TotalWorkers ELSE 0 END) AS MALE_NO_OF_MALE_WORKERS,
            SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalWorkers ELSE 0 END) AS MALE_NOS_OF_SC_ST_WORKERS,
            SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category = 'OBC' THEN AA.TotalWorkers ELSE 0 END) AS MALE_NOS_OF_OBC_WORKERS,
            SUM(CASE WHEN AA.Sex = 'M' THEN AA.TotalMandays ELSE 0 END) AS MALE_MANDAYS,
            SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalMandays ELSE 0 END) AS MALE_MANDAYS_SC_ST,
            SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category = 'OBC' THEN AA.TotalMandays ELSE 0 END) AS MALE_MANDAYS_OBC,

            -- FEMALE
            SUM(CASE WHEN AA.Sex = 'F' THEN AA.TotalWorkers ELSE 0 END) AS FEMALE_NO_OF_FEMALE_WORKERS,
            SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalWorkers ELSE 0 END) AS FEMALE_NOS_OF_SC_ST_WORKERS,
            SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category = 'OBC' THEN AA.TotalWorkers ELSE 0 END) AS FEMALE_NOS_OF_OBC_WORKERS,
            SUM(CASE WHEN AA.Sex = 'F' THEN AA.TotalMandays ELSE 0 END) AS FEMALE_MANDAYS,
            SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalMandays ELSE 0 END) AS FEMALE_MANDAYS_SC_ST,
            SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category = 'OBC' THEN AA.TotalMandays ELSE 0 END) AS FEMALE_MANDAYS_OBC,

            -- SKILL
            SUM(CASE WHEN AA.WorkManCategory = 'Unskilled' THEN AA.TotalWorkers ELSE 0 END) AS UNSKILLED_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Unskilled' THEN AA.TotalMandays ELSE 0 END) AS UNSKILLED_TOTAL_MANDAYS,
            SUM(CASE WHEN AA.WorkManCategory = 'Semi Skilled' THEN AA.TotalWorkers ELSE 0 END) AS SEMISKILLED_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Skilled' THEN AA.TotalWorkers ELSE 0 END) AS SKILLED_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Skilled' THEN AA.TotalMandays ELSE 0 END) AS SKILLED_TOTAL_MANDAYS,
            SUM(CASE WHEN AA.WorkManCategory = 'Highly Skilled' THEN AA.TotalWorkers ELSE 0 END) AS HIGHLYSKILLED_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Highly Skilled' THEN AA.TotalMandays ELSE 0 END) AS HIGHLYSKILLED_TOTAL_MANDAYS,
            SUM(CASE WHEN AA.WorkManCategory = 'Other' THEN AA.TotalWorkers ELSE 0 END) AS Other_NOS_OF_WORKERS,
            SUM(CASE WHEN AA.WorkManCategory = 'Other' THEN AA.TotalMandays ELSE 0 END) AS Other_TOTAL_MANDAYS,

            SUM(ISNULL(AA.TotalWorkers, 0)) AS Total_NOS_OF_WORKERS,
            SUM(ISNULL(AA.TotalMandays, 0)) AS Total_Mandays,
            mis.Description

        FROM WorkOrders mis
        LEFT JOIN App_VendorMaster VM ON VM.V_CODE = mis.vendorcode
        LEFT JOIN App_DepartmentMaster DM ON DM.DepartmentCode = mis.DepartmentCode
        LEFT JOIN App_WorkOrder_Reg WOR ON WOR.WO_NO = mis.workorder
        LEFT JOIN App_LocationMaster LM ON LM.LocationCode = WOR.LOC_OF_WORK
        LEFT JOIN ContractorContact CC ON CC.VendorCode = mis.vendorcode
        LEFT JOIN AttendanceAgg AA ON AA.VendorCode = mis.vendorcode AND AA.WorkOrderNo = mis.workorder
        WHERE mis.vendorcode = '" + vendorCode + @"'
        GROUP BY
            mis.vendorcode, VM.V_NAME, mis.workorder, mis.from_date, mis.to_date,
            DM.DepartmentCode, DM.DepartmentName, LM.Location, CC.RESPONSIBLE_PERSON, mis.Description
        ORDER BY mis.vendorcode;
        ";

        var ds = new BL_Compliance_MIS().Get_Data(strQuery);
        PageRecordsDataSet = ds;
        Mis_Records.DataSource = PageRecordsDataSet;
        Mis_Records.DataBind();
    }
}
