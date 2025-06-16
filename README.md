this is my old query to fetch details

WITH TotalPerDay AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
        COUNT(DISTINCT Pno) AS TotalUsers
    FROM App_FaceVerification_Details
    WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-01' AND '2025-06-30'
    GROUP BY CONVERT(date, DateAndTime)
),
GroupedCounts AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
        CASE 
            WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
            WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
            WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
            ELSE '10+'
        END AS AttemptRange,
        COUNT(DISTINCT Pno) AS NumberOfUsers
    FROM App_FaceVerification_Details
    WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-01' AND '2025-06-30'
    GROUP BY 
        CONVERT(date, DateAndTime),
        CASE 
            WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
            WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
            WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
            ELSE '10+'
        END
)
SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate
ORDER BY g.AttemptDate, g.AttemptRange;


this is my new query to fetch absent user 
DECLARE @StartDate DATE = '2025-06-01';
DECLARE @EndDate DATE = '2025-06-30';

WITH DateList AS (
    SELECT @StartDate AS TheDate
    UNION ALL
    SELECT DATEADD(DAY, 1, TheDate)
    FROM DateList
    WHERE TheDate < @EndDate
)
SELECT d.TheDate,
    (
        SELECT COUNT(*) 
        FROM App_Empl_Master em
        WHERE em.Discharge_Date IS NULL
        AND em.pno NOT IN (
            SELECT TRBDGDA_BD_PNO 
            FROM T_TRBDGDAT_EARS 
            WHERE TRBDGDA_BD_DATE = d.TheDate
        ) 
    ) AS AbsentCount 
FROM DateList d
ORDER BY d.TheDate
OPTION (MAXRECURSION 31);

i want to merge this from above query to found out the absent user also
