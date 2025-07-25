      protected void btnSearch_Click(object sender, EventArgs e)
      {
          string stryear = string.Join(",", Year.Items.Cast<ListItem>()
                                                .Where(i => i.Selected)
                                                .Select(i => i.Value));

          string strmnth = string.Join(",", Month.Items.Cast<ListItem>()
                                                .Where(i => i.Selected)
                                                .Select(i => i.Value));

      
          if (!string.IsNullOrEmpty(stryear) && !string.IsNullOrEmpty(strmnth))
          {
       
              BL_Compliance_MIS blobj = new BL_Compliance_MIS();

        
              string Locationvalue = string.Empty;
              string Dpartmentvalue = string.Empty;


              if (Location.SelectedValue != "")
              {
                  foreach (ListItem item in Location.Items)
                  {
                      if (item.Selected)
                      {
                          Locationvalue += $"'{item.Value}',";
                      }
                  }
                  Locationvalue = Locationvalue.TrimEnd(',');
              }


              if (Department.SelectedValue != "")
              {
                  foreach (ListItem item in Department.Items)
                  {
                      if (item.Selected)
                      {
                          Dpartmentvalue += $"'{item.Value}',";
                      }
                  }
                  Dpartmentvalue = Dpartmentvalue.TrimEnd(',');
              }


              
              string[] yearArray = stryear.Split(',');
              string[] monthArray = strmnth.Split(',');

              foreach (string year in yearArray)
              {
                  foreach (string month in monthArray)
                  {
                      int intMonth = Convert.ToInt32(month);
                      int intYear = Convert.ToInt32(year);

                      int nextMonth = (intMonth == 12) ? 1 : intMonth + 1;
                      int nextYear = (intMonth == 12) ? intYear + 1 : intYear;

                  
                      string monthStr = intMonth.ToString("D2");
                      string nextMonthStr = nextMonth.ToString("D2");

                   
                      string startDate = $"{intYear}-{monthStr}-01";
                      string endDate = new DateTime(nextYear, nextMonth, 1)
                                           .AddTicks(-1)
                                           .ToString("yyyy-MM-dd HH:mm:ss");

                      string locationFilter = "";
                      if (!string.IsNullOrEmpty(Locationvalue))
                      {
                          locationFilter = $" AND WOR.LOC_OF_WORK IN ({Locationvalue}) ";
                      }

                      string departmentFilter = "";
                      if (!string.IsNullOrEmpty(Dpartmentvalue))
                      {
                          departmentFilter =
                              $" AND tab.DepartmentCode IN ({Dpartmentvalue}) ";
                      }

                      string strQuery =$@"";
                     
                  }

                  string connstr = ConfigurationManager.ConnectionStrings["Data Source=10.0.168.50;Initial Catalog=clmsdb;User ID=fs;Password=p@ssW0Rd321;Connect Timeout=3600"].ConnectionString;

                  DataSet ds1 = new DataSet();
                  using (SqlConnection conn = new SqlConnection(connstr))
                  {
                      using (SqlCommand cmd = new SqlCommand(strQuery, conn))
                      {
                          using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                          {
                              da.Fill(ds1);
                          }
                      }
                  }

                  DataSet ds = ds1;

                  if (ds != null && ds.Tables.Count > 0)
                  {
                      Mis_Records.DataSource = ds;
                      Mis_Records.DataBind();
                  }


              }
          }
      }
