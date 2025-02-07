document.addEventListener("DOMContentLoaded", function () {
    var newButton = document.getElementById("newButton");
    var customerForm = document.getElementById("customerForm");
    var refNoLinks = document.querySelectorAll(".refNoLink");
    var deleteButton = document.getElementById("deleteButton");

    // Open the form to create a new customer
    newButton.addEventListener("click", function () {
        customerForm.style.display = "block";
        document.getElementById("CustomerId").value = ""; // Empty GUID for new customer
        document.getElementById("CustomerName").value = "";
        document.getElementById("phone").value = "";
        document.getElementById("address").value = "";
        document.getElementById("TotalBalance").value = "";
        deleteButton.style.display = "none"; // Hide delete button for new customers
    });

    // Open the form to edit an existing customer
    refNoLinks.forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            customerForm.style.display = "block";

            document.getElementById("CustomerId").value = this.getAttribute("data-id"); // GUID
            document.getElementById("CustomerName").value = this.getAttribute("data-name");
            document.getElementById("phone").value = this.getAttribute("data-phone");
            document.getElementById("address").value = this.getAttribute("data-address");
            document.getElementById("TotalBalance").value = this.getAttribute("data-balance");

            deleteButton.style.display = "inline-block"; // Show delete button for existing customers
        });
    });

    // Handle delete action
    deleteButton.addEventListener("click", function () {
        var customerId = document.getElementById("CustomerId").value;
        if (customerId) {
            if (confirm("Are you sure you want to delete this customer?")) {
                fetch(`/Civil/DeleteCustomer/${customerId}`, {
                    method: "POST"
                }).then(response => {
                    if (response.ok) {
                        alert("Customer deleted successfully!");
                        location.reload();
                    } else {
                        alert("Error deleting customer.");
                    }
                });
            }
        }
    });
});

[HttpPost]
public IActionResult DeleteCustomer(Guid id)
{
    var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
    if (customer != null)
    {
        _context.Customers.Remove(customer);
        _context.SaveChanges();
        TempData["Message"] = "Customer deleted successfully!";
    }

    return RedirectToAction("CustomerList");
}



function DOE_Validation(fld) {
    alert("ok");
    var DOJ = document.getElementById("MainContent_EmployeeMasterFormID_Record_DOJ_0").value;
    var DOE = document.getElementById("DOE").value;

    if (!DOJ || !DOE) {
        alert("Both Date of Joining and Date of Exit are required.");
        return false;
    }

    var DOJparts = DOJ.split("/");
    var DOEparts = DOE.split("/");

    var Date_Of_Joining = new Date(DOJparts[2], DOJparts[1] - 1, DOJparts[0]);
    var Date_Of_Exit = new Date(DOEparts[2], DOEparts[1] - 1, DOEparts[0]);

    var Current_Date = new Date();

    // Check if Date of Exit is less than or equal to Date of Joining
    if (Date_Of_Exit <= Date_Of_Joining) {
        alert("Date of Exit must be greater than Date of Joining.");
        document.getElementById("DOE").value = "";
        return false;
    }

    // Check if Date of Exit is greater than Current Date
    if (Date_Of_Exit > Current_Date) {
        alert("Date of Exit can't be greater than the Current Date.");
        document.getElementById("DOE").value = "";
        return false;
    }

    return true;
}

 
 
 function DOE_Validation(fld) {

          alert("ok");
          var DOJ = document.getElementById("MainContent_EmployeeMasterFormID_Record_DOJ_0").value;
          var DOE = document.getElementById("DOE").value;

          var DOJparts = DOJ.split("/");
          var DOEparts = DOE.split('/');

          var Date_Of_Joining = new Date(DOJparts[2], DOJparts[1] - 1, DOJparts[0]);
          var Date_Of_Exit = new Date(DOEparts[2], DOEparts[1] - 1, DOEparts[0]);

          //var Date_Of_Joining = new Date(parseInt(DoJparts[2], 10), parseInt(DOJparts[1], 10) - 1, parseInt(DOJparts[0], 10));
          //var Date_Of_Exit = new Date(parseInt(DOEparts[2], 10), parseInt(DOEparts[1], 10) - 1, parseInt(DOEparts[0], 10));

          alert(Date_Of_Joining);
          alert(Date_Of_Exit);
          var Current_Date = new Date();

       
          if (Date_Of_Exit >= Date_Of_Joining) {
              alert("Date of Exit Not less Than Date Of Joining.");
              document.getElementById("DOE").value = "";
              return false;
          }

          if (Date_Of_Exit > Current_Date) {
              alert("Date of Exit can't be greater than Current Date.");
              document.getElementById("DOE").value = "";
              return false;
          }

          return true;
      }
