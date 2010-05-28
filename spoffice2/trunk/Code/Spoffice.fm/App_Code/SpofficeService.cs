using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Spoffice.Lib;
using Lastfm.Radio;

/// <summary>
/// Summary description for SpofficeService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class SpofficeService : System.Web.Services.WebService
{

    public SpofficeService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public Track GetCurrentTrack()
    {
        return Controller.Current.CurrentTrack;
    }

}

