this is my form  <main class="content">

     <section class="mt-4">
         <div class="row">
             <div class="col-12 mb-4">
                 <div class="card card-body border-0 shadow mb-2">

                     <div class="row">
                         <div class="col-sm-8">
                             <form asp-action="Suppliers" method="get">
                                 <div class="row col-md-12 mb-4">
                                     <div class="">

                                         <div class="search">
                                             <input placeholder="Search..." name="searchString" value="@ViewBag.SearchString" type="text">
                                             <button type="submit">Go</button>
                                         </div>
                                     </div>
                                 </div>
                             </form>
                         </div>
                         <div class="col-sm-4">
                             <div class="col-sm-12 d-flex justify-content-end">
                                 <button type="submit" class="btn btn-primary" id="newButton">New</button>
                             </div>
                         </div>

                         <table class="table table-bordered" id="myTable">
                             <thead class="table" style="background-color:#90AEAD;color:white;">
                                 <tr>
                                     
                                     <th>Customer Code</th>
                                     <th>Name</th>
                                     <th>Contact</th>
                                     <th>Transaction Type</th>
                                     <th>Transaction Date</th>
                                     


                                 </tr>
                             </thead>
                             <tbody style="">
                                 @if (ViewBag.ListData2 != null)
                                 {
                                     @foreach (var item in ViewBag.ListData2)
                                     {
                                         <tr>
                                             <td>
                                                 <a asp-action="Transactions" asp-route-id="@item.Id" class="text-primary refNoLink" style="text-decoration:none;background-color:yellow;font-weight:bolder;"
                                                    data-id="@item.Id" data-name="@item.CustomerName" data-Balance="@item.TotalAmount" data-Desc="@item.Description" data-phone="@item.Phone" data-TransactionType="@item.Type" data-TransactionDate="@item.TransactionDate">
                                                     @item.CustomerCode 
                                                 </a>
                                             </td>

                                             <td>@item.CustomerName</td>
                                             <td>@item.Phone</td>
                                             <td>@item.Type</td>
                                             <td>@item.TransactionDate</td>

                                         </tr>
                                     }
                                 }
                                 else
                                 {
                                     <tr>
                                         <td colspan="4">No data available</td>
                                     </tr>
                                 }
                             </tbody> 
                         </table>
                     </div>
                 </div>
             </div>
         </div>
         <div class="row" id="TransactionForm" style="display:none;">
             <div class="col-12 mb-4">
                 <div class="card card-body border-0 shadow mb-4">
                     <div class="card-body">
                         <h2 class="h5 mb-4">Personal Information</h2>
                         <hr />
                         <form asp-action="SubmitTransactions" asp-controller="Civil" method="post">
                             <input type="hidden" asp-for="Id" id="TransactionId" name="Id" />

                             <div class="row">
                                 <div class="col-md-6 mb-3">
                                     <label for="Phone">Contact</label>
                                     <input asp-for="Phone" class="form-control form-control-sm" id="Contact" type="text" placeholder="Enter Customers Contact" maxlength="10">
                                 </div>
                                 <div class="col-md-6 mb-3">
                                     <label for="CustomerName">Customer Name</label>
                                     <input asp-for="CustomerName" class="form-control form-control-sm" id="Name" type="text" readonly>
                                 </div>
                               
                                 <br />
                                 <table class="table table-bordered mb-2" id="productTable">
                                     <thead style="color: #fff; background: #2d203a;">
                                         <tr>
                                             <th>Product</th>
                                             <th>Price</th>
                                             <th>Quantity</th>
                                             <th>Total Price</th>
                                             <th>Action</th>
                                         </tr>
                                     </thead>
                                     <tbody id="rows-container3">
                                         <tr data-index="0">
                                             <td>
                                                 <select class="form-control form-control-sm product-dropdown" name="AppProducts[0].Product" onchange="fetchPrice(this)">
                                                     <option value="">-- Select Product --</option>
                                                     @foreach (var product in ViewBag.ProductsDD)
                                                     {
                                                         <option value="@product.Value" data-price="@product.Price">@product.Text</option>
                                                     }
                                                 </select>
                                             </td>
                                             <td>
                                                 <input class="form-control form-control-sm price-input" type="text" name="AppProducts[0].Price" readonly />
                                             </td>
                                             <td>
                                                 <input class="form-control form-control-sm quantity-input" type="number" name="AppProducts[0].Quantity" min="1" oninput="calculateTotal(this)" />
                                             </td>
                                             <td>
                                                 <input class="form-control form-control-sm total-price-input" type="text" name="AppProducts[0].TotalPrice" readonly />
                                             </td>
                                             <td>
                                                 <button type="button" class="btn btn-primary add-row-btn" onclick="addRow()">
                                                     <i class="fa fa-plus fa-lg"></i> Add Row
                                                 </button>
                                             </td>
                                         </tr>
                                     </tbody>
                                 </table>

                                 

                                 <input asp-for="CustomerCode" id="CustomerCode" class="form-control form-control-sm" type="hidden">
                                 <input asp-for="CustomerId" id="CustomerId" class="form-control form-control-sm" type="hidden">
                                 <div class="col-md-6 mb-3">
                                     <label for="TransactionType">Transaction Type</label>
                                     <select class="form-control form-control-sm" id="Type" asp-for="Type">
                                         <option value="">Select Transaction Type</option>
                                         <option value="Credit">Credit</option>
                                         <option value="Debit">Debit</option>
                                     </select>
                                 </div>
                                 <div class="col-md-6 mb-3">
                                     <label for="Email">Total Balance</label>
                                     <input class="form-control form-control-sm" id="TotalAmount" asp-for="TotalAmount" readonly>
                                 </div>
                             </div>

                             <h2 class="h5 my-4">Description</h2>
                             <hr />
                             <div class="row">
                                 <div class="col-sm-9 mb-3">
                                     <label for="address">Description</label>
                                     <textarea class="form-control form-control-sm" asp-for="Description" id="Description" type="text" placeholder="Enter Description">

                                     </textarea> 
                                 </div>
                             </div>


                             <div class="row">
                                 <div class="col-md-6 mb-3">
                                     <label for="TransactionDate">Transaction Date</label>
                                     <div class="input-group">
                                         <span class="input-group-text">
                                             <i class="fa fa-calendar"></i>
                                         </span>
                                         <input class="form-control form-control-sm datepicker-input" id="TransactionDate" type="text" placeholder="dd/mm/yyyy" required>
                                     </div>
                                 </div>
                             </div>


                             <div class="mt-3">
                                 <button class="btn btn-secondary" id="submitButton" type="submit">Submit</button>
                                
                                 <button class="button" id="deleteButton" style="display: none;">
                                     <svg viewBox="0 0 448 512" class="svgIcon"><path d="M135.2 17.7L128 32H32C14.3 32 0 46.3 0 64S14.3 96 32 96H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H320l-7.2-14.3C307.4 6.8 296.3 0 284.2 0H163.8c-12.1 0-23.2 6.8-28.6 17.7zM416 128H32L53.2 467c1.6 25.3 22.6 45 47.9 45H346.9c25.3 0 46.3-19.7 47.9-45L416 128z"></path></svg>
                                 </button>

                             </div>
                         </form>

                     </div>
                 </div>
             </div>
         </div>


     </section>

 </main> this is  my all js and jquery    <script>
      $(document).ready(function () {
       $("#Contact").on("input", function () {
           var phone = $(this).val().trim();
           var url = "@Url.Action("GetCustomerByPhone", "Civil")"; 

           
           if (phone.length === 10) {
               $.ajax({
                   url: url,
                   type: "GET",
                   data: { phone: phone },
                   success: function (response) {
                       if (response) {
                           $("#Name").val(response.name);
                           $("#CustomerCode").val(response.customerCode);
                           $("#CustomerId").val(response.id);
                       } else {
                           clearCustomerFields();
                       }
                   }
               });
           } else {
               clearCustomerFields(); 
           }
       });

       
   });
   </script>


   <script>

       document.addEventListener("DOMContentLoaded", function () {
           var newButton = document.getElementById("newButton");
           var customerForm = document.getElementById("TransactionForm");
           var refNoLinks = document.querySelectorAll(".refNoLink");
           var deleteButton = document.getElementById("deleteButton");

        
           newButton.addEventListener("click", function () {
               customerForm.style.display = "block";

              
               document.getElementById("TransactionId").value = "";
               document.getElementById("Name").value = "";
               document.getElementById("Contact").value = "";
               document.getElementById("Type").value = "";
               document.getElementById("CustomerCode").value = "";
               document.getElementById("CustomerId").value = "";
               document.getElementById("TransactionDate").value = "";
               document.getElementById("Description").value = "";
               document.getElementById("TotalAmount").value = "";

               deleteButton.style.display = "none"; 
           });

           
           refNoLinks.forEach(link => {
               link.addEventListener("click", function (event) {
                   event.preventDefault();
                   customerForm.style.display = "block";

                   
                   document.getElementById("TransactionId").value = this.getAttribute("data-id");
                   document.getElementById("CustomerCode").value = this.textContent.trim();
                   document.getElementById("CustomerId").value = this.textContent.trim();
                   document.getElementById("Name").value = this.getAttribute("data-name");
                   document.getElementById("Contact").value = this.getAttribute("data-phone");
                   document.getElementById("Type").value = this.getAttribute("data-TransactionType");
                   document.getElementById("TransactionDate").value = this.getAttribute("data-TransactionDate");
                   document.getElementById("Description").value = this.getAttribute("data-Desc");
                   document.getElementById("TotalAmount").value = this.getAttribute("data-Balance");

                   deleteButton.style.display = "inline-block"; 
               });
           });

          
           deleteButton.addEventListener("click", function () {
               var transactionId = document.getElementById("TransactionId").value;
               if (transactionId) {
                   if (confirm("Are you sure you want to delete this transaction?")) {
                       fetch(`/Civil/DeleteTransaction/${transactionId}`, {
                           method: "POST"
                       }).then(response => {
                           if (response.ok) {
                               alert("Transaction deleted successfully!");
                               location.reload();
                           } else {
                               alert("Error deleting transaction.");
                           }
                       });
                   }
               }
           });
       });


       
   </script>
   <script>
       
       var productsData = @Html.Raw(productsJson);
       console.log(productsData); 

      
       function addRow() {
           var index = $("#productTable tbody tr").length; 
           var newRow = `
               <tr data-index="${index}">
                   <td>
                       <select class="form-control form-control-sm product-dropdown" name="AppProducts[${index}].Product">
                           ${generateProductOptions()}
                       </select>
                   </td>
                   <td>
                       <input class="form-control form-control-sm price-input" type="text" name="AppProducts[${index}].Price" readonly />
                   </td>
                   <td>
                       <input class="form-control form-control-sm quantity-input" type="number" name="AppProducts[${index}].Quantity" min="1" oninput="calculateTotal(this)" />
                   </td>
                   <td>
                       <input class="form-control form-control-sm total-price-input" type="text" name="AppProducts[${index}].TotalPrice" readonly />
                   </td>
                   <td>
                       <i class="fa fa-times fa-lg remove-row" aria-hidden="true" style="color:red;cursor:pointer;" onclick="removeRow(this)"></i>
                   </td>
               </tr>
           `;

           $("#productTable tbody").append(newRow);
       }

       function generateProductOptions() {
           var options = '<option value="">-- Select Product --</option>';
           productsData.forEach(product => {
              
               options += `<option value="${product.Value}" data-price="${product.Price}">${product.Text}</option>`;
           });
           return options;
       }

       $(document).on('change', '.product-dropdown', function () {
           fetchPrice(this); 
       });

       function fetchPrice(selectElement) {
           var selectedOption = $(selectElement).find(':selected');
           var price = selectedOption.attr('data-price');
           var row = $(selectElement).closest('tr');

           row.find('.price-input').val(price || ''); 
           row.find('.quantity-input').val('');
           row.find('.total-price-input').val('');
       }

       function calculateTotal(inputElement) {
           var row = $(inputElement).closest('tr');
           var price = parseFloat(row.find('.price-input').val()) || 0;
           var quantity = parseFloat($(inputElement).val()) || 0;
           var totalPrice = price * quantity;
           row.find('.total-price-input').val(totalPrice.toFixed(2));

           updateTotalBalance(); // Call function to update total balance
       }

       function updateTotalBalance() {
           var totalBalance = 0;

           $(".total-price-input").each(function () {
               totalBalance += parseFloat($(this).val()) || 0;
           });

           $("#TotalAmount").val(totalBalance.toFixed(2)); // Update total balance field
       }

       // Ensure removeRow updates the total balance
       function removeRow(element) {
           $(element).closest("tr").remove();
           updateTotalBalance(); // Recalculate the total balance after row removal
       }

   </script>
  here i want that when i click on new button it opens with clear fields and when i click on refno it opens with the existing records along with products details 





