WITH Dateseries AS (
    SELECT CAST('2024-10-01' AS DATE) AS Dates
    UNION ALL
    SELECT DATEADD(DAY, 1, Dates)
    FROM Dateseries
    WHERE DATEADD(DAY, 1, Dates) <= EOMONTH('2024-10-01')
),
WorkmanData AS (
    -- Ensure we get all workmen even if no attendance is present
    SELECT DISTINCT 
        w.AadharNo, 
        w.WorkOrderNo, 
        w.WorkManSl, 
        w.WorkManName, 
        w.holiday, 
        (SELECT TOP 1 Sex FROM App_EmployeeMaster WHERE Name = w.WorkManName AND VendorCode = w.VendorCode) AS sex,
        (SELECT TOP 1 Father_Name FROM App_EmployeeMaster WHERE Name = w.WorkManName AND VendorCode = w.VendorCode) AS Father_Name
    FROM App_WagesDetailsJharkhand w
    WHERE w.MonthWage = '10' 
      AND w.YearWage = '2024' 
      AND w.VendorCode = '15261' 
      AND w.LocationCode = 'L_45' 
      AND w.WorkOrderNo = '4700025985'
      AND w.AadharNo = '903394859497'
),
AttendanceData AS (
    -- Attendance data separately to allow proper join
    SELECT 
        a.AadharNo,
        CONVERT(DATE, a.Dates) AS Dates, 
        a.Present, 
        a.DayDef
    FROM App_AttendanceDetails a
    WHERE MONTH(a.Dates) = 10 
      AND YEAR(a.Dates) = 2024 
      AND a.WorkOrderNo = '4700025985'
)
SELECT 
    DATEPART(DAY, d.Dates) AS Dates,
    w.WorkOrderNo,
    w.WorkOrderNo,  -- This looks like a duplicate column, consider removing if unintended
    w.WorkManSl,
    w.WorkManName,
    w.holiday,
    w.sex,
    w.Father_Name,
    a.Present,
    a.DayDef
FROM WorkmanData w
CROSS JOIN Dateseries d  -- Ensures all dates exist for each worker
LEFT JOIN AttendanceData a ON w.AadharNo = a.AadharNo AND d.Dates = a.Dates
ORDER BY d.Dates;

 
 
 
 With  Dateseries As ( Select CAST('2024-10-01' as DATE) as Dates
union all
select DATEADD (DAY, 1,Dates)
from Dateseries
where dateadd(DAY, 1,Dates) <= EOMONTH('2024-10-01')
),
workmandata as(
 select distinct w.AadharNo, w.WorkOrderNo as WorkOrderNo,w.WorkManSl as WorkManSl,w.WorkManName as WorkManName,w.holiday as holiday , 
 (select top 1 Sex from App_EmployeeMaster where Name = w.WorkManName and VendorCode = w.VendorCode) sex,
 (select top 1 Father_Name from App_EmployeeMaster where Name = w.WorkManName and VendorCode = w.VendorCode) Father_Name,
CONVERT(date, a.Dates) as Dates,a.Present,a.DayDef  from App_WagesDetailsJharkhand w inner join App_AttendanceDetails a 
 on a.AadharNo = w.AadharNo and month(a.dates)='10' and year(a.dates)= '2024' and a.WorkOrderNo='4700025985'
 where w.MonthWage = '10' and w.YearWage =  '2024' and w.VendorCode = '15261' and w.LocationCode = 'L_45' 
 and w.WorkOrderNo='4700025985' and w.AadharNo='903394859497' )
select 
 datepart(d, d.Dates) as Dates,w.WorkOrderNo,w.WorkOrderNo,w.WorkManSl,w.WorkManName,w.holiday,w.sex,w.Father_Name, w.Present,
 w.DayDef  from Dateseries d left join  workmandata  w on d.Dates = w.Dates 
 order by d.Dates


i want w.WorkOrderNo,w.WorkOrderNo,w.WorkManSl,w.WorkManName,w.sex,w.Father_Name in my result output beacuse it came from App_WagesDetailsJharkhand and rest data can be null if not present
