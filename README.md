this is my two rows . if yes selected it shows CustomerId f no selected it shows Pan, Aadhar and Attachment . logic is working but design is not. i want if yes seleted then customerid is on same row with radio buttons if no selected then pan, aadhar and attachment shows on new row means another row not on radio button 
<div class="row col-md-12 mt-2">

                                           <div class="col-md-1">
                                                  <label for="lblAction" class="m-0 mr-1 p-0 col-form-label-sm  font-weight-bold fs-6">HDFC Bank A/c holder <span class="text-danger">*</span></label>
                                           </div>
                                            <div class="col-md-3 ml-2">
                                               <asp:RadioButtonList ID="HDFC_Bank_Ac_holder" runat="server" CssClass="form-check-input col-lg-8 font-weight-bold fs-6 radio"
                                             RepeatColumns="2"  RepeatDirection="Horizontal" OnClick="HDFC_Bank_Ac_holder_Validate();" >
                                             <asp:ListItem Value="YES">&nbsp YES</asp:ListItem>
                                             <asp:ListItem Value="NO"> &nbsp NO</asp:ListItem>

                                         </asp:RadioButtonList>      
                                                 </div>
                                               <div class="row">
                                            <div id= "Customer_ID_div" class="col-lg-4 mb-1 form-row" style="display:none;">
                                          <div class="col-md-4">
                                             
                                               <label class="m-0 mr-2 p-0 col-form-label-sm  font-weight-bold fs-6"  >Customer ID :<span class="text-danger">*</span></label>
                                             </div>
                                         <div class="col-md-4">
                                                <asp:TextBox  ID="Customer_ID" runat="server"  CssClass="form-control form-control-sm"  ></asp:TextBox>
                                              </div>
                                               </div>

                                               </div>



<div class="row col-md-12 mt-2">


                                        <div id="Attachments_div" class="row mt-2" style="display:none;">

                                               <div class="col-md-1">
                                                <label class="m-0 mr-2 p-0 col-form-label-sm  font-weight-bold fs-6"  >PAN No. :<span class="text-danger">*</span></label>
                                                   </div>

                                              <div class="col-md-3">
                                              <asp:TextBox ID="PAN"  CssClass="form-control form-control-sm"   runat="server"   ></asp:TextBox>

                                                </div>


                                              <div class="col-md-1">
                                                <label class="m-0 mr-2 p-0 col-form-label-sm  font-weight-bold fs-6"  >Aadhaar No :<span class="text-danger">*</span></label>

                                                   </div>
                                              <div class="col-md-3">
                                                  <asp:TextBox ID="Aadhaar"  CssClass="form-control form-control-sm"   runat="server"   ></asp:TextBox>

                                                </div>




                                               
                                          
                                              </div>
