this is my Endate
string END_DATE = ds1.Tables[0].Rows[0]["END_DATE"].ToString();      

this is selected month and year , in this i want to check that first user select enddate then select year and month ,i want that if user select endate as June 2024 and then it select month and year as February 2024 then i want to check date between them like February, march, April , may and June like this and send into this for checking one by one 


 bool billfound = await IsBillAlreadyExistsAsyncPermanent(WoNo, VendorCode, Month, Year, END_DATE);




 string Year = ((DropDownList)nil_wo_Record.Rows[0].FindControl("TEMPORARY_YEAR")).Text;

string Month = ((DropDownList)nil_wo_Record.Rows[0].FindControl("TEMPORARY_MONTH")).Text;


  public async Task<string> GetAccess_TokenAsync()
        {
            using (var client = new HttpClient())
            {
               var tokenUrl = "http://10.0.168.68/DBSTS_PI_SERVICE/token";

                var fromData = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", "159631CC" },
                    { "password", "159631CC@123" }

                };

                var content = new FormUrlEncodedContent(fromData);
                var response = await
                    client.PostAsync(tokenUrl, content);
                if(!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception("Token request failed:" + error);

                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenobj = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

                return tokenobj["access_token"];

               }
        }


        public async Task<bool> IsBillAlreadyExistsAsyncPermanent(string WoNo, string VendorCode, string Month, string Year, string Endadte)
        {
            string apiUrl = $"http://10.0.168.68/DBSTS_PI_SERVICE/api/CommonApi/GetBillingData?won={WoNo}&vendorCode={VendorCode}&month={Month}&year={Year}&Endadte={Endadte}";

            using (var client = new HttpClient())
            {
                string token = await GetAccess_TokenAsync();  // ? Await here

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("API_KEY", "JF31V4OJAYRQZWZ5G7LU");  // Your actual API key

                var result = await client.GetAsync(apiUrl);
                var jsonResponse = await result.Content.ReadAsStringAsync();

                var billingResponse = JsonConvert.DeserializeObject<BillingApiResponse>(jsonResponse);

                if (billingResponse != null && billingResponse.Data != null && billingResponse.Data.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }


 protected async void btnSave_Click(object sender, EventArgs e)
        {

            nil_wo_Record.UnbindData();


            string VendorCode = ((TextBox)nil_wo_Record.Rows[0].FindControl("V_CODE")).Text;
            string WoNo = ((TextBox)nil_wo_Record.Rows[0].FindControl("WO_NO")).Text;
            string Year = ((DropDownList)nil_wo_Record.Rows[0].FindControl("TEMPORARY_YEAR")).Text;
            string Month = ((DropDownList)nil_wo_Record.Rows[0].FindControl("TEMPORARY_MONTH")).Text;
            string No_Work = ((DropDownList)nil_wo_Record.Rows[0].FindControl("NO_WORK")).Text;

            BL_NIL_Workoder blobj1 = new BL_NIL_Workoder();
            DataSet ds1 = blobj1.GetWo_Validity(VendorCode, WoNo);

            string END_DATE = ds1.Tables[0].Rows[0]["END_DATE"].ToString();


            if (No_Work == "Temporary")
            {
                bool billfound = await IsBillAlreadyExistsAsync(WoNo, VendorCode, Month, Year);


                if (billfound)
                {
                    btnSave.Enabled = false;
                    MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Errors, "Bill against this workOrder No. has already been raised, Can't Process for Nil Entry!!!");
                    return;
                }


            }

            if(No_Work == "Permanent")
            {
                

               bool billfound = await IsBillAlreadyExistsAsyncPermanent(WoNo, VendorCode, Month, Year, END_DATE);


                if (billfound)
                {
                    btnSave.Enabled = false;
                    MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Errors, "Bill against this workOrder No. has already been raised, Can't Process for Nil Entry!!!");
                    return;
                }
            }
}
