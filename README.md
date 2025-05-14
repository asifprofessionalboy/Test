SELECT DISTINCT App.Pno
FROM INNOVATIONDB.dbo.App_Login AS Inn
INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
    ON Inn.UserId COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
INNER JOIN APPROVERDB.dbo.App_ApproverMaster AS App
    ON Emp.DepartmentName COLLATE DATABASE_DEFAULT = App.DepartmentName COLLATE DATABASE_DEFAULT;




SELECT DISTINCT App.Pno
FROM INNOVATIONDB.dbo.App_Login AS Inn
INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
    ON Inn.UserId COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
INNER JOIN App_ApproverMaster AS App
    ON Emp.DepartmentName COLLATE DATABASE_DEFAULT = App.DepartmentName COLLATE DATABASE_DEFAULT;





this is my first table where UserId is present, Select UserId from INNOVATIONDB.dbo.App_Login as Inn,
this is my 2nd table from which i want to fetch department against the UserId from First table, Select DepartmentName From UserLoginDB.dbo.App_EmployeeMaster as Emp
and this is my 3rd table i want to fetch Pno against the Department Name, Select Pno From App_ApproverMaster as App 

and this is my query please provide correct query to fetch data that is explained above 
select  distinct App.Pno from INNOVATIONDB.dbo.App_Login as Inn
inner Join UserLoginDB.dbo.App_EmployeeMaster as Emp
on Inn.UserId COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
inner Join App_ApproverMaster as App
on Emp.DepartmentName COLLATE DATABASE_DEFAULT = App.DepartmentName COLLATE DATABASE_DEFAULT
