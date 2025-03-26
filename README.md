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
