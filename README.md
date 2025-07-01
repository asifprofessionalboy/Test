<style>
  #videoContainer {
    display: inline-block;
    border: 4px solid transparent;
    border-radius: 8px;
    padding: 0;
    line-height: 0;
    position: relative;
  }

  video {
    display: block;
    width: 320px;
    height: 240px;
    transform: scaleX(-1);
  }

  .arrow-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 320px;
    height: 240px;
    pointer-events: none;
    display: flex;
    justify-content: center;
    align-items: center;
    color: #fff;
    font-size: 24px;
    font-weight: bold;
    text-shadow: 1px 1px 2px #000;
  }
</style>

<audio id="successSound" src="https://notificationsounds.com/storage/sounds/files/mp3/eventually-590.mp3"></audio>
<audio id="errorSound" src="https://notificationsounds.com/storage/sounds/files/mp3/glitch-589.mp3"></audio>

<div class="form-group text-center">
  <div id="videoContainer">
    <video id="video" autoplay muted playsinline></video>
    <div class="arrow-overlay" id="arrowText"></div>
  </div>
  <canvas id="canvas" style="display:none;"></canvas>
  <p id="statusText" style="font-weight: bold; margin-top: 10px; color: #444;"></p>
</div>

<input type="hidden" name="Type" id="EntryType" />

