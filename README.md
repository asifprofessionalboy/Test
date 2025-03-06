SELECT 
    CONVERT(VARCHAR, PDE_PUNCHDATE, 103) AS PDE_PUNCHDATE, -- 103 = dd/MM/yyyy
    MIN(CASE WHEN PDE_INOUT LIKE '%I%' THEN PDE_PUNCHTIME END) AS PunchInTime,
    MAX(CASE WHEN PDE_INOUT LIKE '%O%' THEN PDE_PUNCHTIME END) AS PunchOutTime
FROM vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS  
WHERE PDE_PSRNO = @PsrNo
GROUP BY PDE_PUNCHDATE 
ORDER BY PDE_PUNCHDATE DESC;



<asp:TemplateField HeaderText="Status" SortExpression="Status" HeaderStyle-Width="3%">
    <ItemTemplate>
        <asp:DropDownList ID="Status" runat="server" CssClass="form-control form-control-sm"  
            Width="100%" Font-Size="X-Small" Font-Bold="true" AutoPostBack="true"
            OnSelectedIndexChanged="Status_SelectedIndexChanged"
            onchange="validateClosingRemarks(this)">
            <asp:ListItem Value="Open">Open</asp:ListItem> 
            <asp:ListItem Value="Close">Close</asp:ListItem> 
        </asp:DropDownList>
    </ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Closing Remarks" SortExpression="Remarks" HeaderStyle-Width="8%">
    <ItemTemplate>
        <asp:TextBox ID="Closing_Remarks" runat="server" CssClass="form-control form-control-sm"  
            Width="100%" Font-Size="X-Small" Font-Bold="true" TextMode="MultiLine"/>
        <asp:CustomValidator ID="CustomValidator66" runat="server" 
            ClientValidationFunction="validateClosingRemarks" 
            ValidationGroup="save"
            ControlToValidate="Closing_Remarks" 
            ValidateEmptyText="true" 
            ErrorMessage="Closing Remarks is required when Status is 'Close'."></asp:CustomValidator>
    </ItemTemplate>
</asp:TemplateField>



function validateClosingRemarks(sender, args) {
    var row = sender.closest("tr"); // Find the closest row
    var ddlStatus = row.querySelector("[id$='Status']"); // Get dropdown
    var txtRemarks = row.querySelector("[id$='Closing_Remarks']"); // Get textbox

    if (ddlStatus && txtRemarks) {
        if (ddlStatus.value === "Close" && txtRemarks.value.trim() === "") {
            args.IsValid = false; // Validation fails
        } else {
            args.IsValid = true; // Validation passes
        }
    }
}




                    if (((DropDownList)LineWalk_ListRecord.Rows[i].FindControl("Status")).SelectedValue != "Close")
                    {
                        bool hasClosing_Remarks = !string.IsNullOrEmpty(PageRecordDataSet.Tables["App_LineWalk_Details"].Rows[i]["Status"].ToString());

                        {
                            ((CustomValidator)LineWalk_ListRecord.Rows[i].FindControl("CustomValidator66")).Enabled = !hasClosing_Remarks;

                        }

                        //DropDownList ddlstatus = (DropDownList)LineWalk_ListRecord.Rows[i].FindControl("Status");
                        //CustomValidator cv = (CustomValidator)LineWalk_ListRecord.Rows[i].FindControl("CustomValidator66");
                        //if(ddlstatus != null && cv != null)
                        //{
                        //    bool isClosing = ddlstatus.SelectedValue == "Close";

                        //    cv.Enabled = isClosing;
                        //}



                    }


  <asp:TemplateField HeaderText="Status" SortExpression="Status" HeaderStyle-Width="3%"  >
                                            <ItemTemplate>
                                                <asp:DropDownList ID="Status" runat="server" CssClass="form-control form-control-sm"  Width="100%" Font-Size="X-Small" Font-Bold="true" AutoPostBack="true"  OnSelectedIndexChanged="Status_SelectedIndexChanged">
                                                                <asp:ListItem value="Open">Open</asp:ListItem> 
                                                                <asp:ListItem value="Close">Close</asp:ListItem> 
                                                               
                                                 </asp:DropDownList>
                                                
                                            </ItemTemplate>
                                        </asp:TemplateField>

 <asp:TemplateField HeaderText="Closing Remarks" SortExpression="Remarks" HeaderStyle-Width="8%"  >
                                            <ItemTemplate>
                                                <asp:TextBox ID="Closing_Remarks" runat="server" CssClass="form-control form-control-sm"  Width="100%" Font-Size="X-Small" Font-Bold="true" TextMode="MultiLine"   />
                                                <asp:CustomValidator ID="CustomValidator66" runat="server" ClientValidationFunction="Validate" ValidationGroup="save"  ControlToValidate="Closing_Remarks" ValidateEmptyText="true"></asp:CustomValidator>
                                        
                                            </ItemTemplate>
                                        </asp:TemplateField>


if dropdown value is close then i want to Required the closing remarks if the dropdown value is open , dont validate remarks 
