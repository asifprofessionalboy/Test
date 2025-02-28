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

                    // const lat = roundTo(position.coords.latitude, 6);
                    // const lon = roundTo(position.coords.longitude, 6);

                    const lat = 22.79675;
                    const lon = 86.183915;
                   

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

in this js if the user is out of range then it shows how much meters he far from range, if the user's range is 50m and user's is in 150m then shows to user that you are far or the distance in this  Swal.fire({
                            icon: "error",
                            title: "Out of Range",
                            text: "You are not within the allowed location range for attendance!"
                        });
