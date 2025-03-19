document.addEventListener("DOMContentLoaded", function () {
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const form = document.getElementById("form");
    const punchIn = document.getElementById('PunchIn');
    const punchOut = document.getElementById('PunchOut');

    // Start Camera
    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
            Swal.fire("Error", "Camera access is required for attendance.", "error");
        });

    function captureImageAndSubmit(entryType) {
        // Set Entry Type (Punch In / Punch Out)
        EntryTypeInput.value = entryType;

        // Capture Image
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        // Convert Image to Base64
        const imageData = canvas.toDataURL("image/png");

        // Append Image Data to Form
        let imageInput = document.querySelector("input[name='ImageData']");
        if (!imageInput) {
            imageInput = document.createElement("input");
            imageInput.type = "hidden";
            imageInput.name = "ImageData";
            form.appendChild(imageInput);
        }
        imageInput.value = imageData;

        // Submit Form
        form.submit();
    }

    function OnOff() {
        punchIn.disabled = true;
        punchOut.disabled = true;
        punchIn.classList.add("disabled");
        punchOut.classList.add("disabled");

        Swal.fire({
            title: 'Please wait...',
            text: 'Fetching your current location.',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    Swal.close();
                    const lat = roundTo(position.coords.latitude, 6);
                    const lon = roundTo(position.coords.longitude, 6);

                    const locations = @Html.Raw(Json.Serialize(ViewBag.PolyData));
                    console.log("Allowed Locations:", locations);

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
                    Swal.fire("Error", "Error fetching location: " + error.message, "error");
                },
                {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 0
                }
            );
        } else {
            Swal.close();
            Swal.fire("Error", "Geolocation is not supported by this browser", "error");
        }
    }

    function calculateDistance(lat1, lon1, lat2, lon2) {
        const R = 6371000; // Earth’s mean radius in meters
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

    // Run location check when page loads
    OnOff();

    // Attach event listeners to buttons
    punchIn.addEventListener("click", () => captureImageAndSubmit("Punch In"));
    punchOut.addEventListener("click", () => captureImageAndSubmit("Punch Out"));
});

[HttpPost]
public IActionResult AttendanceData(string Type, string ImageData)
{
    if (string.IsNullOrEmpty(ImageData))
    {
        return BadRequest("No image captured.");
    }

    // Convert Base64 to Byte Array
    byte[] imageBytes = Convert.FromBase64String(ImageData.Split(',')[1]); // Remove "data:image/png;base64,"

    // Save image (example)
    string filePath = Path.Combine("wwwroot/images", $"{Guid.NewGuid()}.png");
    System.IO.File.WriteAllBytes(filePath, imageBytes);

    return Json(new { success = true, message = "Attendance recorded successfully!" });
}



i have this view side

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>
    <input type="hidden" name="Type" id="EntryType" />

    <div class="row mt-5 form-group">
        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                Punch In
            </button>
        </div>

        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                Punch Out
            </button>
        </div>
    </div>
</form>

and this is my js , make sure to work for this 
<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const form = document.getElementById("form");

    // Start Camera
    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

    function captureImageAndSubmit(entryType) {
        // Set Entry Type (Punch In / Punch Out)
        EntryTypeInput.value = entryType;

        // Capture Image
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        // Convert Image to Base64
        const imageData = canvas.toDataURL("image/png");
        
        // Append Image Data to Form
        const imageInput = document.createElement("input");
        imageInput.type = "hidden";
        imageInput.name = "ImageData";
        imageInput.value = imageData;
        form.appendChild(imageInput);

        // Submit Form
        form.submit();
    }
</script>
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

                 // Get user's current location
                 const lat = roundTo(position.coords.latitude, 6);
                 const lon = roundTo(position.coords.longitude, 6);

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
