WITH FirstLastPunch AS (
    SELECT 
        PNO,
        CONVERT(date, PUNCHDATE) AS PunchDate,
        MIN(CONVERT(datetime, PUNCHDATETIME)) AS FirstIn,
        MAX(CONVERT(datetime, PUNCHDATETIME)) AS LastOut
    FROM T_TRPUNCHDATA_EARS
    GROUP BY PNO, CONVERT(date, PUNCHDATE)
)

SELECT 
    e.PNO,
    e.Intime,
    e.OutTime,
    f.PunchDate,
    f.FirstIn,
    f.LastOut,

    -- Early or Late Arrival
    CASE 
        WHEN DATEADD(minute, -5, CAST(e.Intime AS datetime)) <= CAST(f.FirstIn AS datetime)
             AND CAST(f.FirstIn AS datetime) <= DATEADD(minute, 5, CAST(e.Intime AS datetime))
            THEN 'On Time'
        WHEN CAST(f.FirstIn AS datetime) < DATEADD(minute, -5, CAST(e.Intime AS datetime))
            THEN 'Early Arrival'
        ELSE 'Late Arrival'
    END AS ArrivalStatus,

    -- Early or Late Exit
    CASE 
        WHEN DATEADD(minute, -5, CAST(e.OutTime AS datetime)) <= CAST(f.LastOut AS datetime)
             AND CAST(f.LastOut AS datetime) <= DATEADD(minute, 5, CAST(e.OutTime AS datetime))
            THEN 'On Time'
        WHEN CAST(f.LastOut AS datetime) > DATEADD(minute, 5, CAST(e.OutTime AS datetime))
            THEN 'Late Exit'
        ELSE 'Early Exit'
    END AS ExitStatus

FROM App_Empl_Master e
JOIN FirstLastPunch f ON e.PNO = f.PNO




i have this table App_Empl_Master in this there is 2 columns Intime and OutTime of Every Pno and this is my 2nd table 
select * from T_TRPUNCHDATA_EARS
in this everyday punchData of the Users are there , i want a query to get that how many users are arrive before 5 minutes and late after 5 minutes , and same for punchOut , apply first in last out with these requirement
