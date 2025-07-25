<div style="overflow-x: scroll; height: 450px" id="Mis_Grid" runat="server">
    <cc1:DetailsContainer ID="Mis_Records" runat="server" AutoGenerateColumns="True"
        OnRowCreated="Mis_Records_RowCreated" CssClass="table"
        AllowPaging="false" CellPadding="4" GridLines="None" Width="100%"
        ForeColor="#333333" ShowHeaderWhenEmpty="True"
        PageSize="10" PagerSettings-Visible="True" PagerStyle-HorizontalAlign="Center"
        PagerStyle-Wrap="False" HeaderStyle-Font-Size="Smaller" RowStyle-Font-Size="Smaller">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>

            <asp:BoundField DataField="SlNo" HeaderText="SlNo" />

            <asp:BoundField DataField="vendorcode" HeaderText="Vendor Code" />
            <asp:BoundField DataField="V_NAME" HeaderText="Vendor Name" />

            <asp:BoundField DataField="workorder" HeaderText="Workorder No." />
            <asp:BoundField DataField="from_date" HeaderText="From Date" />
            <asp:BoundField DataField="to_date" HeaderText="To Date" />
            <asp:BoundField DataField="NatureOfWork" HeaderText="Nature Of Work" />


            <asp:BoundField DataField="DepartmentCode" HeaderText="Department Code" />
            <asp:BoundField DataField="DepartmentName" HeaderText="Department Name" />

            <asp:BoundField DataField="Location" HeaderText="Location" />

            <asp:BoundField DataField="RESPONSIBLE_PERSON_OF_THE_CONTRACTOR" HeaderText="Responsible Person" />


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

            <asp:BoundField DataField="UNSKILLED_NOS_OF_WORKERS" HeaderText="Unskilled Workers" />
            <asp:BoundField DataField="UNSKILLED_TOTAL_MANDAYS" HeaderText="Unskilled Mandays" />

            <asp:BoundField DataField="SEMISKILLED_NOS_OF_WORKERS" HeaderText="Semi-Skilled Workers" />
            <asp:BoundField DataField="SEMISKILLED_TOTAL_MANDAYS" HeaderText="Semi-Skilled Mandays" />


            <asp:BoundField DataField="SKILLED_NOS_OF_WORKERS" HeaderText="Skilled Workers" />
            <asp:BoundField DataField="SKILLED_TOTAL_MANDAYS" HeaderText="Skilled Mandays" />
            <asp:BoundField DataField="HIGHLYSKILLED_NOS_OF_WORKERS" HeaderText="Highly Skilled Workers" />
            <asp:BoundField DataField="HIGHLYSKILLED_TOTAL_MANDAYS" HeaderText="Highly Skilled Mandays" />
            <asp:BoundField DataField="Other_NOS_OF_WORKERS" HeaderText="Other Workers" />
            <asp:BoundField DataField="Other_TOTAL_MANDAYS" HeaderText="Other Mandays" />

            <asp:BoundField DataField="Total_NOS_OF_WORKERS" HeaderText="Total Workers" />
            <asp:BoundField DataField="Total_Mandays" HeaderText="Total Mandays" />



            <asp:BoundField DataField="BasicDA" HeaderText="Basic + DA" />
            <asp:BoundField DataField="Allowance" HeaderText="Other Allowance" />
            <asp:BoundField DataField="GrossWages" HeaderText="Gross Wages" />

            <asp:BoundField DataField="Other_DEDUCTION" HeaderText="Other Deduction" />
            <asp:BoundField DataField="NET_WAGES_AMOUNT" HeaderText="Net Wages Amount" />
            <asp:BoundField DataField="WagesStatus" HeaderText="Wages Status" />
            <asp:BoundField DataField="Payment_date_WAGES" HeaderText="Wages Payment Date" />



            <asp:BoundField DataField="PF_DEDUCTION" HeaderText="PF Deduction" />
            <asp:BoundField DataField="ESI_DEDUCTION" HeaderText="ESI Deduction" />
            <asp:BoundField DataField="PFESI_Status" HeaderText="PF/ESI Status" />
            <asp:BoundField DataField="PFPaymentDate" HeaderText="PF Payment Date" />
            <asp:BoundField DataField="ESIPaymentDate" HeaderText="ESI Payment Date" />

            <asp:BoundField DataField="Temporary" HeaderText="Temporary" />
            <asp:BoundField DataField="Permanent" HeaderText="Permanent" />
            <asp:BoundField DataField="Recognized" HeaderText="Recognized" />

        </Columns>
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerSettings Mode="Numeric" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" Font-Bold="True" CssClass="pager1" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="False" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
    </cc1:DetailsContainer>

