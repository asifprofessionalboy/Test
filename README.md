i want to change logic of Attachment upload . i have this logic to upload and download from inside application i want to create a folder in my drive and from there i want to upload and 
download it 

this is my controller logic 
if (InnViewModel.Attach != null && InnViewModel.Attach.Any())
{
				var uploadPath = configuration["FileUpload:Path"];
				foreach (var file in InnViewModel.Attach)
				{
					if (file.Length > 0)
					{
						var uniqueId = Guid.NewGuid().ToString();
						var currentDateTime = DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss");
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

this is my download logic 

		public IActionResult DownloadFile(string fileName)
		{
			var uploadPath = configuration["FileUpload:Path"];
			var filePath = Path.Combine(uploadPath, fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound();
			}

			var memory = new MemoryStream();
			using (var stream = new FileStream(filePath, FileMode.Open))
			{
				stream.CopyTo(memory);
			}
			memory.Position = 0;

			return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
		}

		private string GetContentType(string path)
		{
			var types = GetMimeTypes();
			var ext = Path.GetExtension(path).ToLowerInvariant();
			return types[ext];
		}

		private Dictionary<string, string> GetMimeTypes()
		{
			return new Dictionary<string, string>
	{
		{ ".txt", "text/plain" },
		{ ".pdf", "application/pdf" },
		{ ".doc", "application/vnd.ms-word" },
		{ ".docx", "application/vnd.ms-word" },
		{ ".xls", "application/vnd.ms-excel" },
		{ ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
		{ ".png", "image/png" },
		{ ".jpg", "image/jpeg" },
		{ ".jpeg", "image/jpeg" },
		{ ".gif", "image/gif" },
		{ ".mp4", "video/mp4" },
		{ ".mpkg", "application/vnd.apple.installer+xml" },
		{ ".mov", "video/quicktime" },
		{ ".bmp", "image/x-MS-bmp" },
		{ ".csv", "text/csv" },
		{ ".ppt", "application/vnd.ms-powerpoint" }, 
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" } 
    };
		}


i want like this handler
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;


public class FileDownloadHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        string fileName = context.Request.QueryString["file"];


        object sessionUser = context.Session["UserName"];
        if (sessionUser == null )
        {
            context.Response.Write("Session expired or user not logged in.");
            return;
        }
        else
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = @"D:/Cybersoft_Doc/Innovation/Attachments/" + fileName;
                
                //string filePath = context.Server.MapPath(virtualPath); // Converts to physical path

                if (System.IO.File.Exists(filePath))
                {

                    context.Response.Clear();
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                    context.Response.TransmitFile(filePath);
                    context.Response.Flush(); //  Flush the response
                    context.ApplicationInstance.CompleteRequest(); //  Avoid ThreadAbortException
                    context.Response.Close();
                    context.Response.End();

                }
                else
                {
                    context.Response.Write("File not found.");
                }
            }
        }


       
    }

    public bool IsReusable => false;
}
