SELECT SiteID, *
FROM App_AttendanceDetails
WHERE TRY_CAST(SiteID AS uniqueidentifier) IS NULL
  AND SiteID IS NOT NULL
