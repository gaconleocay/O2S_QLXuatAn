<%@ WebHandler Language="C#" Class="GetImageFromFile" %>

using System;
using System.Web;

public class GetImageFromFile : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        try
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            context.Response.ContentType = "image/png";
            string filename = context.Request.QueryString["FileName"];

            if (System.IO.File.Exists(filename))
                context.Response.WriteFile(filename);
            else
                context.Response.Write("Không tìm thấy file ảnh");
        }
        catch (Exception ex)
        {
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    public static byte[] GetByteFromFile(string FullPath)
    {
        try
        {
            byte[] ImageData = null;
            if (System.IO.File.Exists(FullPath))
            {
                System.IO.FileStream fs = new System.IO.FileStream(FullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                ImageData = new byte[fs.Length];
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
            }
            return ImageData;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}