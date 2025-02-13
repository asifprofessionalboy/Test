this is the method to fetch subjectDD  
public List<SubjectDD> GetSubjectDD()
  {
      string connectionString = GetConnectionString();

      string query = @"select distinct Subject from App_SubjectMaster";


      using (var connection = new SqlConnection(connectionString))
      {
          var divisions = connection.Query<SubjectDD>(query).ToList();

          return divisions;

      }

  }

this is controller view 
public IActionResult TechnicalService()
{
    if (HttpContext.Session.GetString("Session") != null)
    {
		var viewModel = new AppTechnicalService
		{
			Attach = new List<IFormFile>(),

		};
		var Dept = GetDepartmentDD();
        ViewBag.Department = Dept;

		var Month = GetMonthDD();
		ViewBag.Month = Month;

        var subjectDDs = GetSubjectDD();
        ViewBag.Subjects = subjectDDs;


        return View(viewModel);
	}
	else
    {
        return RedirectToAction("Login", "User");
    }

    
}

this is my view side 
<div class="form-group row">
				<div class="col-sm-1">
				<label asp-for="Subject" class="control-label">Subject </label>
				</div>
				<div class="col-sm-3">
					<select asp-for="Subject" class="form-control form-control-sm custom-select" name="Subject">
					<option value="">Select Subject</option>
					@foreach (var item in SubjectDropdown)
					{
						<option value="@item.Subject">@item.Subject</option>
					}
					
					</select>
				</div>

i am getting object reference not set to an instance error 
