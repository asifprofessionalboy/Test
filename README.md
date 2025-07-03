this is my query 

DECLARE @FromDate DATE = '2025-07-01';
DECLARE @ToDate DATE = '2025-07-02';

WITH PunchData AS (
    SELECT
        PDE_PSRNO AS PNO,
        CAST(PDE_PUNCHTIME AS TIME) AS PunchTime,
        CAST(PDE_PUNCHTIME AS DATE) AS PunchDate
    FROM T_TRPUNCHDATA_EARS
    WHERE CAST(PDE_PUNCHTIME AS DATE) BETWEEN @FromDate AND @ToDate
),
FirstLastPunch AS (
    SELECT
        PNO,
        PunchDate,
        MIN(PunchTime) AS FirstPunch,
        MAX(PunchTime) AS LastPunch
    FROM PunchData
    GROUP BY PNO, PunchDate
),
FilteredEmp AS (
    SELECT *
    FROM App_Empl_Master
    WHERE InTime IS NOT NULL AND OutTime IS NOT NULL
          AND InTime >= 0 AND InTime < 24
          AND OutTime >= 0 AND OutTime <= 24
),
ConvertedTimes AS (
    SELECT
        emp.PNO,
        f.PunchDate,
        f.FirstPunch,
        f.LastPunch,

        -- Safe conversion of decimal to TIME using TIMEFROMPARTS
        TIMEFROMPARTS(
            FLOOR(emp.InTime),
            CAST(ROUND((emp.InTime - FLOOR(emp.InTime)) * 60, 0) AS INT),
            0, 0, 0
        ) AS InTimeConverted,

        TIMEFROMPARTS(
            FLOOR(emp.OutTime),
            CAST(ROUND((emp.OutTime - FLOOR(emp.OutTime)) * 60, 0) AS INT),
            0, 0, 0
        ) AS OutTimeConverted

    FROM FilteredEmp emp
    JOIN FirstLastPunch f ON emp.PNO = f.PNO
),
FinalResult AS (
    SELECT *,
        CASE 
            WHEN FirstPunch > DATEADD(MINUTE, 5, InTimeConverted) THEN 'Late'
            ELSE 'On Time'
        END AS ArrivalStatus,
        CASE 
            WHEN LastPunch < DATEADD(MINUTE, -5, OutTimeConverted) THEN 'Left Early'
            ELSE 'On Time'
        END AS DepartureStatus
    FROM ConvertedTimes
)
SELECT *
FROM FinalResult
WHERE ArrivalStatus = 'Late' OR DepartureStatus = 'Left Early'
ORDER BY PNO, PunchDate;

and again getting this error 
Conversion failed when converting date and/or time from character string.
