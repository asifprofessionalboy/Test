		
this is my controller method for Approval form
[HttpPost]
		[RequestSizeLimit(500 * 1024 * 1024)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Approval_Form(InnovationViewModel InnViewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{

					if (InnViewModel.Attach != null && InnViewModel.Attach.Any())
					{
						var uploadPath = configuration["FileUpload:Path"];
						foreach (var file in InnViewModel.Attach)
						{
							if (file.Length > 0)
							{
								var uniqueId = Guid.NewGuid().ToString();
								var currentDateTime = DateTime.UtcNow.ToString("dd-MM-yyyy");
								var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
								var fileExtension = Path.GetExtension(file.FileName);
								var formattedFileName = $"{uniqueId}_{currentDateTime}_{originalFileName}{fileExtension}";
								var fullPath = Path.Combine(uploadPath, formattedFileName);
								using (var stream = new FileStream(fullPath, FileMode.Create))
								{
									await file.CopyToAsync(stream);
								}
								InnViewModel.Attachment += $"{formattedFileName},";
							}
						}
						if (!string.IsNullOrEmpty(InnViewModel.Attachment))
						{
							InnViewModel.Attachment = InnViewModel.Attachment.TrimEnd(',');
						}
					}
					else if (InnViewModel.Id.HasValue)
					{

						var existingInnovation2 = await context.AppInnovations.FindAsync(InnViewModel.Id.Value);
						if (existingInnovation2 != null)
						{
							InnViewModel.Attachment = existingInnovation2.Attachment;
						}
					}

					var Status = Request.Form["Champion"];

					if (!InnViewModel.Id.HasValue)
					{
						ModelState.AddModelError("", "Invalid operation. The innovation Id is required.");
						return View(InnViewModel);
					}

					var existingInnovation = await context.AppInnovations.FindAsync(InnViewModel.Id.Value);

					if (existingInnovation == null)
					{
						return NotFound();
					}

					var user = HttpContext.Session.GetString("Session");

					if (Status == "Draft")
					{
						existingInnovation.ApprovedOn = null;
						existingInnovation.ApprovedBy = null;

					}
					else
					{
						existingInnovation.ApprovedOn = DateTime.Now;
					}


					existingInnovation.PersonalNo = InnViewModel.PersonalNo;
					existingInnovation.Name = InnViewModel.Name;
					existingInnovation.Department = InnViewModel.Department;
					existingInnovation.Designation = InnViewModel.Designation;
					existingInnovation.EmailId = InnViewModel.EmailId;
					existingInnovation.Mobile = InnViewModel.Mobile;
					existingInnovation.Innovation = InnViewModel.Innovation;
					existingInnovation.Description = InnViewModel.Description;
					existingInnovation.StageOfInnovation = InnViewModel.StageOfInnovation;
					existingInnovation.Attachment = InnViewModel.Attachment;
					existingInnovation.Status = Status;
					existingInnovation.ApproverRemarks = InnViewModel.ApproverRemarks;
					existingInnovation.SubmitFlag = "Submit";
					existingInnovation.ApprovedOn = existingInnovation.ApprovedOn;
					existingInnovation.ApprovedBy = "842015";
					existingInnovation.SourceOfInnovation = InnViewModel.SourceOfInnovation;
					existingInnovation.OtherBenefit = InnViewModel.OtherBenefit;
					existingInnovation.DareToTry = InnViewModel.DareToTry;


					if (!string.IsNullOrEmpty(InnViewModel.OtherBenefit))
					{
						var existingBenefit = context.AppBenefitMasters.FirstOrDefault(b => b.Benefit == InnViewModel.OtherBenefit);
						if (existingBenefit == null)
						{
							var newBenefit = new AppBenefitMaster
							{
								Id = Guid.NewGuid(),
								Benefit = InnViewModel.OtherBenefit
							};
							context.AppBenefitMasters.Add(newBenefit);
						}
						else
						{
							existingBenefit.Benefit = InnViewModel.OtherBenefit;
							context.AppBenefitMasters.Update(existingBenefit);
						}
					}

					try
					{
						context.Entry(existingInnovation).State = EntityState.Modified;
						await context.SaveChangesAsync();
					}
					catch (DbUpdateConcurrencyException ex)
					{
						ModelState.AddModelError("", "Unable to save changes.");
						return View(InnViewModel);
					}


					foreach (var benefit in InnViewModel.appInnovationBenefits)
					{
						if (benefit.Id == Guid.Empty)
						{
							benefit.Id = Guid.NewGuid();
							benefit.MasterId = existingInnovation.Id;
							await context.AppInnovationBenefits.AddAsync(benefit);
						}
						else
						{
							benefit.MasterId = existingInnovation.Id;
							context.Entry(benefit).State = EntityState.Modified;
						}
					}
					await context.SaveChangesAsync();


					foreach (var team in InnViewModel.appProjectTeams)
					{
						if (team.Id == Guid.Empty)
						{
							team.Id = Guid.NewGuid();
							team.MasterId = existingInnovation.Id;
							await context.AppProjectTeams.AddAsync(team);
						}
						else
						{
							team.MasterId = existingInnovation.Id;
							context.Entry(team).State = EntityState.Modified;
						}
					}
					await context.SaveChangesAsync();

					var refNo = await context.AppInnovations
						.Where(x => x.Id == existingInnovation.Id)
						.Select(x => x.RefNo)
						.FirstOrDefaultAsync();

					var status = await context.AppInnovations
						.Where(x => x.Id == existingInnovation.Id)
						.Select(x => x.Status)
						.FirstOrDefaultAsync();


					var emailList = await context.AppMailEmployeeMasters
	   .FromSqlRaw(@"SELECT DISTINCT EmailID 
                  FROM App_MailEmployee_Master 
                  WHERE EmailID IS NOT NULL")
	   .Select(e => e.EmailId)
	   .ToListAsync();


					if (status == "Approved")
					{
						var Det = HttpContext.Session.GetString("Session");
						var data = context1.AppEmployeeMasters.FirstOrDefault(x => x.Pno == InnViewModel.Pno);


						var EmailId = await context.AppLogins.FirstOrDefaultAsync(x => x.UserId == InnViewModel.PersonalNo);

						if (EmailId != null)
						{
							var RequesterEmail = EmailId.Email;

							string statusText = status switch
							{
								"Approved" => "Approved",
								"Pending With Requester" => "Returned",
								"Rejected" => "Rejected",
								_ => "Unknown"
							};
							var baseUrl = $"{Request.Scheme}://{Request.Host}";
							var innovationUrl = $"{baseUrl}/Innovation/Innovation/AllAction?Id={existingInnovation.Id}";
							var returnUrl = $"{baseUrl}/Innovation/User/Login?returnUrl={HttpUtility.UrlEncode(innovationUrl)}";

							string subject = "New Innovation Project - Explore Now!";
							string msg = $@"
            <html>
                <body>
					<p> Dear Team,</p>
                    <p><b>New Innovation : ' {existingInnovation.Innovation} ' has been launched on our portal.</b></p>
                   <p>To learn more about the project, please visit the following link : <a href='{returnUrl}'>Click here</a></br></p>
                   </BR><p> EXPLORE . INNOVATE . ELEVATE</p>
                   </BR><p> Regards,</p>
                    <b>Smarani Vuppala</br>
                    Innovation Champion</br>
                    Assistant Manager, Technical Services</b></br>
 <p><b>Tata Steel Utilities & Infrastructure Services Limited</b></br>
 (Formerly Jamshedpur Utilities & Services Company Limited)</br>
Sakchi Boulevard, Northern Town, Bistupur | Jamshedpur 831 001</br>
 Mobile +91-6287396293</br>
</br>smarani.vuppala@tatasteel.com | http://www.tatasteeluisl.com
</br><b style=color:#3a7cda;>TATA STEEL UTILITIES AND INFRASTRUCTURE SERVICES LIMITED</b>
                </body>
            </html>";


							var firstBatch = emailList.Skip(0).Take(300).ToList();
							await emailService.SendApprovedEmailAsync(firstBatch, "", "", subject, msg);

							var secondBatch = emailList.Skip(300).Take(300).ToList();
							await emailService.SendApprovedEmailAsync(secondBatch, "", "", subject, msg);

							var thirdBatch = emailList.Skip(600).Take(307).ToList();
							await emailService.SendApprovedEmailAsync(thirdBatch, "", "", subject, msg);

							//await emailService.SendApprovedEmailAsync("irshad321mhan@gmail.com", "", "", subject, msg);

						}
					}
					else
					{
						var EmailId = await context.AppLogins.FirstOrDefaultAsync(x => x.UserId == InnViewModel.PersonalNo);
						if (EmailId != null)
						{
							var RequesterEmail = EmailId.Email;

							string statusText = status switch
							{
								"Approved" => "Approved",
								"Pending With Requester" => "Returned",
								"Rejected" => "Rejected",
								_ => "Unknown"
							};
							var baseUrl = $"{Request.Scheme}://{Request.Host}";
							var innovationUrl = $"{baseUrl}/Innovation/Innovation/AllRequest?Id={existingInnovation.Id}";
							var returnUrl = $"{baseUrl}/Innovation/User/Login?returnUrl={HttpUtility.UrlEncode(innovationUrl)}";
							string subject = $"Innovation Log: Innovation Id {refNo}";
							string msg = $@"
            <html>
                <body>
                    <p><b>Innovation is {statusText} with Innovation Id: {refNo}</b></p>
                   <p><a href='{returnUrl}'>Click here to view the Innovation</a></br></p>
                   <p> Regards,</p>
                    <b>Smarani Vuppala</br>
                    Innovation Champion</br>
                    Assistant Manager, Technical Services</b></br>
 <p><b>Tata Steel Utilities & Infrastructure Services Limited</b></br>
 (Formerly Jamshedpur Utilities & Services Company Limited)</br>
Sakchi Boulevard, Northern Town, Bistupur | Jamshedpur 831 001</br>
 Mobile +91-6287396293</br>
</br>smarani.vuppala@tatasteel.com | http://www.tatasteeluisl.com
</br><b style=color:#3a7cda;>TATA STEEL UTILITIES AND INFRASTRUCTURE SERVICES LIMITED</b>
                </body>
            </html>";

							await emailService.SendEmailAsync(RequesterEmail, "smarani.vuppala@tatasteel.com", "", subject, msg);
						}
					}

				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError($"{ex.Message}", "Unable to save changes.");
			}


			return RedirectToAction("Homepage", "Innovation");
		}

this is my email class 
	public async Task SendApprovedEmailAsync(List<string> toEmails, string ccEmail, string bccEmail, string subject, string message)
	{
		var emailSettings = _configuration.GetSection("EmailSettings");

		var email = new MimeMessage();
		email.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
		foreach (var toEmail in toEmails)
		{
			email.To.Add(new MailboxAddress(toEmail, toEmail));
		}
		//email.To.Add(new MailboxAddress(toEmails, toEmails));
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
			smtp.Connect(emailSettings["SMTPServer"], int.Parse(emailSettings["SMTPPort"]), MailKit.Security.SecureSocketOptions.None);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);

		}
	}
in this case when approved approved the innovation it send email to 907 employees , is there any issue in this email, check it , some times employee gets double email why?
