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


