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

                 BL_Compliance_MIS blobj = new BL_Compliance_MIS();
                 //string strSql = string.Empty;
                 DataSet ds_L1 = new DataSet();
                 ds_L1 = blobj.Get_Mis_Data(intMonth, intYear, nextMonth, nextYear, monthStr, nextMonthStr, startDate, endDate, locationFilter, departmentFilter);
                 if (ds_L1 != null && ds_L1.Tables.Count > 0 && ds_L1.Tables[0].Rows.Count > 0)
                 {
                     PageRecordDataSet.Merge(ds_L1);
                     Mis_Records.DataSource = PageRecordDataSet;
                     Mis_Records.DataBind();
                     //Mis_Records.BindData();
                     




                 }
                 else
                 {

                     MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Warning, "No Data Found !!!");
                 }


             }



         }
     }
 }



                <div style="overflow-x: scroll; height: 450px" id="Mis_Grid" runat="server">
                    <cc1:DetailsContainer ID="Mis_Records" runat="server" AutoGenerateColumns="False"
                         DataSource="<%# PageRecordDataSet %>" CssClass="table" 
                        AllowPaging="false" CellPadding="4" GridLines="None" Width="100%"
                        ForeColor="#333333" ShowHeaderWhenEmpty="True"
                        PageSize="10" PagerSettings-Visible="True" PagerStyle-HorizontalAlign="Center"
                        PagerStyle-Wrap="False" HeaderStyle-Font-Size="Smaller" RowStyle-Font-Size="Smaller">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <Columns>

                          <%--  <asp:BoundField DataField="SlNo" HeaderText="SlNo" />--%>



                             <asp:BoundField DataField="ProcessMonth" HeaderText="Process Month" />
 <asp:BoundField DataField="ProcessYear" HeaderText="Process Year" />

                            <asp:BoundField DataField="vendorcode" HeaderText="Vendor Code" />
                            <asp:BoundField DataField="V_NAME" HeaderText="Vendor Name" />

                          <%--  <asp:BoundField DataField="workorder" HeaderText="Workorder No." />
                            <asp:BoundField DataField="from_date" HeaderText="From Date" />
                            <asp:BoundField DataField="to_date" HeaderText="To Date" />
                            <asp:BoundField DataField="NatureOfWork" HeaderText="Nature Of Work" />--%>


                          <%--  <asp:BoundField DataField="DepartmentCode" HeaderText="Department Code" />
                            <asp:BoundField DataField="DepartmentName" HeaderText="Department Name" />

                            <asp:BoundField DataField="Location" HeaderText="Location" />

                            <asp:BoundField DataField="RESPONSIBLE_PERSON_OF_THE_CONTRACTOR" HeaderText="Responsible Person" />


                            <asp:BoundField HeaderText="Male Workers" DataField="MALE_NO_OF_MALE_WORKERS" />
                            <asp:BoundField HeaderText="Male SC/ST" DataField="MALE_NOS_OF_SC_ST_WORKERS" />
                            <asp:BoundField HeaderText="Male OBC" DataField="MALE_NOS_OF_OBC_WORKERS" />
                            <asp:BoundField HeaderText="Male Mandays" DataField="MALE_MANDAYS" />
                            <asp:BoundField HeaderText="Male SC/ST Mandays" DataField="MALE_MANDAYS_SC_ST" />
                            <asp:BoundField HeaderText="Male OBC Mandays" DataField="MALE_MANDAYS_OBC" />



                            <asp:BoundField HeaderText="Female Workers" DataField="FEMALE_NO_OF_FEMALE_WORKERS" />
                            <asp:BoundField HeaderText="Female SC/ST" DataField="FEMALE_NOS_OF_SC_ST_WORKERS" />
                            <asp:BoundField HeaderText="Female OBC" DataField="FEMALE_NOS_OF_OBC_WORKERS" />
                            <asp:BoundField HeaderText="Female Mandays" DataField="FEMALE_MANDAYS" />
                            <asp:BoundField HeaderText="Female SC/ST Mandays" DataField="FEMALE_MANDAYS_SC_ST" />
                            <asp:BoundField HeaderText="Female OBC Mandays" DataField="FEMALE_MANDAYS_OBC" />

                            <asp:BoundField DataField="UNSKILLED_NOS_OF_WORKERS" HeaderText="Unskilled Workers" />
                            <asp:BoundField DataField="UNSKILLED_TOTAL_MANDAYS" HeaderText="Unskilled Mandays" />

                            <asp:BoundField DataField="SEMISKILLED_NOS_OF_WORKERS" HeaderText="Semi-Skilled Workers" />
                            <asp:BoundField DataField="SEMISKILLED_TOTAL_MANDAYS" HeaderText="Semi-Skilled Mandays" />


                            <asp:BoundField DataField="SKILLED_NOS_OF_WORKERS" HeaderText="Skilled Workers" />
                            <asp:BoundField DataField="SKILLED_TOTAL_MANDAYS" HeaderText="Skilled Mandays" />
                            <asp:BoundField DataField="HIGHLYSKILLED_NOS_OF_WORKERS" HeaderText="Highly Skilled Workers" />
                            <asp:BoundField DataField="HIGHLYSKILLED_TOTAL_MANDAYS" HeaderText="Highly Skilled Mandays" />
                            <asp:BoundField DataField="Other_NOS_OF_WORKERS" HeaderText="Other Workers" />
                            <asp:BoundField DataField="Other_TOTAL_MANDAYS" HeaderText="Other Mandays" />

                            <asp:BoundField DataField="Total_NOS_OF_WORKERS" HeaderText="Total Workers" />
                            <asp:BoundField DataField="Total_Mandays" HeaderText="Total Mandays" />



                            <asp:BoundField DataField="BasicDA" HeaderText="Basic + DA" />
                            <asp:BoundField DataField="Allowance" HeaderText="Other Allowance" />
                            <asp:BoundField DataField="GrossWages" HeaderText="Gross Wages" />

                            <asp:BoundField DataField="Other_DEDUCTION" HeaderText="Other Deduction" />
                            <asp:BoundField DataField="NET_WAGES_AMOUNT" HeaderText="Net Wages Amount" />
                            <asp:BoundField DataField="WagesStatus" HeaderText="Wages Status" />
                            <asp:BoundField DataField="Payment_date_WAGES" HeaderText="Wages Payment Date" />



                            <asp:BoundField DataField="PF_DEDUCTION" HeaderText="PF Deduction" />
                            <asp:BoundField DataField="ESI_DEDUCTION" HeaderText="ESI Deduction" />
                            <asp:BoundField DataField="PFESI_Status" HeaderText="PF/ESI Status" />
                            <asp:BoundField DataField="PFPaymentDate" HeaderText="PF Payment Date" />
                            <asp:BoundField DataField="ESIPaymentDate" HeaderText="ESI Payment Date" />

                            <asp:BoundField DataField="Temporary" HeaderText="Temporary" />
                            <asp:BoundField DataField="Permanent" HeaderText="Permanent" />
                            <asp:BoundField DataField="Recognized" HeaderText="Recognized" />--%>

                        </Columns>
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerSettings Mode="Numeric" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" Font-Bold="True" CssClass="pager1" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="False" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                    </cc1:DetailsContainer>

                </div>
