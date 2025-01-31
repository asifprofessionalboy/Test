@if (!string.IsNullOrEmpty(Model.Attachment))
{
    <div>
        <ul>
            @foreach (var fileName in Model.Attachment.Split(','))
            {
                var cleanFileName = ExtractFileName(fileName);
                var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                var isPdf = fileExtension == ".pdf";

                <li>
                    <a href="@Url.Action("DownloadFile", new { fileName = fileName })" 
                       @(isPdf ? "target=\"_blank\"" : "")>
                        @cleanFileName
                    </a>
                </li>
            }
        </ul>
    </div>
}



this is my for my three chart
<form method="get" action="@Url.Action("Overview","Innovation")">
   <div class="row">

       <div class="col-sm-6">
           <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;margin-left:2px;">
               <h6 class="text-center overview-heading">
                   Generation & Scrutiny Status - FY

                   <select name="FinYear" id="FinYear" onchange="this.form.submit();">
                       @if (ViewBag.FinYear == "24-25")
                       {
                           <option value="24-25" selected>25</option>
                       }
                       else
                       {
                           <option value="24-25">25</option>
                       }
                       @if (ViewBag.FinYear == "25-26")
                       {
                           <option value="25-26" selected>26</option>
                       }
                       else
                       {
                           <option value="25-26">26</option>
                       }
                   </select>
                  
               </h6>

               <canvas id="barChart" style="width:500px;height:302px;margin-top:20px;"></canvas>

           </fieldset>
       </div>

       <div class="col-sm-5">
           <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;">
               <h6 class="text-center overview-heading">Stage of Innovation Projects - FY
                   <select name="FinYear2" id="FinYear2" onchange="this.form.submit();">
                       @if (ViewBag.FinYear2 == "24-25")
                       {
                           <option value="24-25" selected>25</option>
                       }
                       else
                       {
                           <option value="24-25">25</option>
                       }
                       @if (ViewBag.FinYear2 == "25-26")
                       {
                           <option value="25-26" selected>26</option>
                       }
                       else
                       {
                           <option value="25-26">26</option>
                       }
                   </select>
               </h6>
               <canvas id="pieChart"></canvas>
           </fieldset>
       </div>

   </div>
   <div class="row">
       <div class="col-sm-6">
           <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;margin-top:3px;margin-left:2px;">
               <h6 class="text-center overview-heading">Emerging themes of Innovation Projects(Nos) - FY
                   <select name="FinYear3" id="FinYear3" onchange="this.form.submit();">
                       @if (ViewBag.FinYear3 == "24-25")
                       {
                           <option value="24-25" selected>25</option>
                       }
                       else
                       {
                           <option value="24-25">25</option>
                       }
                       @if (ViewBag.FinYear3 == "25-26")
                       {
                           <option value="25-26" selected>26</option>
                       }
                       else
                       {
                           <option value="25-26">26</option>
                       }
                   </select>
               </h6>
               <canvas id="barChart3" class="" style="width:390px;height:368px;"></canvas>
           </fieldset>
       </div>
       </div>

   </form>


