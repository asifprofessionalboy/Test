$('#submitButton').click(function (e) {
    e.preventDefault();

    if (!validateForm()) {
        alert('Please fill in all required fields.');
        return;
    }

    const id = $('#LocationId').val();
    const pno = $('#Pno').val().trim();
    const position = $('#Position').val().trim();

    $.ajax({
        url: '@Url.Action("EmployeePositionMaster", "Master")',
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        data: JSON.stringify({
            Id: id,
            Pno: pno,
            Position: position,
            actionType: "Submit"
        }),
        success: function (response) {
            alert('Position saved successfully!');
            $('#formContainer').hide();
        },
        error: function (xhr, status, error) {
            alert('An error occurred while saving the locations.');
            console.error(xhr.responseText);
        }
    });
});

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EmployeePositionMaster([FromBody] AppEmpPosition appPosition)
{
    var actionType = Request.Query["actionType"].ToString() ?? "Submit"; // optional if still sending it via query or body

    if (string.IsNullOrEmpty(actionType))
    {
        return BadRequest("No action specified.");
    }

    var existingParameter = await context.AppEmpPositions.FindAsync(appPosition.Id);

    var UserId = HttpContext.Request.Cookies["Session"];
    
    if (actionType == "Submit")
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (existingParameter != null)
        {
            context.Entry(existingParameter).CurrentValues.SetValues(appPosition);
            await context.SaveChangesAsync();
            return Ok("Updated");
        }
        else
        {
            await context.AppEmpPositions.AddAsync(appPosition);
            await context.SaveChangesAsync();
            return Ok("Created");
        }
    }
    else if (actionType == "Delete")
    {
        if (existingParameter != null)
        {
            context.AppEmpPositions.Remove(existingParameter);
            await context.SaveChangesAsync();
            return Ok("Deleted");
        }
    }

    return BadRequest("Invalid action.");
}




this is my view side 

<div id="formContainer" style="display:none;">
    <form asp-action="EmployeePositionMaster" asp-controller="Master" id="form2" method="post">
        @Html.AntiForgeryToken()
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>
        <div class="card rounded-9">
            <div class="card-header text-center" style="background-color: #bbb8bf;color: #000000;font-weight:bold;">
                Location Master Entry
            </div>
            <div class="col-md-12">
                <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px;">
                    <div class="row">

                        <div class="form-group row">

                            <div class="col-sm-1 d-flex">
                                <label asp-for="Pno" class="control-label">Pno</label>
                            </div>
                            <div class="col-sm-3">
                                <input asp-for="Pno" class="form-control form-control-sm PnoInput" id="Pno" placeholder="" required autocomplete="off" />

                            </div>
                            <div class="col-sm-1 d-flex">
                                <label asp-for="Position" class="control-label">Position</label>
                            </div>
                            <div class="col-sm-2">
                                <input asp-for="Position" class="form-control form-control-sm PositionInput" id="Position" placeholder="" required autocomplete="off" />

                            </div>
                         

                        </div>

                        <input type="hidden" name="Id" value="@Model.Id" />
                        <div class="form-group row mt-2">

                            <input asp-for="Id" type="text" value="@Model.Id" id="LocationId" hidden />
                            
                        </div>
                        <input type="hidden" name="actionType" id="actionType" value="" />
                        <div class="form-group row">
                            <div class="col-sm-12 text-center">
                                <!-- Submit Button -->
                                <button type="button" id="submitButton" name="actionType" class="btn btn-primary">Submit</button>
                                <button type="button" id="deleteButton" name="actionType" class="btn btn-danger">Delete</button>

                            </div>
                        </div>

                    </div>
                </fieldset>

            </div>
        </div>
    </form>



  

  

</div>
this is my jquery 

