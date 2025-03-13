WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p'
        AND DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
            <= (SELECT MAX(PDE_PUNCHDATE) FROM vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS)
)
SELECT
    FORMAT(ds.punchdate, 'dd-MM-yyyy') AS PDE_PUNCHDATE,
    ISNULL(MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END), 0) AS PunchInTime,
    ISNULL(MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END), 0) AS PunchOutTime,
    COUNT(CASE WHEN PDE_INOUT LIKE '%I%' THEN 1 END) +
    COUNT(CASE WHEN PDE_INOUT LIKE '%O%' THEN 1 END) AS SumofPunching,
    -- Condition to display shift based on ShiftDuty
    CASE 
        WHEN em.ShiftDuty = 0 THEN em.Shift
        WHEN em.ShiftDuty = 1 THEN 'Shift'
        ELSE NULL
    END AS ShiftInfo
FROM dateseries ds 
LEFT JOIN vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO = @PSRNo
LEFT JOIN App_Employee_Master em 
    ON em.PNO = @PSRNo  -- Ensure that PNO is the column that matches @PSRNo
GROUP BY ds.punchdate, em.ShiftDuty, em.Shift
ORDER BY ds.punchdate ASC;



this is my query 
WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p'
        AND DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
            <= (SELECT MAX(PDE_PUNCHDATE) FROM vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS)
)
SELECT
    FORMAT(ds.punchdate, 'dd-MM-yyyy') AS PDE_PUNCHDATE,
    ISNULL(MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END), 0) AS PunchInTime,
    ISNULL(MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END), 0) AS PunchOutTime,
    COUNT(CASE WHEN PDE_INOUT LIKE '%I%' THEN 1 END)+
    COUNT(CASE WHEN PDE_INOUT LIKE '%O%' THEN 1 END) as SumofPunching
FROM dateseries ds 
LEFT JOIN vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO = @PSRNo
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC

and this is my one table from this i want to fetch ShiftDuty against the pno , there is a column pno and Passing @PSRNo. in this i want a condition that there is two thing shiftduty contains 0 and 1 if the shiftDuty is 0 then shows the Actual column value of Shift if there is 1 then Shows a String Shift
select * from App_Employee_Master
