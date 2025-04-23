WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p'
        AND DATEADD(DAY, number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
            <= EOMONTH(GETDATE())
),
FilteredPunches AS (
    SELECT 
        t.PDE_PSRNO,
        t.PDE_PUNCHDATE,
        CONVERT(TIME, t.PDE_PUNCHTIME) AS PunchTime,
        ROW_NUMBER() OVER (PARTITION BY t.PDE_PSRNO, t.PDE_PUNCHDATE ORDER BY t.PDE_PUNCHTIME) AS rn
    FROM TSUISLRFIDDB.dbo.T_TRPUNCHDATA_EARS t
),
ValidPunches AS (
    SELECT 
        fp.*,
        MIN(PunchTime) OVER (PARTITION BY PDE_PSRNO, PDE_PUNCHDATE) AS FirstPunch,
        DATEDIFF(MINUTE, 
            MIN(PunchTime) OVER (PARTITION BY PDE_PSRNO, PDE_PUNCHDATE),
            PunchTime) AS MinDiff
    FROM FilteredPunches fp
),
FilteredValidPunches AS (
    SELECT * FROM ValidPunches
    WHERE MinDiff >= 5 OR rn = 1
)
SELECT
    FORMAT(ds.punchdate, 'dd-MM-yyyy') AS PDE_PUNCHDATE,
    ISNULL(MIN(PunchTime), '00:00:00') AS PunchInTime,
    ISNULL(
        CASE 
            WHEN COUNT(fvp.PunchTime) > 1 THEN MAX(fvp.PunchTime)
            ELSE NULL
        END, 
        '00:00:00'
    ) AS PunchOutTime,
    COUNT(fvp.PunchTime) AS SumOfPunching
FROM dateseries ds
LEFT JOIN FilteredValidPunches fvp 
    ON ds.punchdate = fvp.PDE_PUNCHDATE 
    AND fvp.PDE_PSRNO = @PsrNo
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC;

  
  
  
  
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
    ISNULL(MIN(CONVERT(TIME, t.PDE_PUNCHTIME)), '00:00:00') AS punchintime,
    ISNULL(
        CASE 
            WHEN COUNT(t.PDE_PUNCHTIME) > 1 
            THEN MAX(CONVERT(TIME, t.PDE_PUNCHTIME))
            ELSE NULL
        END, 
        '00:00:00'
    ) AS Punchouttime,
    COUNT(t.PDE_PUNCHTIME) AS SumofPunching
FROM dateseries ds 
LEFT JOIN TSUISLRFIDDB.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO =@PsrNo
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC
