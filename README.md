i have this post method 
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
