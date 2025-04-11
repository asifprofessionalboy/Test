    <script type="text/javascript">

        Chart.register(ChartDataLabels);
        document.addEventListener("DOMContentLoaded", function () {
            var hiddenField1 = document.getElementById('<%= HiddenChartData1.ClientID %>').value;
            var chartData1 = hiddenField1.split(',').map(Number); // Convert string to array of numbers
            var hoverchart = document.getElementById('<%= HiddenChartDaysCount.ClientID %>').value;
           
            var labels = ['MaleCount', 'FemaleCount', 'TransCount'];
            var filteredData = [];
            var filteredLabels = [];
            var filteredColors = [];
            var filteredhover = [];
            var hover = hoverchart.split(',');
            var colors = ['#48C9B0', '#ffa600', '#ff0000'];

            for (var i = 0; i < chartData1.length; i++) {
                if (chartData1[i] !== 0) {
                    filteredData.push(chartData1[i]);
                    filteredLabels.push(labels[i]);
                    filteredColors.push(colors[i]);
                    filteredhover.push(hover[i]);

                }
            }

            var pieCtx1 = document.getElementById('pieChart1').getContext('2d');
            var pieChart1 = new Chart(pieCtx1, {
                type: 'pie',
                data: {
                    labels: filteredLabels,
                    datasets: [{
                        data: filteredData,
                        backgroundColor: filteredColors,
                        borderColor: filteredColors,
                        borderWidth: 1,
                        hh: filteredhover
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    layout: {
                        padding: 10,


                    },
                    plugins: {
                        legend: {
                            display: true,
                            position: 'right',
                            labels: {
                                boxWidth: 10,
                                padding: 5

                            }
                        },
                        //tooltip: {
                        //    callbacks: {
                        //        label: function (tooltipItem) {

                        //            let dataset = tooltipItem.dataset.data;

                        //            let hoverdatas = tooltipItem.dataset.hh;

                        //            let index = tooltipItem.dataIndex;
                        //            let hovervalues = hover[index] ? hover[index].split(',') : [];
                        //            let day1 = hovervalues.length > 0 ? hovervalues[0] : "0";
                        //            let day2 = hovervalues.length > 1 ? hovervalues[1] : "0";
                        //            let day3 = hovervalues.length > 2 ? hovervalues[2] : "0";
                        //            let Greaterthan3Days = hovervalues.length > 3 ? hovervalues[3] : "0";
                        //            return [
                        //                /*tooltipItem.label+":",*/
                        //                " Total : " + tooltipItem.dataset.data[index],
                        //                " Day 1 : " + day1,
                        //                " Day 2 : " + day2,
                        //                " Day 3 : " + day3,
                        //                " Greater than 3 : " + Greaterthan3Days

                        //            ]; // Show number on hover
                        //        }
                        //    },
                        //    titleFont: {
                        //        size: 10
                        //    },
                        //    bodyFont: {
                        //        size: 10,
                        //        FooterFont: {
                        //            size: 10
                        //        }
                        //    },
                        //},
                        datalabels: {
                            formatter: (value, ctx) => {
                                let sum = ctx.dataset.data.reduce((a, b) => a + b, 0);
                                let percentage = ((value / sum) * 100).toFixed(1) + "%";
                                return percentage; // Show percentage inside the pie chart
                            },
                            color: '#000',
                            font: {
                                weight: 'bold'
                            }
                        }
                    }
                },
                plugins: [ChartDataLabels] // Make sure to include ChartDataLabels

            });

            ////pieCtx1.canvas.onclick = function (evt) {
            ////    var activePoints = pieChart1.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);
            ////    if (activePoints.length) {
            ////        var firstPoint = activePoints[0];
            ////        var PieChartStatus = pieChart1.data.labels[firstPoint.index];
            ////        //alert(PieChartStatus);
            ////        //var value = pieChart1.data.datasets[firstPoint.datasetIndex].data[firstPoint.index];
            ////        //alert(value);
            ////       // var url = "PF_ESI_Dashboard_Data.aspx?label=" + encodeURIComponent(label) + "&value=" + encodeURIComponent(value);
            ////        var url = "PF_ESI_Dashboard_Data.aspx?PieChartStatus=" + encodeURIComponent(PieChartStatus);
            ////       /* window.location.href = url; */// Redirect to the new URL

            ////        window.open(url, '_blank');
            ////    }
            ////};

           
        }
       

    </script>
