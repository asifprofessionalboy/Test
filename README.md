 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
     <ContentTemplate>

         <div class="card m-2 shadow-lg">
             <div class="card-header bg-info text-light">
                 <h6 class="m-0">Compliance MIS</h6>
             </div>

             <div class="row m-0 justify-content-end">
             </div>

             <div class="card-body pt-1">
                 <div>
                     <uc1:MyMsgBox ID="MyMsgBox" runat="server" />
                 </div>
                 <fieldset class="" style="border: 1px solid #bfbebe; padding: 5px 20px 5px 20px; border-radius: 6px">
                     <legend style="width: auto; border: 0; font-size: 14px; margin: 0px 6px 0px 6px; padding: 0px 5px 0px 5px; color: #0000FF"><b>Search</b></legend>

                     <div class="form-inline row">
                         <div class="form-group col-md-4 mb-2">
                             <label for="Month" class="m-0 mr-2 p-0 col-form-label-sm col-sm-5  font-weight-bold fs-6 justify-content-start">Month:<span style="color: #FF0000;">*</span></label>

                             <div class="form-group col-md-6 mb-2" style="margin-left: -14px">
                                 <div>
                                     <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                         <ContentTemplate>
                                             <div class="SearchCheckBoxList" style="width: 146%;">
                                                 <button class="btn btn-sm btn-default selectArea w-100" type="button" onclick="togglefloatDiv(this);" id="btn-dwn" style="border: 0.5px solid #ced2d5;">
                                                     <span class="filter-option float-left">No item Selected</span>
                                                     <span class="caret"></span>
                                                 </button>
                                                 <div class="floatDiv" runat="server" style="border: 1px solid #ced2d5; position: absolute; z-index: 1000; width: 400px; box-shadow: 0 6px 12px rgb(0 0 0 / 18%); max-height: 400px; height: 450px; background-color: white; padding: 5px; display: none">
                                                     <asp:TextBox runat="server" ID="TextBox2" CssClass="form-control form-control-sm " oninput="filterCheckBox(this)" AutoComplete="off" ViewStateMode="Disabled" placeholder="Enter PNO or Name" Font-Size="Smaller" />
                                                     <div style="overflow: auto; max-height: 300px; height: 100%" class="searchList p-0">
                                                         <asp:CheckBoxList ID="Month" runat="server" DataMember="Month_MIS" DataSource="<%# PageDDLDataset %>" DataTextField="Month"
                                                             DataValueField="MonthValue" CssClass="form-control-sm radio" AutoPostBack="true">
                                                         </asp:CheckBoxList>
                                                     </div>
                                                     <asp:TextBox runat="server" ID="TextBox4" Width="0%" Style="display: none" />
                                                 </div>


                                             </div>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                             </div>
                         </div>

                         <div class="form-group col-md-4 mb-2">
                             <label for="Year" class="m-0 mr-2 p-0 col-form-label-sm col-sm-5  font-weight-bold fs-6 justify-content-start">Year:<span style="color: #FF0000;">*</span></label>

                             <div class="form-group col-md-6 mb-2" style="margin-left: -14px">
                                 <div>
                                     <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                         <ContentTemplate>
                                             <div class="SearchCheckBoxList" style="width: 146%;">
                                                 <button class="btn btn-sm btn-default selectArea w-100" type="button" onclick="togglefloatDiv(this);" id="btn-dwn" style="border: 0.5px solid #ced2d5;">
                                                     <span class="filter-option float-left">No item Selected</span>
                                                     <span class="caret"></span>
                                                 </button>
                                                 <div class="floatDiv" runat="server" style="border: 1px solid #ced2d5; position: absolute; z-index: 1000; width: 400px; box-shadow: 0 6px 12px rgb(0 0 0 / 18%); max-height: 400px; height: 450px; background-color: white; padding: 5px; display: none">
                                                     <asp:TextBox runat="server" ID="TextBox1" CssClass="form-control form-control-sm " oninput="filterCheckBox(this)" AutoComplete="off" ViewStateMode="Disabled" placeholder="Enter PNO or Name" Font-Size="Smaller" />
                                                     <div style="overflow: auto; max-height: 360px; height: 100%" class="searchList p-0">
                                                         <asp:CheckBoxList ID="Year" runat="server" DataMember="Year_MIS" DataSource="<%# PageDDLDataset %>" DataTextField="Year"
                                                             DataValueField="Year" CssClass="form-control-sm radio" AutoPostBack="true">
                                                         </asp:CheckBoxList>
                                                     </div>
                                                     <asp:TextBox runat="server" ID="TextBox3" Width="0%" Style="display: none" />
                                                 </div>


                                             </div>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                             </div>
                         </div>


                         <div class="form-group col-md-4 mb-2">
                             <label for="Location" class="m-0 mr-2 p-0 col-form-label-sm col-sm-5  font-weight-bold fs-6 justify-content-start">Location:</label>

                             <div class="form-group col-md-6 mb-2" style="margin-left: -14px">
                                 <div>
                                     <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                         <ContentTemplate>
                                             <div class="SearchCheckBoxList" style="width: 146%;">
                                                 <button class="btn btn-sm btn-default selectArea w-100" type="button" onclick="togglefloatDiv(this);" id="btn-dwn" style="border: 0.5px solid #ced2d5;">
                                                     <span class="filter-option float-left">No item Selected</span>
                                                     <span class="caret"></span>
                                                 </button>
                                                 <div class="floatDiv" runat="server" style="border: 1px solid #ced2d5; position: absolute; z-index: 1000; width: 400px; box-shadow: 0 6px 12px rgb(0 0 0 / 18%); max-height: 400px; height: 450px; background-color: white; padding: 5px; display: none">
                                                     <asp:TextBox runat="server" ID="TextBox5" CssClass="form-control form-control-sm " oninput="filterCheckBox(this)" AutoComplete="off" ViewStateMode="Disabled" placeholder="Enter PNO or Name" Font-Size="Smaller" />
                                                     <div style="overflow: auto; max-height: 360px; height: 100%" class="searchList p-0">
                                                         <asp:CheckBoxList ID="Location" runat="server" DataMember="Location_MIS" DataSource="<%# PageDDLDataset %>" DataTextField="Location"
                                                             DataValueField="LocationCode" CssClass="form-control-sm radio" AutoPostBack="true">
                                                         </asp:CheckBoxList>
                                                     </div>
                                                     <asp:TextBox runat="server" ID="TextBox6" Width="0%" Style="display: none" />
                                                 </div>


                                             </div>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                             </div>
                         </div>



                     </div>

                     <div class="form-inline row">


                         <div class="form-group col-md-4 mb-2">
                             <label for="Department" class="m-0 mr-2 p-0 col-form-label-sm col-sm-5  font-weight-bold fs-6 justify-content-start">Department:</label>

                             <div class="form-group col-md-6 mb-2" style="margin-left: -14px">
                                 <div>
                                     <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                         <ContentTemplate>
                                             <div class="SearchCheckBoxList" style="width: 146%;">
                                                 <button class="btn btn-sm btn-default selectArea w-100" type="button" onclick="togglefloatDiv(this);" id="btn-dwn" style="border: 0.5px solid #ced2d5;">
                                                     <span class="filter-option float-left">No item Selected</span>
                                                     <span class="caret"></span>
                                                 </button>
                                                 <div class="floatDiv" runat="server" style="border: 1px solid #ced2d5; position: absolute; z-index: 1000; width: 400px; box-shadow: 0 6px 12px rgb(0 0 0 / 18%); max-height: 400px; height: 450px; background-color: white; padding: 5px; display: none">
                                                     <asp:TextBox runat="server" ID="TextBox9" CssClass="form-control form-control-sm " oninput="filterCheckBox(this)" AutoComplete="off" ViewStateMode="Disabled" placeholder="Enter PNO or Name" Font-Size="Smaller" />
                                                     <div style="overflow: auto; max-height: 360px; height: 100%" class="searchList p-0">
                                                         <asp:CheckBoxList ID="Department" runat="server" DataMember="Deaprtment_MIS" DataSource="<%# PageDDLDataset %>" DataTextField="DepartmentName"
                                                             DataValueField="DepartmentCode" CssClass="form-control-sm radio" AutoPostBack="true">
                                                         </asp:CheckBoxList>
                                                     </div>
                                                     <asp:TextBox runat="server" ID="TextBox10" Width="0%" Style="display: none" />
                                                 </div>


                                             </div>
                                         </ContentTemplate>
                                     </asp:UpdatePanel>
                                 </div>
                             </div>
                         </div>
                         <div class="form-group col-md-1 mb-2">
                         </div>
                         <div class="form-group col-md-4 mb-2">
                             <div class="row m-0 justify-content-center mt-2 mb-2">
                                 <asp:Button ID="btnSearch" runat="server" Text="View Details" OnClick="btnSearch_Click" CssClass="btn btn-sm btn-success" />
                                 &nbsp &nbsp
                        
                              <asp:Button ID="btn_Summary_Report" runat="server" Text="View Summary" OnClick="btn_Summary_Report_Click" CssClass="btn btn-sm btn-info" />
                             </div>

                         </div>
                     </div>
             </div>

             </fieldset>

        
              
         </div>
         </div>
             
     </ContentTemplate>
 </asp:UpdatePanel>



 <script type="text/javascript">
     function filterCheckBox(e) {
         // Declare variables
         var input, filter, table, tbody, tr, td, a, i, txtValue;
         input = e.value.trim();

         filter = input.toUpperCase();

         table = e.nextSibling.nextSibling.childNodes[1];
         tbody = table.getElementsByTagName('tbody');
         tr = table.getElementsByTagName('tr');

         // Loop through all list items, and hide those who don't match the search query
         for (i = 0; i < tr.length; i++) {
             a = tr[i].getElementsByTagName("td")[0];
             txtValue = a.textContent || a.innerText;
             if (txtValue.toUpperCase().indexOf(filter) > -1) {

                 tr[i].style.display = "";
             } else {
                 tr[i].style.display = "none";
             }
         }


     }

     function getClickedElement() {


         var specifiedElement = document.getElementsByClassName('VendorColumn');
         for (var i = 0; i < specifiedElement.length; i++) {

             specifiedElement[i].childNodes[3].childNodes[3].onclick = function (event) {
                 var target = getEventTarget(event);
                 var data = target.parentNode.getAttribute("data-index-no");

                 event.currentTarget.nextElementSibling.value = data;
                 event.currentTarget.previousElementSibling.value = "";
                 event.currentTarget.nextElementSibling.onchange();
             };


             if (specifiedElement[i].childNodes[3].childNodes[5].value.trim() == "")
                 specifiedElement[i].childNodes[1].childNodes[1].innerHTML = "No item selected";
             else
                 specifiedElement[i].childNodes[1].childNodes[1].innerHTML = specifiedElement[i].childNodes[3].childNodes[5].value.trim();
         }


 </script>


   protected void Page_Load(object sender, EventArgs e)
   {
       if (!IsPostBack)
       {



           ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "getClickedElement();", true);
           GetDropdowns("Month_MIS");
           Month.DataBind();
           GetDropdowns("Year_MIS");
           Year.DataBind();
           GetDropdowns("Location_MIS");
           Location.DataBind();
           GetDropdowns("Deaprtment_MIS");
           Department.DataBind();
           //string vendorCode = Session["UserName"].ToString();
           DataSet ds = objMIS.Get_Mis_Data();

           if (ds != null && ds.Tables.Count > 0)
           {
               Mis_Records.DataSource = ds;
               Mis_Records.DataBind();
           }
       }
   }



  protected void btnSearch_Click(object sender, EventArgs e)
  {

  // i want to my execute here when i click button then my data comes
  }


