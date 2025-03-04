this is my two query 

select PDE_PUNCHTIME as PunchIn from vbdesk_ACPL.dbo.T_TRPUNCHDATA_EARS  where PDE_INOUT like '%I%' and PDE_PunchDate >='2025-04-03'

select PDE_PUNCHTIME as PunchOut from T_TRPUNCHDATA_EARS  where PDE_INOUT like '%O%' and PDE_PunchDate >='2025-04-03'  

i want to use these 2 query in my report , one to show PunchIn And One is to show PunchOut

 private DataTable GetStudentData()
        {
           
            string query = "select * from T_TRPUNCHDATA_EARS where PDE_PunchDate >='2025-04-03'";

           
            DataTable dt = new DataTable();

           
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }
