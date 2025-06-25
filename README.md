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
                string filePath = @"D:/Cybersoft_Doc/CLMS/Attachments/" + fileName;
                
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
