<div style="height: 400px; overflow-y: scroll; border: 1px solid #ccc;">
    <asp:GridView ID="Mis_Grid" runat="server"
                  AutoGenerateColumns="False"
                  CssClass="fixed-header-grid"
                  OnRowCreated="Mis_Records_RowCreated">
        <!-- Your columns here -->
    </asp:GridView>
</div>


<style>
    .fixed-header-grid {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0;
    }

    .fixed-header-grid th {
        position: sticky;
        top: 0;
        background-color: #f9f9f9;
        z-index: 2;
        border-bottom: 2px solid #ddd;
    }

    .groupHeader {
        background-color: #e1e1e1;
        font-weight: bold;
    }
</style>
