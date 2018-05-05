<%@ WebHandler Language="C#" Class="UploaderPhotos" %>

using System;
using System.Web;
using System.IO;
using System.Drawing;

public class UploaderPhotos : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "multipart/form-data";
        context.Response.Expires = -1;

        try
        {
            HttpPostedFile postedFile = context.Request.Files["file"];

            string uploads = context.Request["uploads"].ToString();
            string savepath = HttpContext.Current.Server.MapPath("../" + uploads + "/"); // Duong dan luu tru anh
            var extension = Path.GetExtension(postedFile.FileName);

            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);

            var newid = Guid.NewGuid();
            var id = newid + extension; // Ten file anh
            var fileLocation = string.Format("{0}/{1}", savepath, id); // Duong dan day du
            var thumbnail = string.Format("{0}/{1}", savepath, newid + "_thumbnail" + extension); // anh dai dien

            // Kiem tra kich thuoc file
            int nFileLen = postedFile.ContentLength;
            //if (nFileLen > 1048576)  // 1MB
            //{
            // File exceeds the file maximum size
            //context.Response.Write("Kích thước file vượt qua 1MB. Bạn chỉ có thể tải file với kích thước < 1MB!");
            //context.Response.StatusCode = 413;
            //return;

            // Read file into a data stream
            byte[] myData = new Byte[nFileLen];
            postedFile.InputStream.Read(myData, 0, nFileLen);
            postedFile.InputStream.Dispose();
            // Save the stream to disk as temporary file. make sure the path is unique!
            string tempPath = HttpContext.Current.Server.MapPath("../" + "temps" + "/"); // Duong dan luu tru anh tam thoi
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            string filetempLocation = string.Format("{0}/{1}", tempPath, id); // Duong dan day du

            System.IO.FileStream newFile = new System.IO.FileStream(filetempLocation, System.IO.FileMode.Create);
            newFile.Write(myData, 0, myData.Length);
            // anh dai dien
            bool success = ResizeImageAndUpload(newFile, thumbnail, 250, 250, false); // Luu anh kich thuoc 250x250 (rong x cao)
            success = ResizeImageAndUpload(newFile, fileLocation, 600, 800, true); // Luu anh kich thuoc 800x600 (rong x cao)

            //delete the temp file.
            newFile.Close();

            System.IO.File.Delete(filetempLocation);
            context.Response.Write(uploads + "/" + id);
            context.Response.StatusCode = 200;

            //}
            //else
            //{
            //    if (extension != null)
            //    {

            //        postedFile.SaveAs(fileLocation); // lu anh
            //        context.Response.Write(uploads + "/" + id);
            //        context.Response.StatusCode = 200;
            //    }
            //}
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

    public bool ResizeImageAndUpload(System.IO.FileStream newFile, string fileLocation, double maxHeight, double maxWidth, bool isRatio)
    {
        try
        {
            // Declare variable for the conversion
            float ratio;
            // Create variable to hold the image
            System.Drawing.Image thisImage = System.Drawing.Image.FromStream(newFile);
            // Get height and width of current image
            int width = (int)thisImage.Width;
            int height = (int)thisImage.Height;
            // Ratio and conversion for new size
            if (width > maxWidth)
            {
                ratio = (float)width / (float)maxWidth;
                width = (int)(width / ratio);
                height = (int)(height / ratio);
            }
            // Ratio and conversion for new size
            if (height > maxHeight)
            {
                ratio = (float)height / (float)maxHeight;
                height = (int)(height / ratio);
                width = (int)(width / ratio);
            }

            if (!isRatio)
            {
                width = (int)maxWidth;
                height = (int)maxHeight;
            }
            
            // Create "blank" image for drawing new image
            Bitmap outImage = new Bitmap(width, height);
            Graphics outGraphics = Graphics.FromImage(outImage);
            SolidBrush sb = new SolidBrush(System.Drawing.Color.White);
            // Fill "blank" with new sized image
            outGraphics.FillRectangle(sb, 0, 0, outImage.Width, outImage.Height);
            outGraphics.DrawImage(thisImage, 0, 0, outImage.Width, outImage.Height);
            sb.Dispose();
            outGraphics.Dispose();
            thisImage.Dispose();
            // Save new image 
            outImage.Save(fileLocation);
            outImage.Dispose();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxHeight)
    {
        var ratio = (double)maxHeight / image.Height;

        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        var newImage = new Bitmap(newWidth, newHeight);
        using (var g = Graphics.FromImage(newImage))
        {
            g.DrawImage(image, 0, 0, newWidth, newHeight);
        }
        return newImage;
    }
    
    

}