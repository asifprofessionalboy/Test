this is my two buttons 
<div class="row mt-5 form-group">
    <div class="col d-flex justify-content-center mb-4">
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


[Authorize]
public IActionResult GeoFencing()
 {
 var session = HttpContext.Request.Cookies["Session"];
 var UserName = HttpContext.Request.Cookies["UserName"];

           
  var data = GetLocations();
  return View();
  }

and this is my query 
select * from T_TRBDGDAT_EARS where TRBDGDA_BD_PNO = '151514' and TRBDGDA_BD_DATE = '2025-04-01' and TRBDGDA_BD_Inout like '%o

in this i want that , fetch data from this table if current date has value of I in this column TRBDGDA_BD_Inout then shows PunchOut button if TRBDGDA_BD_Inout has O value in column then show PunchIn Button against the Pno
