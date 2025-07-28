
        [HttpGet]
        public async Task<IActionResult> CoordinatorMaster(Guid? id, string searchString = "", int page = 1)
        {

            string connectionString = GetRFIDConnectionString();



            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query2 = @"
      select DepartmentName from App_DepartmentMaster";

                using (SqlCommand cmd = new SqlCommand(query2, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", pno);
                }

            }

        

                    var UserId = HttpContext.Request.Cookies["Session"];

            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "User");

            var allowedPnos = context.AppPermissionMasters.Select(x => x.Pno).ToList();

            if (!allowedPnos.Contains(UserId))
                return RedirectToAction("Login", "User");

            ViewBag.CreatedBy = UserId;

            // Dropdown for Pno from App_EmployeeMaster
            ViewBag.PnoList = context1.AppEmployeeMasters
                .Select(e => new SelectListItem
                {
                    Value = e.Pno,
                    Text = e.Pno
                })
                .ToList();

            // Dropdown for DeptName from DepartmentMaster
            ViewBag.DeptList = context1.
                .Select(d => new SelectListItem
                {
                    Value = d.DeptName,
                    Text = d.DeptName
                })
                .ToList();

            // Rest of your existing logic...
            int pageSize = 5;
            var query = context.AppCoordinatorMasters.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(c => c.Pno.Contains(searchString));

            var data = query.OrderBy(c => c.Pno).ToList();
            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.pList = pagedData;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            ViewBag.SearchString = searchString;

            if (id.HasValue)
            {
                var model = await context.AppCoordinatorMasters.FindAsync(id.Value);
                if (model == null)
                    return NotFound();

                return Json(new
                {
                    id = model.Id,
                    pno = model.Pno,
                    dept = model.DeptName,
                    createdby = model.CreatedBy,
                    createdon = model.CreatedOn
                });
            }

            return View(new AppCoordinatorMaster());
        }


