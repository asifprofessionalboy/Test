i have this model 

    public partial class AppUserFormPermission
    {
        public Guid UserId { get; set; }
        public Guid FormId { get; set; }
        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        public bool? AllowDelete { get; set; }
        public bool? AllowAll { get; set; }
        public bool? AllowModify { get; set; }
        public bool DownTime { get; set; }
    }


i have this viewside 

 <form asp-action="UserPermission_Button" asp-controller="User">
 <fieldset class="mt-2" style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px">
    
     <legend class="legend"><b>Select Permission for the User</b></legend>
     <div class="form" id="formContainer" style="display:none;">
     <div class="w-100 border" style="overflow:auto;height:250px;">
       

        
                 <table class="table-hover table-responsive-sm" cellspacing="0" cellpadding="4" id="MainContent_userPermissions" style="color:#333333;width:100%;border-collapse:collapse;">
                     <tbody>
                       
                         <tr style="color:White;background-color:#49477a;font-size:Smaller;font-weight:bold;">
                             <th align="left" scope="col">Form Name</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                             <th scope="col">&nbsp;</th>
                         </tr>

                         @if (ViewBag.formList != null)
                         {
                             var formList = ViewBag.formList as List<AppFormDetail>;
                             int rowIndex = 0;

                             @foreach (var form in formList)
                             {
                                 string bgColor = (rowIndex % 2 == 1 && rowIndex != 0) ? "#e3dff3" : "transparent";
                                 <tr style="color:#333333; background-color:@bgColor; font-size:Smaller;">
                                     <td>
                                         <span>@form.Description</span>
                                     </td>
                                     <td style="width:100px;">
                                         <input  type="checkbox" id="Read"><label class="control-label">&nbsp;Read</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input  type="checkbox" id="Create"><label class="control-label">&nbsp;Create</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input  type="checkbox" id="Modify"><label class="control-label">&nbsp;Modify</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input  type="checkbox" id="Delete"><label class="control-label">&nbsp;Delete</label>
                                     </td>
                                     <td style="width:100px;">
                                         <input  type="checkbox" id="All"><label class="control-label">&nbsp;All</label>
                                     </td>
                                 </tr>
                                 rowIndex++;
                             }
                         }
                     </tbody>
                 </table>
        
     </div>
             <div class="row m-0 justify-content-center mt-2">
                 <input type="submit" value="Save" id="MainContent_btnSave" class="btn btn-primary btn-sm ">
             </div>
     </div>
        
     
 </fieldset>
 </form> 

and this is controller method 

 public IActionResult UserPermission()
 {

     var PnoEnameList = context1.AppEmployeeMasters
            .Select(x => new
            {
                Pno = x.Pno,
                Id = x.Id,
                Ename = x.Ename,
            })
            .ToList();
     ViewBag.PnoEnameList = PnoEnameList;

     var formList = context.AppFormDetails.Select(x =>new AppFormDetail
     {
         Id = x.Id,
         Description = x.Description
     }).OrderBy(x=>x.Description).ToList();
     ViewBag.formList = formList;


     return View();
 }
 [HttpPost]
 public IActionResult UserPermission_Button()
 {
     return View();
 }

in this i want to insert data based on checked checkbox ,
each form has There Id and Each user has there Id , both will insert in this which form is checked or which one 

public Guid UserId { get; set; }
        public Guid FormId { get; set; }
