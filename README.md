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
