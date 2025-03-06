
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
