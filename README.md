<script>
    function OnOff() {
        var punchIn = document.getElementById('PunchIn');
        var punchOut = document.getElementById('PunchOut');

        // Disable the buttons initially
        punchIn.disabled = true;
        punchOut.disabled = true;
        punchIn.classList.add("disabled");
        punchOut.classList.add("disabled");

        // Show a loading message
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
                    Swal.close(); // Close the loading alert

                    const lat = roundTo(position.coords.latitude, 6);
                    const lon = roundTo(position.coords.longitude, 6);

                    // Display current coordinates for debugging/testing
                    Swal.fire({
                        title: 'Your Current Location',
                        html: 'Latitude: ' + lat + '<br>Longitude: ' + lon,
                        icon: 'info'
                    });

                    const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                    console.log(locations);

                    let isInsideRadius = false;
                    locations.forEach((location) => {
                        const allowedRange = parseFloat(location.range || location.Range);
                        const distance = calculateDistance(lat, lon, location.latitude || location.Latitude, location.longitude || location.Longitude);
                        console.log(`Distance to location (${location.latitude}, ${location.longitude}): ${Math.round(distance)} meters`);
                        if (distance <= allowedRange) {
                            isInsideRadius = true;
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
                            text: "You are not within the allowed location range for attendance!"
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

    // Haversine formula to calculate distance
    function calculateDistance(lat1, lon1, lat2, lon2) {
        const R = 6371000; // Radius of the Earth in meters
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
                    // Round the latitude and longitude for precision
                    const lat = roundTo(position.coords.latitude, 6);
                    const lon = roundTo(position.coords.longitude, 6);

                    // Alert with current position details for mobile testing
                    Swal.fire({
                        title: 'Your Current Location',
                        html: 'Latitude: ' + lat + '<br>Longitude: ' + lon,
                        icon: 'info'
                    });

                    // Retrieve locations (each location should have Latitude, Longitude and Range)
                    const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                    console.log(locations);

                    // Check each stored location and calculate distance
                    let isInsideRadius = false;
                    let details = '';
                    locations.forEach((location, index) => {
                        // Ensure the range is a number
                        const allowedRange = parseFloat(location.range || location.Range);
                        const distance = calculateDistance(lat, lon, location.latitude || location.Latitude, location.longitude || location.Longitude);
                        details += 'Distance from your location to location ' + (index + 1) + ' (' +
                                   (location.latitude || location.Latitude) + ', ' +
                                   (location.longitude || location.Longitude) + '): ' +
                                   Math.round(distance) + ' meters<br>';
                        if (distance <= allowedRange) {
                            isInsideRadius = true;
                        }
                    });

                    // Show distance details in an alert
                    Swal.fire({
                        title: 'Distance Details',
                        html: details,
                        icon: 'info',
                        width: '600px'
                    });

                    // Enable buttons if within any allowed range
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

    // Calculate the distance between two coordinates using the Haversine formula
    function calculateDistance(lat1, lon1, lat2, lon2) {
        const R = 6371000; // Earth's radius in meters
        const toRad = angle => (angle * Math.PI) / 180;

        let dLat = toRad(lat2 - lat1);
        let dLon = toRad(lon2 - lon1);

        let a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
                Math.sin(dLon / 2) * Math.sin(dLon / 2);
        let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c; // Distance in meters
    }

    // Helper function to round numbers
    function roundTo(num, places) {
        return +(Math.round(num + "e" + places) + "e-" + places);
    }

    window.onload = OnOff;
</script>




this is my js , i want to test this in Mobile Phone, so please gave me proper alert to understand, and how many longitude and latitude required, how to measure long and lat, is
<script>
    function OnOff() {
        var punchIn = document.getElementById('PunchIn');
        var punchOut = document.getElementById('PunchOut');

      
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


                    console.log(locations);

                    let isInsideRadius = locations.some(location => {
                        return isWithinRadius(lat, long, location.Latitude, location.Longitude, location.Range);
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

   
    function isWithinRadius(userLat, userLon, locLat, locLon, range) {
        const R = 6371000; 
        const toRad = angle => (angle * Math.PI) / 180;

        let dLat = toRad(locLat - userLat);
        let dLon = toRad(locLon - userLon);

        let a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(toRad(userLat)) * Math.cos(toRad(locLat)) *
                Math.sin(dLon / 2) * Math.sin(dLon / 2);

        let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        let distance = R * c; 

        return distance <= range; 
    }

    function roundTo(num, places) {
        return +(Math.round(num + "e" + places) + "e-" + places);
    }

    window.onload = OnOff;
</script>

this is my polygon data , is this proper

Array(9)0: {latitude: 22.797986, longitude: 86.183358, range: '100'}1: {latitude: 22.804407, longitude: 86.183932, range: '100'}2: {latitude: 22.796364, longitude: 86.183014, range: '100'}3: {latitude: 22.796404, longitude: 86.184595, range: '100'}4: {latitude: 22.804431, longitude: 86.183302, range: '100'}5: {latitude: 22.804583, longitude: 86.183633, range: '100'}6: {latitude: 22.798088, longitude: 86.184976, range: '100'}7: {latitude: 22.797673, longitude: 86.182989, range: '100'}8: {latitude: 22.804145, longitude: 86.183529, range: '100'}length: 9[[Prototype]]: Array(0)


