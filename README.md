select DE.Pno,Emp.DepartmentName,DE.DateAndTime,DE.PunchIn_FailedCount,DE.PunchOut_FailedCount from App_FaceVerification_Details As DE 
INNER JOIN UserLoginDB.dbo.App_EmployeeMaster AS Emp
ON DE.Pno COLLATE DATABASE_DEFAULT = Emp.Pno COLLATE DATABASE_DEFAULT where 
Emp.DepartmentName = 'Corporate Function' And CAST(DateAndTime as Date) = '28-05-2025' Order By Emp.DepartmentName



protected void Page_Load(object sender, EventArgs e)
        {
            

                if (!IsPostBack)
                {
                    string today = DateTime.Now.ToString("dd/MM/yyyy");
                    Date.Text = today;
                   

                BindDepartmentDropdown();

            }

        }

         <div class="form-group col-md-4 mb-1">
                                 <label for="Date" class="m-0 mr-2 p-0 col-form-label-sm col-sm-3 font-weight-bold fs-6">Date:</label>
                                 <asp:TextBox ID="Date" runat="server" CssClass="form-control form-control-sm col-sm-8" AutoComplete="off" ToolTip="dd/MM/yyyy"></asp:TextBox>
                                    <ask:CalendarExtender ID="CalendarExtender2" runat="server" Enabled="True"  Format="dd/MM/yyyy" PopupPosition="TopRight" TargetControlID="Date" TodaysDateFormat="dd/MM/yyyy" ></ask:CalendarExtender>  
                             </div>

 getting this error 
   System.Data.SqlClient.SqlException
  HResult=0x80131904
  Message=Conversion failed when converting date and/or time from character string.
  Source=.Net SqlClient Data Provider
  StackTrace:
<Cannot evaluate the exception stack trace>
