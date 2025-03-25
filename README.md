WITH MonthDates AS (
    -- Generate a list of dates from 1st to 30th October 2024
    SELECT DATEADD(DAY, n, '2024-10-01') AS FullDate, DAY(DATEADD(DAY, n, '2024-10-01')) AS Dates
    FROM (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n 
          FROM master.dbo.spt_values) AS Numbers
    WHERE n < 30 -- Since October has 30 days
)
SELECT 
    w.AadharNo, 
    w.WorkOrderNo, 
    w.WorkManSl, 
    w.WorkManName, 
    w.holiday, 
    (SELECT TOP 1 Sex FROM App_EmployeeMaster WHERE Name = w.WorkManName AND VendorCode = w.VendorCode) AS sex,
    (SELECT TOP 1 Father_Name FROM App_EmployeeMaster WHERE Name = w.WorkManName AND VendorCode = w.VendorCode) AS Father_Name,
    md.Dates,  -- All dates from 1 to 30
    COALESCE(a.Present, NULL) AS Present,  -- If no data, show NULL
    COALESCE(a.DayDef, NULL) AS DayDef
FROM MonthDates md
LEFT JOIN App_WagesDetailsJharkhand w 
    ON w.MonthWage = '10' 
    AND w.YearWage = '2024' 
    AND w.VendorCode = '15261' 
    AND w.LocationCode = 'L_45' 
    AND w.WorkOrderNo = '4700021069' 
    AND w.WorkManName = 'LALIT KUMAR JHA'
LEFT JOIN App_AttendanceDetails a 
    ON a.AadharNo = w.AadharNo 
    AND a.WorkOrderNo = w.WorkOrderNo
    AND DAY(a.Dates) = md.Dates  -- Match each generated date
    AND MONTH(a.Dates) = 10 
    AND YEAR(a.Dates) = 2024
ORDER BY w.AadharNo, md.Dates;

 
 
 
 select distinct w.AadharNo, w.WorkOrderNo as WorkOrderNo,w.WorkManSl as WorkManSl,w.WorkManName 
 as WorkManName,w.holiday as holiday , (select top 1 Sex from App_EmployeeMaster where Name = w.WorkManName and VendorCode = w.VendorCode) sex,
 (select top 1 Father_Name from App_EmployeeMaster where Name = w.WorkManName and VendorCode = w.VendorCode) Father_Name,
 datepart(d, a.Dates) as Dates,a.Present,a.DayDef from App_WagesDetailsJharkhand w inner join
 App_AttendanceDetails a on a.AadharNo = w.AadharNo and month(a.dates)='10' and year(a.dates)= '2024' and a.WorkOrderNo='4700021069'
 where w.MonthWage = '10' and w.YearWage =  '2024' and w.VendorCode = '15261' and w.LocationCode = 'L_45' 
 and w.WorkOrderNo='4700021069' and w.WorkManName='LALIT KUMAR JHA'
 order by  w.AadharNo,datepart(d, a.Dates) 



AadharNo	WorkOrderNo	WorkManSl	WorkManName	holiday	sex	Father_Name	Dates	Present	DayDef
205635127363	4700021069	492		LALIT KUMAR JHA	0	M	BAID NATH JHA	2	0	OD
205635127363	4700021069	492		LALIT KUMAR JHA	0	M	BAID NATH JHA	3	0	WD


this is output i want to get dates according to month here month 10 choosen than i want to list all ocober months dates 1 to 30 in 
the row  with all data if data is not prsent than put null there 
