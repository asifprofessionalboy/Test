public IActionResult GetLocations()
{
    var UserId = HttpContext.Request.Cookies["Session"];
    string connectionString = GetConnectionString();

    string query = @"SELECT ps.Worksite FROM GEOFENCEDB.DBO.App_Position_Worksite AS ps 
                     INNER JOIN GEOFENCEDB.DBO.App_Emp_position AS es ON es.position = ps.position 
                     WHERE es.Pno = @UserId";

    using (var connection = new SqlConnection(connectionString))
    {
        var worksiteNames = connection.Query<string>(query, new { UserId }).ToList();

        if (!worksiteNames.Any()) 
        {
            ViewBag.PolyData = new List<object>();
            return View();
        }

        // Format worksite names into a valid SQL IN clause
        string s = string.Join("','", worksiteNames);
        s = "'" + s + "'";

        string query2 = @$"SELECT Longitude, Latitude, Range FROM GEOFENCEDB.DBO.App_LocationMaster 
                           WHERE work_site IN ({s})";

        var locations = connection.Query(query2).Select(loc => new
        {
            Latitude = (double)loc.Latitude,
            Longitude = (double)loc.Longitude,
            Range = (double)loc.Range
        }).ToList();

        ViewBag.PolyData = locations;
        return View();
    }
}




public IActionResult GetLocations()
{
    var UserId = HttpContext.Request.Cookies["Session"];
    string connectionString = GetConnectionString();

    string query = @"SELECT ps.Worksite FROM GEOFENCEDB.DBO.App_Position_Worksite AS ps 
                     INNER JOIN GEOFENCEDB.DBO.App_Emp_position AS es ON es.position = ps.position 
                     WHERE es.Pno = @UserId";

    using (var connection = new SqlConnection(connectionString))
    {
        var worksiteNames = connection.Query<string>(query, new { UserId }).ToList();

        if (!worksiteNames.Any()) 
        {
            ViewBag.PolyData = new List<object>();
            return View();
        }

        // Use parameterized query for multiple values
        string query2 = @"SELECT Latitude, Longitude, Range FROM GEOFENCEDB.DBO.App_LocationMaster 
                          WHERE work_site IN @Worksites";

        var locations = connection.Query(query2, new { Worksites = worksiteNames }).Select(loc => new
        {
            Latitude = (double)loc.Latitude,
            Longitude = (double)loc.Longitude,
            Range = (double)loc.Range
        }).ToList();

        ViewBag.PolyData = locations;
        return View();
    }
}




this is my two query when debugging it is get 

      SELECT Latitude, Longitude, Range FROM GEOFENCEDB.DBO.App_LocationMaster 
            WHERE work_site IN ('KMPM')

            SELECT Latitude, Longitude, Range FROM GEOFENCEDB.DBO.App_LocationMaster 
            WHERE work_site IN ('Corporate Service,KMPM')

in this when i put one worksite it shows the data but when i put two worksite that is in 2nd query is not working why?
