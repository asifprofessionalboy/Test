please change shape of camera and any other design to look more attractive and good 
<div class="form-group d-flex justify-content-center">
 <div class="camera-container">
     <video id="video" autoplay playsinline></video>
     <div class="overlay-text">📷 Live Camera</div>
     <div class="animated-border"></div>
 </div>
 </div>

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
    transform: scaleX(-1);
    -webkit-transform: scaleX(-1);
    -moz-transform: scaleX(-1);
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
