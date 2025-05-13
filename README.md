var worksiteDictionary = context.AppLocationMasters
    .ToDictionary(x => x.Id.ToString().ToLower(), x => x.WorkSite);

foreach (var item in pagedData)
{
    if (!string.IsNullOrWhiteSpace(item.Worksite))
    {
        var ids = item.Worksite
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim().ToLower())
            .ToList();

        var names = ids
            .Where(id => worksiteDictionary.ContainsKey(id))
            .Select(id => worksiteDictionary[id])
            .ToList();

        item.Worksite = names.Count > 0 ? string.Join(", ", names) : "(Invalid Worksite IDs)";
    }
    else
    {
        item.Worksite = "(No Worksite)";
    }
}




public async Task<IActionResult> PositionMaster(Guid? id, AppPositionWorksite appPosition, int page = 1, string searchValue = "")
{
    var UserId = HttpContext.Request.Cookies["Session"];

    if (string.IsNullOrEmpty(UserId) || (UserId != "151515" && UserId != "151514" && UserId != "155478"))
    {
        return RedirectToAction("Login", "User");
    }

    int pageSize = 5;
    var query = context.AppPositionWorksites.AsQueryable();

    var position = context.AppEmpPositions
        .Where(e => e.Pno == searchValue)
        .Select(e => e.Position)
        .FirstOrDefault();

    if (!string.IsNullOrEmpty(position?.ToString()))
    {
        query = query.Where(p => p.Position == position);
    }
    else
    {
        ViewBag.ErrorMessage = "No Position found for this P.No.";
    }

    var pagedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    var totalCount = query.Count();

    // Get Location Master Dictionary
    var worksiteDictionary = context.AppLocationMasters
        .ToDictionary(x => x.Id.ToString(), x => x.WorkSite);

    // Replace Worksite GUIDs with names
    foreach (var item in pagedData)
    {
        if (!string.IsNullOrEmpty(item.Worksite))
        {
            var ids = item.Worksite.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var names = ids
                .Where(id => worksiteDictionary.ContainsKey(id))
                .Select(id => worksiteDictionary[id])
                .ToList();

            item.Worksite = string.Join(", ", names);
        }
    }

    ViewBag.pList = pagedData;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    ViewBag.SearchValue = searchValue;

    var WorksiteList = context.AppLocationMasters
        .Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
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





i have this 2 two table , one is 

Location Master and another one is Position Master , from Location Master on Dropdown it shows the Worksite Name but store Id of that Location like this 
428614DE-BCBB-4310-B13F-8080D973C6D2, D5397476-DDA4-466C-8C0E-67BC57CF83B9,E5A35ECD-D84F-4912-9DC8-D0209F5F6A03

i want to show in grid not the ID i want to show Worksite against the ID


i have this controller logic 

   public async Task<IActionResult> PositionMaster(Guid? id, AppPositionWorksite appPosition, int page = 1, string searchValue = "")
   {
       var UserId = HttpContext.Request.Cookies["Session"];

       if (!string.IsNullOrEmpty(UserId))
       {

           if (UserId != "151515" && UserId != "151514" && UserId != "155478")
           {
               return RedirectToAction("Login", "User");
           }

           int pageSize = 5;
           var query = context.AppPositionWorksites.AsQueryable();


           var position = context.AppEmpPositions
               .Where(e => e.Pno == searchValue)
               .Select(e => e.Position)
               .FirstOrDefault();

           if (!string.IsNullOrEmpty(position.ToString()))
           {
               query = query.Where(p => p.Position == position);
           }
           else
           {

               ViewBag.ErrorMessage = "No Position found for this P.No.";
           }



           var pagedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
           var totalCount = query.Count();

           ViewBag.pList = pagedData;
           ViewBag.CurrentPage = page;
           ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
           ViewBag.SearchValue = searchValue;


           var WorksiteList = context.AppLocationMasters
               .Select(x => new SelectListItem
               {
                   Value = x.Id.ToString(),
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

and this is my grid 

 <div class="col-md-12">
     <table class="table" id="myTable">
         <thead class="table" style="background-color: #d2b1ff;color: #000000;">
             <tr>
                 <th width="10%">Position</th>

                 <th>Worksite</th>

             </tr>
         </thead>
         <tbody>
             @if (ViewBag.pList != null)
             {
                 @foreach (var item in ViewBag.pList)
                 {
                     <tr>
                         <td>
                             <a href="javascript:void(0);" data-id="@item.Id" class="OpenFilledForm btn gridbtn" style="text-decoration:none;background-color:;font-weight:bolder;">
                                 @item.Position
                             </a>
                         </td>

                         <td>@item.Worksite</td>


                     </tr>
                 }
             }
             else
             {
                 <tr>
                     <td colspan="3">No data available</td>
                 </tr>
             }
         </tbody>
     </table>
 

 </div>

