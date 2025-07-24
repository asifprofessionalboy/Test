
     <div class="container-fluid mb-5">


          <fieldset class="shadow-sm" style="border: 1px solid #bfbebe; padding: 15px 25px; border-radius: 6px;">
 <legend style="width: auto; font-size: 16px; color: #0000FF"><b>Compliance Mis Report</b></legend>

                   <div style="overflow:scroll;height:152px" id="Mis_Grid" runat="server">
    <cc1:DetailsContainer ID="Mis_Records" runat="server" AutoGenerateColumns="False"
        AllowPaging="false" CellPadding="4" GridLines="None" Width="100%" DataMember="" OnRowDataBound=""
        DataKeyNames="V_Code" DataSource="<%# PageRecordsDataSet %>"
        ForeColor="#333333" ShowHeaderWhenEmpty="True"
        
        PageSize="10" PagerSettings-Visible="True" PagerStyle-HorizontalAlign="Center"
        PagerStyle-Wrap="False" HeaderStyle-Font-Size="Smaller" RowStyle-Font-Size="Smaller">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
          

          

            <asp:BoundField DataField="V_Code" HeaderText="Vendor Code" HeaderStyle-Width="100px"
                SortExpression="V_Code" HeaderStyle-HorizontalAlign="Left"
                ItemStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>

              <asp:BoundField DataField="V_NAME" HeaderText="Vendor Name" HeaderStyle-Width="100px"
                SortExpression="V_NAME" HeaderStyle-HorizontalAlign="Left"
                ItemStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>

           
            <asp:BoundField DataField="WO_NO" HeaderText="Workorder No." HeaderStyle-Width="110px"
                SortExpression="WO_NO" HeaderStyle-HorizontalAlign="Left"
                ItemStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField> 

         <%--   similarly all fields--%>

          

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






                </fieldset>
   </div>


      public partial class Compliance_Mis_Report : Classes.basePage
  {
      App_Compliance_MIS_DS dsRecords = new App_Compliance_MIS_DS();
      protected void Page_Load(object sender, EventArgs e)
      {
          Mis_Records.DataSource = PageRecordsDataSet;

          if (!IsPostBack)
          {

              GetRecords(GetFilterCondition(), Mis_Records.PageSize, 10, "");
             Mis_Records.DataBind();

          }
      }

      protected override void SetBaseControls()
      {
          base.SetBaseControls();
          PageRecordsDataSet = dsRecords;
          BLObject = new BL_Compliance_MIS();
      }
      private StringDictionary GetFilterCondition()
      {
          StringDictionary d = null;
          d = new StringDictionary();
          d.Add("V_Code", Session["UserName"].ToString());
          return d;
      }

  
  }
