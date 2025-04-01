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
