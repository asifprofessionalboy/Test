@model List<AppSubjectMaster>

<div class="row">
    @foreach (var subject in Model)
    {
        <div class="col-sm-4">
            <a asp-action="ViewerForm" asp-route-Subject="@subject.Subject">
                <div class="card l-bg-cyan-dark">
                    <div class="card-statistic-3 p-4">
                        <div class="">
                            <h6 class="card-title mb-0 head">@subject.Subject</h6>
                        </div>
                        <div class="row align-items-center mb-4 d-flex">
                            <div class="col-8">
                                <h7 class="d-flex align-items-center mb-1"></h7>
                            </div>
                        </div>
                    </div>
                </div>
            </a>
        </div>
    }
</div>





this is my view side for dashboard 
public IActionResult Dashboard()
{
	if (HttpContext.Session.GetString("Session") != null)
	{
		return View();
	}
	else
	{
		return RedirectToAction("Login", "User");
	}
}

this is my viewside 

 <div class="col-sm-4">
              <a asp-action="ViewerForm" asp-route-MD="MD Communication pack">
             <div class="card l-bg-cyan-dark">
                 <div class="card-statistic-3 p-4">

                     <div class="">
                         <h6 class="card-title mb-0 head" name="MD">

                             MD Communication pack

                         </h6>
                     </div>
                     <div class="row align-items-center mb-4 d-flex">
                         <div class="col-8">
                             <h7 class="d-flex align-items-center mb-1">

                             </h7>
                         </div>

                     </div>
                 </div>
             </div>
         </a>
    
 </div>

     <div class="col-sm-4">

          <a asp-action="ViewerForm" asp-route-Flash="Flash Report">
             <div class="card l-bg-purple-dark">
                 <div class="card-statistic-3 p-4">

                     <div class="">
                         <h6 class="card-title mb-0 head">

                             Flash Report

                         </h6>
                     </div>
                     <div class="row align-items-center mb-4 d-flex">
                         <div class="col-8">
                             <h7 class="d-flex align-items-center mb-1">

                             </h7>
                         </div>

                     </div>

                 </div>
             </div>
         </a>


     </div>

, i have subjectMaster model , i want to make this dynamic 
public partial class AppSubjectMaster
{
   

    public Guid Id { get; set; }
    public string? Subject { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
}

i want that when there is new subject added to master table then make the card like this cards above ,

and this for my filter when clicking on cards it shows filtered data, make this dynamic also
