this is my old query to fetch details

WITH TotalPerDay AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
        COUNT(DISTINCT Pno) AS TotalUsers
    FROM App_FaceVerification_Details
    WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-01' AND '2025-06-30'
    GROUP BY CONVERT(date, DateAndTime)
),
GroupedCounts AS (
    SELECT 
        CONVERT(date, DateAndTime) AS AttemptDate,
        CASE 
            WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
            WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
            WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
            ELSE '10+'
        END AS AttemptRange,
        COUNT(DISTINCT Pno) AS NumberOfUsers
    FROM App_FaceVerification_Details
    WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-01' AND '2025-06-30'
    GROUP BY 
        CONVERT(date, DateAndTime),
        CASE 
            WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
            WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
            WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
            ELSE '10+'
        END
)
SELECT 
    g.AttemptDate,
    g.AttemptRange,
    g.NumberOfUsers,
    t.TotalUsers,
    CAST(g.NumberOfUsers * 100.0 / t.TotalUsers AS DECIMAL(5, 2)) AS Percentage
FROM GroupedCounts g
JOIN TotalPerDay t ON g.AttemptDate = t.AttemptDate
ORDER BY g.AttemptDate, g.AttemptRange;


this is my new query to fetch absent user 
DECLARE @StartDate DATE = '2025-06-01';
DECLARE @EndDate DATE = '2025-06-30';

WITH DateList AS (
    SELECT @StartDate AS TheDate
    UNION ALL
    SELECT DATEADD(DAY, 1, TheDate)
    FROM DateList
    WHERE TheDate < @EndDate
)
SELECT d.TheDate,
    (
        SELECT COUNT(*) 
        FROM App_Empl_Master em
        WHERE em.Discharge_Date IS NULL
        AND em.pno NOT IN (
            SELECT TRBDGDA_BD_PNO 
            FROM T_TRBDGDAT_EARS 
            WHERE TRBDGDA_BD_DATE = d.TheDate
        ) 
    ) AS AbsentCount 
FROM DateList d
ORDER BY d.TheDate
OPTION (MAXRECURSION 31);

i want to merge this from above query to found out the absent user also



<script src="https://cdn.jsdelivr.net/npm/@fingerprintjs/fingerprintjs@3/dist/fp.min.js"></script>

<script>
    let punchIn = null, punchOut = null;
    let deviceFingerprint = "";

    window.onload = async function () {
        punchIn = document.getElementById('PunchIn');
        punchOut = document.getElementById('PunchOut');

        // FingerprintJS init
        const fp = await FingerprintJS.load();
        const result = await fp.get();
        deviceFingerprint = result.visitorId;

        disableButtons();
        getLocationAndVerify();
    };

    function disableButtons() {
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
    }

    function enableButtons() {
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
    }

    async function getLocationAndVerify() {
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

                const ipRes = await fetch("https://ipapi.co/json");
                const ipData = await ipRes.json();

                const ipLat = ipData.latitude;
                const ipLon = ipData.longitude;

                const ipDistance = calculateDistance(lat, lon, ipLat, ipLon);
                const isIPMismatch = ipDistance > 10000; // 10km mismatch = suspicious

                const motionStatus = await detectMotion();
                const suspiciousMotion = motionStatus === false;

                // Send all data to your backend
                const response = await fetch('/TSUISLARS/Geo/VerifyLocation', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        latitude: lat,
                        longitude: lon,
                        accuracy: accuracy,
                        device: deviceInfo,
                        fingerprint: deviceFingerprint,
                        ipLatitude: ipLat,
                        ipLongitude: ipLon,
                        ipDistance: ipDistance,
                        motionDetected: motionStatus
                    })
                });

                const result = await response.json();

                if (!result.isValid) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Location Spoofing Detected!',
                        text: result.message
                    });
                    disableButtons();
                    return;
                }

                // Your existing allowed-range check
                const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                let isInsideRadius = false;
                let minDistance = Number.MAX_VALUE;

                locations.forEach((location) => {
                    const allowedRange = parseFloat(location.range || location.Range);
                    const distance = calculateDistance(lat, lon, location.latitude || location.Latitude, location.longitude || location.Longitude);
                    if (distance <= allowedRange) isInsideRadius = true;
                    else minDistance = Math.min(minDistance, distance);
                });

                if (isInsideRadius) {
                    enableButtons();
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Out of Range",
                        text: `You are ${Math.round(minDistance)} meters away from allowed location!`
                    });
                }

            }, function () {
                Swal.close();
                Swal.fire({
                    title: "Location Error",
                    text: "Enable location services and permissions.",
                    icon: "error"
                });
            }, {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 0
            });
        } else {
            Swal.close();
            alert("Geolocation is not supported by your browser.");
        }
    }

    function roundTo(num, places) {
        return +(Math.round(num + "e" + places) + "e-" + places);
    }

    function calculateDistance(lat1, lon1, lat2, lon2) {
        const R = 6371000;
        const toRad = angle => (angle * Math.PI) / 180;
        const dLat = toRad(lat2 - lat1);
        const dLon = toRad(lon2 - lon1);
        const a = Math.sin(dLat / 2) ** 2 +
            Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
            Math.sin(dLon / 2) ** 2;
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    function detectMotion() {
        return new Promise(resolve => {
            let hasMoved = false;
            function motionListener(e) {
                const acc = e.acceleration || e.accelerationIncludingGravity;
                if (acc && (acc.x !== null || acc.y !== null || acc.z !== null)) {
                    hasMoved = true;
                }
                window.removeEventListener("devicemotion", motionListener);
                resolve(hasMoved);
            }
            window.addEventListener("devicemotion", motionListener);
            setTimeout(() => {
                window.removeEventListener("devicemotion", motionListener);
                resolve(hasMoved);
            }, 3000); // Wait 3 seconds max
        });
    }
</script>

[HttpPost]
public IActionResult VerifyLocation([FromBody] ExtendedLocationRequest model)
{
    var suspicious = false;
    var reasons = new List<string>();

    if (model.Accuracy > 50)
    {
        suspicious = true;
        reasons.Add("Low GPS accuracy (>50m).");
    }

    if (model.IpDistance > 10000) // 10km+ IP/GPS mismatch
    {
        suspicious = true;
        reasons.Add("GPS location does not match IP-based location.");
    }

    if (!model.MotionDetected)
    {
        suspicious = true;
        reasons.Add("No device motion detected. Possibly spoofed.");
    }

    if (model.Device.UserAgent.ToLower().Contains("emulator") ||
        model.Device.UserAgent.ToLower().Contains("mock") ||
        model.Device.UserAgent.ToLower().Contains("sdk"))
    {
        suspicious = true;
        reasons.Add("Emulator or spoofing app detected.");
    }

    if (suspicious)
    {
        // Optional: Save to log table

        return BadRequest(new
        {
            isValid = false,
            message = string.Join(" ", reasons)
        });
    }

    return Ok(new { isValid = true });
}

public class ExtendedLocationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Accuracy { get; set; }
    public DeviceDetails Device { get; set; }
    public string Fingerprint { get; set; }
    public double IpLatitude { get; set; }
    public double IpLongitude { get; set; }
    public double IpDistance { get; set; }
    public bool MotionDetected { get; set; }

    public class DeviceDetails
    {
        public string UserAgent { get; set; }
        public string Platform { get; set; }
        public string Language { get; set; }
        public string Timestamp { get; set; }
    }
}






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