<script>
window.addEventListener("DOMContentLoaded", async () => {
  const video = document.getElementById("video");
  const canvas = document.getElementById("canvas");
  const EntryTypeInput = document.getElementById("EntryType");
  const successSound = document.getElementById("successSound");
  const errorSound = document.getElementById("errorSound");
  const statusText = document.getElementById("statusText");
  const videoContainer = document.getElementById("videoContainer");
  const arrowText = document.getElementById("arrowText");

  let blinkCount = 0, headMoved = false, blinked = false;
  let blinkValidUntil = 0, cooldownUntil = 0;
  let blinkCountdownInterval;

  const EAR_THRESHOLD = 0.27;
  const BLINK_GAP = 300;
  const DOUBLE_BLINK_WINDOW = 1500;
  const ALLOW_SUBMIT_DURATION = 10000;
  const HEAD_THRESHOLD = 20;

  let lastEARBelowThresholdTime = 0, firstBlinkTime = 0;
  let initialNosePos = null, movementDirection = "";

  const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

  await Promise.all([
    faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
    faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi')
  ]);

  console.log("Models loaded");
  startVideo();

  function startVideo() {
    navigator.mediaDevices.getUserMedia({
      video: { facingMode: "user", width: { ideal: 640 }, height: { ideal: 480 } }
    })
    .then(stream => {
      video.srcObject = stream;
      video.play();
      video.addEventListener("loadeddata", () => {
        detectLiveness();
      });
    })
    .catch(console.error);
  }

  function getEAR(eye) {
    const a = distance(eye[1], eye[5]);
    const b = distance(eye[2], eye[4]);
    const c = distance(eye[0], eye[3]);
    return (a + b) / (2.0 * c);
  }

  function distance(p1, p2) {
    return Math.hypot(p1.x - p2.x, p1.y - p2.y);
  }

  function getHeadMovement(nose, initialNose) {
    const dx = nose.x - initialNose.x;
    const dy = nose.y - initialNose.y;

    if (Math.abs(dx) > Math.abs(dy)) {
      if (dx > HEAD_THRESHOLD) return "Move Right";
      else if (dx < -HEAD_THRESHOLD) return "Move Left";
    } else {
      if (dy > HEAD_THRESHOLD) return "Move Down";
      else if (dy < -HEAD_THRESHOLD) return "Move Up";
    }
    return "";
  }

  async function detectLiveness() {
    const now = Date.now();

    if (now < cooldownUntil) {
      requestAnimationFrame(detectLiveness);
      return;
    }

    const detection = await faceapi.detectSingleFace(video, detectorOptions).withFaceLandmarks();
    if (detection) {
      const leftEye = detection.landmarks.getLeftEye();
      const rightEye = detection.landmarks.getRightEye();
      const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;
      const nose = detection.landmarks.getNose()[3];

      if (!initialNosePos) {
        initialNosePos = nose;
        movementDirection = "Move head left";
        arrowText.textContent = movementDirection;
        headMoved = false;
      } else {
        const move = getHeadMovement(nose, initialNosePos);
        if (move && !headMoved) {
          headMoved = true;
          arrowText.textContent = "✅ Head Moved";
        }
      }

      if (avgEAR < EAR_THRESHOLD) {
        if (now - lastEARBelowThresholdTime > BLINK_GAP) {
          blinkCount++;
          if (blinkCount === 1) firstBlinkTime = now;

          if (blinkCount === 2 && now - firstBlinkTime <= DOUBLE_BLINK_WINDOW) {
            if (headMoved) {
              blinked = true;
              blinkValidUntil = now + ALLOW_SUBMIT_DURATION;
              cooldownUntil = blinkValidUntil;
              blinkCount = 0;
              arrowText.textContent = "✅ Double Blink + Head Move";
              showGreenBorder();
              startCountdown();
            } else {
              arrowText.textContent = "Now move head";
              blinkCount = 0;
            }
          } else if (blinkCount > 2 || now - firstBlinkTime > DOUBLE_BLINK_WINDOW) {
            blinkCount = 0;
          }

          lastEARBelowThresholdTime = now;
        }
      }

      if (!blinked) {
        statusText.textContent = "Double blink + move head";
        videoContainer.style.borderColor = "red";
      }
    } else {
      statusText.textContent = "No face detected";
      videoContainer.style.borderColor = "gray";
      blinked = false;
      blinkCount = 0;
      initialNosePos = null;
    }

    requestAnimationFrame(detectLiveness);
  }

  function showGreenBorder() {
    videoContainer.style.borderColor = "limegreen";
  }

  function startCountdown() {
    let remaining = ALLOW_SUBMIT_DURATION / 1000;
    statusText.textContent = `Liveness verified! You can proceed (${remaining}s)`;

    clearInterval(blinkCountdownInterval);
    blinkCountdownInterval = setInterval(() => {
      remaining--;
      if (remaining > 0) {
        statusText.textContent = `Proceed (${remaining}s)`;
      } else {
        clearInterval(blinkCountdownInterval);
        blinked = false;
        headMoved = false;
        initialNosePos = null;
        arrowText.textContent = "";
        statusText.textContent = "Please double blink + move head again";
        videoContainer.style.borderColor = "red";
      }
    }, 1000);
  }

  window.captureImageAndSubmit = function (entryType) {
    if (!blinked || Date.now() > blinkValidUntil) {
      videoContainer.style.borderColor = "red";
      statusText.textContent = "Double blink + head move required";
      Swal.fire("Liveness Check Failed", "Double blink + move head to verify.", "warning");
      return;
    }

    blinked = false;
    clearInterval(blinkCountdownInterval);
    videoContainer.style.borderColor = "transparent";
    statusText.textContent = "";
    arrowText.textContent = "";

    EntryTypeInput.value = entryType;

    const ctx = canvas.getContext("2d");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

    const imageData = canvas.toDataURL("image/jpeg");

    Swal.fire({ title: "Verifying Face...", allowOutsideClick: false, showConfirmButton: false, didOpen: () => Swal.showLoading() });

    fetch("/AS/Geo/AttendanceData", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Type: entryType, ImageData: imageData })
    })
    .then(res => res.json())
    .then(data => {
      const now = new Date().toLocaleString();
      if (data.success) {
        successSound.play();
        Swal.fire("Face Matched!", `Attendance Recorded at ${now}`, "success").then(() => location.reload());
      } else {
        errorSound.play();
        Swal.fire("Face Not Recognized", "Please retry", "error");
      }
    })
    .catch(err => {
      console.error("Error:", err);
      errorSound.play();
      Swal.fire("Error!", "Something went wrong", "error");
    });
  };

  function triggerHapticFeedback(type) {
    if ("vibrate" in navigator) {
      navigator.vibrate(type === "success" ? 100 : [200, 100, 200]);
    }
  }
});
</script>




