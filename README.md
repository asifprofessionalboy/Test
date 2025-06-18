const datasets = ranges.map(range => {
    return {
        label: range,
        borderColor: colors[range],
        backgroundColor: colors[range],
        tension: 0.3,
        fill: false,
        data: labels.map(date => {
            const match = data.find(d => d.attemptDate === date && d.attemptRange === range);
            return {
                x: date,
                y: match ? match.percentage : 0,
                numberOfUsers: match ? match.numberOfUsers : 0
            };
        })
    };
});

borderDash: range === 'Absent' ? [5, 5] : undefined,

const datasets = ranges.map(range => {
    return {
        label: range,
        borderColor: colors[range],
        backgroundColor: colors[range],
        tension: 0.3,
        fill: false,
        borderDash: range === 'Absent' ? [5, 5] : undefined,
        data: labels.map(date => {
            const match = data.find(d => d.attemptDate === date && d.attemptRange === range);
            return {
                x: date,
                y: match ? match.percentage : 0,
                numberOfUsers: match ? match.numberOfUsers : 0
            };
        })
    };
});




WITH TotalEmployees AS (
    SELECT COUNT(*) AS Total FROM App_Empl_Master WHERE Discharge_Date IS NULL
),
TotalPerDay AS (
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
        COUNT(*) AS NumberOfUsers
    FROM DateList d
    JOIN App_Empl_Master em ON em.Discharge_Date IS NULL
    WHERE em.pno NOT IN (
        SELECT TRBDGDA_BD_PNO 
        FROM T_TRBDGDAT_EARS 
        WHERE TRBDGDA_BD_DATE = d.TheDate
    )
    GROUP BY d.TheDate
)
SELECT 
    a.AttemptDate,
    a.AttemptRange,
    a.NumberOfUsers,
    t.Total AS TotalUsers,
    CAST(a.NumberOfUsers * 100.0 / t.Total AS DECIMAL(5, 2)) AS Percentage
FROM AbsentCounts a
CROSS JOIN TotalEmployees t

UNION ALL

SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate

ORDER BY AttemptDate, 
    CASE 
        WHEN AttemptRange = '0-2' THEN 1
        WHEN AttemptRange = '3-5' THEN 2
        WHEN AttemptRange = '6-10' THEN 3
        WHEN AttemptRange = '10+' THEN 4
        WHEN AttemptRange = 'Absent' THEN 5
        ELSE 6
    END
OPTION (MAXRECURSION 100);

                
                
                
                
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
        COUNT(*) AS NumberOfUsers
    FROM DateList d
    JOIN App_Empl_Master em ON em.Discharge_Date IS NULL
    WHERE em.pno NOT IN (
        SELECT TRBDGDA_BD_PNO 
        FROM T_TRBDGDAT_EARS 
        WHERE TRBDGDA_BD_DATE = d.TheDate
    )
    GROUP BY d.TheDate
)
SELECT 
    AttemptDate,
    AttemptRange,
    NumberOfUsers,
    NULL AS TotalUsers,
    NULL AS Percentage
FROM AbsentCounts

UNION ALL

SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate

ORDER BY AttemptDate, AttemptRange;

in this query i want absent in percentage total number of users in App_Empl_Master and absent user percentage
