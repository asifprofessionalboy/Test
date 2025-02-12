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
