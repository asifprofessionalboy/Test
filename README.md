string attendanceLocation = "";

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
            var worksiteIds = result.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim())
                .ToList();

            if (worksiteIds.Count > 0)
            {
                // Convert to a SQL IN clause format: 'guid1','guid2',...
                string idsForQuery = string.Join(",", worksiteIds.Select(id => $"'{id}'"));

                string nameQuery = $@"
                    SELECT WorkSite 
                    FROM TSUISLRFIDDB.DBO.App_Location_Master
                    WHERE Id IN ({idsForQuery})";

                using (SqlCommand nameCmd = new SqlCommand(nameQuery, conn))
                {
                    using (SqlDataReader reader = nameCmd.ExecuteReader())
                    {
                        List<string> locationNames = new List<string>();
                        while (reader.Read())
                        {
                            locationNames.Add(reader["WorkSite"].ToString());
                        }

                        attendanceLocation = string.Join(", ", locationNames);
                    }
                }
            }
        }
    }
}

ViewBag.AttendanceLocation = attendanceLocation;





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
