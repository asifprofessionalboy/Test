<script>
    $(document).ready(function () {
        $('#form').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);

            // Get current date and time
            var now = new Date();
            var formattedDateTime = now.toLocaleString(); // Formats the date and time based on the user's locale

            Swal.fire({
                title: "Data Saved Successfully",
                text: "Date & Time: " + formattedDateTime, // Display the current date and time
                width: 600,
                padding: "3em",
                color: "#716add",
                backdrop: `
                    rgba(0,0,123,0.4)
                    left top
                    no-repeat
                `
            }).then((result) => {
                if (result.isConfirmed) {
                    form.off('submit');
                    form.submit();
                }
            });

        });
    });
</script>




<script>
    $(document).ready(function () {
    $('#form').on('submit', function (e) {
        e.preventDefault();
            var form = $(this);
        Swal.fire({
            title: "Data Saved Successfully",
            width: 600,
            padding: "3em",
            color: "#716add",
            backdrop: `
                rgba(0,0,123,0.4)
                left top
                no-repeat
              `
        }).then((result) => {
            if (result.isConfirmed) {
                form.off('submit');
                form.submit();
            }
        });
       
    });
    });
</script>
