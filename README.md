https://localhost:7153/faceApi/tiny_face_detector_model-weights_manifest.json

Promise.all([
    faceapi.nets.tinyFaceDetector.loadFromUri('/faceApi'),
    faceapi.nets.faceLandmark68Net.loadFromUri('/faceApi')
])
.then(startVideo)
.catch(error => {
    console.error("Failed to load face-api models:", error);
});



getting this error 

(index):150 Failed to load face-api models: SyntaxError: Unexpected token '<', "<!--# SNNs"... is not valid JSON

:7153/faceApi/tiny_f…ctor_model-shard1:1 
 Failed to load resource: the server responded with a status of 404 ()
