

<asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <!-- Your GridView and other stuff here -->
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>








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
