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