</div>



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string stryear = string.Join(",", Year.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));
            string strmnth = string.Join(",", Month.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));

            if (!string.IsNullOrEmpty(stryear) && !string.IsNullOrEmpty(strmnth))
            {
                string Locationvalue = string.Join(",", Location.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => $"'{i.Value}'"));
                string Dpartmentvalue = string.Join(",", Department.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => $"'{i.Value}'"));

                string locationFilter = string.IsNullOrEmpty(Locationvalue) ? "" : $" AND WOR.LOC_OF_WORK IN ({Locationvalue}) ";
                string departmentFilter = string.IsNullOrEmpty(Dpartmentvalue) ? "" : $" AND tab.DepartmentCode IN ({Dpartmentvalue}) ";

                StringBuilder strQueryBuilder = new StringBuilder();
                string[] yearArray = stryear.Split(',');
                string[] monthArray = strmnth.Split(',');

                foreach (string year in yearArray)
                {
                    foreach (string month in monthArray)
                    {
                        int intMonth = Convert.ToInt32(month);
                        int intYear = Convert.ToInt32(year);

                        int nextMonth = (intMonth == 12) ? 1 : intMonth + 1;
                        int nextYear = (intMonth == 12) ? intYear + 1 : intYear;

                        string monthStr = intMonth.ToString("D2");
                        string startDate = $"{intYear}-{monthStr}-01";
                        string endDate = new DateTime(nextYear, nextMonth, 1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss");

                       string subQuery =
                            $@"
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
                     WHERE AD.dates >= '{startDate}' AND AD.dates < '{endDate}'
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
                     WHERE START_DATE <= '{endDate}' AND END_DATE >= '{startDate}'
                 ),

                 WageAgg AS (
                     SELECT DISTINCT
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
                     WHERE MonthWage = '{intMonth}' AND YearWage = '{intYear}'
                     GROUP BY VendorCode, WorkOrderNo
                 )

                 SELECT
                     ROW_NUMBER() OVER (ORDER BY mis.vendorcode) AS SlNo,
                     '{monthStr}' AS ProcessMonth,
                     '{intYear}' AS ProcessYear,
                     mis.vendorcode,
                     VM.V_NAME,
                     mis.workorder,
                     mis.from_date,
                     mis.to_date,
                     ISNULL(DM.DepartmentCode, 'Work Order Not Registered') AS DepartmentCode,
                     ISNULL(DM.DepartmentName, 'Work Order Not Registered') AS DepartmentName,
                     ISNULL(LM.Location, 'Work Order Not Registered') AS Location,
                     ISNULL(CC.RESPONSIBLE_PERSON, 'Vendor Registration Not Done') AS RESPONSIBLE_PERSON_OF_THE_CONTRACTOR,
                     mis.Description as NatureOfWork,

                     SUM(CASE WHEN AA.Sex = 'M' THEN AA.TotalWorkers ELSE 0 END) AS MALE_NO_OF_MALE_WORKERS,
                     SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalWorkers ELSE 0 END) AS MALE_NOS_OF_SC_ST_WORKERS,
                     SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category = 'OBC' THEN AA.TotalWorkers ELSE 0 END) AS MALE_NOS_OF_OBC_WORKERS,
                     SUM(CASE WHEN AA.Sex = 'M' THEN AA.TotalMandays ELSE 0 END) AS MALE_MANDAYS,
                     SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalMandays ELSE 0 END) AS MALE_MANDAYS_SC_ST,
                     SUM(CASE WHEN AA.Sex = 'M' AND AA.Social_Category = 'OBC' THEN AA.TotalMandays ELSE 0 END) AS MALE_MANDAYS_OBC,

                     SUM(CASE WHEN AA.Sex = 'F' THEN AA.TotalWorkers ELSE 0 END) AS FEMALE_NO_OF_FEMALE_WORKERS,
                     SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalWorkers ELSE 0 END) AS FEMALE_NOS_OF_SC_ST_WORKERS,
                     SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category = 'OBC' THEN AA.TotalWorkers ELSE 0 END) AS FEMALE_NOS_OF_OBC_WORKERS,
                     SUM(CASE WHEN AA.Sex = 'F' THEN AA.TotalMandays ELSE 0 END) AS FEMALE_MANDAYS,
                     SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category IN ('ST','SC') THEN AA.TotalMandays ELSE 0 END) AS FEMALE_MANDAYS_SC_ST,
                     SUM(CASE WHEN AA.Sex = 'F' AND AA.Social_Category = 'OBC' THEN AA.TotalMandays ELSE 0 END) AS FEMALE_MANDAYS_OBC,

                     ISNULL(SUM(AA.TotalWorkers), 0) AS Total_NOS_OF_WORKERS,
                     ISNULL(SUM(AA.TotalMandays), 0) AS Total_Mandays,

                     ISNULL((SELECT DISTINCT CONVERT(varchar(50), OW.PAYMENT_DATE, 103)
                             FROM App_Online_Wages OW
                             INNER JOIN App_Online_Wages_Details OWD
                                 ON OWD.MonthWage = OW.MonthWage AND OWD.YearWage = OW.YearWage
                                 AND OWD.VendorCode = OW.V_CODE AND OWD.WorkOrderNo = mis.workorder
                             WHERE OW.MonthWage = '{intMonth}' AND OW.YearWage = '{intYear}' AND OW.STATUS = 'Request Closed' AND OW.V_CODE = mis.vendorcode), '') AS Payment_date_WAGES,

                     ISNULL((SELECT DISTINCT OW.STATUS
                             FROM App_Online_Wages OW
                             INNER JOIN App_Online_Wages_Details OWD
                                 ON OWD.MonthWage = OW.MonthWage AND OWD.YearWage = OW.YearWage
                                 AND OWD.VendorCode = OW.V_CODE AND OWD.WorkOrderNo = mis.workorder
                             WHERE OW.MonthWage = '{intMonth}' AND OW.YearWage = '{intYear}' AND OW.STATUS = 'Request Closed' AND OW.V_CODE = mis.vendorcode), '') AS WagesStatus,

                     ISNULL((SELECT DISTINCT CONVERT(varchar(50), OW.PFChallanDate, 103)
                             FROM App_PF_ESI_Summary OW
                             INNER JOIN App_PF_ESI_Details OWD
                                 ON OWD.MonthWage = OW.MonthWage AND OWD.YearWage = OW.YearWage
                                 AND OWD.VendorCode = OW.VendorCode AND OWD.WorkOrderNo = mis.workorder
                             WHERE OW.MonthWage = '{intMonth}' AND OW.YearWage = '{intYear}' AND OW.STATUS = 'Request Closed' AND OW.VendorCode = mis.vendorcode), '') AS PFPaymentDate,

                     ISNULL((SELECT DISTINCT CONVERT(varchar(50), OW.ESIChallanDate, 103)
                             FROM App_PF_ESI_Summary OW
                             INNER JOIN App_PF_ESI_Details OWD
                                 ON OWD.MonthWage = OW.MonthWage AND OWD.YearWage = OW.YearWage
                                 AND OWD.VendorCode = OW.VendorCode AND OWD.WorkOrderNo = mis.workorder
                             WHERE OW.MonthWage = '{intMonth}' AND OW.YearWage = '{intYear}' AND OW.STATUS = 'Request Closed' AND OW.VendorCode = mis.vendorcode), '') AS ESIPaymentDate,

                     ISNULL((SELECT DISTINCT OW.Status
                             FROM App_PF_ESI_Summary OW
                             INNER JOIN App_PF_ESI_Details OWD
                                 ON OWD.MonthWage = OW.MonthWage AND OWD.YearWage = OW.YearWage
                                 AND OWD.VendorCode = OW.VendorCode AND OWD.WorkOrderNo = mis.workorder
                             WHERE OW.MonthWage = '{intMonth}' AND OW.YearWage = '{intYear}' AND OW.STATUS = 'Request Closed' AND OW.VendorCode = mis.vendorcode), '') AS PFESI_Status,

                     SUM(CASE WHEN AA.WorkManCategory = 'Unskilled' THEN AA.TotalWorkers ELSE 0 END) AS UNSKILLED_NOS_OF_WORKERS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Unskilled' THEN AA.TotalMandays ELSE 0 END) AS UNSKILLED_TOTAL_MANDAYS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Semi Skilled' THEN AA.TotalWorkers ELSE 0 END) AS SEMISKILLED_NOS_OF_WORKERS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Semi Skilled' THEN AA.TotalMandays ELSE 0 END) AS SEMISKILLED_TOTAL_MANDAYS,

                     SUM(CASE WHEN AA.WorkManCategory = 'Skilled' THEN AA.TotalWorkers ELSE 0 END) AS SKILLED_NOS_OF_WORKERS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Skilled' THEN AA.TotalMandays ELSE 0 END) AS SKILLED_TOTAL_MANDAYS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Highly Skilled' THEN AA.TotalWorkers ELSE 0 END) AS HIGHLYSKILLED_NOS_OF_WORKERS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Highly Skilled' THEN AA.TotalMandays ELSE 0 END) AS HIGHLYSKILLED_TOTAL_MANDAYS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Other' THEN AA.TotalWorkers ELSE 0 END) AS Other_NOS_OF_WORKERS,
                     SUM(CASE WHEN AA.WorkManCategory = 'Other' THEN AA.TotalMandays ELSE 0 END) AS Other_TOTAL_MANDAYS,

                     ISNULL(WA.BasicA, 0) AS BasicDA,
                     ISNULL(WA.Allowance, 0) AS Allowance,
                     ISNULL(WA.GrossWages, 0) AS GrossWages,
                     ISNULL(WA.PF_DEDUCTION, 0) AS PF_DEDUCTION,
                     ISNULL(WA.ESI_DEDUCTION, 0) AS ESI_DEDUCTION,
                     ISNULL(WA.Other_DEDUCTION, 0) AS Other_DEDUCTION,
                     ISNULL(WA.NET_WAGES_AMOUNT, 0) AS NET_WAGES_AMOUNT,

                     CASE WHEN EXISTS (
                         SELECT 1 FROM App_Wo_Nil T
                         WHERE T.WO_NO = mis.workorder
                           AND T.NO_WORK = 'Temporary'
                           AND T.TEMPORARY_YEAR = '{intYear}'
                           AND T.TEMPORARY_MONTH = '{intMonth}'
                     ) THEN 'Yes' ELSE 'No' END AS Temporary,

                     CASE WHEN EXISTS (
                         SELECT 1 FROM App_Wo_Nil P
                         WHERE P.WO_NO = mis.workorder
                           AND P.NO_WORK = 'Permanent'
                           AND CONVERT(INT, P.TEMPORARY_YEAR + FORMAT(CONVERT(INT, P.CLOSER_DATE), '00')) <= {intYear}{monthStr}
                     ) THEN 'Yes' ELSE 'No' END AS Permanent,

                     CASE WHEN EXISTS (
                         SELECT 1 FROM APP_RECOGNIZED_WO R
                         WHERE R.WO_NO = mis.workorder
                     ) THEN 'Yes' ELSE 'No' END AS Recognized

                 FROM WorkOrders mis
                 LEFT JOIN App_VendorMaster VM ON VM.V_CODE = mis.vendorcode
                 LEFT JOIN App_DepartmentMaster DM ON DM.DepartmentCode = mis.DepartmentCode
                 LEFT JOIN App_WorkOrder_Reg WOR ON WOR.WO_NO = mis.workorder
                 LEFT JOIN App_LocationMaster LM ON LM.LocationCode = WOR.LOC_OF_WORK
                 LEFT JOIN ContractorContact CC ON CC.VendorCode = mis.vendorcode
                 LEFT JOIN AttendanceAgg AA ON AA.VendorCode = mis.vendorcode AND AA.WorkOrderNo = mis.workorder
                 LEFT JOIN WageAgg WA ON WA.VendorCode = mis.vendorcode AND WA.WorkOrderNo = mis.workorder

                 WHERE 1=1
                 {locationFilter}
                 {departmentFilter}

                 GROUP BY
                     mis.vendorcode, VM.V_NAME, mis.workorder, mis.from_date, mis.to_date,
                     DM.DepartmentCode, DM.DepartmentName, LM.Location,
                     CC.RESPONSIBLE_PERSON, WA.BasicA, WA.Allowance, WA.GrossWages,
                     WA.PF_DEDUCTION, WA.ESI_DEDUCTION, WA.Other_DEDUCTION, WA.NET_WAGES_AMOUNT,
                     mis.Description

                 ORDER BY mis.vendorcode;
                 ";



                        strQueryBuilder.AppendLine(subQuery);
                    }
                }

                string finalQuery = strQueryBuilder.ToString();

                string connstr = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

                DataSet ds = new DataSet();

                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    using (SqlCommand cmd = new SqlCommand(finalQuery, conn))
                    {
                      
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }

                if (ds != null && ds.Tables.Count > 0)
                {
                    Mis_Records.DataSource = ds;
                    Mis_Records.DataBind();
                }
            }
        }