<script>
window.addEventListener("DOMContentLoaded", async () => {
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");
    const statusText = document.getElementById("statusText");
    const videoContainer = document.getElementById("videoContainer");

    let blinkCount = 0;
    let blinked = false;
    let blinkValidUntil = 0;
    let blinkCountdownInterval;
    let lastEARBelowThresholdTime = 0;
    let firstBlinkTime = 0;

    let initialNosePosition = null;
    let headMoved = false;

    const EAR_THRESHOLD = 0.27;
    const BLINK_GAP = 300;
    const DOUBLE_BLINK_WINDOW = 1500;
    const ALLOW_SUBMIT_DURATION = 5000;
    const MOVE_THRESHOLD = 15;

    const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

    await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
        faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi')
    ]);
    console.log("Models loaded");
    startVideo();

    function startVideo() {
        navigator.mediaDevices.getUserMedia({
            video: { facingMode: "user", width: { ideal: 640 }, height: { ideal: 480 } }
        }).then(stream => {
            video.srcObject = stream;
            video.play();
            video.addEventListener("loadeddata", () => {
                const checkReady = setInterval(() => {
                    if (video.videoWidth > 0 && video.videoHeight > 0) {
                        clearInterval(checkReady);
                        detectBlinkAndMovement();
                    }
                }, 100);
            });
        }).catch(console.error);
    }

    function distance(p1, p2) {
        return Math.hypot(p1.x - p2.x, p1.y - p2.y);
    }

    function getEAR(eye) {
        const a = distance(eye[1], eye[5]);
        const b = distance(eye[2], eye[4]);
        const c = distance(eye[0], eye[3]);
        return (a + b) / (2.0 * c);
    }

    async function detectBlinkAndMovement() {
        const now = Date.now();

        const detection = await faceapi.detectSingleFace(video, detectorOptions).withFaceLandmarks();
        if (!detection) {
            statusText.textContent = "No face detected";
            videoContainer.style.borderColor = "gray";
            requestAnimationFrame(detectBlinkAndMovement);
            return;
        }

        const landmarks = detection.landmarks;
        const leftEye = landmarks.getLeftEye();
        const rightEye = landmarks.getRightEye();
        const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

        // Track nose movement
        const nose = landmarks.getNose()[3];
        if (!initialNosePosition) {
            initialNosePosition = nose;
        } else {
            const dx = nose.x - initialNosePosition.x;
            const dy = nose.y - initialNosePosition.y;
            if (Math.abs(dx) > MOVE_THRESHOLD || Math.abs(dy) > MOVE_THRESHOLD) {
                headMoved = true;
            }
        }

        // Double blink detection
        if (avgEAR < EAR_THRESHOLD && now - lastEARBelowThresholdTime > BLINK_GAP) {
            blinkCount++;
            if (blinkCount === 1) {
                firstBlinkTime = now;
            }

            if (blinkCount === 2 && now - firstBlinkTime <= DOUBLE_BLINK_WINDOW && headMoved) {
                blinked = true;
                blinkValidUntil = now + ALLOW_SUBMIT_DURATION;
                blinkCount = 0;
                headMoved = false;
                showGreenBorder();
                startCountdown();
            } else if (blinkCount > 2 || now - firstBlinkTime > DOUBLE_BLINK_WINDOW) {
                blinkCount = 0;
            }

            lastEARBelowThresholdTime = now;
        }

        if (!blinked) {
            statusText.textContent = "Please double blink and move head (left/right)";
            videoContainer.style.borderColor = "red";
        }

        if (blinked && now < blinkValidUntil) {
            // still valid time
        } else if (now > blinkValidUntil) {
            blinked = false;
            videoContainer.style.borderColor = "red";
            statusText.textContent = "Please double blink and move head again";
        }

        requestAnimationFrame(detectBlinkAndMovement);
    }

    function showGreenBorder() {
        videoContainer.style.borderColor = "limegreen";
    }

    function startCountdown() {
        let remaining = ALLOW_SUBMIT_DURATION / 1000;
        statusText.textContent = `Double blink & head moved! You can proceed. (${remaining}s)`;
        clearInterval(blinkCountdownInterval);
        blinkCountdownInterval = setInterval(() => {
            remaining--;
            if (remaining > 0) {
                statusText.textContent = `You can proceed. (${remaining}s)`;
            } else {
                clearInterval(blinkCountdownInterval);
                blinked = false;
                videoContainer.style.borderColor = "red";
                statusText.textContent = "Double blink & move head again";
            }
        }, 1000);
    }

    window.captureImageAndSubmit = function (entryType) {
        if (!blinked || Date.now() > blinkValidUntil) {
            videoContainer.style.borderColor = "red";
            statusText.textContent = "Double blink & head move required before submitting";
            Swal.fire({
                title: "Liveness Check Failed",
                text: "Please double blink and move head to verify.",
                icon: "warning"
            });
            return;
        }

        blinked = false;
        clearInterval(blinkCountdownInterval);
        statusText.textContent = "";
        videoContainer.style.borderColor = "transparent";

        EntryTypeInput.value = entryType;

        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        const imageData = canvas.toDataURL("image/jpeg");

        Swal.fire({ title: "Verifying Face...", allowOutsideClick: false, showConfirmButton: false, didOpen: () => Swal.showLoading() });

        fetch("/AS/Geo/AttendanceData", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Type: entryType, ImageData: imageData })
        })
        .then(res => res.json())
        .then(data => {
            const now = new Date().toLocaleString();
            if (data.success) {
                successSound.play();
                Swal.fire({ title: "Face Matched!", text: `Attendance Recorded.\nDate & Time: ${now}`, icon: "success", timer: 3000, showConfirmButton: false }).then(() => location.reload());
            } else {
                errorSound.play();
                Swal.fire({ title: "Face Not Recognized.", text: `Try again.\nDate & Time: ${now}`, icon: "error" });
            }
        })
        .catch(error => {
            console.error("Error:", error);
            Swal.fire("Error!", "An error occurred while processing your request.", "error");
        });
    };
});
</script>




