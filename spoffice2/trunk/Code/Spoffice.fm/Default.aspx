<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script type="text/javascript">
        var ws;
        $(document).ready(function() {

            $('#sendsomething').click(function(e) {
                try {
                    debug("sending something...", "send");
                    ws.send("hello");
                } catch (e) {
                    debug(e.message, "error");
                }
                return false;
            });

            // test if the browser supports web sockets
            if ("WebSocket" in window) {
                debug("Browser supports web sockets!", 'success');
                debug("Connecting to", "...");
                try {
                    ws = new WebSocket("ws://localhost:8181/service"); // create the web socket
                } catch (err) {
                    console.log(err);
                    debug(err.message, 'error');
                }
                debug("connected", "msg");

                ws.onopen = function() {
                    debug("connected... ", 'success'); // we are in! :D
                };

                ws.onmessage = function(evt) {
                    console.log(evt.data); // we got some data - show it omg!!
                };

                ws.onclose = function() {
                    debug("Socket closed!", 'error'); // the socket was closed (this could be an error or simply that there is no server)
                };
            } else {
                debug("Browser does not support web sockets", 'error');
            };

            // function to display stuff, the second parameter is the class of the <p> (used for styling)
            function debug(msg, type) {
                $('#debug').append(msg + " | " + type);
            };

        });
</script> 
</head>
<body>
    <button id="sendsomething">SENDING A MESSAGE!</button>
    <form id="form1" runat="server">
    <div id="debug"></div>
    </form>
</body>
</html>
