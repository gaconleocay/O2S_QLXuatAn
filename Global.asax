<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        try
        {
            // Code that runs on application startup
            Application.Lock();
            Application["CurrentUsers"] = 0; // So luong nguoi truy cap online
            Application.UnLock();
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        try
        {
            //  Code that runs on application shutdown
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        try
        {
            // Code that runs when an unhandled error occurs
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    void Session_Start(object sender, EventArgs e) 
    {
        try
        {
            // Code that runs when a new session is started
            Application.Lock();
            Application["CurrentUsers"] = System.Convert.ToInt32(Application["CurrentUsers"]) + 1; // Tang so luong nguoi truy cap online
            Application.UnLock();
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    void Session_End(object sender, EventArgs e) 
    {
        try
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            Application.Lock();
            Application["CurrentUsers"] = System.Convert.ToInt32(Application["CurrentUsers"]) - 1;
            Application.UnLock();
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
       
</script>
