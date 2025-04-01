function OnOff() {
    var punchIn = document.getElementById('PunchIn');
    var punchOut = document.getElementById('PunchOut');

    // Check if elements exist before accessing them
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

                const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                console.log(locations);

                let isInsideRadius = false;
                let minDistance = Number.MAX_VALUE;

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
                    if (punchIn) {
                        punchIn.disabled = false;
                        punchIn.classList.remove("disabled");
                    }
                    if (punchOut) {
                        punchOut.disabled = false;
                        punchOut.classList.remove("disabled");
                    }
                    Swal.fire({
                        title: 'Within Range',
                        text: 'You are within the allowed range for attendance.',
                        icon: 'success'
                    });
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




<div class="mt-5 form-group">
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

<script>
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
                        Swal.fire({
                            title: 'Within Range',
                            text: 'You are within the allowed range for attendance.',
                            icon: 'success'
                        });
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

after applying viewbag.InOut then getting this error Uncaught TypeError: Cannot set properties of null (setting 'disabled')
    at OnOff ((index):301:26)