var emailList = await context.AppMailEmployeeMasters
    .Where(e => e.EmailId != null)
    .Select(e => e.EmailId.Trim().ToLower()) // Normalize email casing
    .Distinct()
    .ToListAsync();



for (int i = 0; i < emailList.Count; i += batchSize)
{
    var batch = emailList.Skip(i).Take(batchSize).ToList();
    await emailService.SendApprovedEmailAsync(batch, "", "", subject, msg);
    
    await Task.Delay(1000).ConfigureAwait(false); // Small delay to avoid SMTP issues
}



public async Task SendApprovedEmailAsync(List<string> toEmails, string ccEmail, string bccEmail, string subject, string message)
{
    var emailSettings = _configuration.GetSection("EmailSettings");

    var email = new MimeMessage();
    email.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));

    // Ensure unique email IDs to prevent duplicates
    var uniqueEmails = toEmails.Distinct().ToList();

    foreach (var toEmail in uniqueEmails)
    {
        email.To.Add(new MailboxAddress(toEmail, toEmail));
    }

    if (!string.IsNullOrEmpty(ccEmail))
    {
        email.Cc.Add(new MailboxAddress(ccEmail, ccEmail));
    }
    if (!string.IsNullOrEmpty(bccEmail))
    {
        email.Bcc.Add(new MailboxAddress(bccEmail, bccEmail));
    }

    email.Subject = subject;
    email.Body = new TextPart(TextFormat.Html)
    {
        Text = message
    };

    using (var smtp = new SmtpClient())
    {
        try
        {
            await smtp.ConnectAsync(emailSettings["SMTPServer"], int.Parse(emailSettings["SMTPPort"]), MailKit.Security.SecureSocketOptions.StartTls).ConfigureAwait(false);
            await smtp.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]).ConfigureAwait(false);
            await smtp.SendAsync(email).ConfigureAwait(false);
            await smtp.DisconnectAsync(true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Email sending failed: {ex.Message}");
        }
    }
}




When I hit a break point and f10 it sometimes goes to the next line but then just starts jumping all over the code or jumps back with "the process or thread has changed since the last step".
may be this is the reason of it send emails double
