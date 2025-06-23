this is my js 
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

                    // const lat = 22.79714;
                    // const lon = 86.183471;

                    const accuracy = position.coords.accuracy;

                    const deviceInfo = {
                        userAgent: navigator.userAgent,
                        platform: navigator.platform,
                        language: navigator.language,
                        timestamp: new Date().toISOString()
                    };

                   
                    const response = await fetch('/TSUISLARS/Geo/VerifyLocation', {
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
                            title: 'Fake Location Detected',
                            text: result.message || 'Location spoofing detected. Punch options are disabled.'
                        });

                      
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
                    function (error) {
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

this is my controller code  
[HttpPost]
 public IActionResult VerifyLocation([FromBody] LocationRequest model)
 {
     var suspicious = false;
     string reason = "";

    
     if (model.Accuracy > 100)
     {
         suspicious = true;
         reason = "Low GPS accuracy detected (>100m). Mock location or poor signal suspected.";
     }

   
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


after implementing of this again it fetches the fake location and button is enabled. i want to stop these for fake location spoofing 
