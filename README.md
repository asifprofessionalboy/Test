 -- 1.Leave
  
  WITH Processed AS (
    SELECT 
        *, 
        CreatedOn AS ApplicationDate
    FROM App_Leave_Comp_Summary
    WHERE ReSubmiteddate IS NULL

    UNION ALL

    SELECT 
        *, 
        ReSubmiteddate AS ApplicationDate
    FROM App_Leave_Comp_Summary
    WHERE ReSubmiteddate IS NOT NULL
),
Filtered AS (
    SELECT *
    FROM Processed
    WHERE ApplicationDate >= '2025-04-01' AND ApplicationDate < '2026-04-01'
),
Aggregated AS (
    SELECT
        FORMAT(ApplicationDate, 'yyyy-MM') AS MonthYear,
        DATENAME(MONTH, ApplicationDate) AS MonthName,
        LEFT(DATENAME(MONTH, ApplicationDate), 3) AS MonthShort,
        DATEPART(MONTH, ApplicationDate) AS MonthNum,
        SUM(CASE WHEN Status = 'Request Closed' THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE 
            WHEN Status = 'Request Closed' 
              AND CC_CreatedOn_L2 IS NOT NULL 
              AND DATEDIFF(DAY, ApplicationDate, CC_CreatedOn_L2) <= 5 
            THEN 1 ELSE 0 
        END) AS ApprovedUnderSLA
    FROM Filtered
    GROUP BY FORMAT(ApplicationDate, 'yyyy-MM'), DATEPART(MONTH, ApplicationDate), DATENAME(MONTH, ApplicationDate)
),
Pivoted AS (
    SELECT
        'Leave Compliance' AS Object,
        '5 days' AS SLG,
        '5 days' AS RevisedSLG,

     
       ISNULL( MAX(CASE WHEN MonthNum = 4 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS APR,
        ISNULL (MAX(CASE WHEN MonthNum = 4 THEN ApprovedUnderSLA ELSE NULL END),0) AS APR_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 5 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS MAY,
        ISNULL ( MAX(CASE WHEN MonthNum = 5 THEN ApprovedUnderSLA ELSE NULL END),0) AS MAY_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 6 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS JUN,
        ISNULL ( MAX(CASE WHEN MonthNum = 6 THEN ApprovedUnderSLA ELSE NULL END),0) AS JUN_Value,

        ISNULL ( MAX(CASE WHEN MonthNum = 7 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS JUL,
        ISNULL ( MAX(CASE WHEN MonthNum = 7 THEN ApprovedUnderSLA ELSE NULL END),0) AS JUL_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 8 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS AUG,
         ISNULL (MAX(CASE WHEN MonthNum = 8 THEN ApprovedUnderSLA ELSE NULL END),0) AS AUG_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 9 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS SEP,
         ISNULL (MAX(CASE WHEN MonthNum = 9 THEN ApprovedUnderSLA ELSE NULL END),0) AS SEP_Value,

        ISNULL ( MAX(CASE WHEN MonthNum = 10 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS OCT,
         ISNULL (MAX(CASE WHEN MonthNum = 10 THEN ApprovedUnderSLA ELSE NULL END),0) AS OCT_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 11 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS NOV,
         ISNULL (MAX(CASE WHEN MonthNum = 11 THEN ApprovedUnderSLA ELSE NULL END),0) AS NOV_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 12 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS DEC,
         ISNULL (MAX(CASE WHEN MonthNum = 12 THEN ApprovedUnderSLA ELSE NULL END),0) AS DEC_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 1 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS JAN,
         ISNULL (MAX(CASE WHEN MonthNum = 1 THEN ApprovedUnderSLA ELSE NULL END),0) AS JAN_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 2 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS FEB,
         ISNULL (MAX(CASE WHEN MonthNum = 2 THEN ApprovedUnderSLA ELSE NULL END),0) AS FEB_Value,

        ISNULL ( MAX(CASE WHEN MonthNum = 3 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS MAR,
         ISNULL (MAX(CASE WHEN MonthNum = 3 THEN ApprovedUnderSLA ELSE '' END),0) AS MAR_Value

    FROM Aggregated
)
SELECT * FROM Pivoted


Union ALL
 
 SELECT * FROM Pivoted;




  -- 2. Wage

   WITH Processed AS (
    SELECT 
        *, 
        CREATEDON AS ApplicationDate
    FROM App_Online_Wages
    WHERE ReSubmitedOn IS NULL

    UNION ALL

    SELECT 
        *, 
        ReSubmitedOn AS ApplicationDate
    FROM App_Online_Wages
    WHERE ReSubmitedOn IS NOT NULL
),
Filtered AS (
    SELECT *
    FROM Processed
    WHERE ApplicationDate >= '2025-04-01' AND ApplicationDate < '2026-04-01'
),
Aggregated AS (
    SELECT
        FORMAT(ApplicationDate, 'yyyy-MM') AS MonthYear,
        DATENAME(MONTH, ApplicationDate) AS MonthName,
        LEFT(DATENAME(MONTH, ApplicationDate), 3) AS MonthShort,
        DATEPART(MONTH, ApplicationDate) AS MonthNum,
        SUM(CASE WHEN Status = 'Request Closed' THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE 
            WHEN Status = 'Request Closed' 
              AND LEVEL_2_UPDATEDON IS NOT NULL 
              AND DATEDIFF(DAY, ApplicationDate, LEVEL_2_UPDATEDON) <= 3 
            THEN 1 ELSE 0 
        END) AS ApprovedUnderSLA
    FROM Filtered
    GROUP BY FORMAT(ApplicationDate, 'yyyy-MM'), DATEPART(MONTH, ApplicationDate), DATENAME(MONTH, ApplicationDate)
),
Pivoted AS (
    SELECT
        'Wage Compliance' AS Object,
        '3 days' AS SLG,
        '5 days' AS RevisedSLG,

      ISNULL( MAX(CASE WHEN MonthNum = 4 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS APR,
        ISNULL (MAX(CASE WHEN MonthNum = 4 THEN ApprovedUnderSLA ELSE NULL END),0) AS APR_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 5 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS MAY,
        ISNULL ( MAX(CASE WHEN MonthNum = 5 THEN ApprovedUnderSLA ELSE NULL END),0) AS MAY_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 6 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS JUN,
        ISNULL ( MAX(CASE WHEN MonthNum = 6 THEN ApprovedUnderSLA ELSE NULL END),0) AS JUN_Value,

        ISNULL ( MAX(CASE WHEN MonthNum = 7 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS JUL,
        ISNULL ( MAX(CASE WHEN MonthNum = 7 THEN ApprovedUnderSLA ELSE NULL END),0) AS JUL_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 8 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS AUG,
         ISNULL (MAX(CASE WHEN MonthNum = 8 THEN ApprovedUnderSLA ELSE NULL END),0) AS AUG_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 9 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS SEP,
         ISNULL (MAX(CASE WHEN MonthNum = 9 THEN ApprovedUnderSLA ELSE NULL END),0) AS SEP_Value,

        ISNULL ( MAX(CASE WHEN MonthNum = 10 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS OCT,
         ISNULL (MAX(CASE WHEN MonthNum = 10 THEN ApprovedUnderSLA ELSE NULL END),0) AS OCT_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 11 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS NOV,
         ISNULL (MAX(CASE WHEN MonthNum = 11 THEN ApprovedUnderSLA ELSE NULL END),0) AS NOV_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 12 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS DEC,
         ISNULL (MAX(CASE WHEN MonthNum = 12 THEN ApprovedUnderSLA ELSE NULL END),0) AS DEC_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 1 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS JAN,
         ISNULL (MAX(CASE WHEN MonthNum = 1 THEN ApprovedUnderSLA ELSE NULL END),0) AS JAN_Value,

         ISNULL (MAX(CASE WHEN MonthNum = 2 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS FEB,
         ISNULL (MAX(CASE WHEN MonthNum = 2 THEN ApprovedUnderSLA ELSE NULL END),0) AS FEB_Value,

        ISNULL ( MAX(CASE WHEN MonthNum = 3 THEN CAST(100.0 * ApprovedUnderSLA / NULLIF(Approved, 0) AS DECIMAL(5,2)) ELSE NULL END),0) AS MAR,
         ISNULL (MAX(CASE WHEN MonthNum = 3 THEN ApprovedUnderSLA ELSE '' END),0) AS MAR_Value

    FROM Aggregated
)



Msg 102, Level 15, State 1, Line 175
Incorrect syntax near ')'.



