i have this model 
 public partial class AppDetails
 {
     public int? Users { get; set; }
     public int? FailedAttempt { get; set; }
 }

this is my controller code 
      public IActionResult Dashboard()
      {
          return View();
      }


      [HttpGet]
      public IActionResult GetCount(string FinYear4 ="")
      {
          try
          {
              string connectionString = GetRFIDConnectionString();


              DateTime startDate = DateTime.Today;
            
              string query = @"  select PunchIn_FailedCount as FailedAttempt, count(PunchIn_FailedCount) as Users from App_FaceVerification_Details where PunchIn_Success =1
and CAST(DateAndTime as Date) = '2025-05-29'
group by PunchIn_FailedCount";

              using (var connection = new SqlConnection(connectionString))
              {
                  var divisionCounts = connection.Query<AppDetails>(query).ToList();
                  return Json(divisionCounts);
              }

          }
          catch (Exception ex)
          {

              Console.WriteLine(ex.Message);
              return StatusCode(500, "An error occurred while processing your request.");
          }
      }

this is my view 

<div class="row">
    <div class="col-sm-12">
        <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;margin-top:3px;">
            <h6 class="text-center overview-heading">
                Punch In Attempts
            </h6>
            <h6 class="text-center overview-heading">
                FY
            </h6>
            <canvas id="barChart4" class="" style="width:800px;height:370px;"></canvas>

            <div id="legendContainer" style="display:flex;flex-wrap:wrap;"></div>
        </fieldset>
    </div>
</div>

i want barchat using chart js and filter using current date
