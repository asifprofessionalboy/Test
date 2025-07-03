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
