<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:GridView ID="Mis_Grid" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
            CssClass="grid" GridLines="None" Width="100%" AllowPaging="true" PageSize="10"
            PagerStyle-HorizontalAlign="Center" PagerStyle-Wrap="False"
            HeaderStyle-Font-Size="Smaller" RowStyle-Font-Size="Smaller">
            
            <Columns>
                <asp:BoundField DataField="ProcessMonth" HeaderText="Process Month" />
                <asp:BoundField DataField="ProcessYear" HeaderText="Process Year" />
                <asp:BoundField DataField="vendorcode" HeaderText="Vendor Code" />
                <asp:BoundField DataField="V_NAME" HeaderText="Vendor Name" />
                <asp:BoundField DataField="workorder" HeaderText="Workorder No." />
                <asp:BoundField DataField="from_date" HeaderText="From Date" />
                <asp:BoundField DataField="to_date" HeaderText="To Date" />
                <!-- Add more BoundFields here as needed -->
            </Columns>

        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>


BL_Compliance_MIS blobj = new BL_Compliance_MIS();
DataSet ds_L1 = blobj.Get_Mis_Data(intMonth, intYear, nextMonth, nextYear, monthStr, nextMonthStr, startDate, endDate, locationFilter, departmentFilter);

if (ds_L1 != null && ds_L1.Tables.Count > 0 && ds_L1.Tables[0].Rows.Count > 0)
{
    Mis_Grid.DataSource = ds_L1.Tables[0];
    Mis_Grid.DataBind();

    UpdatePanel1.Update(); // Only if UpdateMode="Conditional"
}
else
{
    MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Warning, "No Data Found !!!");
}


