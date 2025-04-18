string query = @"
SELECT COUNT(*) 
FROM T_TRBDGDAT_EARS 
WHERE TRBDGDA_BD_PNO = @Pno 
AND TRBDGDA_BD_DATE = @CurrentDate";

int punchCount = 0;

using (var connection = new SqlConnection(connectionString))
{
    punchCount = connection.QuerySingleOrDefault<int>(query, new { Pno = pno, CurrentDate = currentDate });
}

int mod = punchCount % 2;
ViewBag.InOut = mod == 0 ? "I" : "O"; // 0 => Punch In, 1 => Punch Out

<div class="mt-5 form-group">
    <div class="col d-flex justify-content-center mb-4">
        @if (ViewBag.InOut == "I")
        {
            <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                Punch In
            </button>
        }
    </div>

    <div class="col d-flex justify-content-center">
        @if (ViewBag.InOut == "O")
        {
            <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                Punch Out
            </button>
        }
    </div>
</div>



this is my query 
    string query = @"
SELECT count(*)
FROM T_TRBDGDAT_EARS 
WHERE TRBDGDA_BD_PNO = @Pno 
AND TRBDGDA_BD_DATE = @CurrentDate";

    string inoutValue = "";

    using (var connection = new SqlConnection(connectionString))
    {
        inoutValue = connection.QuerySingleOrDefault<string>(query, new { Pno = pno, CurrentDate = currentDate })?.Trim();
    }

    ViewBag.InOut = inoutValue; 


in this the data i want to modulus that if modulus is 0 then shows punchIn and if modulus values is 1 then shows PunchOut

this is my view side 
 <div class="mt-5 form-group">
     <div class="col d-flex justify-content-center mb-4">
         @if (ViewBag.InOut == "O" || string.IsNullOrEmpty(ViewBag.InOut))
         {
             <button type="button" class="Btn" id="PunchIn" onclick="captureImageAndSubmit('Punch In')">
                 Punch In
             </button>
         }
     </div>

     <div class="col d-flex justify-content-center">
         @if (ViewBag.InOut == "I")
         {
             <button type="button" class="Btn2" id="PunchOut" onclick="captureImageAndSubmit('Punch Out')">
                 Punch Out
             </button>
         }
     </div>

 </div>
