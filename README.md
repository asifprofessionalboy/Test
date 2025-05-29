<script>
    document.addEventListener("DOMContentLoaded", function () {
        fetch('/YourControllerName/GetCount') // Replace 'YourControllerName' if needed
            .then(response => response.json())
            .then(data => {
                const labels = data.map(item => `Failed Attempts: ${item.failedAttempt}`);
                const users = data.map(item => item.users);

                const ctx = document.getElementById('barChart4').getContext('2d');
                const barChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Users',
                            data: users,
                            backgroundColor: 'rgba(54, 162, 235, 0.7)',
                            borderColor: 'rgba(54, 162, 235, 1)',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'top'
                            },
                            title: {
                                display: true,
                                text: 'Face Verification - Punch In Attempts'
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Number of Users'
                                }
                            },
                            x: {
                                title: {
                                    display: true,
                                    text: 'Failed Attempt Count'
                                }
                            }
                        }
                    }
                });
            })
            .catch(error => {
                console.error('Error fetching chart data:', error);
            });
    });
</script>




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
