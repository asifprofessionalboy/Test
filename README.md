 this is my query to use all dates using recurrsion , make it simple 
 
 select count(*) from App_Empl_Master where Discharge_Date is null and pno not in 
 (select Distinct TRBDGDA_BD_PNO from T_TRBDGDAT_EARS where  TRBDGDA_BD_DATE='06/16/2025' )
