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

        string formattedWorksites = string.Join("','", worksiteNames);
        string query2 = @$"SELECT Latitude, Longitude, Radius FROM GEOFENCEDB.DBO.App_LocationMaster 
                           WHERE work_site IN ('{formattedWorksites}')";

        var locations = connection.Query(query2).Select(loc => new
        {
            Latitude = (double)loc.Latitude,
            Longitude = (double)loc.Longitude,
            Radius = (double)loc.Radius // Ensure the database has a radius column in meters
        }).ToList();

        ViewBag.PolyData = locations;
        return View();
    }
}


<script>
    function OnOff() {
        var punchIn = document.getElementById('PunchIn');
        var punchOut = document.getElementById('PunchOut');

        // Initially disable buttons
        punchIn.disabled = true;
        punchOut.disabled = true;
        punchIn.classList.add("disabled");
        punchOut.classList.add("disabled");

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    const lat = roundTo(position.coords.latitude, 6);
                    const long = roundTo(position.coords.longitude, 6);

                    const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));

                    let isInsideRadius = locations.some(location => {
                        return isWithinRadius(lat, long, location.Latitude, location.Longitude, location.Radius);
                    });

                    if (isInsideRadius) {
                        punchIn.disabled = false;
                        punchOut.disabled = false;
                        punchIn.classList.remove("disabled");
                        punchOut.classList.remove("disabled");
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "You are not within the allowed location range for attendance!"
                        });
                    }
                },
                function (error) {
                    alert('Error fetching location: ' + error.message);
                }
            );
        } else {
            alert("Geolocation is not supported by this browser");
        }
    }

    // Haversine formula to calculate distance between two points
    function isWithinRadius(userLat, userLon, locLat, locLon, radius) {
        const R = 6371000; // Earth's radius in meters
        const toRad = angle => (angle * Math.PI) / 180;

        let dLat = toRad(locLat - userLat);
        let dLon = toRad(locLon - userLon);

        let a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(toRad(userLat)) * Math.cos(toRad(locLat)) *
                Math.sin(dLon / 2) * Math.sin(dLon / 2);

        let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        let distance = R * c; // Distance in meters

        return distance <= radius; // Returns true if within radius
    }

    function roundTo(num, places) {
        return +(Math.round(num + "e" + places) + "e-" + places);
    }

    window.onload = OnOff;
</script>



this is my polygon js
<script>
    function OnOff() {

        var punchIn = document.getElementById('PunchIn');
        var punchOut = document.getElementById('PunchOut');

        punchIn.disabled = true;//to be seen
        punchOut.disabled = true;

        punchIn.classList.add("disabled");
        punchOut.classList.add("disabled");


        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    const roundTo = (num, places) => +(Math.round(num + "e" + places) + "e-" + places);
                    const lat = roundTo(position.coords.latitude,6);
                    const long = roundTo(position.coords.longitude,6);

                    

                    const loc = @Html.Raw(Json.Serialize(ViewBag.PolyData));

                        // loc.forEach(loc => {
                        //     console.log(loc);
                        // });

                    let isInsidePolygon = false;
                    loc.forEach(polygon => {
                        if (IspointInPolygon({ latitude: lat, longitude: long }, polygon)) {
                            isInsidePolygon = true;
                            
                        }
                    });
                    if (isInsidePolygon) {
                        punchIn.disabled = false;//to be seen
                        punchOut.disabled = false;

                        punchIn.classList.remove("disabled");
                        punchOut.classList.remove("disabled");
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "You are not in range of location of your attendance!"
                        });

                        punchIn.disabled = true;//to be disabled
                        punchOut.disabled = true;

                        punchIn.classList.add("disabled");
                        punchOut.classList.add("disabled");
                    }
                    
                },
                function (error) {
                    alert('Error fetching location' + error.message);
                }
            );
        }
        else{
            alert("Geo location is not supported by this browser");
        }
    }
    function IspointInPolygon(point, polygon) {
       
        let x = point.latitude, y = point.longitude;
        let inside = false;
        console.log(polygon);
        for (let i = 0, j = polygon.length - 1; i < polygon.length; j = i++) {
          
            let xi = polygon[i].latitude;
           
            let yi = polygon[i].Longitude;
           
            let xj = polygon[j].latitude;
           
            let yj = polygon[j].Longitude;
            
            let intersect = ((yi > y) !== (yj > y)) && (x < ((xj - xi) * (y - yi)) / (yj - yi) + xi);
 
            if (intersect) {
                inside = !inside;
            }
        }
       
        return inside;
    }
        
      
        
    window.onload = OnOff;

</script>

this is my method 

   public IActionResult GetLocations()
   {

       var UserId = HttpContext.Request.Cookies["Session"];
       string connectionString = GetConnectionString();
       string query = @"select ps.Worksite from GEOFENCEDB.DBO.App_Position_Worksite as ps 
                           inner join GEOFENCEDB.DBO.App_Emp_position as es on es.position = ps.position 
                           where es.Pno = '" + UserId + "'";

       using (var connection = new SqlConnection(connectionString))
       {
           string locations = connection.QuerySingleOrDefault<string>(query);

           string s = locations;
           s = "'" + s;

           s = s.Replace(",", "','");
           s = s + "'";

           string query2 = @"select Longitude,Latitude from GEOFENCEDB.DBO.App_LocationMaster 
                                 where work_site in (" + s + ")";


           var locations2 = connection.Query<Location>(query2).ToList();

           var polygons = new List<List<Dictionary<string, double>>>();

           var polygon = new List<Dictionary<string, double>>();

           foreach(var location in locations2)
           {
               polygon.Add(new Dictionary<string, double>
               {
                   {"latitude",(double)location.Latitude },
                   {"Longitude",(double)location.Longitude },
               });
           }
           polygons.Add(polygon);
           ViewBag.PolyData = polygons;

           return View();
       }


   }


i want to change logic of js to circle with harvesine not polygon ,
please make this issue free and best logic to implement location 
