<script>
    function OnOff() {
        setTimeout(() => {
            var punchIn = document.getElementById('PunchIn');
            var punchOut = document.getElementById('PunchOut');

            if (punchIn) {
                punchIn.disabled = true;
                punchIn.classList.add("disabled");
            }
            if (punchOut) {
                punchOut.disabled = true;
                punchOut.classList.add("disabled");
            }

            Swal.fire({
                title: 'Please wait...',
                text: 'Fetching your current location...',
                allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(async function (position) {
                    Swal.close();

                    const lat = roundTo(position.coords.latitude, 6);
                    const lon = roundTo(position.coords.longitude, 6);
                    const accuracy = position.coords.accuracy;

                    const deviceInfo = {
                        userAgent: navigator.userAgent,
                        platform: navigator.platform,
                        language: navigator.language,
                        timestamp: new Date().toISOString()
                    };

                    // Send location and device info to backend
                    const response = await fetch('/Location/VerifyLocation', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            latitude: lat,
                            longitude: lon,
                            accuracy: accuracy,
                            device: deviceInfo
                        })
                    });

                    const result = await response.json();

                    if (!result.isValid) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Suspicious Location',
                            text: result.message
                        });
                        return;
                    }

                    // Continue with your existing logic to check proximity
                    const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                    let isInsideRadius = false;
                    let minDistance = Number.MAX_VALUE;

                    locations.forEach((location) => {
                        const allowedRange = parseFloat(location.range || location.Range);
                        const distance = calculateDistance(lat, lon, location.latitude || location.Latitude, location.longitude || location.Longitude);

                        if (distance <= allowedRange) {
                            isInsideRadius = true;
                        } else {
                            minDistance = Math.min(minDistance, distance);
                        }
                    });

                    if (isInsideRadius) {
                        if (punchIn) {
                            punchIn.disabled = false;
                            punchIn.classList.remove("disabled");
                        }
                        if (punchOut) {
                            punchOut.disabled = false;
                            punchOut.classList.remove("disabled");
                        }
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "Out of Range",
                            text: `You are ${Math.round(minDistance)} meters away from the allowed location!`
                        });
                    }

                }, function (error) {
                    Swal.close();
                    Swal.fire({
                        title: "Error Fetching Location!",
                        text: "Please enable location and ensure permissions are allowed.",
                        icon: "error"
                    });
                }, {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 0
                });
            } else {
                Swal.close();
                alert("Geolocation is not supported by this browser.");
            }
        }, 500);
    }

    window.onload = OnOff;

    function calculateDistance(lat1, lon1, lat2, lon2) {
        const R = 6371000;
        const toRad = angle => (angle * Math.PI) / 180;
        let dLat = toRad(lat2 - lat1);
        let dLon = toRad(lon2 - lon1);
        let a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
                Math.sin(dLon / 2) * Math.sin(dLon / 2);
        let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    function roundTo(num, places) {
        return +(Math.round(num + "e" + places) + "e-" + places);
    }
</script>

[HttpPost]
public IActionResult VerifyLocation([FromBody] LocationRequest model)
{
    var suspicious = false;
    string reason = "";

    // Flag suspicious accuracy
    if (model.Accuracy > 100) // >100m is often non-GPS
    {
        suspicious = true;
        reason = "Low accuracy detected (>100m). Possible mock or network-based location.";
    }

    // Optional: Check User-Agent or Platform
    if (model.Device != null && model.Device.UserAgent.Contains("Build/")) // very generic agent in spoofing
    {
        // Additional flag logic (if needed)
    }

    // Log the attempt (store in DB if needed)
    Console.WriteLine($"Location attempt: Lat={model.Latitude}, Lon={model.Longitude}, Acc={model.Accuracy}, UA={model.Device?.UserAgent}");

    if (suspicious)
    {
        return BadRequest(new
        {
            isValid = false,
            message = reason
        });
    }

    return Ok(new { isValid = true });
}

public class LocationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Accuracy { get; set; }
    public DeviceInfo Device { get; set; }

    public class DeviceInfo
    {
        public string UserAgent { get; set; }
        public string Platform { get; set; }
        public string Language { get; set; }
        public string Timestamp { get; set; }
    }
}

navigator.geolocation.getCurrentPosition(async function (position) {
    Swal.close();

    const lat = roundTo(position.coords.latitude, 6);
    const lon = roundTo(position.coords.longitude, 6);
    const accuracy = position.coords.accuracy;

    const deviceInfo = {
        userAgent: navigator.userAgent,
        platform: navigator.platform,
        language: navigator.language,
        timestamp: new Date().toISOString()
    };

    // Send location + device info to backend for spoofing validation
    const response = await fetch('/Location/VerifyLocation', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            latitude: lat,
            longitude: lon,
            accuracy: accuracy,
            device: deviceInfo
        })
    });

    const result = await response.json();

    // Handle spoofing detection
    if (!result.isValid) {
        Swal.fire({
            icon: 'error',
            title: 'Fake Location Detected',
            text: result.message || 'Location spoofing detected. Punch options are disabled.'
        });

        // Hide and disable the buttons
        if (punchIn) {
            punchIn.disabled = true;
            punchIn.classList.add("disabled");
            punchIn.style.display = "none";
        }
        if (punchOut) {
            punchOut.disabled = true;
            punchOut.classList.add("disabled");
            punchOut.style.display = "none";
        }

        return;
    }

    // Your existing allowed-range checking logic continues here
    const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
    let isInsideRadius = false;
    let minDistance = Number.MAX_VALUE;

    locations.forEach((location) => {
        const allowedRange = parseFloat(location.range || location.Range);
        const distance = calculateDistance(lat, lon, location.latitude || location.Latitude, location.longitude || location.Longitude);

        if (distance <= allowedRange) {
            isInsideRadius = true;
        } else {
            minDistance = Math.min(minDistance, distance);
        }
    });

    if (isInsideRadius) {
        if (punchIn) {
            punchIn.disabled = false;
            punchIn.classList.remove("disabled");
            punchIn.style.display = "inline-block";
        }
        if (punchOut) {
            punchOut.disabled = false;
            punchOut.classList.remove("disabled");
            punchOut.style.display = "inline-block";
        }
    } else {
        Swal.fire({
            icon: "error",
            title: "Out of Range",
            text: `You are ${Math.round(minDistance)} meters away from the allowed location!`
        });
    }
},

