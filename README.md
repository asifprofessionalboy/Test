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
    ISNULL(MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END), 0) AS PunchOutTime
FROM dateseries ds
LEFT JOIN vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO = @PsrNo
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC;



with dateseries as
 (select dateadd (day,number, '2025-03-01' ) as punchdate from  master.dbo.spt_values where type ='p'
 and  dateadd(day,number,'2025-03-01') <= (select max (PDE_PUNCHDATE) from vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS ))
 select
      format(ds.punchdate,'dd-MM-yyyy') as PDE_PUNCHDATE,
         isnull(MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END),0) AS PunchInTime,
         isnull(MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END),0) AS PunchOutTime
     FROM dateseries ds left join  vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS t on ds.punchdate = t.PDE_PUNCHDATE and t.PDE_PSRNO =@PsrNo 
     GROUP BY ds.punchdate
     ORDER BY ds.punchdate Asc
