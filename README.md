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
            }, 3000); 
        });
    }
</script>

this is also not working when using fake location app it is not working it fetches the location and make button visible
