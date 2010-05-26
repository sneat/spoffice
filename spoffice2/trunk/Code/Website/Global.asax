<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        Spoffice.Lib.MusicServiceManager.AddService("spotify", new Spoffice.Lib.MusicServices.SpotifyMusicService());
        Spoffice.Lib.MusicServiceManager.AddService("filesystem", new Spoffice.Lib.MusicServices.FileSystemMusicService());
        Spoffice.Lib.Controller.Current.Init(new Spoffice.Lib.Repositories.XmlRepository(Server.MapPath("~/Test_Data/trackrepos.xml")));
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }
       
</script>
