public async Task<IActionResult> PositionMaster(Guid? id, AppPositionWorksite appPosition, int page = 1, string searchValue = "", string searchType = "")
{
    var UserId = HttpContext.Request.Cookies["Session"];

    if (!string.IsNullOrEmpty(UserId))
    {
        if (UserId != "842015" && UserId != "151514")
        {
            return RedirectToAction("Login", "User");
        }

        int pageSize = 5;
        var query = context.AppPositionWorksites.AsQueryable();

        if (!string.IsNullOrEmpty(searchValue))
        {
            if (searchType == "Pno")
            {
                // Get the position associated with the given Pno
                var position = context.AppEmpPositions
                    .Where(e => e.Pno == searchValue)
                    .Select(e => e.Position)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(position))
                {
                    query = query.Where(p => p.Position == position);
                }
            }
            else if (searchType == "Position")
            {
                query = query.Where(p => p.Position == searchValue);
            }
        }

        var pagedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var totalCount = query.Count();

        ViewBag.pList = pagedData;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.SearchValue = searchValue;
        ViewBag.SearchType = searchType;

        var WorksiteList = context.AppLocationMasters
            .Select(x => new SelectListItem
            {
                Value = x.WorkSite,
                Text = x.WorkSite
            }).Distinct().ToList();

        ViewBag.WorksiteDDList = WorksiteList;

        var WorksiteList2 = context.AppEmpPositions
            .Select(x => new SelectListItem
            {
                Value = x.Position.ToString(),
                Text = x.Position.ToString()
            }).ToList();

        ViewBag.PositionDDList = WorksiteList2;

        if (id.HasValue)
        {
            var model = await context.AppPositionWorksites.FindAsync(id.Value);
            if (model == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = model.Id,
                position = model.Position,
                worksite = model.Worksite,
                createdby = UserId,
                createdon = model.CreatedOn,
            });
        }

        return View(new AppPositionWorksite());
    }
    else
    {
        return RedirectToAction("Login", "User");
    }
}




i have this form to get filter
 <form method="get" action="@Url.Action("PositionMaster")" style="display:flex;">
     <div class="form-group row">
         <div class="col-sm-2">
             <label class="control-label">Search </label>
         </div>
         <div class="col-md-4 val">
             <input type="text" name="searchValue" class="form-control" value="@ViewBag.SearchValue" placeholder="Enter search value..." autocomplete="off" />
         </div>
         <div class="col-sm-5 d-flex justify-content-end srch2">
             <select class="form-control custom-select" name="searchType">
                 @if (ViewBag.SearchType == "Pno")
                 {
                     <option value="Pno" selected>Search by P.No.</option>
                 }
                 else
                 {
                     <option value="Pno">Search by P.No.</option>
                 }
                 @if (ViewBag.SearchType == "Position")
                 {
                     <option value="Position" selected>Search by Position</option>
                 }
                 else
                 {
                     <option value="Position">Search by Position</option>
                 }
             </select>
         </div>
         <div class="col-sm-1 srchbtn">
             <button type="submit" class="btn btn-primary">Search</button>
         </div>
     </div>


 </form>

and this is my controller method 
 public async Task<IActionResult> PositionMaster(Guid? id, AppPositionWorksite appPosition, int page = 1, string searchString = "", string position = "")
 {
     
         var UserId = HttpContext.Request.Cookies["Session"];

         if (!string.IsNullOrEmpty(UserId))
         {
             //var user = HttpContext.Session.GetString("Session");

             if (UserId != "842015" && UserId != "151514")
             {
                 return RedirectToAction("Login", "User");
             }


             int pageSize = 5;
             var query = context.AppPositionWorksites.AsQueryable();


             if (!string.IsNullOrEmpty(searchString))
             {
                 query = query.Where(p => p.Position.ToString().Contains(searchString));
             }

           
         var pagedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
             var totalCount = query.Count();

             ViewBag.pList = pagedData;
             ViewBag.CurrentPage = page;
             ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
             ViewBag.SearchString = searchString;

             var WorksiteList = context.AppLocationMasters
                 .Select(x => new SelectListItem
                 {
                     Value = x.WorkSite,
                     Text = x.WorkSite
                 }).Distinct().ToList();

             ViewBag.WorksiteDDList = WorksiteList;

             var WorksiteList2 = context.AppEmpPositions
                .Select(x => new SelectListItem
                {
                    Value = x.Position.ToString(),
                    Text = x.Position.ToString()
                }).ToList();

             ViewBag.PositionDDList = WorksiteList2;

             if (id.HasValue)
             {
                 var model = await context.AppPositionWorksites.FindAsync(id.Value);
                 if (model == null)
                 {
                     return NotFound();
                 }

                 return Json(new
                 {
                     id = model.Id,
                     position = model.Position,
                     worksite = model.Worksite,
                     createdby = UserId,
                     createdon = model.CreatedOn,
                 });
             }




             return View(new AppPositionWorksite());
         }
       
     
     else
     {
         return RedirectToAction("Login", "User");
     }

 }

if search value is Pno then filter according to this 
select * from App_Position_Worksite where Position = (select Position from App_Emp_Position where Pno = '159445')
and if search value is position then filter according to this 
select * from App_Position_Worksite where Position='51111819'
