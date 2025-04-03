function captureImageAndSubmit(entryType) {
    EntryTypeInput.value = entryType;

    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");

    if (!video || !canvas) {
        Swal.fire({
            title: "Error!",
            text: "Camera not available. Please check your device.",
            icon: "error"
        });
        return;
    }

    const context = canvas.getContext("2d");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    const imageData = canvas.toDataURL("image/jpeg");

    Swal.fire({
        title: "Verifying Face...",
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    const punchIn = document.getElementById("PunchIn");
    const punchOut = document.getElementById("PunchOut");

    if (punchIn) punchIn.disabled = true;
    if (punchOut) punchOut.disabled = true;

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
        Swal.close();

        if (data.success) {
            var now = new Date();
            var formattedDateTime = now.toLocaleString();
            successSound.play();
            triggerHapticFeedback("success");

            Swal.fire({
                title: "Face Matched!",
                text: "Attendance Recorded.\nDate & Time: " + formattedDateTime,
                icon: "success",
                timer: 5000,
                showConfirmButton: false
            }).then(() => {
                location.reload();  // Refresh the page after success
            });
        } else {
            errorSound.play();
            triggerHapticFeedback("error");

            Swal.fire({
                title: "Face Not Recognized!",
                text: "Click the button again to retry.",
                icon: "error",
                confirmButtonText: "Retry"
            }).then(() => {
                location.reload();  // Refresh the page after failure
            });
        }
    })
    .catch(error => {
        Swal.close();
        console.error("Error:", error);
        triggerHapticFeedback("error");

        Swal.fire({
            title: "Error!",
            text: "An error occurred while processing your request.",
            icon: "error"
        }).then(() => {
            location.reload();  // Refresh the page in case of an error
        });
    })
    .finally(() => {
        if (punchIn) punchIn.disabled = false;
        if (punchOut) punchOut.disabled = false;
    });
}

  
  
  
  function captureImageAndSubmit(entryType) {
      EntryTypeInput.value = entryType;

      const context = canvas.getContext("2d");
      canvas.width = video.videoWidth;
      canvas.height = video.videoHeight;
      context.drawImage(video, 0, 0, canvas.width, canvas.height);

      const imageData = canvas.toDataURL("image/jpeg"); // Save as JPG

      
      Swal.fire({
          title: "Verifying Face...",
          allowOutsideClick: false,
          showConfirmButton: false,
          didOpen: () => {
              Swal.showLoading();
          }
      });

     
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
              if (data.success) {
                  var now = new Date();
                  var formattedDateTime = now.toLocaleString();
                  successSound.play();
                  triggerHapticFeedback("success"); 

                  Swal.fire({
                      title: "Face Matched!",
                      text: "Attendance Recorded.\nDate & Time: " + formattedDateTime,
                      icon: "success",
                      timer: 5000,
                      showConfirmButton: false
                  });
              } else {
                  errorSound.play();
                  triggerHapticFeedback("error"); 

                  Swal.fire({
                      title: "Face Not Recognized!",
                      text: "Click the button again to retry.",
                      icon: "error",
                      confirmButtonText: "retry"
                  });
              }
          })
          .catch(error => {
              console.error("Error:", error);
              triggerHapticFeedback("error"); 

              Swal.fire({
                  title: "Error!",
                  text: "An error occurred while processing your request.",
                  icon: "error"
              });
          })
          .finally(() => {
            
              document.getElementById("PunchIn").disabled = false;
              document.getElementById("PunchOut").disabled = false;
          });
  }
