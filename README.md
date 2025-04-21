this is my query for Rdlc Attendance Report 
 WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p'
        AND DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
            <= (SELECT MAX(PDE_PUNCHDATE) FROM TSUISLRFIDDB.dbo.T_TRPUNCHDATA_EARS)
)
SELECT
    FORMAT(ds.punchdate, 'dd-MM-yyyy') AS PDE_PUNCHDATE,
    ISNULL(MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END), 0) AS PunchInTime,
    ISNULL(MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END), 0) AS PunchOutTime,
    COUNT(CASE WHEN PDE_INOUT LIKE '%I%' THEN 1 END)+
    COUNT(CASE WHEN PDE_INOUT LIKE '%O%' THEN 1 END) as SumofPunching
FROM dateseries ds 
LEFT JOIN TSUISLRFIDDB.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO = @PsrNo
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC


in this some issue i want to resolve that , i want to show current month records but it is showing All months of year, let this month is April then i want only April if the May start then may show and in this i want check the first value of day as PunchIn and the last value shows PunchOut 
