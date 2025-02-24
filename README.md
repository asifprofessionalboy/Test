    
<asp:Chart ID="Chart2" runat="server" Height="400px" Width="600px" Visible = "true" >
    <Titles>
        <asp:Title  Name="Items" Text="WAGES FOR LEVEL 2"/>
    </Titles> 
    <Legends>
        <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default" LegendStyle="Row" />
    </Legends>
    <Series>
        <asp:Series Name="Default" ChartType="Pie" IsValueShownAsLabel="true" />
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="ChartArea1" BorderWidth="0" />
      
    </ChartAreas>
    </asp:Chart>



        private void Bindchart_wagesL2()
        {
            SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connect"].ConnectionString);
            con.Open();
            string strSQL = string.Empty;
            strSQL = "select (select count(*) as Application_Count1 from App_Online_Wages where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) = 1 and status = 'Pending With L2 Level') as DaysCount1, " +
                     "(select count(*) as Application_Count2 from App_Online_Wages where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) = 2 and status = 'Pending With L2 Level' ) as DaysCount2, (select count(*) as Application_Count3 from App_Online_Wages " +
                     "where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) = 3 and status = 'Pending With L2 Level' ) as DaysCount3, (select count(*) as Application_Count3 from App_Online_Wages where (DATEDIFF(DAY, ISNULL(ReSubmitedOn, CREATEDON), GETDATE())) > 3  and status = 'Pending With L2 Level' ) as DaysCountGreater3";
            SqlCommand cmd = new SqlCommand(strSQL);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds2 = new DataSet();
            da.Fill(ds2);
            cmd.ExecuteNonQuery();


            Chart2.Series["Default"].Points.Clear();
            
            if (ds2.Tables[0].Rows.Count > 0)
            {

                //Chart2.Series["Default"].Points
                //.Where(p => p.YValues[0] == 0)
                //.ToList()
                //.ForEach(p => p.Label = "");

                DataRow row = ds2.Tables[0].Rows[0];
                DataPoint dp1 = new DataPoint();
                dp1.YValues = new double[] { Convert.ToInt32(row["DaysCount1"]) };
                //dp1.Label = "#VALX - #VALY";
                dp1.LegendText = "1 Pending \n Days - " + row["DaysCount1"].ToString();
                Chart2.Series["Default"].Points.Add(dp1);

                DataPoint dp2 = new DataPoint();
                dp2.YValues = new double[] { Convert.ToInt32(row["DaysCount2"]) };
                //dp2.Label = "#VALX - #VALY";
                dp2.LegendText = "2 Pending \nDays - " + row["DaysCount2"].ToString();
                Chart2.Series["Default"].Points.Add(dp2);

                DataPoint dp3 = new DataPoint();
                dp3.YValues = new double[] { Convert.ToInt32(row["DaysCount3"]) };
                //dp3.Label = "#VALX - #VALY";
                dp3.LegendText = "3 Pending \n  Days - " + row["DaysCount3"].ToString();
                Chart2.Series["Default"].Points.Add(dp3);

                DataPoint dp4 = new DataPoint();
                dp4.YValues = new double[] { Convert.ToInt32(row["DaysCountGreater3"]) };
                //dp4.Label = "#VALX - #VALY";
                dp4.LegendText = "More than 3\n Pending Days - " + row["DaysCountGreater3"].ToString();
                Chart2.Series["Default"].Points.Add(dp4);


                foreach(DataPoint point in Chart2.Series["Default"].Points)
                {
                    if(point.YValues[0]==0)
                    {
                        point.Label = "";
                    }
                    else
                    {
                        point.Label = "#VALY";
                    }

                }

            }

            Chart2.Series["Default"].ChartType = SeriesChartType.Pie;
            Chart2.Series["Default"].IsValueShownAsLabel = true;
            //Chart2.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            //Chart2.Series["Default"]["PieLabelStyle"] = "Outside";
            //Chart2.Series["Default"]["Font"] = "Areal, 8pt";
            Chart2.Legends[0].Enabled = true;

            con.Close();


        }

