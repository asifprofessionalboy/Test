                    <div class="scrollable-grid">
    <asp:GridView ID="Mis_Grid" runat="server" CssClass="grid"
        AutoGenerateColumns="False" DataKeyNames="ID"
        OnRowCreated="Mis_Records_RowCreated"
        
        GridLines="None" Width="100%" AllowPaging="false"
        ForeColor="#333333" ShowHeaderWhenEmpty="True"
        PageSize="10"
        PagerSettings-Visible="True"
        PagerStyle-HorizontalAlign="Center"
        PagerStyle-Wrap="False"
        HeaderStyle-Font-Size="Smaller"
        RowStyle-Font-Size="Smaller"
        HeaderStyle-HorizontalAlign="Center"
        RowStyle-HorizontalAlign="Center">
        
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerSettings Mode="Numeric" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" Font-Bold="True" CssClass="pager1" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="False" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />

       
           <Columns>

       <asp:TemplateField HeaderText="ID" SortExpression="ID" Visible="False">
           <ItemTemplate>
               <asp:Label ID="ID" runat="server"></asp:Label>
           </ItemTemplate>
       </asp:TemplateField>


        <asp:BoundField DataField="SlNo" HeaderText="SlNo" />

      <%-- <asp:BoundField DataField="ProcessMonth" HeaderText="Month" />
       <asp:BoundField DataField="ProcessYear" HeaderText="Year" />--%>

       <asp:BoundField DataField="vendorcode" HeaderText="Vendor Code" />
       <asp:BoundField DataField="V_NAME" HeaderText="Vendor Name" />

       <asp:BoundField DataField="workorder" HeaderText="Workorder No." />
       <asp:BoundField DataField="from_date" HeaderText="From Date" />
       <asp:BoundField DataField="to_date" HeaderText="To Date" />
       <asp:BoundField DataField="NatureOfWork" HeaderText="Nature Of Work" />


       <asp:BoundField DataField="DepartmentCode" HeaderText="Department Code" />
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
       <asp:BoundField DataField="Recognized" HeaderText="Recognized" />
   </Columns>

   

    </asp:GridView>

   

</div>


  <style>
      .groupHeader {
          background-color: #dceefb;
          color: #000;
          font-weight: bold;
          text-align: center;
          border: 1px solid #ccc;
      }


       .grid {
      border-collapse: collapse;
      width: 100%;
      font-size: 13px;
      table-layout: auto;
  }

  .grid th, .grid td {
      padding: 6px 10px;
      text-align: center;
      border: 1px solid #ccc;
      vertical-align: middle;
      white-space: nowrap;
  }

  .grid th {
      background-color: #4A90E2;
      color: white;
      position: sticky;
      top: 0;
      z-index: 2;
  }

  .grid tr:nth-child(even) {
      background-color: #f9f9f9;
  }

  .grid tr:hover {
      background-color: #2783ef;
  }

  .scrollable-grid {
      height: 450px;
      overflow: auto;
      border: 1px solid #ccc;
      padding: 5px;
  }


  </style>



    protected void Mis_Records_RowCreated(object sender,
                                        GridViewRowEventArgs e)
  {
      if (e.Row.RowType == DataControlRowType.Header)
      {
          GridViewRow headerRow = new GridViewRow(0, 0, DataControlRowType.Header,
                                                  DataControlRowState.Insert);

          headerRow.Cells.Add(CreateHeaderCell("SlNo", 1));
          //headerRow.Cells.Add(CreateHeaderCell("Process Month/Year", 2));

          headerRow.Cells.Add(CreateHeaderCell("Vendor Details", 2));

          headerRow.Cells.Add(CreateHeaderCell("WorkOrder Details", 4));

          headerRow.Cells.Add(CreateHeaderCell("Department Details", 2));
          headerRow.Cells.Add(CreateHeaderCell("Location", 1));

          headerRow.Cells.Add(CreateHeaderCell("Responsible", 1));

          headerRow.Cells.Add(CreateHeaderCell("Male", 6));

          headerRow.Cells.Add(CreateHeaderCell("Female", 6));

          headerRow.Cells.Add(CreateHeaderCell("UnSkilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Semi Skilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Skilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Highly Skilled", 2));
          headerRow.Cells.Add(CreateHeaderCell("Others", 2));
          headerRow.Cells.Add(CreateHeaderCell("Total", 2));
          headerRow.Cells.Add(CreateHeaderCell("Wages", 7));
          headerRow.Cells.Add(CreateHeaderCell("PF/ESI", 5));
          headerRow.Cells.Add(CreateHeaderCell("Nil & Recognised", 5));

          Mis_Grid.Controls[0].Controls.AddAt(0, headerRow);
      }
  }

  private TableHeaderCell CreateHeaderCell(string text, int colSpan)
  {
      TableHeaderCell cell =
          new TableHeaderCell
          {
              Text = text,
              ColumnSpan = colSpan,
              HorizontalAlign = HorizontalAlign.Center,
              CssClass = "groupHeader"
          };
      return cell;
  }
