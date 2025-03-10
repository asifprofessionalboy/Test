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
