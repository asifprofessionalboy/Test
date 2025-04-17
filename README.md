private void SendSmsToUser(string userContact, string otp)
{
    try
    {
        string connectionString = GetRFIDConnectionString();

        string query = @"
        SELECT Phone
        FROM UserLoginDB.dbo.App_EmployeeMaster
        WHERE Pno = @pno ";

        string phoneNumber = "";

        using (var connection = new SqlConnection(connectionString))
        {
            phoneNumber = connection.QuerySingleOrDefault<string>(query, new { pno = userContact });
        }

        if (string.IsNullOrEmpty(phoneNumber))
        {
            Console.WriteLine("Phone number not found for user.");
            return;
        }

        string strdate = DateTime.Now.ToString("yyyy-MM-dd");
        string strTime = DateTime.Now.ToString("HH:mm:ss");

        string message = $"Your OTP for attendance is {otp}. Generated at {strTime} on {strdate}. -Tata Steel UISL (JUSCO)";
        string smsUrl = $"https://enterprise.smsgupshup.com/GatewayAPI/rest?method=SendMessage&send_to={phoneNumber}&msg={Uri.EscapeDataString(message)}&msg_type=TEXT&userid=2000060285&auth_scheme=plain&password=jusco&v=1.1&format=text";

        WebRequest request = WebRequest.Create(smsUrl);
        request.Proxy = WebRequest.DefaultWebProxy;
        request.UseDefaultCredentials = true;
        request.Proxy.Credentials = new NetworkCredential("###", "###");

        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                Console.WriteLine("SMS Sent: " + result);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error sending SMS: " + ex.Message);
    }
}

 
 
 


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
