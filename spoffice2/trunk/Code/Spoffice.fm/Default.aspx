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
                console.log(respObj);
            }
        });    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
     <asp:Literal ID="currentTrack" runat="server" />
     <asp:Literal ID="playerState" runat="server" />
    
    </div>
    </form>
</body>
</html>
