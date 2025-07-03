now this is working fine, i want to make changes in this query to fetch that there is only for InPunch means firstIn value i want , who is coming 5 minutes before and 5 min late of their InTime And OutTime and there is column Shift if the user has U,V,W,x,y and z in their shift then skip them because they has flexy time. and make my query simple like normal user writes it 
DECLARE @FromDate DATE = '2025-06-01';
DECLARE @ToDate DATE = '2025-06-30';

WITH PunchData AS (
    SELECT
        PDE_PSRNO AS PNO,
        TRY_CAST(PDE_PUNCHTIME AS TIME) AS PunchTime,
        PDE_PUNCHDATE AS PunchDate
    FROM T_TRPUNCHDATA_EARS
    WHERE PDE_PUNCHDATE BETWEEN @FromDate AND @ToDate
      AND ISDATE(PDE_PUNCHTIME) = 1 
),
ValidPunchData AS (
    SELECT *
    FROM PunchData
    WHERE PunchTime IS NOT NULL
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
