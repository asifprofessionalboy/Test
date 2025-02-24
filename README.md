   public IActionResult Dashboard()
   {
       var sessionUser = HttpContext.Session.GetString("Session");
       if (sessionUser != null)
       {
           var subjects = context.AppSubjectMasters.ToList();
           ViewBag.Subjects = subjects;

         
           string query = @"
       SELECT Subject, COUNT(*) AS UnreadCount
       FROM App_Notification
       WHERE Pno = @pno AND IsViewed = 0
       GROUP BY Subject";

           string connectionString = GetConnectionString();
           using (IDbConnection connection = new SqlConnection(connectionString))
           {
               connection.Open();
               var unreadNotifications = connection.Query<(string Subject, int UnreadCount)>(
                   query,
                   new { pno = sessionUser }
               ).ToDictionary(x => x.Subject, x => x.UnreadCount);

               ViewBag.UnreadNotifications = unreadNotifications;
           }
		// List of subjects inside the popup
		
		return View();
       }
       else
       {
           return RedirectToAction("Login", "User");
       }
   }


this is my dashboard , i want that inside of the popup1 content, i want that count that is inside popup1 and shows on popup1 in notification badge
