<div class="form-group d-flex justify-content-center">
    <div class="camera-container">
        <video id="video" autoplay playsinline></video>
        
        <!-- Red blinking light -->
        <div class="recording-light"></div>
        
        <!-- Animated overlay -->
        <div class="overlay-text">LIVE</div>
        
        <!-- Futuristic border animation -->
        <div class="animated-border"></div>
    </div>
</div>

.camera-container {
    position: relative;
    width: 350px;
    height: 270px;
    border-radius: 20px;
    overflow: hidden;
    background: rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(15px);
    border: 2px solid rgba(255, 255, 255, 0.3);
    box-shadow: 0px 0px 20px rgba(0, 0, 0, 0.3);
}

video {
    width: 100%;
    height: 100%;
    border-radius: 20px;
    object-fit: cover;
    transform: scaleX(-1);
}

/* Red Blinking Light */
.recording-light {
    position: absolute;
    top: 10px;
    right: 10px;
    width: 12px;
    height: 12px;
    background-color: red;
    border-radius: 50%;
    box-shadow: 0 0 8px rgba(255, 0, 0, 0.8);
    animation: blink 1s infinite alternate;
}

@keyframes blink {
    from {
        opacity: 1;
        box-shadow: 0 0 10px rgba(255, 0, 0, 1);
    }
    to {
        opacity: 0.3;
        box-shadow: 0 0 5px rgba(255, 0, 0, 0.5);
    }
}

/* Overlay Text (Live) */
.overlay-text {
    position: absolute;
    top: 10px;
    left: 50%;
    transform: translateX(-50%);
    color: white;
    background: rgba(0, 0, 0, 0.6);
    padding: 5px 12px;
    border-radius: 10px;
    font-family: Arial, sans-serif;
    font-size: 14px;
    font-weight: bold;
    letter-spacing: 1px;
    text-transform: uppercase;
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

/* Glowing Border Effect */
.animated-border {
    position: absolute;
    width: 100%;
    height: 100%;
    border-radius: 20px;
    box-shadow: 0 0 15px rgba(0, 255, 255, 0.9);
    animation: glow 1.5s infinite alternate;
}

@keyframes glow {
    from {
        box-shadow: 0 0 10px rgba(0, 255, 255, 0.8);
    }
    to {
        box-shadow: 0 0 25px rgba(0, 255, 255, 1);
    }
}




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
