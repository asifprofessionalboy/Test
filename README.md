SELECT 
    R.ID,
    R.ReqNo,
    R.Status,
    CASE 
        WHEN R.Status = 'Pending with HOD' THEN 
            R.HOD_Pno COLLATE SQL_Latin1_General_CP1_CI_AS + '-' + R.HOD_Name COLLATE SQL_Latin1_General_CP1_CI_AS
        WHEN R.Status = 'Pending with Reporting Manager' THEN 
            R.Reporting_Manager_Pno COLLATE SQL_Latin1_General_CP1_CI_AS + '-' + R.Reporting_Manager_Name COLLATE SQL_Latin1_General_CP1_CI_AS
        WHEN R.Status = 'Pending with Division Head' THEN 
            (SELECT 
                E.Pno COLLATE SQL_Latin1_General_CP1_CI_AS + ' - ' + E.Ename COLLATE SQL_Latin1_General_CP1_CI_AS 
             FROM UserLoginDB.dbo.App_EmployeeMaster E
             WHERE E.pno COLLATE SQL_Latin1_General_CP1_CI_AS = R.Division_Head COLLATE SQL_Latin1_General_CP1_CI_AS
            )
    END AS PendingWith
FROM App_Online_Resignation_Entry R
