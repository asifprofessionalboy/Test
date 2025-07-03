DECLARE @FromDate DATE = '2025-06-01';
DECLARE @ToDate DATE = '2025-06-20';

WITH PunchData AS (
    SELECT
        PDE_PSRNO AS PNO,
        TRY_CAST(PDE_PUNCHTIME AS TIME) AS PunchTime,
        PDE_PUNCHDATE AS PunchDate
    FROM T_TRPUNCHDATA_EARS
    WHERE PDE_PUNCHDATE BETWEEN @FromDate AND @ToDate
      AND ISDATE(PDE_PUNCHTIME) = 1 -- Optional: extra filtering for bad time values
),
ValidPunchData AS (
    SELECT *
    FROM PunchData
    WHERE PunchTime IS NOT NULL -- ensures we skip invalid time rows
),
FirstLastPunch AS (
    SELECT
        PNO,
        PunchDate,
        MIN(PunchTime) AS FirstPunch,
        MAX(PunchTime) AS LastPunch
    FROM ValidPunchData
    GROUP BY PNO, PunchDate
),
ConvertedTimes AS (
    SELECT
        emp.PNO,
        f.PunchDate,
        f.FirstPunch,
        f.LastPunch,

        TIMEFROMPARTS(
            FLOOR(emp.InTime),
            CAST(
                CASE 
                    WHEN ROUND((emp.InTime - FLOOR(emp.InTime)) * 60, 0) >= 60 THEN 59
                    ELSE ROUND((emp.InTime - FLOOR(emp.InTime)) * 60, 0)
                END AS INT
            ),
            0, 0, 0
        ) AS InTimeConverted,

        TIMEFROMPARTS(
            FLOOR(emp.OutTime),
            CAST(
                CASE 
                    WHEN ROUND((emp.OutTime - FLOOR(emp.OutTime)) * 60, 0) >= 60 THEN 59
                    ELSE ROUND((emp.OutTime - FLOOR(emp.OutTime)) * 60, 0)
                END AS INT
            ),
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




when putting fromDate and TODate i am getting same error 
Conversion failed when converting date and/or time from character string.


i have this 2 columns in T_TRPUNCHDATA_EARS 

 [PDE_PUNCHDATE] DATE         NOT NULL,
 [PDE_PUNCHTIME] VARCHAR (8)  NOT NULL,

this is query

DECLARE @FromDate DATE = '2025-06-01';
DECLARE @ToDate DATE = '2025-06-20';

WITH PunchData AS (
    SELECT
        PDE_PSRNO AS PNO,
        CAST(PDE_PUNCHTIME AS TIME) AS PunchTime,
        CAST(PDE_PUNCHTIME AS DATE) AS PunchDate
    FROM T_TRPUNCHDATA_EARS
    WHERE PDE_PUNCHDATE BETWEEN @FromDate AND @ToDate
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
            FLOOR(emp.InTime),
            CAST(
                CASE 
                    WHEN ROUND((emp.InTime - FLOOR(emp.InTime)) * 60, 0) >= 60 THEN 59
                    ELSE ROUND((emp.InTime - FLOOR(emp.InTime)) * 60, 0)
                END AS INT
            ),
            0, 0, 0
        ) AS InTimeConverted,

        -- Convert OutTime decimal to TIME
        TIMEFROMPARTS(
            FLOOR(emp.OutTime),
            CAST(
                CASE 
                    WHEN ROUND((emp.OutTime - FLOOR(emp.OutTime)) * 60, 0) >= 60 THEN 59
                    ELSE ROUND((emp.OutTime - FLOOR(emp.OutTime)) * 60, 0)
                END AS INT
            ),
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
