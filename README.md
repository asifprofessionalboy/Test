<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Stylish Camera</title>
    <style>
        body {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            background: url('https://source.unsplash.com/random/1920x1080?nature') no-repeat center center/cover;
        }

        .camera-container {
            position: relative;
            width: 340px;
            height: 260px;
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0px 0px 15px rgba(0, 0, 0, 0.3);
            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(10px);
            border: 2px solid rgba(255, 255, 255, 0.2);
        }

        video {
            width: 100%;
            height: 100%;
            border-radius: 15px;
            object-fit: cover;
        }

        .overlay-text {
            position: absolute;
            top: 10px;
            left: 50%;
            transform: translateX(-50%);
            color: #fff;
            background: rgba(0, 0, 0, 0.5);
            padding: 5px 10px;
            border-radius: 10px;
            font-family: Arial, sans-serif;
            font-size: 14px;
            letter-spacing: 1px;
            animation: fadeIn 1s ease-in-out;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .animated-border {
            position: absolute;
            width: 100%;
            height: 100%;
            border-radius: 15px;
            box-shadow: 0 0 10px rgba(0, 255, 255, 0.8);
            animation: glow 1.5s infinite alternate;
        }

        @keyframes glow {
            from {
                box-shadow: 0 0 10px rgba(0, 255, 255, 0.8);
            }
            to {
                box-shadow: 0 0 20px rgba(0, 255, 255, 1);
            }
        }
    </style>
</head>
<body>

    <div class="camera-container">
        <video id="video" autoplay playsinline></video>
        <div class="overlay-text">📷 Live Camera</div>
        <div class="animated-border"></div>
    </div>

    <canvas id="canvas" style="display: none;"></canvas>

    <script>
        navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
            .then(function (stream) {
                let video = document.getElementById("video");
                video.srcObject = stream;
                video.play();
            })
            .catch(function (error) {
                console.error("Error accessing camera: ", error);
            });
    </script>

</body>
</html>





<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const videoContainer = document.getElementById("videoContainer");
    const EntryTypeInput = document.getElementById("EntryType");

    // Show loading effect before the camera starts
    videoContainer.classList.add("loading");

    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
            setTimeout(() => {
                videoContainer.classList.remove("loading");
            }, 1500); // Remove loading effect after 1.5s
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

        const imageData = canvas.toDataURL("image/jpeg");

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
<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <div class="video-container loading" id="videoContainer">
            <video id="video" width="320" height="240" autoplay playsinline></video>
        </div>
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

<style>
    /* Video Styling */
    video {
        transform: scaleX(-1);
        -webkit-transform: scaleX(-1); 
        -moz-transform: scaleX(-1);
        border-radius: 15px;
        box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.3);
        transition: all 0.5s ease-in-out;
        border: 3px solid rgba(255, 255, 255, 0.5);
        animation: fadeIn 1s ease-in-out;
    }

    /* Background Effect */
    .video-container {
        position: relative;
        width: 320px;
        height: 240px;
        display: flex;
        justify-content: center;
        align-items: center;
        background: linear-gradient(135deg, #6e8efb, #a777e3);
        padding: 10px;
        border-radius: 20px;
        box-shadow: 0px 8px 20px rgba(0, 0, 0, 0.4);
        overflow: hidden;
    }

    /* Animated Glow */
    .video-container::before {
        content: "";
        position: absolute;
        width: 100%;
        height: 100%;
        background: rgba(255, 255, 255, 0.2);
        filter: blur(20px);
        z-index: -1;
        opacity: 0.5;
        animation: pulseGlow 2s infinite alternate;
    }

    /* Loading Effect */
    .video-container.loading::after {
        content: "Loading Camera...";
        position: absolute;
        color: white;
        font-size: 16px;
        font-weight: bold;
        text-shadow: 2px 2px 5px rgba(0, 0, 0, 0.5);
        animation: blinkText 1.5s infinite;
    }

    /* Animations */
    @keyframes fadeIn {
        from { opacity: 0; transform: scale(0.8); }
        to { opacity: 1; transform: scale(1); }
    }

    @keyframes pulseGlow {
        from { opacity: 0.5; }
        to { opacity: 0.8; }
    }

    @keyframes blinkText {
        0% { opacity: 1; }
        50% { opacity: 0.5; }
        100% { opacity: 1; }
    }
</style>



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
