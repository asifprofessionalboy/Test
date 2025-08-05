 why button  not showing after clicking 

       <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
       <ContentTemplate>
   <div class="scrollable-grid" style="height: 400px; overflow-y: scroll;">
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


        <asp:BoundField DataField="SlNo" HeaderText="SlNo" Visible="false" />
            
               <asp:TemplateField HeaderText="Sl No.">
                                    <ItemTemplate><%# Container.DataItemIndex + 1 + "."%></ItemTemplate>
                                        <ItemStyle Width="4%" Font-Size="Small" />
                                </asp:TemplateField>

      <%-- <asp:BoundField DataField="ProcessMonth" HeaderText="Month" />
       <asp:BoundField DataField="ProcessYear" HeaderText="Year" />--%>

       <asp:BoundField DataField="vendorcode" HeaderText="Vendor Code"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="V_NAME" HeaderText="Vendor Name"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="workorder" HeaderText="Workorder No."><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="from_date" HeaderText="From Date"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="to_date" HeaderText="To Date"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="NatureOfWork" HeaderText="Nature Of Work"><ItemStyle Font-Size="Small"/></asp:BoundField>


       <asp:BoundField DataField="DepartmentCode" HeaderText="Department Code"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="DepartmentName" HeaderText="Department Name"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="Location" HeaderText="Location"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="RESPONSIBLE_PERSON_OF_THE_CONTRACTOR" HeaderText="Responsible Person"><ItemStyle Font-Size="Small"/></asp:BoundField>


       <asp:BoundField HeaderText="Male Workers" DataField="MALE_NO_OF_MALE_WORKERS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Male SC/ST" DataField="MALE_NOS_OF_SC_ST_WORKERS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Male OBC" DataField="MALE_NOS_OF_OBC_WORKERS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Male Mandays" DataField="MALE_MANDAYS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Male SC/ST Mandays" DataField="MALE_MANDAYS_SC_ST"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Male OBC Mandays" DataField="MALE_MANDAYS_OBC"><ItemStyle Font-Size="Small"/></asp:BoundField>



       <asp:BoundField HeaderText="Female Workers" DataField="FEMALE_NO_OF_FEMALE_WORKERS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Female SC/ST" DataField="FEMALE_NOS_OF_SC_ST_WORKERS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Female OBC" DataField="FEMALE_NOS_OF_OBC_WORKERS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Female Mandays" DataField="FEMALE_MANDAYS"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Female SC/ST Mandays" DataField="FEMALE_MANDAYS_SC_ST"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField HeaderText="Female OBC Mandays" DataField="FEMALE_MANDAYS_OBC"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="UNSKILLED_NOS_OF_WORKERS" HeaderText="Unskilled Workers"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="UNSKILLED_TOTAL_MANDAYS" HeaderText="Unskilled Mandays"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="SEMISKILLED_NOS_OF_WORKERS" HeaderText="Semi-Skilled Workers"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="SEMISKILLED_TOTAL_MANDAYS" HeaderText="Semi-Skilled Mandays"><ItemStyle Font-Size="Small"/></asp:BoundField>


       <asp:BoundField DataField="SKILLED_NOS_OF_WORKERS" HeaderText="Skilled Workers"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="SKILLED_TOTAL_MANDAYS" HeaderText="Skilled Mandays"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="HIGHLYSKILLED_NOS_OF_WORKERS" HeaderText="Highly Skilled Workers"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="HIGHLYSKILLED_TOTAL_MANDAYS" HeaderText="Highly Skilled Mandays"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="Other_NOS_OF_WORKERS" HeaderText="Other Workers"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="Other_TOTAL_MANDAYS" HeaderText="Other Mandays"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="Total_NOS_OF_WORKERS" HeaderText="Total Workers"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="Total_Mandays" HeaderText="Total Mandays"><ItemStyle Font-Size="Small"/></asp:BoundField>



       <asp:BoundField DataField="BasicDA" HeaderText="Basic + DA"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="Allowance" HeaderText="Other Allowance"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="GrossWages" HeaderText="Gross Wages"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="Other_DEDUCTION" HeaderText="Other Deduction"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="NET_WAGES_AMOUNT" HeaderText="Net Wages Amount"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="WagesStatus" HeaderText="Wages Status"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="Payment_date_WAGES" HeaderText="Wages Payment Date"><ItemStyle Font-Size="Small"/></asp:BoundField>



       <asp:BoundField DataField="PF_DEDUCTION" HeaderText="PF Deduction"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="ESI_DEDUCTION" HeaderText="ESI Deduction"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="PFESI_Status" HeaderText="PF/ESI Status"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="PFPaymentDate" HeaderText="PF Payment Date"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="ESIPaymentDate" HeaderText="ESI Payment Date"><ItemStyle Font-Size="Small"/></asp:BoundField>

       <asp:BoundField DataField="Temporary" HeaderText="Temporary"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="Permanent" HeaderText="Permanent"><ItemStyle Font-Size="Small"/></asp:BoundField>
       <asp:BoundField DataField="Recognized" HeaderText="Recognized"><ItemStyle Font-Size="Small"/></asp:BoundField>
   </Columns>

   

    </asp:GridView>

   

    </div>

         
       </ContentTemplate>
   </asp:UpdatePanel>
                 <asp:Button ID="btnExport" runat="server" Text="Export to Excel" 
OnClick="btnExport_Click" CssClass="btn btn-success mt-2"  Visible="false"/> 



if (ds_L1 != null && ds_L1.Tables.Count > 0 && ds_L1.Tables[0].Rows.Count > 0)
 {
     Mis_Grid.DataSource = ds_L1.Tables[0];
     Mis_Grid.DataBind();
     btnExport.Visible = true;

   
     ViewState["MisData"] =ds_L1.Tables [0];
     
     UpdatePanel5.Update();
    
 }
