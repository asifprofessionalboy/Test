this is my controller method 
 [HttpPost]
 public IActionResult AttendanceData(string EntryType)
 {
     try
     {
         var UserId = HttpContext.Request.Cookies["Session"];

         string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
         string currentTime = DateTime.Now.ToString("HH:mm");
         string Pno = UserId;

         if (EntryType == "Punch In")
         {
             StoreData(currentDate, currentTime, null, Pno);
         }
         else
         {
             StoreData(currentDate, null, currentTime, Pno);
         }

         return Json(new { success = true, message = "Data Saved Successfully" });
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }
 }

and this is my form 

<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post" enctype="multipart/form-data">
    <div class="form-group text-center">
        <video id="video" width="320" height="240" autoplay playsinline></video>
        <canvas id="canvas" style="display: none;"></canvas>
    </div>
    <input type="hidden" name="EntryType" id="EntryType" />

    <div class="row mt-5 form-group" style="margin-top:50%;">
        <div class="col d-flex justify-content-center ">
            <button type="submit" class="Btn form-group" id="PunchIn" onclick="setEntryType('Punch In')">
                Punch In
            </button>
        </div>

        <div class="col d-flex justify-content-center form-group">
            <button type="submit" class="Btn2 form-group" id="PunchOut" onclick="setEntryType('Punch Out')">
                Punch Out
            </button>
        </div>
    </div>
</form>
this is my js 

<script>
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const captureBtn = document.getElementById("captureBtn");
    const photoInput = document.getElementById("photoInput");


    navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
        .then(function (stream) {
            let video = document.querySelector("video");
            video.srcObject = stream;
            video.play();
        })
        .catch(function (error) {
            console.error("Error accessing camera: ", error);
        });

         

    captureBtn.addEventListener("click", () => {
        const context = canvas.getContext("2d");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        photoInput.value = canvas.toDataURL("image/png"); 
    });
</script>

in this i want that when user clicks on punchIn or Punchout then it capture the Image of the user and Verify using OpenCV in my controller method , if it matches then it going inside EntryType Punch In or Punch Out otherwise shows an alert Face doesnot matches