<script>
    $(document).ready(function () {

        function validateForm() {
            let isValid = true;

          
            $('.is-invalid').removeClass('is-invalid');

           
            if ($('#Pno').val().trim() === '') {
                $('#Pno').addClass('is-invalid');
                isValid = false;
            }
            if ($('#Position').val().trim() === '') {
                $('#Position').addClass('is-invalid');
                isValid = false;
            }

            return isValid;
        }

        // Function to set the actionType before form submission
        function setAction(actionType, event = null) {
            if (event) event.preventDefault();
            $('#actionType').val(actionType);
            $('#form2').submit();
        }

        // Show the form for adding a new entry
        $('#showFormButton2').click(function () {
            $('#formContainer').show();
            $('#form2')[0].reset(); // Clear form fields
            $('#deleteButton').hide();
            $('#addRowButton').show();
        });

        // Open filled form for editing
        $(".OpenFilledForm").click(function (e) {
            e.preventDefault();
            $('#deleteButton').show();
            $('#addRowButton').hide();

            var id = $(this).data("id");
            $.ajax({
                url: '@Url.Action("EmployeePositionMaster", "Master")',
                type: 'GET',
                data: { id: id },
                success: function (response) {
                    // Populate form fields with response data
                    $('#form2 #LocationId').val(response.id);
                   
                    $('#form2 #Pno').val(response.pno);
                    $('#form2 #Position').val(response.position);
                   

                    // Show the form
                    $('#formContainer').show();
                },
                error: function () {
                    alert("An error occurred while loading the form data.");
                }
            });
        });

      


        // Handle the submit button click
        $('#submitButton').click(function (e) {
            e.preventDefault();

            // Validate form fields
            if (!validateForm()) {
                alert('Please fill in all required fields.');
                return;
            }

            const id = $('#LocationId').val();
            const rowsData = [];
          
                const pno = $(this).find('.PnoInput').val();
                const position = $(this).find('.PositionInput').val();
               

                rowsData.push({
                    Position: position,
                    Pno: pno,
                    Id: id
                });
       

            $.ajax({
                url: '@Url.Action("EmployeePositionMaster", "Master")',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    Id: id,
                    actionType: "Submit"
                }),
                success: function (response) {
                    alert('Position saved successfully!');
                    $('#formContainer').hide();
                },
                error: function () {
                    alert('An error occurred while saving the locations.');
                }
            });
        });



        // Handle the delete button click
        $('#deleteButton').click(function (e) {
            e.preventDefault();
            const id = $('#LocationId').val();
            const rowsData = [];
            $('.location-row').each(function () {
                //const id = $(this).data('id');
                rowsData.push({ Id: id });
            });

            $.ajax({
                url: '@Url.Action("EmployeePositionMaster", "Master")',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ Id: id, appLocations: rowsData, actionType: "Delete" }),
                success: function (response) {
                    alert('Locations deleted successfully!');
                    $('#formContainer').hide();
                },
                error: function () {
                    alert('An error occurred while deleting the locations.');
                }
            });
        });
    });
</script>

and this is my controller logic 

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EmployeePositionMaster(AppEmpPosition appPosition, string actionType)
{
    if (string.IsNullOrEmpty(actionType))
    {
        return BadRequest("No action specified.");
    }

    var existingParameter = await context.AppEmpPositions.FindAsync(appPosition.Id);

    var UserId = HttpContext.Request.Cookies["Session"];
    if (actionType == "Submit")
    {
        if (!ModelState.IsValid)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Key:{state.Key},Error:{error.ErrorMessage}");
                }
            }
        }


        if (ModelState.IsValid)
        {


            if (existingParameter != null)
            {
              
                context.Entry(existingParameter).CurrentValues.SetValues(appPosition);
                await context.SaveChangesAsync();
                TempData["Updatedmsg"] = "Position Updated Successfully!";
                return RedirectToAction("PositionMaster");
            }
            else
            {

                
                await context.AppEmpPositions.AddAsync(appPosition);
                await context.SaveChangesAsync();
                TempData["msg"] = "Position Added Successfully!";
                return RedirectToAction("PositionMaster");
            }
        }
    }
    else if (actionType == "Delete")
    {
        if (existingParameter != null)
        {
            context.AppEmpPositions.Remove(existingParameter);
            await context.SaveChangesAsync();
            TempData["Dltmsg"] = "Position Deleted Successfully!";
        }
    }

    return RedirectToAction("PositionMaster");
}

in this i am getting An error occurred while saving the locations.
