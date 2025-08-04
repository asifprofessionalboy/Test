WITH LeaveProcessed AS (
   -- Your existing Leave logic...
),
LeaveFiltered AS (
   -- ...
),
LeaveAggregated AS (
   -- ...
),
LeavePivoted AS (
   -- ...
)

-- Now second chain
, WageProcessed AS (
   -- Your existing Wage logic...
),
WageFiltered AS (
   -- ...
),
WageAggregated AS (
   -- ...
),
WagePivoted AS (
   -- ...
)

-- Final SELECTs unioned:
SELECT * FROM LeavePivoted
UNION ALL
SELECT * FROM WagePivoted;
