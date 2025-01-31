i have this form 
<div id="formContainer" style="display:none;">
    <form asp-action="LocationMaster" asp-controller="Master" id="form2" method="post">
        @Html.AntiForgeryToken()
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>
        <div class="card rounded-9">
            <div class="card-header text-center" style="background-color: #bbb8bf;color: #000000;font-weight:bold;">
                Location Master Entry
            </div>
            <div class="col-md-12">
                <fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px;">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group row">

                                <div class="col-sm-2 d-flex align-items-center">
                                    <label asp-for="WorkSite" class="control-label">Worksite</label>
                                </div>
                                <div class="col-sm-8">
                                    <input asp-for="WorkSite" class="form-control form-control-sm WorkSiteInput" id="WorkSite" placeholder="" required />

                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="card rounded-j">
                                <div class="row">
                                    <div class="col-sm-5 d-flex align-items-center teamtext">
                                        <label class="control-label">Latitude</label>
                                    </div>
                                    <div class="col-sm-4 d-flex align-items-center teamtext">
                                        <label class="control-label">Longitude</label>
                                    </div>
                                </div>
                                <div id="locationContainer">
                                    <div class="location-row">
                                        <div class="row mt-2">
                                            <div class="col-sm-5 d-flex align-items-center">
                                                <input asp-for="Latitude" class="form-control form-control-sm LatitudeInput" id="Latitude" placeholder="Enter Latitude" required/>
                                            </div>
                                            <div class="col-sm-5 d-flex align-items-center">
                                                <input asp-for="Latitude" name="Longitude[]" class="form-control form-control-sm LongInput" id="Longitude" placeholder="Enter Longitude" required />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                                <div class="text-center mt-2">
                                    <button type="button" class="btn btn-primary col-sm-2 p-0" id="addRowButton">Add</button>
                                </div>
                            </div>
                        </div>





                        <input type="hidden" name="Id" value="@Model.Id" />
                        <div class="form-group row mt-2">

                            <input asp-for="Id" type="text" value="@Model.Id" id="LocationId" hidden />
                            <input name="CreatedOn" value="@Model.CreatedOn" hidden id="CreatedOn" />
                            <input name="CreatedBy" value="@ViewBag.CreatedBy" hidden id="CreatedBy" />
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
this is my jquery

  $(document).ready(function () {
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
              url: '@Url.Action("LocationMaster", "Master")',
              type: 'GET',
              data: { id: id },
              success: function (response) {
                  // Populate form fields with response data
                  $('#form2 #LocationId').val(response.id);
                  $('#form2 #CreatedBy').val(response.createdby);
                  $('#form2 #CreatedOn').val(response.createdon);
                  $('#form2 #WorkSite').val(response.worksite);
                  $('#form2 #Longitude').val(response.longitude);
                  $('#form2 #Latitude').val(response.latitude);
                 

                  // Show the form
                  $('#formContainer').show();
              },
              error: function () {
                  alert("An error occurred while loading the form data.");
              }
          });
      });

      // Add a new row dynamically
      $('#addRowButton').click(function () {
          const newRow = `
                              <div class="location-row">
                                  <div class="row mt-2">
                                      <div class="col-sm-5 d-flex align-items-center">
                                              <input asp-for="Latitude" name="Latitude[]" class="form-control form-control-sm LatitudeInput" placeholder="Enter Latitude" required/>
                                      </div>
                                      <div class="col-sm-5 d-flex align-items-center">
                                              <input asp-for="Longitude" name="Longitude[]" class="form-control form-control-sm LongInput" placeholder="Enter Longitude" required/>
                                      </div>
                                      <div class="col-sm-2 d-flex align-items-center">
                                          <button type="button" class="btn btn-danger btn-sm remove-row">Remove</button>
                                      </div>
                                  </div>
                              </div>`;
          $('#locationContainer').append(newRow);
      });

      // Remove a row dynamically
      $(document).on('click', '.remove-row', function () {
          $(this).closest('.location-row').remove();
      });

      // Handle the submit button click
      $('#submitButton').click(function (e) {
          e.preventDefault();
          
          const id = $('#LocationId').val();
          const rowsData = [];
          $('.location-row').each(function () {
              const latitude = $(this).find('.LatitudeInput').val();
              const longitude = $(this).find('.LongInput').val();
              const worksite = $('#WorkSite').val();
              const id = $('#LocationId').val();


              rowsData.push({
                  WorkSite: worksite,     // Matches "WorkSite" in the model
                  Latitude: parseFloat(latitude),  // Ensure numeric values for decimal fields
                  Longitude: parseFloat(longitude),
                  Id: id
              });
          });

          
          $.ajax({
              url: '@Url.Action("LocationMaster", "Master")', // Replace with your correct URL
              type: 'POST',
              contentType: 'application/json',  // This is crucial for JSON data
              data: JSON.stringify({
                  Id: id,
                  appLocations: rowsData,  // This is the data you're sending
                  actionType: "Submit"
              }),
              success: function (response) {
                  alert('Locations saved successfully!');
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
              url: '@Url.Action("LocationMaster", "Master")',
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

in this i want to add a validation that if there is blank fields i want to add this class .is-invalid {
    border: 1px solid red;
    background-color: #f8d7da; /* Optional: Light red background for better visibility */
} and form not submits until all fields have value
