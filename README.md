now this is to fetch the Location of that Position 

 using (SqlConnection conn = new SqlConnection(connectionString))
 {
     conn.Open();
     string query = @"
 SELECT ps.Worksite 
 FROM TSUISLRFIDDB.DBO.App_Position_Worksite AS ps
 INNER JOIN TSUISLRFIDDB.DBO.App_Emp_position AS es ON es.position = ps.position
 WHERE es.Pno = @UserId";

     using (SqlCommand cmd = new SqlCommand(query, conn))
     {
         cmd.Parameters.AddWithValue("@UserId", pno);
         var result = cmd.ExecuteScalar();
         if (result != null)
         {
             attendanceLocation = result.ToString();
         }
     }
 }


 ViewBag.AttendanceLocation = attendanceLocation;

in this also i want to fetch using ID that is comma separated and Shows the Location 
