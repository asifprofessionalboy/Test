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



please make my camera look some stylish when user is access the camera    
<div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>

navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
    .then(function (stream) {
        video.srcObject = stream;
        video.play();
    })
    .catch(function (error) {
        console.error("Error accessing camera: ", error);
    });
