<style>
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
        background-color: #e6f7ff;
    }

    .scrollable-grid {
        height: 450px;
        overflow: auto;
        border: 1px solid #ccc;
        padding: 5px;
    }
</style>

<div class="scrollable-grid">
    <asp:GridView ID="Mis_Grid" runat="server" CssClass="grid"
        AutoGenerateColumns="False" DataKeyNames="ID"
        OnRowCreated="Mis_Records_RowCreated"
        OnPageIndexChanging="Mis_Grid_PageIndexChanging"
        GridLines="None" Width="100%" AllowPaging="true"
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

        <!-- Your full column list remains unchanged -->
        <!-- Example of first few columns: -->
        <Columns>
            <asp:TemplateField HeaderText="ID" Visible="False">
                <ItemTemplate>
                    <asp:Label ID="ID" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="SlNo" HeaderText="SlNo" />
            <asp:BoundField DataField="ProcessMonth" HeaderText="Process Month" />
            <asp:BoundField DataField="ProcessYear" HeaderText="Process Year" />
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
            
            <!-- Continue with all other BoundFields as you already have -->
        </Columns>
    </asp:GridView>
</div>
