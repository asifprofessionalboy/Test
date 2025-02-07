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
	
	return RedirectToAction("EditDocument",new {id=(Guid?)null});
}

after implement this id is not null it gives me same error 
This localhost page can’t be found
No webpage was found for the web address: https://localhost:7073/Technical/EditDocument/3ed3dc06-73c8-4105-a7ad-6bfaef4bdaa0?page=1
