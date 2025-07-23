 select L.Location,A.[WorkOrderNo],A.[EngagementType],A.[Dates],A.[DayWeek],A.[Shifts],A.[InTimeHr],A.[InTimeMin],A.[OutTimeHr],
 A.[OutTimeMin],A.[Present],S.Site,A.[WorkManSl],A.[WorkManName],A.[VendorCode],A.[AadharNo],A.[WorkManCategory],A.[DayDef],A.[OT_hrs] 
 from App_AttendanceDetails A left join App_LocationMaster L on A.LocationCode = L.LocationCode  left join App_SiteMaster S on S.ID = A.SiteID 
 where VendorCode = '10482' and Dates = '2025/06/02' order by WorkManName
