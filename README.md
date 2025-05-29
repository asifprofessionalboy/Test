i have this controller code 
      [HttpGet]
      public IActionResult GetCount()
      {
          try
          {
              string connectionString = GetRFIDConnectionString();


              DateTime startDate = DateTime.Today;
            
              string query = @"select PunchIn_FailedCount as FailedAttempt, count(PunchIn_FailedCount) as Users from App_FaceVerification_Details where PunchIn_Success =1
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
check the dapper query is correct or not 

FailedAttempt	Users
0	394
1	56
2	35
3	8
4	9
5	3
6	7
7	5
8	2
11	3
16	1
17	1

using the query using Group by i want to show failed Attempts on x axis and Users count on Y axis . let failed attempt 0 on x axis and 394 on bar like this i want my chartjs

