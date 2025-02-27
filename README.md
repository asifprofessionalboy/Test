
public IActionResult GetLocations()
{
    var UserId = HttpContext.Request.Cookies["Session"];
    string connectionString = GetConnectionString();

    string query = @"SELECT ps.Worksite FROM GEOFENCEDB.DBO.App_Position_Worksite AS ps 
                     INNER JOIN GEOFENCEDB.DBO.App_Emp_position AS es ON es.position = ps.position 
                     WHERE es.Pno = @UserId";

    using (var connection = new SqlConnection(connectionString))
    {
        // Get worksite names as a single string (e.g., "Corporate Service,KMPM")
        var worksiteNamesString = connection.QuerySingleOrDefault<string>(query, new { UserId });

        if (string.IsNullOrEmpty(worksiteNamesString)) 
        {
            ViewBag.PolyData = new List<object>();
            return View();
        }

        // 🔹 Split into a list (["Corporate Service", "KMPM"])
        var worksiteNames = worksiteNamesString.Split(',').Select(w => w.Trim()).ToList();

        // 🔹 Format correctly for SQL IN clause: ('Corporate Service', 'KMPM')
        var formattedWorksites = worksiteNames
            .Select(name => $"'{name.Replace("'", "''")}'") // Prevent SQL injection issues
            .ToList();

        string s = string.Join(",", formattedWorksites);

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



worksiteNames :Corporate Service,KMPM


formattedWorksites: 'Corporate Service,KMPM'

s : 'Corporate Service,KMPM'

i want s in format like this , 'Corporate Service','KMPM'
