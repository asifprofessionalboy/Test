WITH TotalPerDay AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
        COUNT(DISTINCT Pno) AS TotalUsers
    FROM App_FaceVerification_Details
    WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
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
    WHERE CONVERT(date, DateAndTime) BETWEEN @FromDate AND @ToDate
    GROUP BY 
        CONVERT(date, DateAndTime),
        CASE 
            WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
            WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
            WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
            ELSE '10+'
        END
),
DateList AS (
    SELECT @FromDate AS TheDate
    UNION ALL
    SELECT DATEADD(DAY, 1, TheDate)
    FROM DateList
    WHERE TheDate < @ToDate
),
AbsentCounts AS (
    SELECT 
        d.TheDate AS AttemptDate,
        'Absent' AS AttemptRange,
        COUNT(*) AS NumberOfUsers,
        (SELECT COUNT(*) FROM App_Empl_Master WHERE Discharge_Date IS NULL) AS TotalUsers,
        CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM App_Empl_Master WHERE Discharge_Date IS NULL) AS DECIMAL(5,2)) AS Percentage
    FROM DateList d
    JOIN App_Empl_Master em ON em.Discharge_Date IS NULL
    WHERE 
        -- Employee was NOT present
        em.pno NOT IN (
            SELECT TRBDGDA_BD_PNO 
            FROM T_TRBDGDAT_EARS 
            WHERE TRBDGDA_BD_DATE = d.TheDate
        )
        -- AND it was NOT one of their official OffDays
        AND DATENAME(WEEKDAY, d.TheDate) NOT IN (em.OffDay1, em.OffDay2)
    GROUP BY d.TheDate
)
-- Final union
SELECT 
    a.AttemptDate,
    a.AttemptRange,
    a.NumberOfUsers,
    a.TotalUsers,
    a.Percentage
FROM AbsentCounts a

UNION ALL

SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate

ORDER BY AttemptDate, AttemptRange
OPTION (MAXRECURSION 1000);




i have this 2 columns for weekend i want to include these also for graph looks same not the upper some Pnos 2 days off and some are one , these 2 are in App_Empl_Master
OffDay1	        OffDay2
Saturday	Sunday
