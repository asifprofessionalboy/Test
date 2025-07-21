(select distinct count(EM.AadharCard) from App_AttendanceDetails AD inner join App_EmployeeMaster EM on
EM.AadharCard = AD.AadharNo and EM.VendorCode = AD.VendorCode and EM.WorkManSlNo = AD.WorkManSl 
and EM.Sex = 'M'  where datepart(month,Ad.dates)='1'
and datepart(year,Ad.dates)='2025'   and AD.VendorCode = mis.vendorcode
and AD.WorkOrderNo = mis.workorder and AD.Present =1    ) as MALE_NO_OF_MALE_WORKERS 
