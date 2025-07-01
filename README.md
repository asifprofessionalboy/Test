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

 
