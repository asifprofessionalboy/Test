



select distinct '01' as ProcessMonth,'2025' as ProcessYear, mis.vendorcode,VM.V_NAME,mis.workorder,mis.from_date,mis.to_date
,isnull(DM.DepartmentCode, 'Work Order Not Registered') as DepartmentCode,
isnull(DM.DepartmentName, 'Work Order Not Registered') as DepartmentName,
isnull(LM.Location, 'Work Order Not Registered') Location,
isnull((select top 1 EMAIL = STUFF((SELECT DISTINCT ', ' + (NAME + '-' + CONTACT_NO) from App_Vendor_Representative as b 
where b.CREATEDBY = mis.vendorcode FOR XML PATH('')), 1, 2, '') 
FROM App_Vendor_Representative as a  ),'Vendor Registration Not Done') as RESPONSIBLE_PERSON_OF_THE_CONTRACTOR ,




(select distinct count(EM.AadharCard) from App_AttendanceDetails AD inner join App_EmployeeMaster EM on
EM.AadharCard = AD.AadharNo and EM.VendorCode = AD.VendorCode and EM.WorkManSlNo = AD.WorkManSl 
and EM.Sex = 'M'  where AD.Dates= DATEPART(month, '2025-01-31 23:59:59') 
and AD.Dates= DATEPART(YEAR, '2025-01-31 23:59:59')   and AD.VendorCode = mis.vendorcode
and AD.WorkOrderNo = mis.workorder and AD.Present =1    ) as MALE_NO_OF_MALE_WORKERS ,



(isnull((select distinct sum(convert(int,AD.Present)) from App_AttendanceDetails AD where 
AD.Dates= DATEPART(month, '2025-01-31 23:59:59') and AD.Dates= DATEPART(YEAR, '2025-01-31 23:59:59') 
and AD.VendorCode = mis.vendorcode and AD.WorkManCategory = 'Unskilled' 
and AD.WorkOrderNo = mis.workorder), 0) + isnull((select distinct sum(convert(int,AD.Present))
from App_AttendanceDetails AD where AD.Dates = DATEPART(month, '2025-01-31 23:59:59')
and AD.Dates= DATEPART(YEAR, '2025-01-31 23:59:59')  and AD.VendorCode = mis.vendorcode
and AD.WorkManCategory = 'Semi Skilled' and AD.WorkOrderNo = mis.workorder),0)+ isnull((select distinct sum(convert(int,AD.Present))
from App_AttendanceDetails AD where AD.Dates = DATEPART(month, '2025-01-31 23:59:59')
and AD.Dates= DATEPART(YEAR, '2025-01-31 23:59:59')  and AD.VendorCode = mis.vendorcode 
and AD.WorkManCategory = 'Skilled' and AD.WorkOrderNo = mis.workorder),0)+  isnull((select distinct sum(convert(int,AD.Present))
from App_AttendanceDetails AD where AD.Dates= DATEPART(month, '2025-01-31 23:59:59') and 
AD.Dates= DATEPART(YEAR, '2025-01-31 23:59:59')  and AD.VendorCode = mis.vendorcode 
and AD.WorkManCategory = 'Highly Skilled' and AD.WorkOrderNo = mis.workorder),0)+ isnull((select distinct sum(convert(int,AD.Present))
from App_AttendanceDetails AD where AD.Dates= DATEPART(month, '2025-01-31 23:59:59') and 
AD.Dates= DATEPART(YEAR, '2025-01-31 23:59:59')  and AD.VendorCode = mis.vendorcode and AD.WorkManCategory = 'Other' 
and AD.WorkOrderNo = mis.workorder),0) ) as Total_Mandays,mis.Description from (select V_CODE as vendorcode, WO_NO as workorder,
Convert(varchar, START_DATE, 103) as from_date, Convert(varchar, END_DATE, 103) as to_date, DEPT_CODE as DepartmentCode,
TXZ01 as Description from App_Vendorwodetails where START_DATE < '2025-01-31 23:59:59' and END_DATE > '2025-1-01 00:00:00.000')
mis  left join App_VendorMaster VM on VM.V_CODE = mis.vendorcode  left join
App_DepartmentMaster DM on DM.DepartmentCode = mis.DepartmentCode  left join App_WorkOrder_Reg WOR on WOR.WO_NO = mis.workorder 
left join App_LocationMaster LM on LM.LocationCode = WOR.LOC_OF_WORK  where  1=1   order by mis.vendorcode 


