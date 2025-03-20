        function HDFC_Bank_Ac_holder_Validate()

        {
            alert("ok");
            var HDFC_Ac_Holder = document.getElementById("MainContent_ImprestCard_Record_HDFC_Bank_Ac_holder_0_0_0").value;
            
            var CustomerId = document.getElementById("Customer_ID_div");
            var PAN_no = document.getElementById("MainContent_ImprestCard_Record_PAN_0").value;
            var Adhar_No = document.getElementById("MainContent_ImprestCard_Record_Aadhaar_0").value;
            var AdharAttach = document.getElementById("MainContent_ImprestCard_Record_Adhar_Attach_0").file;
            var PAN_Attach = document.getElementById("MainContent_ImprestCard_Record_PAN_Attach_0").file;
            alert(CustomerId);
            
            alert(HDFC_Ac_Holder);
            if (HDFC_Ac_Holder == "YES")

            {
                //document.getElementById("MainContent_ImprestCard_Record_Customer_ID_0").disabled = false;
                CustomerId.style.display = "block";
                return true;
            }

            else
            {
                alert("HDFC_Ac_Holder");
               // document.getElementById("MainContent_ImprestCard_Record_Customer_ID_0").disabled = true;
                CustomerId.style.display = "none";
                return true
            }

        }


radio button :


                                         </asp:RadioButtonList>     --%>
                                            
                                            <asp:RadioButtonList ID="HDFC_Bank_Ac_holder" runat="server" CssClass="form-check-input col-lg-8 font-weight-bold fs-6 radio"
                                             RepeatColumns="2"  RepeatDirection="Horizontal" OnClick="HDFC_Bank_Ac_holder_Validate();" >
                                             <asp:ListItem Value="YES">&nbsp YES</asp:ListItem>
                                             <asp:ListItem Value="NO"> &nbsp NO</asp:ListItem>

                                         </asp:RadioButtonList>  


 <div id= "Customer_ID_div" class="col-lg-4 mb-1 form-row" style="display:none;">
                                             <div class="col-md-4">
                                                <label class="m-0 mr-2 p-0 col-form-label-sm  font-weight-bold fs-6"  >Customer ID :<span class="text-danger">*</span></label>
                                            </div>
                                            <div class="col-lg-7">
                                                <asp:TextBox  ID="Customer_ID" runat="server"  CssClass="form-control form-control-sm" required ></asp:TextBox>
                                                 </div>
                                              
                                            </div>
    



i want to valdate ID="HDFC_Bank_Ac_holder"  when ID="HDFC_Bank_Ac_holder"  choose yes than  ID="Customer_ID"  should visible other wise not visible and on page load it is also not visible when yes then customerId is visible