[HttpPost]
public IActionResult VerifyLocation([FromBody] LocationRequest model)
{
    var suspicious = false;
    string reason = "";

    // Rule: GPS accuracy > 100 meters = unreliable (likely fake or tower-based)
    if (model.Accuracy > 100)
    {
        suspicious = true;
        reason = "Low GPS accuracy detected (>100m). Mock location or poor signal suspected.";
    }

    // Optional: Detect possible emulator or automation
    if (model.Device != null && model.Device.UserAgent.ToLower().Contains("sdk") ||
        model.Device.UserAgent.ToLower().Contains("emulator"))
    {
        suspicious = true;
        reason = "Emulator or automated environment detected.";
    }

    if (suspicious)
    {
        return BadRequest(new
        {
            isValid = false,
            message = reason
        });
    }

    return Ok(new { isValid = true });
}



i have this js for geolocation
<script>
    function OnOff() {
        setTimeout(() => {
            var punchIn = document.getElementById('PunchIn');
            var punchOut = document.getElementById('PunchOut');

           
            if (punchIn) {
                punchIn.disabled = true;
                punchIn.classList.add("disabled");
            }
            if (punchOut) {
                punchOut.disabled = true;
                punchOut.classList.add("disabled");
            }

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
                        // const lat = 22.79714;
                        // const lon = 86.183471;

                        const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                        

                        let isInsideRadius = false;
                        let minDistance = Number.MAX_VALUE;

                        locations.forEach((location) => {
                            const allowedRange = parseFloat(location.range || location.Range);
                            const distance = calculateDistance(lat, lon, location.latitude || location.Latitude, location.longitude || location.Longitude);
                            //console.log(`Distance to location (${location.latitude}, ${location.longitude}): ${Math.round(distance)} meters`);

                            if (distance <= allowedRange) {
                                isInsideRadius = true;
                            } else {
                                minDistance = Math.min(minDistance, distance);
                            }
                        });

                        if (isInsideRadius) {
                            if (punchIn) {
                                punchIn.disabled = false;
                                punchIn.classList.remove("disabled");
                            }
                            if (punchOut) {
                                punchOut.disabled = false;
                                punchOut.classList.remove("disabled");
                            }
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
                        Swal.fire({
                            title: "Error Fetching Location!",
                            text: "please check your location permission or enable location",
                            icon: "error",
                            confirmButtonText: "OK"
                        });
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
        }, 500); 
    }

   
    window.onload = OnOff;

    function calculateDistance(lat1, lon1, lat2, lon2) {
        const R = 6371000;
        const toRad = angle => (angle * Math.PI) / 180;
        let dLat = toRad(lat2 - lat1);
        let dLon = toRad(lon2 - lon1);
        let a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
            Math.sin(dLon / 2) * Math.sin(dLon / 2);
        let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    function roundTo(num, places) {
        return +(Math.round(num + "e" + places) + "e-" + places);
    }

    window.onload = OnOff;
</script>

this is my controller code 
 public IActionResult GetLocations()
 {
     var UserId = HttpContext.Request.Cookies["Session"];
     string connectionString = GetRFIDConnectionString();

     string query = @"SELECT ps.Worksite FROM TSUISLRFIDDB.DBO.App_Position_Worksite AS ps 
              INNER JOIN TSUISLRFIDDB.DBO.App_Emp_position AS es ON es.position = ps.position 
              WHERE es.Pno = @UserId";

     using (var connection = new SqlConnection(connectionString))
     {
        
         var worksiteNamesString = connection.QuerySingleOrDefault<string>(query, new { UserId });

         if (string.IsNullOrEmpty(worksiteNamesString))
         {
             ViewBag.PolyData = new List<object>();
             return View();
         }

         
         var worksiteNames = worksiteNamesString.Split(',').Select(w => w.Trim()).ToList();

        
         var formattedWorksites = worksiteNames
             .Select(name => $"'{name.Replace("'", "''")}'") 
             .ToList();

         string s = string.Join(",", formattedWorksites);

         string query2 = @$"SELECT Longitude, Latitude, Range FROM TSUISLRFIDDB.DBO.App_LocationMaster 
                    WHERE ID IN ({s})";

         var locations = connection.Query(query2).Select(loc => new
         {
             Latitude = (double)loc.Latitude,
             Longitude = (double)loc.Longitude,
             Range = loc.Range
         }).ToList();

         ViewBag.PolyData = locations;
         return View();
     }
 }


i am having a big issue bug that someone uses fake location app to get the button enable , i am sharing the full process how they do it , firstly they open develepor mode of the android and then install the fake app of location and then it use mack something and after that it gets the fake location , i want to verify this , please do something to resolve this 
