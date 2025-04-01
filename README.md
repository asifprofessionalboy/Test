public IActionResult GeoFencing()
{
    var session = HttpContext.Request.Cookies["Session"];
    var userName = HttpContext.Request.Cookies["UserName"];

    var pno = session; // Ensure session holds a valid Pno
    var currentDate = DateTime.Now.ToString("yyyy-MM-dd");

    string connectionString = GetRFIDConnectionString();

    string query = @"
        SELECT TRBDGDA_BD_Inout 
        FROM T_TRBDGDAT_EARS 
        WHERE TRBDGDA_BD_PNO = @Pno 
        AND TRBDGDA_BD_DATE = @CurrentDate";

    string inoutValue = "";

    using (var connection = new SqlConnection(connectionString))
    {
        inoutValue = connection.QuerySingleOrDefault<string>(query, new { Pno = pno, CurrentDate = currentDate })?.Trim();
    }

    ViewBag.InOut = inoutValue; // Pass data to View

    var data = GetLocations();
    return View();
}

 <div class="row mt-5 form-group">
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
  
   
   
   public IActionResult GeoFencing()
   {
       var session = HttpContext.Request.Cookies["Session"];
       var UserName = HttpContext.Request.Cookies["UserName"];

       var Pno = session; 
       var currentDate = DateTime.Now.ToString("yyyy-MM-dd");

       string connectionString = GetRFIDConnectionString();

       string query = @"SELECT TRBDGDA_BD_Inout FROM T_TRBDGDAT_EARS WHERE TRBDGDA_BD_PNO = @Pno
                        AND TRBDGDA_BD_DATE = @currentDate";

       using (var connection = new SqlConnection(connectionString))
       {
           var worksiteNamesString = connection.QuerySingleOrDefault<string>(query, new { Pno });

           var parameters = new
           {
               Pno = Pno,
               currentDate = currentDate
           };

           connection.QuerySingleOrDefault(query, parameters);
       }

           var data = GetLocations();
           return View();
       

   }
