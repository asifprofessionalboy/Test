DECLARE @StartDate DATE = '2025-06-01';
DECLARE @EndDate DATE = '2025-06-30';

-- Recursive CTE to generate all dates from start to end
WITH DateList AS (
    SELECT @StartDate AS TheDate
    UNION ALL
    SELECT DATEADD(DAY, 1, TheDate)
    FROM DateList
    WHERE TheDate < @EndDate
)
SELECT 
    d.TheDate,
    COUNT(*) AS AbsentCount
FROM DateList d
OUTER APPLY (
    SELECT COUNT(*) AS Cnt
    FROM App_Empl_Master em
    WHERE em.Discharge_Date IS NULL
    AND em.pno NOT IN (
        SELECT TRBDGDA_BD_PNO 
        FROM T_TRBDGDAT_EARS 
        WHERE TRBDGDA_BD_DATE = d.TheDate
    )
) AS Result
GROUP BY d.TheDate
ORDER BY d.TheDate
OPTION (MAXRECURSION 100);

 
 
 
 this is my query to use all dates using recurrsion , make it simple 
 
 select count(*) from App_Empl_Master where Discharge_Date is null and pno not in 
 (select Distinct TRBDGDA_BD_PNO from T_TRBDGDAT_EARS where  TRBDGDA_BD_DATE='06/16/2025' )
