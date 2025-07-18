function captureImageAndShow(entryType) {
    if (entryType === "PunchIn") {
        const canvas = document.createElement("canvas");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        const ctx = canvas.getContext("2d");

        // Unmirror the image
        ctx.translate(canvas.width, 0);
        ctx.scale(-1, 1);
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageDataUrl = canvas.toDataURL("image/jpeg");

        // Hide video, show captured image
        video.style.display = "none";
        capturedImage.src = imageDataUrl;
        capturedImage.style.display = "block";

        // Send image only for Punch In
        window.captureImageAndSubmit(entryType, imageDataUrl);
    } else {
        // For Punch Out, just send the request without image
        window.captureImageAndSubmit(entryType, null);
    }
}

window.captureImageAndSubmit = async function (entryType, imageData) {
    EntryTypeInput.value = entryType;

    Swal.fire({
        title: "Please wait...",
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => Swal.showLoading()
    });

    fetch("/AS/Geo/AttendanceData", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Type: entryType, ImageData: imageData })
    })
        .then(res => res.json())
        .then(data => {
            const now = new Date().toLocaleString();
            if (data.success) {
                Swal.fire({
                    title: "Thank you!",
                    text: `Attendance Recorded.\nDate & Time: ${now}`,
                    icon: "success",
                    timer: 3000,
                    showConfirmButton: false
                }).then(() => location.reload());
            } else {
                Swal.fire({
                    title: "Face Recognized, But Error!",
                    text: `Server didn't accept attendance.\nDate & Time: ${now}`,
                    icon: "error"
                });
            }
        })
        .catch(error => {
            console.error("Error:", error);
            Swal.fire("Error!", "An error occurred while processing your request.", "error");
        });
};




<script>
let blinkCount = 0;
let lastBlinkTime = 0;
let faceMatched = false;
let detectionActive = true;

const expectedUserId = "151514"; // Change based on actual user
const userLabel = "151514"; // label used in LabeledFaceDescriptors
let faceMatcher = null;
let labeledDescriptors = [];