<script>
window.addEventListener("DOMContentLoaded", async () => {
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");
    const successSound = document.getElementById("successSound");
    const errorSound = document.getElementById("errorSound");
    const statusText = document.getElementById("statusText");
    const videoContainer = document.getElementById("videoContainer");

    let blinkCount = 0;
    let blinked = false;
    let blinkValidUntil = 0;
    let blinkCountdownInterval;

    const EAR_THRESHOLD = 0.27;
    const BLINK_GAP = 300; 
    const DOUBLE_BLINK_WINDOW = 1500; 
    const ALLOW_SUBMIT_DURATION = 5000; 

    let lastEARBelowThresholdTime = 0;
    let firstBlinkTime = 0;

    const detectorOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

    await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri('/AS/faceApi'),
        faceapi.nets.faceLandmark68Net.loadFromUri('/AS/faceApi')
    ]);
    console.log("Models loaded");
    startVideo();

    function startVideo() {
        navigator.mediaDevices.getUserMedia({
            video: { facingMode: "user", width: { ideal: 640 }, height: { ideal: 480 } }
        })
        .then(stream => {
            video.srcObject = stream;
            video.play();
            video.addEventListener("loadeddata", () => {
                const checkReady = setInterval(() => {
                    if (video.videoWidth > 0 && video.videoHeight > 0) {
                        clearInterval(checkReady);
                        detectBlink();
                    }
                }, 100);
            });
        })
        .catch(console.error);
    }

    function getEAR(eye) {
        const a = distance(eye[1], eye[5]);
        const b = distance(eye[2], eye[4]);
        const c = distance(eye[0], eye[3]);
        return (a + b) / (2.0 * c);
    }

    function distance(p1, p2) {
        return Math.hypot(p1.x - p2.x, p1.y - p2.y);
    }

    async function detectBlink() {
        const now = Date.now();

        if (blinked && now < blinkValidUntil) {
            requestAnimationFrame(detectBlink);
            return;
        }

        const detection = await faceapi.detectSingleFace(video, detectorOptions).withFaceLandmarks();

        if (detection) {
            const leftEye = detection.landmarks.getLeftEye();
            const rightEye = detection.landmarks.getRightEye();
            const avgEAR = (getEAR(leftEye) + getEAR(rightEye)) / 2.0;

            if (avgEAR < EAR_THRESHOLD) {
                if (now - lastEARBelowThresholdTime > BLINK_GAP) {
                    blinkCount++;

                    if (blinkCount === 1) {
                        firstBlinkTime = now;
                    }

                    if (blinkCount === 2 && now - firstBlinkTime <= DOUBLE_BLINK_WINDOW) {
                        blinked = true;
                        blinkValidUntil = now + ALLOW_SUBMIT_DURATION;
                        blinkCount = 0;
                        showGreenBorder();
                        startCountdown();
                    } else if (blinkCount > 2 || now - firstBlinkTime > DOUBLE_BLINK_WINDOW) {
                        blinkCount = 0;
                    }

                    lastEARBelowThresholdTime = now;
                }
            }

            if (!blinked) {
                statusText.textContent = "Please double blink to verify liveness";
                videoContainer.style.borderColor = "red";
            }
        } else {
            statusText.textContent = "No face detected";
            videoContainer.style.borderColor = "gray";
            blinked = false;
            blinkCount = 0;
        }

        requestAnimationFrame(detectBlink);
    }

    function showGreenBorder() {
        videoContainer.style.borderColor = "limegreen";
    }

    function startCountdown() {
        let remaining = ALLOW_SUBMIT_DURATION / 1000;
        statusText.textContent = `Double blink detected! You can proceed. (${remaining}s)`;

        clearInterval(blinkCountdownInterval);
        blinkCountdownInterval = setInterval(() => {
            remaining--;
            if (remaining > 0) {
                statusText.textContent = `You can proceed. (${remaining}s)`;
            } else {
                clearInterval(blinkCountdownInterval);
                blinked = false;
                videoContainer.style.borderColor = "red";
                statusText.textContent = "Please double blink to verify liveness";
            }
        }, 1000);
    }

    window.captureImageAndSubmit = function (entryType) {
        if (!blinked || Date.now() > blinkValidUntil) {
            videoContainer.style.borderColor = "red";
            statusText.textContent = "Double blink required before submitting";
            Swal.fire({
                title: "Liveness Check Failed",
                text: "Please double blink to verify you're not using a static image.",
                icon: "warning"
            });
            return;
        }

        blinked = false;
        clearInterval(blinkCountdownInterval);
        statusText.textContent = "";
        videoContainer.style.borderColor = "transparent";

        EntryTypeInput.value = entryType;

        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageData = canvas.toDataURL("image/jpeg");

        Swal.fire({
            title: "Verifying Face...",
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
                successSound.play();
                triggerHapticFeedback("success");
                Swal.fire({
                    title: "Face Matched!",
                    text: `Attendance Recorded.\nDate & Time: ${now}`,
                    icon: "success",
                    timer: 3000,
                    showConfirmButton: false
                }).then(() => location.reload());
            } else {
                errorSound.play();
                triggerHapticFeedback("error");
                Swal.fire({
                    title: "Face Not Recognized.",
                    text: `Click the button again to retry.\nDate & Time: ${now}`,
                    icon: "error"
                });
            }
        })
        .catch(error => {
            console.error("Error:", error);
            triggerHapticFeedback("error");
            Swal.fire("Error!", "An error occurred while processing your request.", "error");
        });
    };

    function triggerHapticFeedback(type) {
        if ("vibrate" in navigator) {
            navigator.vibrate(type === "success" ? 100 : [200, 100, 200]);
        }
    }
});
</script>
