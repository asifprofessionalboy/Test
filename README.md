this is my jquery, in this i want first confirm to delete then delete 
 $('#deleteButton').click(function (e) {
     e.preventDefault();
     const id = $('#LocationId').val();
     const rowsData = [];
     $('.location-row').each(function () {
        
         rowsData.push({ Id: id });
     });

     $.ajax({
         url: '@Url.Action("LocationMaster", "Master")',
         type: 'POST',
         contentType: 'application/json',
         data: JSON.stringify({ Id: id, appLocations: rowsData, actionType: "Delete" }),
         success: function (response) {
             Swal.fire({
                 title: "Success.",
                 text: "Location Deleted Successfully.",
                 icon: "success",
                 timer: 5000,
                 showConfirmButton: true
             });
             
             $('#formContainer').hide();
         },
         error: function () {
             alert('An error occurred while deleting the locations.');
         }
     });

 });
