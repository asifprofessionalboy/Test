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
