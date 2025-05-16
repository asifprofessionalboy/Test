<td>
    @(item.ApprovedYn == true ? "Approved" :
      item.ApprovedYn == false ? "Rejected" : "Pending")
</td>
<td>
    @{
        if (item.ApprovedYn == true)
        {
            @:Approved
        }
        else if (item.ApprovedYn == false)
        {
            @:Rejected
        }
        else
        {
            @:Pending
        }
    }
</td>



using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

public async Task<IActionResult> CorrectionOfAttendance(Guid? id, int page = 1, string Date = "")
{
    var session = HttpContext.Request.Cookies["Session"];
    var userName = HttpContext.Request.Cookies["UserName"];

    if (string.IsNullOrEmpty(session) || string.IsNullOrEmpty(userName))
    {
        return RedirectToAction("Login", "User");
    }

    ViewBag.Pno = session;
    ViewBag.Name = userName;

    string connectionString = GetRFIDConnectionString();

    // 🔹 Get Approver Pno
    string sqlApproverQuery = @"
        SELECT ema_reporting_to_pno AS Apno 
        FROM SAPHRDB.dbo.T_EMPL_ALL 
        WHERE ema_perno = @Pno 
        ORDER BY Apno";

    string approverPno = "";
    using (var connection = new SqlConnection(connectionString))
    {
        approverPno = await connection.QuerySingleOrDefaultAsync<string>(sqlApproverQuery, new { Pno = session });
    }
    ViewBag.Approver = approverPno;

    // 🔹 Get Department
    string sqlDeptQuery = @"
        SELECT DISTINCT Emp.DepartmentName
        FROM INNOVATIONDB.dbo.App_Login AS Inn
        INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
            ON Inn.UserId COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
        WHERE Inn.UserId = @Pno";

    string department = "";
    using (var connection = new SqlConnection(connectionString))
    {
        department = await connection.QuerySingleOrDefaultAsync<string>(sqlDeptQuery, new { Pno = session });
    }

    // 🔹 Get HOD
    string sqlHODQuery = @"
        SELECT DISTINCT App.Pno
        FROM INNOVATIONDB.dbo.App_Login AS Inn
        INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
            ON Inn.UserId COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
        INNER JOIN App_ApproverMaster AS App
            ON Emp.DepartmentName COLLATE DATABASE_DEFAULT = App.DepartmentName COLLATE DATABASE_DEFAULT
        WHERE App.DepartmentName = @Pno";

    string hodPno = "";
    using (var connection = new SqlConnection(connectionString))
    {
        hodPno = await connection.QuerySingleOrDefaultAsync<string>(sqlHODQuery, new { Pno = department });
    }
    ViewBag.HOD = hodPno;

    // 🔹 Filtering and Pagination Logic
    int pageSize = 5;

    var filteredQuery = context.AppCoas.Where(x => x.Pno == session).AsQueryable();

    if (DateTime.TryParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
    {
        filteredQuery = filteredQuery.Where(a => a.Cdate.HasValue && a.Cdate.Value.Date == parsedDate.Date);
    }

    var pagedData = await filteredQuery
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var totalCount = await filteredQuery.CountAsync();

    ViewBag.ListData2 = pagedData;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    ViewBag.subject = Date;

    AppCoa viewModel = null;
    if (id.HasValue)
    {
        viewModel = await context.AppCoas.FirstOrDefaultAsync(a => a.Id == id);
    }

    return View(viewModel);
}




this is my controller code 

        public async Task<IActionResult> CorrectionOfAttendance(Guid? id, int page = 1, string Date = "")
        {
            var session = HttpContext.Request.Cookies["Session"];
            var userName = HttpContext.Request.Cookies["UserName"];

            if (string.IsNullOrEmpty(session) || string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "User");
            }

            ViewBag.Pno = session;
            ViewBag.Name = userName;

            string connectionString = GetRFIDConnectionString();

            string query = @"
select ema_reporting_to_pno as Apno ,ema_Dotted_Pno as adminPno,EMA_ENAME,EMA_EMPL_SGRADE 

from SAPHRDB.dbo.T_EMPL_ALL where ema_perno=@Pno order by Apno";

            string Pno = "";

            using (var connection = new SqlConnection(connectionString))
            {
                Pno = connection.QuerySingleOrDefault<string>(query, new { Pno = session });
            }

            ViewBag.Approver = Pno;

            string query2 = @"
SELECT DISTINCT Emp.DepartmentName
FROM INNOVATIONDB.dbo.App_Login AS Inn
INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
    ON Inn.UserId COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
    where Inn.UserId = @Pno";

            string Department = "";

            using (var connection = new SqlConnection(connectionString))
            {
                Department = connection.QuerySingleOrDefault<string>(query, new { Pno = session });
            }

            string query3 = @"
    SELECT DISTINCT App.Pno
FROM INNOVATIONDB.dbo.App_Login AS Inn
INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
    ON Inn.UserId COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT
INNER JOIN App_ApproverMaster AS App
    ON Emp.DepartmentName COLLATE DATABASE_DEFAULT = App.DepartmentName COLLATE DATABASE_DEFAULT
where App.DepartmentName = @Pno";

            string PNo = "";

            using (var connection = new SqlConnection(connectionString))
            {
                PNo = connection.QuerySingleOrDefault<string>(query, new { Pno = Department });
            }

            ViewBag.HOD = PNo;

            int pageSize = 5;
            var query4 = context.AppCoas.Where(x => x.Pno== session).AsQueryable();

            if (DateTime.TryParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                query = query4.Where(a => a.Cdate.HasValue && a.Cdate.Value.Date == parsedDate.Date);
            }





            var pagedData = await query4.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalCount = query4.Count();

            ViewBag.ListData2 = pagedData;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.subject = Date;


            AppCoa viewModel = null;

            if (id.HasValue)
            {
                viewModel = await context.AppCoas.FirstOrDefaultAsync(a => a.Id == id);
            }

            return View(viewModel);



            
        }
