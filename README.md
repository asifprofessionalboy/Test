this is my delete button
<input type="submit" value="Delete" id="DeleteButton" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" />

this is my js for delete button
document.addEventListener("DOMContentLoaded", function () {

	var deleteButton = document.getElementById("DeleteButton");

	deleteButton.addEventListener("click", function () {
		var EditId = document.getElementById("Id").value;
		alert(EditId);
		if (EditId) {
			if (confirm("Are you sure you want to delete this customer?")) {
				fetch(`/Technical/DeleteDocument/${EditId}`, {
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

this is my anchor which gets the Id
	<a asp-action="EditDocument"
	   asp-route-id="@item.Id"
asp-route-FinYear="@ViewBag.FinYear"
asp-route-SearchMonth="@ViewBag.SearchMonth"
	   asp-route-page="@ViewBag.CurrentPage"
	   class="btn glow"
	   style="text-decoration:none;background-color:;font-weight:;">
					@item.RefNo
	</a>
in this i am not getting the Id
