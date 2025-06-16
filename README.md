<script>
    async function OnOff() {
        // Disable buttons initially
        var punchIn = document.getElementById('PunchIn');
        var punchOut = document.getElementById('PunchOut');

        if (punchIn) punchIn.disabled = true;
        if (punchOut) punchOut.disabled = true;

        Swal.fire({
            title: 'Please wait...',
            text: 'Fetching your device info and location.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        // Load FingerprintJS
        const fpPromise = FingerprintJS.load();

        // Get location
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                async function (position) {
                    const lat = roundTo(position.coords.latitude, 6);
                    const lon = roundTo(position.coords.longitude, 6);
                    const accuracy = position.coords.accuracy;
                    const timestamp = position.timestamp;

                    // Get fingerprint
                    const fp = await fpPromise;
                    const result = await fp.get();
                    const fingerprint = result.visitorId;
                    const components = result.components;

                    // Build payload
                    const payload = {
                        latitude: lat,
                        longitude: lon,
                        accuracy: accuracy,
                        timestamp: timestamp,
                        fingerprint: fingerprint,
                        deviceInfo: {
                            userAgent: navigator.userAgent,
                            timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
                            screen: {
                                width: screen.width,
                                height: screen.height,
                                pixelDepth: screen.pixelDepth
                            },
                            components: {
                                platform: components.platform?.value,
                                languages: components.languages?.value,
                                vendor: components.vendor?.value,
                                osCpu: components.osCpu?.value
                            }
                        }
                    };

                    // Send to server
                    fetch('/Geo/ValidateLocationWithFingerprint', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(payload)
                    })
                        .then(response => response.json())
                        .then(result => {
                            Swal.close();
                            if (result.success) {
                                if (punchIn) punchIn.disabled = false;
                                if (punchOut) punchOut.disabled = false;
                            } else {
                                Swal.fire({
                                    icon: "error",
                                    title: "Warning",
                                    text: result.message
                                });
                            }
                        });
                },
                function (error) {
                    Swal.close();
                    Swal.fire({
                        icon: "error",
                        title: "Location Error",
                        text: "Please allow location access or disable fake location apps"
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
            alert("Geolocation is not supported by this browser.");
        }
    }

    function roundTo(num, places) {
        return +(Math.round(num + "e" + places) + "e-" + places);
    }

    window.onload = OnOff;
</script>

[HttpPost]
[Route("Geo/ValidateLocationWithFingerprint")]
public IActionResult ValidateLocationWithFingerprint([FromBody] GeoRequestWithFingerprint request)
{
    // Example validation logic
    bool isLocationSpoofed = request.Accuracy > 1000;
    bool isTimezoneMismatch = request.DeviceInfo.Timezone != "Asia/Kolkata"; // Example only

    // Log for review
    Console.WriteLine($"User: {request.Fingerprint}");
    Console.WriteLine($"Lat: {request.Latitude}, Lon: {request.Longitude}, Accuracy: {request.Accuracy}");
    Console.WriteLine($"TimeZone: {request.DeviceInfo.Timezone}, UserAgent: {request.DeviceInfo.UserAgent}");

    if (isLocationSpoofed || isTimezoneMismatch)
    {
        return Json(new { success = false, message = "Suspicious device or location data." });
    }

    return Json(new { success = true, message = "Device and location OK." });
}

public class GeoRequestWithFingerprint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Accuracy { get; set; }
    public long Timestamp { get; set; }
    public string Fingerprint { get; set; }
    public DeviceInfo DeviceInfo { get; set; }
}

public class DeviceInfo
{
    public string UserAgent { get; set; }
    public string Timezone { get; set; }
    public ScreenInfo Screen { get; set; }
    public Dictionary<string, string> Components { get; set; }
}

public class ScreenInfo
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int PixelDepth { get; set; }
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


i am having a big issue bug that someone uses fake location app to get the button enable , i am sharing the full process how they do it , firstly they open develepor mode of the android and then install the fake app of location and then it use mack something and after that it gets the fake location , i want to verify this , please do something to resolve this 
