-- Define your date range
DECLARE @FromDate DATE = '2025-07-01';
DECLARE @ToDate DATE = '2025-07-03';

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





-- Define your date range here
DECLARE @FromDate DATE = '2025-07-01';
DECLARE @ToDate DATE = '2025-07-03';

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
ConvertedTimes AS (
    SELECT
        emp.PNO,
        f.PunchDate,
        f.FirstPunch,
        f.LastPunch,

        -- Convert InTime decimal to TIME
        TIMEFROMPARTS(
            FLOOR(emp.InTime),
            CAST(ROUND((emp.InTime - FLOOR(emp.InTime)) * 60, 0) AS INT),
            0, 0, 0
        ) AS InTimeConverted,

        -- Convert OutTime decimal to TIME
        TIMEFROMPARTS(
            FLOOR(emp.OutTime),
            CAST(ROUND((emp.OutTime - FLOOR(emp.OutTime)) * 60, 0) AS INT),
            0, 0, 0
        ) AS OutTimeConverted

    FROM App_Empl_Master emp
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





WITH PunchData AS (
    SELECT
        PDE_PSRNO AS PNO,
        CAST(PDE_PUNCHTIME AS TIME) AS PunchTime,
        CAST(PDE_PUNCHTIME AS DATE) AS PunchDate
    FROM T_TRPUNCHDATA_EARS
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
ConvertedTimes AS (
    SELECT
        emp.PNO,
        f.PunchDate,
        f.FirstPunch,
        f.LastPunch,

        -- Convert InTime decimal (e.g. 9.5) to TIME
        TIMEFROMPARTS(
            FLOOR(emp.InTime),                              -- Hours
            CAST(ROUND((emp.InTime - FLOOR(emp.InTime)) * 60, 0) AS INT), -- Minutes
            0, 0, 0
        ) AS InTimeConverted,

        -- Convert OutTime decimal to TIME
        TIMEFROMPARTS(
            FLOOR(emp.OutTime), 
            CAST(ROUND((emp.OutTime - FLOOR(emp.OutTime)) * 60, 0) AS INT),
            0, 0, 0
        ) AS OutTimeConverted

    FROM App_Empl_Master emp
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





WITH PunchData AS (
    SELECT
        PDE_PSRNO AS PNO,
        CAST(PDE_PUNCHTIME AS TIME) AS PunchTime,
        CAST(PDE_PUNCHTIME AS DATE) AS PunchDate
    FROM T_TRPUNCHDATA_EARS
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
ConvertedTimes AS (
    SELECT
        emp.PNO,
        f.PunchDate,
        f.FirstPunch,
        f.LastPunch,
        
        -- Convert Intime decimal to TIME
        CAST(CAST(FLOOR(emp.Intime) AS VARCHAR) + ':' + 
             RIGHT('00' + CAST(CAST((emp.Intime - FLOOR(emp.Intime)) * 60 AS INT) AS VARCHAR), 2) 
        AS TIME) AS InTimeConverted,
        
        -- Convert OutTime decimal to TIME
        CAST(CAST(FLOOR(emp.OutTime) AS VARCHAR) + ':' + 
             RIGHT('00' + CAST(CAST((emp.OutTime - FLOOR(emp.OutTime)) * 60 AS INT) AS VARCHAR), 2) 
        AS TIME) AS OutTimeConverted
    FROM App_Empl_Master emp
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




getting this error 
The data types datetime and time are incompatible in the less than operator.

WITH PunchData AS (
    SELECT
        PDE_PSRNO AS PNO,
        CAST(PDE_PUNCHTIME AS TIME) AS PunchTime,
        CAST(PDE_PUNCHTIME AS DATE) AS PunchDate
    FROM T_TRPUNCHDATA_EARS
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
FinalResult AS (
    SELECT
        emp.PNO,
        emp.Intime,
        emp.OutTime,
        f.PunchDate,
        f.FirstPunch,
        f.LastPunch,
        CASE 
            WHEN DATEADD(MINUTE, 5, emp.Intime) < f.FirstPunch THEN 'Late' 
            ELSE 'On Time' 
        END AS ArrivalStatus,
        CASE 
            WHEN DATEADD(MINUTE, -5, emp.OutTime) > f.LastPunch THEN 'Left Early' 
            ELSE 'On Time' 
        END AS DepartureStatus
    FROM App_Empl_Master emp
    JOIN FirstLastPunch f ON emp.PNO = f.PNO
)
SELECT *
FROM FinalResult
WHERE ArrivalStatus = 'Late' OR DepartureStatus = 'Left Early'
ORDER BY PNO, PunchDate;




i have these 2 tables first one is App_Empl_Master where Every Pno has InTime and OutTime like this 
Intime    OutTime
9.00      18.50

and another table is this T_TRPUNCHDATA_EARS. in this table PunchIn and PunchOut Time is Stored like this 
PDE_PUNCHTIME and Pno column is PDE_PSRNO
06:53

i want to find out that if a user has InTime is 9.00 and he comes at 9:05 and also OutTime is 18:50 and he is gone at 18:45 i want to find that
