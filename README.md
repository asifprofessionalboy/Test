WITH dateseries AS (
    SELECT 
        DATEADD(DAY, number, '2025-03-01') AS punchdate 
    FROM master.dbo.spt_values 
    WHERE type = 'p'
    AND DATEADD(DAY, number, '2025-03-01') <= (SELECT MAX(PDE_PUNCHDATE) FROM vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS)
)
SELECT
    FORMAT(ds.punchdate, 'yyyy-MM-dd') AS PDE_PUNCHDATE,
    ISNULL(MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END), 0) AS PunchInTime,
    ISNULL(MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END), 0) AS PunchOutTime,
    COUNT(CASE WHEN PDE_INOUT LIKE '%I%' THEN 1 END) AS PunchInCount,
    COUNT(CASE WHEN PDE_INOUT LIKE '%O%' THEN 1 END) AS PunchOutCount
FROM dateseries ds 
LEFT JOIN vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS t 
    ON ds.punchdate = t.PDE_PUNCHDATE 
    AND t.PDE_PSRNO = '151514'
GROUP BY ds.punchdate
ORDER BY ds.punchdate ASC;




with dateseries as
(select dateadd (day,number, '2025-03-01' ) as punchdate from  master.dbo.spt_values where type ='p'
and  dateadd(day,number,'2025-03-01') <= (select max (PDE_PUNCHDATE) from vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS ))
select
     format(ds.punchdate,'yyyy-MM-dd') as PDE_PUNCHDATE,
        isnull(MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END),0) AS PunchInTime,
        isnull(MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END),0) AS PunchOutTime
    FROM dateseries ds left join  vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS t on ds.punchdate = t.PDE_PUNCHDATE and t.PDE_PSRNO ='151514' 
    GROUP BY ds.punchdate
    ORDER BY ds.punchdate asc

in this i want to add a column for No. of PunchIn and PunchOut for everyday
