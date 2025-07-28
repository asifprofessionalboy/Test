
select R.ID,R.ReqNo,R.Status,
CASE WHEN (R.Status= 'Pending with HOD') THEN (R.HOD_Pno + '-' + R.HOD_Name)
WHEN (R.Status='Pending with Reporting Manager') THEN (R.Reporting_Manager_Pno + '-' + R.Reporting_Manager_Name)
WHEN (R.Status = 'Pending with Division Head') THEN 
(select (E.Pno+' - '+E.Ename) from UserLoginDB.dbo.App_EmployeeMaster E
where E.pno COLLATE SQL_Latin1_General_CP1_CI_AS = R.Division_Head COLLATE SQL_Latin1_General_CP1_CI_AS) 
end
from App_Online_Resignation_Entry R

Msg 457, Level 16, State 1, Line 2
Implicit conversion of varchar value to varchar cannot be performed because the collation of the value is unresolved due to a collation conflict between "Latin1_General_CI_AI" and "SQL_Latin1_General_CP1_CI_AS" in CASE operator.
