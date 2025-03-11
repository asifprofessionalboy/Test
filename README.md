<script>
    $(document).ready(function () {
        $('#form').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);
            var formData = form.serialize(); // Serialize form data

            $.ajax({
                type: form.attr('method'),
                url: form.attr('action'),
                data: formData,
                success: function (response) {
                    if (response.success) {
                        var now = new Date();
                        var formattedDateTime = now.toLocaleString();

                        Swal.fire({
                            title: "Success!",
                            text: response.message + "\nDate & Time: " + formattedDateTime,
                            icon: "success",
                            width: 600,
                            padding: "3em",
                            color: "#716add",
                            backdrop: `rgba(0,0,123,0.4) left top no-repeat`
                        }).then(() => {
                            window.location.href = "/Geo/GeoFencing"; // Redirect after success
                        });
                    } else {
                        Swal.fire({
                            title: "Error!",
                            text: response.message,
                            icon: "error"
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        title: "Error!",
                        text: "An error occurred while saving data.",
                        icon: "error"
                    });
                }
            });
        });
    });

    function setEntryType(type) {
        document.getElementById("EntryType").value = type;
    }
</script>

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



this is my button and js for success message 
<form asp-action="AttendanceData" id="form" asp-controller="Geo" method="post">
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


<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
<script>
    $(document).ready(function () {
        $('#form').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);

           
            var now = new Date();
            var formattedDateTime = now.toLocaleString(); 

            Swal.fire({
                title: "Data Saved Successfully",
                text: "Date & Time: " + formattedDateTime, 
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
    function setEntryType(type) {
        document.getElementById("EntryType").value = type;
    }

</script>

and this is my method 
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

         return RedirectToAction("GeoFencing");
     }
     catch (Exception ex)
     {
         return Json(new { success = false, message = ex.Message });
     }

 }

 public void StoreData(string ddMMyy, string tmIn, string tmOut, string Pno)
 {
     using (var connection = new SqlConnection(configuration.GetConnectionString("RFID")))
     {
         connection.Open();

         if (!string.IsNullOrEmpty(tmIn))
         {
             var query = @"
         INSERT INTO T_TRBDGDAT_EARS(TRBDGDA_BD_DATE, TRBDGDA_BD_TIME, TRBDGDA_BD_INOUT, TRBDGDA_BD_READER,
         TRBDGDA_BD_CHKHS, TRBDGDA_BD_SUBAREA, TRBDGDA_BD_PNO) 
         VALUES 
         (@TRBDGDA_BD_DATE,
         @TRBDGDA_BD_TIME, 
         @TRBDGDA_BD_INOUT,
         @TRBDGDA_BD_READER, 
         @TRBDGDA_BD_CHKHS, 
         @TRBDGDA_BD_SUBAREA, 
         @TRBDGDA_BD_PNO)";

             var parameters = new
             {
                 TRBDGDA_BD_DATE = ddMMyy,
                 TRBDGDA_BD_TIME = ConvertTimeToMinutes(tmIn),
                 TRBDGDA_BD_INOUT = "I",
                 TRBDGDA_BD_READER = "2",
                 TRBDGDA_BD_CHKHS = "2",
                 TRBDGDA_BD_SUBAREA = "JUSC12",
                 TRBDGDA_BD_PNO = Pno
             };

             connection.Execute(query, parameters);

             var Punchquery = @"
         INSERT INTO T_TRPUNCHDATA_EARS(PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,
         PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO) 
         VALUES 
         (@PDE_PUNCHDATE,
         @PDE_PUNCHTIME, 
         @PDE_INOUT,
         @PDE_MACHINEID, 
         @PDE_READERNO, 
         @PDE_CHKHS, 
         @PDE_SUBAREA, 
         @PDE_PSRNO)";

             var parameters2 = new
             {
                 PDE_PUNCHDATE = ddMMyy,
                 PDE_PUNCHTIME = tmIn,
                 PDE_INOUT = "I",
                 PDE_MACHINEID = "2",
                 PDE_READERNO = "2",
                 PDE_CHKHS = "2",
                 PDE_SUBAREA = "JUSC12",
                 PDE_PSRNO = Pno
             };

             connection.Execute(Punchquery, parameters2);
         }

         if (!string.IsNullOrEmpty(tmOut))
         {
             var queryOut = @"
         INSERT INTO T_TRBDGDAT_EARS(TRBDGDA_BD_DATE, TRBDGDA_BD_TIME, TRBDGDA_BD_INOUT, TRBDGDA_BD_READER, 
          TRBDGDA_BD_CHKHS, TRBDGDA_BD_SUBAREA, TRBDGDA_BD_PNO) 
         VALUES 
         (@TRBDGDA_BD_DATE,
         @TRBDGDA_BD_TIME, 
         @TRBDGDA_BD_INOUT, 
         @TRBDGDA_BD_READER, 
         @TRBDGDA_BD_CHKHS,
         @TRBDGDA_BD_SUBAREA,
         @TRBDGDA_BD_PNO)";

             var parametersOut = new
             {
                 TRBDGDA_BD_DATE = ddMMyy,
                 TRBDGDA_BD_TIME = ConvertTimeToMinutes(tmOut),
                 TRBDGDA_BD_INOUT = "O",
                 TRBDGDA_BD_READER = "2",
                 TRBDGDA_BD_CHKHS = "2",
                 TRBDGDA_BD_SUBAREA = "JUSC12",
                 TRBDGDA_BD_PNO = Pno
             };

             connection.Execute(queryOut, parametersOut);

             var Punchquery = @"
         INSERT INTO T_TRPUNCHDATA_EARS(PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,
         PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO) 
         VALUES 
         (@PDE_PUNCHDATE,
         @PDE_PUNCHTIME, 
         @PDE_INOUT,
         @PDE_MACHINEID, 
         @PDE_READERNO, 
         @PDE_CHKHS, 
         @PDE_SUBAREA, 
         @PDE_PSRNO)";

             var parameters2 = new
             {
                 PDE_PUNCHDATE = ddMMyy,
                 PDE_PUNCHTIME = tmOut,
                 PDE_INOUT = "O",
                 PDE_MACHINEID = "2",
                 PDE_READERNO = "2",
                 PDE_CHKHS = "2",
                 PDE_SUBAREA = "JUSC12",
                 PDE_PSRNO = Pno
             };

             connection.Execute(Punchquery, parameters2);
         }
     }
 }

in this i want that when data is saved successfully then shows success message, it shows when button is click, but i want when data is properly and successfully stored then shows alert