window.addEventListener("DOMContentLoaded", async () => {
    const video = document.getElementById("video");
    const capturedImage = document.getElementById("capturedImage");
    const EntryTypeInput = document.getElementById("EntryType");
    const statusText = document.getElementById("statusText");

    await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri("/models"),
        faceapi.nets.faceLandmark68Net.loadFromUri("/models"),
        faceapi.nets.faceRecognitionNet.loadFromUri("/models"),
        faceapi.nets.ssdMobilenetv1.loadFromUri("/models")
    ]);

    const stream = await navigator.mediaDevices.getUserMedia({ video: true });
    video.srcObject = stream;

    const storedDescriptors = [
        await loadStoredFaceDescriptor("/AS/Images/151514-Captured.jpg"),
        await loadStoredFaceDescriptor("/AS/Images/151514-Shashi Kumar.jpg")
    ].filter(d => d !== null);

    labeledDescriptors = [
        new faceapi.LabeledFaceDescriptors(userLabel, storedDescriptors)
    ];

    faceMatcher = new faceapi.FaceMatcher(labeledDescriptors, 0.35); // Threshold

    video.addEventListener("play", () => {
        const canvas = faceapi.createCanvasFromMedia(video);
        document.body.append(canvas);
        const displaySize = { width: video.width, height: video.height };
        faceapi.matchDimensions(canvas, displaySize);

        setInterval(async () => {
            if (!detectionActive) return;

            const detection = await faceapi
                .detectSingleFace(video, new faceapi.SsdMobilenetv1Options())
                .withFaceLandmarks()
                .withFaceDescriptor();

            if (!detection) return;

            const landmarks = detection.landmarks;
            const leftEAR = getEAR(landmarks.getLeftEye());
            const rightEAR = getEAR(landmarks.getRightEye());
            const ear = (leftEAR + rightEAR) / 2;

            const blinkThreshold = 0.23;
            const now = performance.now();

            if (ear < blinkThreshold && now - lastBlinkTime > 300) {
                blinkCount++;
                lastBlinkTime = now;
            }

            if (blinkCount >= 2) {
                detectionActive = false;
                blinkCount = 0;

                const bestMatch = faceMatcher.findBestMatch(detection.descriptor);
                if (bestMatch.label === userLabel) {
                    alert("Face matched successfully!");
                    captureImageAndShow("Punch In"); // or "Punch Out"
                } else {
                    alert("Face not matched. Retrying in 10 seconds.");
                    startRetryCountdown();
                }
            }
        }, 300);
    });

    function getEAR(eye) {
        const a = distance(eye[1], eye[5]);
        const b = distance(eye[2], eye[4]);
        const c = distance(eye[0], eye[3]);
        return (a + b) / (2.0 * c);
    }

    function distance(p1, p2) {
        return Math.hypot(p2.x - p1.x, p2.y - p1.y);
    }

    async function loadStoredFaceDescriptor(imageUrl) {
        try {
            const img = await faceapi.fetchImage(imageUrl);
            const detection = await faceapi
                .detectSingleFace(img)
                .withFaceLandmarks()
                .withFaceDescriptor();

            if (!detection) {
                alert("No face found in stored image: " + imageUrl);
                return null;
            }
            return detection.descriptor;
        } catch (e) {
            alert("Error loading stored descriptor: " + imageUrl);
            return null;
        }
    }

    function captureImageAndShow(entryType) {
        const canvas = document.createElement("canvas");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        const ctx = canvas.getContext("2d");
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageDataUrl = canvas.toDataURL("image/jpeg");

        video.style.display = "none";
        capturedImage.src = imageDataUrl;
        capturedImage.style.display = "block";

        window.captureImageAndSubmit(entryType, imageDataUrl);
    }

    function startRetryCountdown() {
        setTimeout(() => {
            detectionActive = true;
        }, 10000); // 10 seconds
    }

    // Your existing POST function (minor edit to accept image param)
    window.captureImageAndSubmit = async function (entryType, imageData) {
        EntryTypeInput.value = entryType;

        Swal.fire({
            title: "Please wait...",
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => Swal.showLoading()
        });

        fetch("/AS/Geo/AttendanceData", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Type: entryType, ImageData: imageData })
        })
        .then(res => res.json())
        .then(data => {
            const now = new Date().toLocaleString();
            if (data.success) {
                Swal.fire({
                    title: "Thank you!",
                    text: `Attendance Recorded.\nDate & Time: ${now}`,
                    icon: "success",
                    timer: 3000,
                    showConfirmButton: false
                }).then(() => location.reload());
            } else {
                Swal.fire({
                    title: "Face Recognized, But Error!",
                    text: `Server didn't accept attendance.\nDate & Time: ${now}`,
                    icon: "error"
                });
            }
        })
        .catch(error => {
            Swal.fire("Error!", "An error occurred while processing your request.", "error");
        });
    };
});
</script>

function startRetryCountdown() {
    // Hide image and show video again after 10 seconds
    setTimeout(() => {
        capturedImage.style.display = "none";
        video.style.display = "block";
        detectionActive = true;
    }, 10000); // 10 seconds
}




function captureImageAndShow(entryType) {
    const video = document.getElementById("video");
    const canvas = document.createElement("canvas");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    const ctx = canvas.getContext("2d");
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

    const imageDataUrl = canvas.toDataURL("image/jpeg");

    // Hide video, show captured image
    video.style.display = "none";
    const img = document.getElementById("capturedImage");
    img.src = imageDataUrl;
    img.style.display = "block";

    // Call your existing method
    window.captureImageAndSubmit(entryType);
}




i



have this post method 
    window.captureImageAndSubmit = async function (entryType) {
      
        EntryTypeInput.value = entryType;
        const imageData = capturedImage.src;

        Swal.fire({
            title: "Please wait...",
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => Swal.showLoading()
        });

        
        fetch("/AS/Geo/AttendanceData", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Type: entryType, ImageData: imageData})
        })
            .then(res => res.json())
            .then(data => {
                const now = new Date().toLocaleString();
                if (data.success) {
                    Swal.fire({
                        title: "Thank you!",
                        text: `Attendance Recorded.\nDate & Time: ${now}`,
                        icon: "success",
                        timer: 3000,
                        showConfirmButton: false
                    }).then(() => location.reload());
                } else {
                    Swal.fire({
                        title: "Face Recognized, But Error!",
                        text: `Server didn't accept attendance.\nDate & Time: ${now}`,
                        icon: "error"
                    });
                }
            })
            .catch(error => {
                console.error("Error:", error);
                Swal.fire("Error!", "An error occurred while processing your request.", "error");
            });
    };
});
