this is my style for video
<style>
    video {
        transform: scaleX(-1);
        -webkit-transform: scaleX(-1); 
        -moz-transform: scaleX(-1);
    }

</style>

this is my form where Video camera and two buttons are 

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>

    <input type="hidden" name="Type" id="EntryType" />

    <div class="row mt-5 form-group">
        <div class="col d-flex justify-content-center mb-4">
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

and this is my script

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");

    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

    function captureImageAndSubmit(entryType) {
        EntryTypeInput.value = entryType;

        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageData = canvas.toDataURL("image/jpeg"); // Save as JPG

        document.getElementById("PunchIn").disabled = true;
        document.getElementById("PunchOut").disabled = true;

        fetch("/Geo/AttendanceData", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Type: entryType,
                ImageData: imageData
            })
        })
            .then(response => response.json())
            .then(data => {
               
                    var now = new Date();
                    var formattedDateTime = now.toLocaleString();
                   
                    Swal.fire({
                        title: "Attendance Recorded.",
                        text: "\nDate & Time: " + formattedDateTime,
                        icon: "success",
                        timer: 5000,
                        showConfirmButton: false
                    });
                
            })
            .catch(error => {
                console.error("Error:", error);
                triggerHapticFeedback("error"); 

                Swal.fire({
                    title: "Error!",
                    text: "An error occurred while processing your request.",
                    icon: "error"
                });
            })
            .finally(() => {
              
                document.getElementById("PunchIn").disabled = false;
                document.getElementById("PunchOut").disabled = false;
            });
    }
</script>


i want my camera to look cool and good for user . add animation and other think to look good of camera 
