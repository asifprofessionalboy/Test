 document.addEventListener("DOMContentLoaded", function () {

            var chartval = document.getElementById("MainContent_ESI_Radialchartdata").value;

            var originalValues = chartval.split(',').map(Number);
            //var originalValues = str.split(',').map(Number);



            //var originalValues = [876, 553, 44, 1122];
            var total = originalValues.reduce((acc, val) => acc + val, 0);
            var seriesPercentages = originalValues.map(val => (val / total) * 100);
            var formatNumber = function (num) {
                return new Intl.NumberFormat('en-US').format(num);
            };
            var chartColors = ['#1E90FF', '#FF7F50', '#8A2BE2'];
            var options = {
                series: seriesPercentages,
                chart: {
                    height: 350,
                    type: 'radialBar',
                },
                plotOptions: {
                    radialBar: {
                        dataLabels: {
                            name: {
                                fontSize: '16px',
                            },
                            value: {
                                fontSize: '14px',
                                formatter: function (val, opts) {
                                    var index = opts.seriesIndex;
                                    var value = originalValues[index];
                                    var percent = seriesPercentages[index].toFixed(2);
                                    return formatNumber(value) + ' (' + percent + '%)';
                                }
                            },
                            total: {
                                show: true,
                                label: 'Total',
                                formatter: function () {
                                    return formatNumber(total);
                                }
                            }
                        }
                    }
                },
                tooltip: {
                    enabled: true,
                    custom: function ({ seriesIndex, w }) {
                        var label = w.globals.labels[seriesIndex];
                        var value = originalValues[seriesIndex];
                        var percent = seriesPercentages[seriesIndex].toFixed(2);
                        var color = chartColors[seriesIndex];

                        return `
                          <div class="apex-tooltip-custom">
                              <div class="label">
                                  <span class="dot" style="background:${color}"></span>${label}
                              </div>
                              <div class="value">Value: ${formatNumber(value)}</div>
                              <div class="value">Percent: ${percent}%</div>
                          </div>
                        `;
                    }
                },
                labels: ['Between 1-15', 'Between 15-25', 'After 25'],
                colors: chartColors,


            };
            var chart = new ApexCharts(document.querySelector("#ESIchart"), options);
            chart.render();
        });



              <div id="ESIchart_Div" runat="server"  class="col-sm-3">
                  <h6 class="overview-heading">ESI Radial Chart </h6>
                <table>
        <tr>
            <td>
                <div id="ESIchart" style="width:200px; height:120px;"></div>
            </td>
            <td>
                <ul>
                    
                    <li style="list-style-type:none;"><div style="display:inline-block;width:12px;height:12px;background:#1E90FF;"></div>between 1-15 </li>
                     <li style="list-style-type:none;"><div style="display:inline-block;width:12px;height:12px;background:#FF7F50;"></div>between 15-25 </li>
                     <li  style="list-style-type:none;"> <div style="display:inline-block;width:12px;height:12px;background:#8A2BE2; "></div>After 25</li>
                </ul>
            
            </td>
            
        </tr>
    </table>
    <asp:HiddenField ID="ESI_Radialchartdata" runat="server" />
            </div>




i want to set font size of label 10px
