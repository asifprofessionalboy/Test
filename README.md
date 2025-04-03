when applying this condition for buttons , function OnOff() is not working properly. any other process to do this, i want after function onoff then this logic happens
 [Authorize]
 public IActionResult GeoFencing()
 {
     var session = HttpContext.Request.Cookies["Session"];
     var userName = HttpContext.Request.Cookies["UserName"];

     var data = GetLocations();

     var pno = session; 
     var currentDate = DateTime.Now.ToString("yyyy-MM-dd");

     string connectionString = GetRFIDConnectionString();

     string query = @"
 SELECT TOP 1 TRBDGDA_BD_Inout 
 FROM T_TRBDGDAT_EARS 
 WHERE TRBDGDA_BD_PNO = @Pno 
 AND TRBDGDA_BD_DATE = @CurrentDate ORDER By TRBDGDA_BD_DATE DESC";

     string inoutValue = "";

     using (var connection = new SqlConnection(connectionString))
     {
         inoutValue = connection.QuerySingleOrDefault<string>(query, new { Pno = pno, CurrentDate = currentDate })?.Trim();
     }

     ViewBag.InOut = inoutValue; // Pass data to View

    
     return View();
 }

<div class="row mt-5 form-group">
    <div class="col d-flex justify-content-center mb-4">
        @if (ViewBag.InOut == "O" || string.IsNullOrEmpty(ViewBag.InOut)) 
        {
            <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                Punch In
            </button>
        }
    </div>

    <div class="col d-flex justify-content-center">
        @if (ViewBag.InOut == "I") 
        {
            <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                Punch Out
            </button>
        }
    </div>
</div>


   function OnOff() {
       var punchIn = document.getElementById('PunchIn');
       var punchOut = document.getElementById('PunchOut');

       punchIn.disabled = true;
       punchOut.disabled = true;
       punchIn.classList.add("disabled");
       punchOut.classList.add("disabled");

       Swal.fire({
           title: 'Please wait...',
           text: 'Fetching your current location.',
           allowOutsideClick: false,
           didOpen: () => {
               Swal.showLoading();
           }
       });

       if (navigator.geolocation) {
           navigator.geolocation.getCurrentPosition(
               function (position) {
                   Swal.close();

                  
                   const lat = roundTo(position.coords.latitude, 6);
                   const lon = roundTo(position.coords.longitude, 6);
                   // console.log("Current Lat:"+lat);
                   // console.log("Current Lon:" + lon);
                   // const lat = 22.79675;
                   // const lon = 86.183915;

                   const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                   console.log(locations);

                   let isInsideRadius = false;
                   let minDistance = Number.MAX_VALUE; // Store minimum distance

                   locations.forEach((location) => {
                       const allowedRange = parseFloat(location.range || location.Range);
                       const distance = calculateDistance(lat, lon, location.latitude || location.Latitude, location.longitude || location.Longitude);
                       console.log(`Distance to location (${location.latitude}, ${location.longitude}): ${Math.round(distance)} meters`);

                       if (distance <= allowedRange) {
                           isInsideRadius = true;
                       } else {
                           minDistance = Math.min(minDistance, distance);
                       }
                   });

                   if (isInsideRadius) {
                       punchIn.disabled = false;
                       punchOut.disabled = false;
                       punchIn.classList.remove("disabled");
                       punchOut.classList.remove("disabled");
                       // Swal.fire({
                       //     title: 'Within Range',
                       //     text: 'You are within the allowed range for attendance.',
                       //     icon: 'success'
                       // });
                   } else {
                       Swal.fire({
                           icon: "error",
                           title: "Out of Range",
                           text: `You are ${Math.round(minDistance)} meters away from the allowed location!`
                       });
                   }
               },
               function (error) {
                   Swal.close();
                   alert('Error fetching location: ' + error.message);
               },
               {
                   enableHighAccuracy: true,
                   timeout: 10000,
                   maximumAge: 0
               }
           );
       } else {
           Swal.close();
           alert("Geolocation is not supported by this browser");
       }
   }
