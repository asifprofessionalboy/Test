using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class YourPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Bindchart_wagesL1();
        }
    }

    private void Bindchart_wagesL1()
    {
        string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM App_Online_Wages WHERE DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE()) = 1 AND status = 'Pending With L1 Level') AS DaysCount1,
                    (SELECT COUNT(*) FROM App_Online_Wages WHERE DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE()) = 2 AND status = 'Pending With L1 Level') AS DaysCount2,
                    (SELECT COUNT(*) FROM App_Online_Wages WHERE DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE()) = 3 AND status = 'Pending With L1 Level') AS DaysCount3,
                    (SELECT COUNT(*) FROM App_Online_Wages WHERE DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE()) > 3 AND status = 'Pending With L1 Level') AS DaysCountGreater3";

            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    string chartData = $"{dt.Rows[0]["DaysCount1"]},{dt.Rows[0]["DaysCount2"]},{dt.Rows[0]["DaysCount3"]},{dt.Rows[0]["DaysCountGreater3"]}";
                    HiddenChartData.Value = chartData; // Store in HiddenField
                }
            }
        }
    }
}

<asp:HiddenField ID="HiddenChartData" runat="server" />

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels"></script>
<script type="text/javascript">
    Chart.register(ChartDataLabels);

    document.addEventListener("DOMContentLoaded", function () {
        var hiddenField = document.getElementById('<%= HiddenChartData.ClientID %>').value;
        var chartData = hiddenField.split(',').map(Number); // Convert string to array of numbers

        var pieCtx = document.getElementById('pieChart').getContext('2d');
        var pieChart = new Chart(pieCtx, {
            type: 'pie',
            data: {
                labels: ['1 Day', '2 Days', '3 Days', '> 3 Days'],
                datasets: [{
                    data: chartData,
                    backgroundColor: ['#f9b037', '#5a7bf9', '#ed7b8e', '#76c893'],
                    borderColor: ['#f9b037', '#5a7bf9', '#ed7b8e', '#76c893'],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    datalabels: {
                        formatter: (value, context) => {
                            let total = context.chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
                            let percentage = ((value / total) * 100).toFixed(1) + '%';
                            return percentage;
                        },
                        color: '#000',
                        font: { weight: 'bold', size: 12 },
                        anchor: 'center',
                        align: 'center'
                    }
                }
            }
        });
    });
</script>





i have this cs 
        protected void Page_Load(object sender, EventArgs e)
        {
            Bindchart_wagesL1();
        }

        private void Bindchart_wagesL1()
        {
            SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connect"].ConnectionString);
            con.Open();
            string strSQL = string.Empty;
            strSQL = "select (select count(*) as Application_Count1 from App_Online_Wages where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) = 1 and status = 'Pending With L1 Level') as DaysCount1, " +
                     "(select count(*) as Application_Count2 from App_Online_Wages where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) = 2 and status = 'Pending With L1 Level' ) as DaysCount2, (select count(*) as Application_Count3 from App_Online_Wages " +
                     "where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) = 3 and status = 'Pending With L1 Level' ) as DaysCount3, (select count(*) as Application_Count3 from App_Online_Wages where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) > 3 " +
                      " and status = 'Pending With L1 Level' ) as DaysCountGreater3";
            SqlCommand cmd = new SqlCommand(strSQL);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds1 = new DataSet();
            da.Fill(ds1);
            cmd.ExecuteNonQuery();


            con.Close();


      
}

this is my aspx ,

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels"></script>
       <script type="text/javascript">
        Chart.register(ChartDataLabels);

        var pieCtx = document.getElementById('pieChart').getContext('2d');
        var pieChart = new Chart(pieCtx, {
            type: 'pie',
            data: {
                labels: ['Concept Stage', 'Under Trial', 'Implemented Successfully'],
                datasets: [{
                    data: [@ViewBag.totalConcept, @ViewBag.totalTrail, @ViewBag.totalImplementedSuccess],
        backgroundColor: [
            '#f9b037',
            '#5a7bf9',
            '#ed7b8e'
        ],
            borderColor: [
                '#f9b037',
                '#5a7bf9',
                '#ed7b8e'
            ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
                plugins: {
                datalabels: {
                    formatter: (value, context) => {
                        let total = context.chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
                        let percentage = ((value / total) * 100).toFixed(1) + '%';
                        return percentage;
                    },
                        color: '#000',
                            font: {
                        weight: 'bold',
                            size: 12
                    },
                    anchor: 'center',
                        align: 'center'
                }
            }

        }

    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
     <div class="jumbotron mb-0 pb-0" style=" background-color:#dddddd;margin-top:-50px">
        <div class="form-inline row">
                        <div class="form-group col-sm-12 justify-content-center " style="font-size:0.9em">
               <h5 style="font-size:20px;font-family: 'Montserrat';font-weight: 600">PENDING APPLICATION DASHBOARD 2 </h5>
              </div>
          </div>

            <canvas id="pieChart"></canvas>

       
        </div>


    
</asp:Content>


i want to use chart.js in this , how to send the data from cs to js and shows on chart
