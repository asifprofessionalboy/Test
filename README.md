WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p'
        AND DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
            <= EOMONTH(GETDATE())  -- Only current month
)
SELECT
    FORMAT(ds.punchdate, 'dd-MM-yyyy') AS PDE_PUNCHDATE,
    ISNULL(MIN(CONVERT(TIME, t.PDE_PUNCHTIME)), '00:00:00') AS FirstIn,
    CASE 
        WHEN COUNT(t.PDE_PUNCHTIME) > 1 
        THEN ISNULL(MAX(CONVERT(TIME, t.PDE_PUNCHTIME)), NULL)
        ELSE NULL
    END AS LastOut,
    COUNT(t.PDE_PUNCHTIME) AS SumofPunching
FROM dateseries ds 
LEFT JOIN TSUISLRFIDDB.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO = @PsrNo
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC





WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p'
        AND DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
            <= EOMONTH(GETDATE())  -- Only current month
)
SELECT
    FORMAT(ds.punchdate, 'dd-MM-yyyy') AS PDE_PUNCHDATE,
    ISNULL(MIN(CONVERT(TIME, t.PDE_PUNCHTIME)), '00:00:00') AS FirstIn,
    ISNULL(MAX(CONVERT(TIME, t.PDE_PUNCHTIME)), '00:00:00') AS LastOut,
    COUNT(t.PDE_PUNCHTIME) AS SumofPunching
FROM dateseries ds 
LEFT JOIN TSUISLRFIDDB.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO = @PsrNo
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC

data of current date

21-04-2025	09:23:00.0000000	09:23:00.0000000

using this query i am getting a problem that user is punchIn then why it is showing in punchOut column?

	1
