;WITH Leave_Processed AS (
    SELECT *, CreatedOn AS ApplicationDate
    FROM App_Leave_Comp_Summary
    WHERE ReSubmiteddate IS NULL

    UNION ALL

    SELECT *, ReSubmiteddate AS ApplicationDate
    FROM App_Leave_Comp_Summary
    WHERE ReSubmiteddate IS NOT NULL
),
Leave_Filtered AS (
    SELECT *
    FROM Leave_Processed
    WHERE ApplicationDate >= '2025-04-01' AND ApplicationDate < '2026-04-01'
),
Leave_Aggregated AS (
    SELECT
        DATEPART(MONTH, ApplicationDate) AS MonthNum,
        SUM(CASE WHEN Status = 'Request Closed' THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE 
            WHEN Status = 'Request Closed' 
                 AND CC_CreatedOn_L2 IS NOT NULL 
                 AND DATEDIFF(DAY, ApplicationDate, CC_CreatedOn_L2) <= 5 
            THEN 1 ELSE 0 
        END) AS ApprovedUnderSLA
    FROM Leave_Filtered
    GROUP BY DATEPART(MONTH, ApplicationDate)
),
Leave_Pivoted AS (
    SELECT
        'Leave Compliance' AS Object,
        '5 days' AS SLG,
        '5 days' AS RevisedSLG,
        MAX(CASE WHEN MonthNum = 4 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS APR,
        MAX(CASE WHEN MonthNum = 5 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS MAY,
        MAX(CASE WHEN MonthNum = 6 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS JUN,
        MAX(CASE WHEN MonthNum = 7 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS JUL
),
Wage_Processed AS (
    SELECT *, CREATEDON AS ApplicationDate
    FROM App_Online_Wages
    WHERE ReSubmitedOn IS NULL

    UNION ALL

    SELECT *, ReSubmitedOn AS ApplicationDate
    FROM App_Online_Wages
    WHERE ReSubmitedOn IS NOT NULL
),
Wage_Filtered AS (
    SELECT *
    FROM Wage_Processed
    WHERE ApplicationDate >= '2025-04-01' AND ApplicationDate < '2026-04-01'
),
Wage_Aggregated AS (
    SELECT
        DATEPART(MONTH, ApplicationDate) AS MonthNum,
        SUM(CASE WHEN Status = 'Request Closed' THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE 
            WHEN Status = 'Request Closed' 
                 AND LEVEL_2_UPDATEDON IS NOT NULL 
                 AND DATEDIFF(DAY, ApplicationDate, LEVEL_2_UPDATEDON) <= 3 
            THEN 1 ELSE 0 
        END) AS ApprovedUnderSLA
    FROM Wage_Filtered
    GROUP BY DATEPART(MONTH, ApplicationDate)
),
Wage_Pivoted AS (
    SELECT
        'Wage Compliance' AS Object,
        '3 days' AS SLG,
        '5 days' AS RevisedSLG,
        MAX(CASE WHEN MonthNum = 4 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS APR,
        MAX(CASE WHEN MonthNum = 5 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS MAY,
        MAX(CASE WHEN MonthNum = 6 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS JUN,
        MAX(CASE WHEN MonthNum = 7 THEN ApprovedUnderSLA * 100.0 / NULLIF(Approved, 0) END) AS JUL
)

-- Final SELECT combining both
SELECT * FROM Leave_Pivoted
UNION ALL
SELECT * FROM Wage_Pivoted;
