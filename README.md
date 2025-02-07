this is my controller to DeleteData 
[HttpPost]
public IActionResult DeleteDocument(Guid id)
{
	var Data = context.AppTechnicalServices.FirstOrDefault(c => c.Id == id);
	if (Data != null)
	{
		context.AppTechnicalServices.Remove(Data);
		context.SaveChanges();
		TempData["Message"] = "Customer deleted successfully!";
	}
	
	return RedirectToAction("EditDocument");
}

in this i am facing an issue that when i Delete the Data i want to redirect to same page but i am getting error that it catches the asp-route-id and this error is giving 

This localhost page can’t be found
No webpage was found for the web address: https://localhost:7073/Technical/EditDocument/a21a0b55-7e4b-47ed-955d-9e7ca2a191e1?page=1
HTTP ERROR 404

this is my anchor tag where asp-route-id is catching
<td>
				<a asp-action="EditDocument"
				   asp-route-id="@item.Id"
   asp-route-FinYear="@ViewBag.FinYear"
   asp-route-SearchMonth="@ViewBag.SearchMonth"
				   asp-route-page="@ViewBag.CurrentPage"
				   class="btn glow"
				   style="text-decoration:none;background-color:;font-weight:;">
								@item.RefNo
				</a>
</td>

 
