this is my two query when debugging it is get 

      SELECT Latitude, Longitude, Range FROM GEOFENCEDB.DBO.App_LocationMaster 
            WHERE work_site IN ('KMPM')

            SELECT Latitude, Longitude, Range FROM GEOFENCEDB.DBO.App_LocationMaster 
            WHERE work_site IN ('Corporate Service,KMPM')

in this when i put one worksite it shows the data but when i put two worksite that is in 2nd query is not working why?