this is my controller 
public IActionResult Overview(string FinYear="", string FinYear2 = "", string FinYear3 = "", string FinYear4 = "")
{
	if (HttpContext.Session.GetString("Session") != null)
	{
        string connectionString = "Server=10.0.168.50;Database=INNOVATIONDB;User Id=fs;Password=p@ssW0Rd321";
		using (IDbConnection connection = new SqlConnection(connectionString))
		{

			if (!string.IsNullOrEmpty(FinYear) || FinYear == "")
			{
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;


                if (FinYear == "24-25" || FinYear=="")
                {
                    startDate = new DateTime(2024, 4, 1);
                    endDate = new DateTime(2024, 10, 31);
                }
                else if (FinYear == "25-26")
                {
                    startDate = new DateTime(2024, 11, 1);
                    endDate = new DateTime(2025, 3, 31);
                }
				var totalInnovationsquery = "select count(*) from App_Innovation where CreatedOn>='" + startDate + "'and CreatedOn<='" + endDate + "' and Status != 'Draft'";
				var totalApprovedInnovationsquery = "select count(*) from App_Innovation where CreatedOn>='" + startDate + "'and CreatedOn<='" + endDate + "' and Status = 'Approved'";
				var totalPendingInnovationsquery = "select count(*) from App_Innovation where CreatedOn>='" + startDate + "'and CreatedOn<='" + endDate + "' and Status = 'Pending for Approval'";
				
				var totalsustainquery = "select Count(distinct Master_Id) from App_Innovation_Benefits where Master_ID in (select Master_ID from App_Innovation where CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+"') and  Benefits like '%sustain%'";
                var totalsafetyquery = "select count(distinct Master_Id)  from App_Innovation_Benefits where Master_ID in (select Master_ID from App_Innovation where CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+"') and Benefits  like '%safe%'";
                var totalothersquery = "select count(distinct Master_Id)  from App_Innovation_Benefits where Master_ID in (select Master_ID from App_Innovation where CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+"') and Benefits not like '%sustain%' and Benefits NOT like '%safe%'";


				int totalInnovations = connection.QuerySingleOrDefault<int>(totalInnovationsquery);
				int totalApprovedInnovations = connection.QuerySingleOrDefault<int>(totalApprovedInnovationsquery);
				int totalPendingInnovations = connection.QuerySingleOrDefault<int>(totalPendingInnovationsquery);
				
				int totalsustain = connection.QuerySingleOrDefault<int>(totalsustainquery);
                int totalsafety = connection.QuerySingleOrDefault<int>(totalsafetyquery);
                int totalothers = connection.QuerySingleOrDefault<int>(totalothersquery);
				ViewBag.TotalInnovations = totalInnovations;
				ViewBag.TotalApprovedInnovations = totalApprovedInnovations;
				ViewBag.TotalPendingInnovations = totalPendingInnovations;
				
				ViewBag.totalsafety = totalsafety;
                ViewBag.totalsustainability = totalsustain;
                ViewBag.totalothers = totalothers;
            }
			 if (!string.IsNullOrEmpty(FinYear2) || FinYear2 == "")
			{
				DateTime startDate = DateTime.MinValue;
				DateTime endDate = DateTime.MaxValue;


				if (FinYear2 == "24-25" || FinYear2=="")
				{
					startDate = new DateTime(2024, 4, 1);
					endDate = new DateTime(2024, 10, 31);
				}
				else if (FinYear2 == "25-26")
				{
					startDate = new DateTime(2024, 11, 1);
					endDate = new DateTime(2025, 3, 31);
				}
                var totalConceptquery = "select count(*) from App_Innovation where CreatedOn>='" + startDate + "'and CreatedOn<='" + endDate + "' and Stage_of_Innovation = 'concept'and Status='Approved'";
                var totalImplementedSuccessquery = "select count(*) from App_Innovation where CreatedOn>='" + startDate + "'and CreatedOn<='" + endDate + "' and Stage_of_Innovation = 'Implemented Successfully'and Status='Approved'";
                var totalTrailquery = "select count(*) from App_Innovation where CreatedOn>='" + startDate + "'and CreatedOn<='" + endDate + "' and Stage_of_Innovation = 'Under Trial'and Status='Approved'";

                int totalConcept = connection.QuerySingleOrDefault<int>(totalConceptquery);
                int totalImplementedSuccess = connection.QuerySingleOrDefault<int>(totalImplementedSuccessquery);
                int totalTrail = connection.QuerySingleOrDefault<int>(totalTrailquery);

                ViewBag.totalConcept = totalConcept;
                ViewBag.totalTrail = totalTrail;
                ViewBag.totalImplementedSuccess = totalImplementedSuccess;
            }

            if (!string.IsNullOrEmpty(FinYear3) || FinYear3 == "")
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;


                if (FinYear3 == "24-25" || FinYear3 == "")
                {
                    startDate = new DateTime(2024, 4, 1);
                    endDate = new DateTime(2024, 10, 31);
                }
                else if (FinYear3 == "25-26")
                {
                    startDate = new DateTime(2024, 11, 1);
                    endDate = new DateTime(2025, 3, 31);
                }
              
                var totalsustainquery = "select count(distinct IB.Master_Id) as Others from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+"' and Benefits  like '%sustain%'";
                var totalsafetyquery = "select count(distinct IB.Master_Id) as Others from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+"' and Benefits  like '%safe%'";
                var totalothersquery = "select count(distinct IB.Master_Id) as Others from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+"'and Benefits not like '%sustain%' and Benefits NOT like '%safe%'";

                int totalsustain = connection.QuerySingleOrDefault<int>(totalsustainquery);
                int totalsafety = connection.QuerySingleOrDefault<int>(totalsafetyquery);
                int totalothers = connection.QuerySingleOrDefault<int>(totalothersquery);
                
                ViewBag.totalsafety = totalsafety;
                ViewBag.totalsustainability = totalsustain;
                ViewBag.totalothers = totalothers;
            }

            ViewBag.FinYear = FinYear;
            ViewBag.FinYear2 = FinYear2;
            ViewBag.FinYear3 = FinYear3;
            var divisions = GetDivisionDD();
			ViewBag.Divisions = divisions;
		

        }

    }
	else
	{
		return RedirectToAction("Login", "User");
	}
  

	
    return View();
}


in this when i changes the dropdown of finyear it refresh the pages , i dont want that i want that when it i change dropdown value it shows the data but not refresh the page
