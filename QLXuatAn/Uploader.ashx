<%@ WebHandler Language="C#" Class="Uploader" %>

using System;
using System.Web;
using System.IO;

public class Uploader : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "multipart/form-data";
        context.Response.Expires = -1;

        try
        {
            HttpPostedFile postedFile = context.Request.Files["file"];

            // Kich thuoc file
            int nFileLen = postedFile.ContentLength;
            
            string uploads = context.Request["uploads"].ToString();
            string savepath = HttpContext.Current.Server.MapPath("~/" + uploads + "/");
            var extension = Path.GetExtension(postedFile.FileName);

            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);
           
            var id = Guid.NewGuid() + extension;
            if (extension != null)
            {
                var fileLocation = string.Format("{0}/{1}", savepath, id);

                postedFile.SaveAs(fileLocation);
                //context.Response.Write(fileLocation);
                context.Response.Write(uploads + "/" + id);
                context.Response.StatusCode = 200;
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("Error: " + ex.Message);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
    
}