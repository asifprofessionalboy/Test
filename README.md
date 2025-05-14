    <script>
    document.getElementById("form").addEventListener("submit", function (e) {
        var hh = document.getElementById("IntimeHH").value.padStart(2, '0');
        var mm = document.getElementById("IntimeMM").value.padStart(2, '0');
        var hh2 = document.getElementById("OutTimeHH").value.padStart(2, '0');
        var mm2 = document.getElementById("OutTimeMM").value.padStart(2, '0');
        if (hh && mm) {
            document.getElementById("Intime").value = hh + ":" + mm;
        }
         if (hh2 && mm2) {
            document.getElementById("OutTime").value = hh + ":" + mm;
        }
    });
</script>





                    <div class="col-sm-2">
    <div class="row">
        <div class="col-sm-6">
            <input class="form-control form-control-sm" id="IntimeHH" type="text" placeholder="(HH)">
        </div>
        <div class="col-sm-6">
            <input class="form-control form-control-sm" id="IntimeMM" type="text" placeholder="(mm)">
        </div>
    </div>

   
    <input asp-for="Intime" type="hidden" id="Intime" />
</div>
           </div>
           <div class="row form-group">
               <div class="col-sm-1">
                        <label class="control-label">Out Time:</label>
                    </div>

                    <div class="col-sm-2">

                        <div class="row">
                            <div class="col-sm-6">
                                <input class="form-control form-control-sm" id="OutTimeHH" value="" type="text" placeholder="(HH)">

                                </div>
                                <div class="col-sm-6">

                                     <input class="form-control form-control-sm" id="OutTimeMM" value="" type="text"  placeholder="(mm)">
                                </div>
                            
                        <input asp-for="OutTime" type="hidden" id="OutTime" />
                            </div>

                        
                    </div>

is this correct logic 
