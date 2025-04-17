 private void SendSmsToUser(string userContact, string otp)
 {
     string connectionString = GetRFIDConnectionString();

     string query = @"
 SELECT Phone
 FROM UserLoginDB.dbo.App_EmployeeMaster
 WHERE Pno = @pno ";

     string inoutValue = "";

     using (var connection = new SqlConnection(connectionString))
     {
         inoutValue = connection.QuerySingleOrDefault<string>(query, new { pno = userContact});
     }


     WebRequest request = WebRequest.Create("https://enterprise.smsgupshup.com/GatewayAPI/rest?method=SendMessage&send_to=" + strMobNo + "&msg=Dear Employee, You have punched at " + strTime + " on " + strdate + " -Tata Steel UISL (JUSCO)&msg_type=TEXT&userid=2000060285&auth_scheme=plain&password=jusco&v=1.1&format=text");

     request.Proxy = WebRequest.DefaultWebProxy;
     request.UseDefaultCredentials = true;
     request.Proxy.Credentials = new NetworkCredential("###", "###");

 }
