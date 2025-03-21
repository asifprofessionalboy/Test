var modalId = '<%= myModal.ClientID %>';
$('#' + modalId).modal('show');



<!-- Add these before closing </head> or at the end of <body> -->
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
console.log(typeof $); // Should print "function" if jQuery is loaded
console.log($('#myModal')); // Should not be empty
#myModal {
    display: block !important;
    opacity: 1 !important;
}



this is my js functions     
<script language="Javascript" type="text/javascript"> 
      
        function showModal()
        {
            alert("123");
            $('#myModal').modal('show'); // Use Bootstrap modal
           
        }
   
   
        function HDFC_Bank_Ac_holder_Validate()

        {
            var CustomerID = document.getElementById("MainContent_ImprestCard_Record_Customer_ID_0");
            var PAN = document.getElementById("MainContent_ImprestCard_Record_PAN_0");
            var Aadhaar = document.getElementById("MainContent_ImprestCard_Record_Aadhaar_0");
            var Adhar_Attach = document.getElementById("MainContent_ImprestCard_Record_Adhar_Attach_0");
            var PAN_Attach = document.getElementById("MainContent_ImprestCard_Record_PAN_Attach_0");

            var HDFC_Ac_Holder = document.querySelector('input[name*="HDFC_Bank_Ac_holder"]:checked').value;
            
            var CustomerId = document.getElementById("Customer_ID_div");
           
            var Attachmentsdiv = document.getElementById("Attachments_div");
            
            if (HDFC_Ac_Holder == "YES")

            {
               

                PAN.value = "";
                Aadhaar.value = "";
                PAN_Attach.value = "";
                Adhar_Attach.value = "";
                CustomerId.style.display = "block"; // show
                
                Attachmentsdiv.style.display = "none"; //hide
               
               

                return true;
            }

            else
            {
                CustomerID.value = "";
                CustomerId.style.display = "none";
               
                Attachmentsdiv.style.display = "block";

                return true
            }

        }
           

    </script> 


this is my modal 

    <div id="myModal" class="modal fade" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" runat="server" >
    <div class="modal-dialog w-100" role="document">
        <div class="modal-content">
            <div class="modal-header" style="background-color: #548ac580;">
                <h5 class="modal-title">Declaration for Taking Imprest Card</h5>
               <%-- <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>--%>
            </div>
            <div class="modal-body">
                <p>Responsibilities of the card holder:</p>
         <div class="content">
              </div>

            </div>
            <div class="modal-footer">
              <asp:Button ID="button_Save" class="btn btn-secondary" runat="server" Text="Agree"  OnClick="button_Save_Click"/>
              <asp:Button ID="button_Cancel" class="btn btn-primary" runat="server" Text="Not Agree"  OnClick="button_Cancel_Click"/>


            </div>
        </div>
    </div>
</div>


protected void Page_Load(object sender, EventArgs e)
        {
            ImprestCard_Record.DataSource = PageRecordDataSet;

            
            if (!IsPostBack)
            {

                ImprestCard_Record.Visible = false;
                Buttons_div.Visible = true;


                NewCard_Div.Visible = false;
                string Pno = Session["UserName"].ToString();

                BL_ImprestCard_Request blobj = new BL_ImprestCard_Request();
                DataSet ds2 = blobj.Chk_Pno(Pno);

               
                
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    
                    //Response.Redirect("~/Default.aspx");
                    //Response.Redirect("~/App/Input/Imprest_Card_Request.aspx");
                }
                else
                {
                    // Set a hidden field value or a JavaScript variable
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "showModal();", true);

                    BL_ImprestCard_Request blobj1 = new BL_ImprestCard_Request();
                    DataSet ds3 = blobj1.Pno_name(Pno);
                    if (ds3.Tables[0].Rows.Count > 0)
                    {
                        Pno_Name.Text = ds3.Tables[0].Rows[0]["Ename"].ToString();

                    }
                }
            }
        }


why modal is not open, it executes the function ShowModal but Modal is not Opening
