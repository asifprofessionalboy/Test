<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>
    
    <input type="hidden" name="Type" id="EntryType" />

    <!-- Verifying Face Animation -->
    <div id="verifyingMessage" class="hidden">
        <div class="loader"></div>
        <p>Verifying Face...</p>
    </div>

    <!-- Success Message -->
    <div id="successMessage" class="hidden">
        <div class="checkmark"></div>
        <p>Face Matched! Attendance Recorded.</p>
    </div>

    <!-- Error Message -->
    <div id="errorMessage" class="hidden">
        <div class="crossmark"></div>
        <p>Face Not Recognized! Try Again.</p>
    </div>

    <div class="row mt-5 form-group">
        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                Punch In
            </button>
        </div>

        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                Punch Out
            </button>
        </div>
    </div>
</form>

<style>
    /* Loader animation */
    .hidden {
        display: none;
    }
    #verifyingMessage, #successMessage, #errorMessage {
        text-align: center;
        margin-top: 20px;
        font-size: 18px;
        font-weight: bold;
    }
    .loader {
        border: 6px solid #f3f3f3;
        border-top: 6px solid #3498db;
        border-radius: 50%;
        width: 40px;
        height: 40px;
        animation: spin 1s linear infinite;
        display: inline-block;
    }
    @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
    }

    /* Success Animation */
    .checkmark {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        background-color: #2ecc71;
        position: relative;
        display: inline-block;
    }
    .checkmark:after {
        content: "";
        position: absolute;
        left: 12px;
        top: 18px;
        width: 12px;
        height: 6px;
        border: solid white;
        border-width: 0 0 3px 3px;
        transform: rotate(-45deg);
    }

    /* Error Animation */
    .crossmark {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        background-color: #e74c3c;
        position: relative;
        display: inline-block;
    }
    .crossmark:before, .crossmark:after {
        content: "";
        position: absolute;
        top: 18px;
        left: 10px;
        width: 20px;
        height: 3px;
        background: white;
    }
    .crossmark:before {
        transform: rotate(45deg);
    }
    .crossmark:after {
        transform: rotate(-45deg);
    }
</style>

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");

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

        const imageData = canvas.toDataURL("image/jpeg"); // Save as JPG

        // Hide previous messages
        document.getElementById("successMessage").classList.add("hidden");
        document.getElementById("errorMessage").classList.add("hidden");
        document.getElementById("verifyingMessage").classList.remove("hidden");

        // Disable buttons during verification
        document.getElementById("PunchIn").disabled = true;
        document.getElementById("PunchOut").disabled = true;

        fetch("/GFAS/Geo/AttendanceData", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Type: entryType,
                ImageData: imageData
            })
        })
        .then(response => response.json())
        .then(data => {
            document.getElementById("verifyingMessage").classList.add("hidden"); // Hide verifying message

            if (data.success) {
                document.getElementById("successMessage").classList.remove("hidden"); // Show success message
            } else {
                document.getElementById("errorMessage").classList.remove("hidden"); // Show error message
            }
        })
        .catch(error => {
            console.error("Error:", error);
            document.getElementById("verifyingMessage").classList.add("hidden");
            document.getElementById("errorMessage").classList.remove("hidden");
        })
        .finally(() => {
            // Re-enable buttons
            document.getElementById("PunchIn").disabled = false;
            document.getElementById("PunchOut").disabled = false;
        });
    }
</script>




i have this viewside, in this i want a good animation to show. if verify face is happening then show user that verifying face after face match it shows success accondingly for a face recognition system, i want some good UI 

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>
    <input type="hidden" name="Type" id="EntryType" />

    <div class="row mt-5 form-group">
        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                Punch In
            </button>
        </div>

        <div class="col d-flex justify-content-center">
            <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                Punch Out
            </button>
        </div>
    </div>
</form>


 <script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const EntryTypeInput = document.getElementById("EntryType");

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

        const imageData = canvas.toDataURL("image/jpeg"); // Save as JPG

       

        fetch("/GFAS/Geo/AttendanceData", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Type: entryType,
                ImageData: imageData
            })
        })
        .then(response => response.json())
        .then(data => {
            alert(data.message);
        })
        .catch(error => {
            console.error("Error:", error);
            alert("An error occurred while submitting the image.");
        });
    }
</script>
