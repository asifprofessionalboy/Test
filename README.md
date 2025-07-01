<style>
    .video-wrapper {
        display: flex;
        justify-content: center;
        align-items: center;
    }

    #videoContainer {
        border: 4px solid limegreen;
        border-radius: 8px;
        padding: 0;
        line-height: 0;
        display: inline-block;
    }

    #videoContainer video {
        width: 320px;
        height: 240px;
        margin: 0;
        padding: 0;
        border: none;
        display: block;
        transform: scaleX(-1);
    }

    #statusText {
        font-weight: bold;
        margin-top: 10px;
        color: #444;
        text-align: center;
    }
</style>

<div class="form-group video-wrapper">
    <div id="videoContainer">
        <video id="video" autoplay muted playsinline></video>
    </div>
</div>
<canvas id="canvas" style="display:none;"></canvas>
<p id="statusText"></p>




i have code but getting the same result as first i told 
<style>

   
    #videoContainer {
    display: inline-block;
   
    border: 4px solid limegreen; 
    border-radius: 8px;
    padding: 0;
    line-height: 0; 
}

video {
    display: block;
    width: 320px;
    height: 240px;
    transform: scaleX(-1);
    -webkit-transform: scaleX(-1); 
    -moz-transform: scaleX(-1);
}

</style>

    <div class="form-group text-center">
        <div id="videoContainer">
            <video id="video" width="320" height="240" autoplay muted playsinline></video>
        </div>
        <canvas id="canvas" style="display:none;"></canvas>
        <p id="statusText" style="font-weight: bold; margin-top: 10px; color: #444;"></p>
    </div>

 
