@if (Model != null && Model.RefNo != null)
{
	<form asp-action="EditDocument" id="form2" method="post" enctype="multipart/form-data">

		<div class="card-header text-center" style="background-color:#49477a;color:white;font-weight:bold;">Edit Document</div>
		<div class="col-md-12" style="padding:10px;">


			<fieldset style="border:1px solid #bfbebe;padding:5px 20px 5px 20px;border-radius:6px;">
				<div class="form-group row">
					<div class="col-sm-1 d-flex align-items-center">
						<label asp-for="FinYear" class="control-label">Fin Year </label>
					</div>

					<div class="col-sm-3">
						<select asp-for="FinYear" class="form-control form-control-sm custom-select" name="FinYear">
							<option value="">Select Fin Year</option>
							<option value="23-24">FY'24</option>
							<option value="24-25">FY'25</option>
							<option value="25-26">FY'26</option>

						</select>
					</div>
					<div class="col-sm-1 d-flex align-items-center">
						<label asp-for="Month" class="control-label">Months </label>
					</div>
					<div class="col-sm-3">
						<select asp-for="Month" class="form-control form-control-sm custom-select">
							<option value="">Select Month</option>
							@foreach (var item in MonthDropdown)
							{
								<option value="@item.Month">@item.Month</option>
							}
						</select>
					</div>

					<input type="hidden" value="@Model.RefNo" name="RefNo"/>
					<input type="hidden" value="@Model.Attachment" name="Attachment"/>
					<input type="hidden" value="@Model.Id" name="Id" id="EditId"/>
					
					<div class="col-sm-1 d-flex align-items-center">
						<label asp-for="Department" class="control-label">Department </label>
					</div>
					<div class="col-sm-3">
						<select asp-for="Department" class="form-control form-control-sm custom-select">
							<option value="">Select Department</option>
							<option value="Admin & CC">Admin & CC</option>
							<option value="Bidding">Bidding</option>
							<option value="BE">BE</option>
							<option value="Data Analytics">Data Analytics</option>
							<option value="BD">BD</option>
							<option value="CRM">CRM</option>
							<option value="DETP">DETP</option>
							<option value="Technical Services">Technical Services</option>
						</select>
					</div>
				</div>

				<div class="form-group row">
					<div class="col-sm-1">
						<label asp-for="Subject" class="control-label">Subject </label>
					</div>
					<div class="col-sm-3">
						<select asp-for="Subject" class="form-control form-control-sm custom-select" name="Subject">
							<option value="">Select Subject</option>
							<option value="Flash Report">Flash Report</option>
							<option value="MD Communication pack">MD Communication pack</option>
							<option value="L2 KPIs - Technical Services">L2 KPIs - Technical Services</option>
							<option value="L3 KPIs - Bidding">L3 KPIs - Bidding</option>
							<option value="L3 KPIs - DETP">L3 KPIs - DETP</option>
							<option value="L3 KPIs - BE,Data Analytics,BD,CRM">L3 KPIs - BE,Data Analytics,BD,CRM</option>
							<option value="L3 KPIs - Admin & CC">L3 KPIs - Admin & CC</option>
						</select>
					</div>
					<div class="col-sm-1 align-items-center">
						<label asp-for="Attach" class="control-label">Attachment </label>
					</div>
					<div class="col-sm-3">
						@if (!string.IsNullOrEmpty(Model.Attachment))
						{
							<div>
								<ul>
									@foreach (var fileName in Model.Attachment.Split(','))
									{
										var cleanFileName = ExtractFileName(fileName);
										var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
										var isPdf = fileExtension == ".pdf";

										<li>
											<a href="@Url.Action("DownloadFile", new { fileName = fileName })"
											   target="_blank">
												@cleanFileName
											</a>
										</li>
									}
								</ul>
							</div>
						}
					</div>
					@functions {
					private string ExtractFileName(string fileNameWithPrefix)
					{

						var parts = fileNameWithPrefix.Split('_');
						if (parts.Length > 1)
						{
							return parts[parts.Length - 1];
						}
						return fileNameWithPrefix;
					}
				}

				<div class="col-sm-1 align-items-center">
					<label asp-for="Attach" class="control-label">Attachment </label>
				</div>
				<div class="col-sm-3">
					<input asp-for="Attach" type="file" class="form-control form-control-sm" multiple id="fileInput" />
					<span asp-validation-for="Attach" class="text-danger"></span>

				</div>
				<div class="col-sm-10 file" style="">
					<div id="fileNames" class="mt-2" style="font-size:13px;font-family:arial;font-weight:500;color:#3a7cda;"></div>
				</div>
			</div>

			


	
			<div class="form-group row d-flex justify-content-center mt-3">

			<div class="text-center">
			<div class="text-center">
			<input type="submit" value="Save" id="Savebtn" class="btn" style="border-radius:7px"/>
			<input type="submit" value="Delete" id="DeleteButton" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" />
			</div>

			</div>
			</div>

		</fieldset>
	</div>

</form>
}


<input type="hidden" value="00000000-0000-0000-0000-000000000000" name="Id" id="EditId">
