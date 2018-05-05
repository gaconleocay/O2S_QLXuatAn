<%@ WebHandler Language="C#" Class="GetImageFromFile" %>

using System;
using System.Web;
using System.Net;
using System.Configuration;

public class GetImageFromFile : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            context.Response.ContentType = "image/png";
            string filename = context.Request.QueryString["FileName"];

            if (System.IO.File.Exists(filename))
                context.Response.WriteFile(filename);
            else
            {
                //format:    \\CUONGNC\pic\23-07-2016\09h17m48s_b175105b-1719-4028-bd30-8080b6136fe8PLATEOUT.JPG

                string[] fileitem = filename.Split(Convert.ToChar(@"\"));
                if (fileitem != null && fileitem.Length == 6)
                {
                    string username = ConfigurationManager.AppSettings["FTPUserName"].ToString();
                    string pass = ConfigurationManager.AppSettings["FTPPassword"].ToString();

                    string _fileonftp = "ftp://" + username + ":" + pass + "@" + fileitem[2] + "/" + fileitem[3] + "/" + fileitem[4] + "/" + fileitem[5];
                    if (CheckIfFileExistsOnServer(_fileonftp) == true)
                    {
                        byte[] data = null;
                        WebClient web = new WebClient();
                        data = web.DownloadData(_fileonftp);
                        context.Response.Clear();
                        context.Response.Buffer = true;
                        context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + "a");
                        context.Response.ContentType = "image/png";
                        context.Response.BinaryWrite(data);
                        context.Response.End();
                    }
                    //else
                    // context.Response.Write("Không tìm thấy file ảnh");

                }
                else
                    context.Response.Write("Không tìm thấy file ảnh");
            }



            //test ok

            //string filename = context.Request.QueryString["FileName"];

            //context.Response.Clear();
            //context.Response.Buffer = true;
            //context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + "a");
            //context.Response.ContentType = "image/png";

            //byte[] bytes = System.IO.File.ReadAllBytes(filename);
            //context.Response.BinaryWrite(bytes);
            //context.Response.End();
        }
        catch (Exception ex)
        {
        }
    }

    public bool IsReusable
    {
        get
        {
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

    private bool CheckIfFileExistsOnServer(string fileName)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fileName);
        string username = ConfigurationManager.AppSettings["FTPUserName"].ToString();
        string pass = ConfigurationManager.AppSettings["FTPPassword"].ToString();
        request.Credentials = new NetworkCredential(username, pass);
        request.Method = WebRequestMethods.Ftp.GetFileSize;

        try
        {
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            return true;
        }
        catch (WebException ex)
        {
            FtpWebResponse response = (FtpWebResponse)ex.Response;
            if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                return false;
        }
        return false;
    }
}