<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script type="text/javascript" src="jqSoapClient.beta.js"></script>
    <script type="text/javascript">
        $(function() {
            var soapBody = new SOAPObject("GetCurrentTrack");
            soapBody.ns = "http://tempuri.org/";
            var sr = new SOAPRequest("http://tempuri.org/GetCurrentTrack", soapBody); //Request is ready to be sent
            SOAPClient.Proxy = "SpofficeService.asmx"; //Specify web-service address or a proxy file
            SOAPClient.SendRequest(sr, processResponse);
            function processResponse(respObj) {
                $('#trackTitle').html(respObj.find("Title").text());
                $('#artistName').html(respObj.find("Artist").text());
                $('#albumTitle').html(respObj.find("AlbumTitle").text());
            }
        });    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <h1 id="trackTitle"></h1>
    <h2 id="artistName"></h2>
    <h3 id="albumTitle"></h3>
    </form>
</body>
</html>
