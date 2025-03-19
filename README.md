 const video = document.getElementById("video");
 const canvas = document.getElementById("canvas");
 const EntryTypeInput = document.getElementById("EntryType");
 const form = document.getElementById("form");


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

     context.translate(canvas.width, 0);
     context.scale(-1, 1);
     context.drawImage(video, 0, 0, canvas.width, canvas.height);
     context.setTransform(1, 0, 0, 1, 0, 0);

     const imageData = canvas.toDataURL("image/png");
     
    
     const imageInput = document.createElement("input");
     imageInput.type = "hidden";
     imageInput.name = "ImageData";
     imageInput.value = imageData;
     form.appendChild(imageInput);

    
     form.submit();
 }
