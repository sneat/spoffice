


(function($) {
    $.fn.spofficeInterface = function(settings) {
        var config = { 'foo': 'bar' };
        if (settings) $.extend(config, settings);
        this.each(function() {
            if (this.nodeName.toLowerCase() != "body") return;
            var loginForm;
            var loginButton;
            var layout;
            var tabs;
            var trackHistoryTable;
            function init() {
                console.log("init");
                removeStaticContent();
                getLoginStatus();
            }
            function getLoginStatus() {
                load("/Account/Logon");
            }
            function load(url, data, callback) {
                console.log("load");
                $.ajax({
                    url: url,
                    type: data != null ? "POST" : "GET",
                    dataType: 'json',
                    data: data,
                    beforeSend: function(xhr) {
                        xhr.setRequestHeader("HTTP_X_EXPECT_FORMAT", "json");
                    },
                    success: function(data) {
                        if (isLoginData(data)) {
                            dealWithLoginData(data);
                        } else if (callback) {
                            callback(data);
                        }

                    }
                });
            }
            function isLoginData(data) {
                console.log("isLoginData");
                return (data.LoggedIn != null);
            }
            function dealWithLoginData(data) {
                console.log("dealWithLoginData");
                var loadingdiv = $('#loading');
                if (loadingdiv.is(":visible")) {
                    loadingdiv.fadeOut();
                };
                if (data.LoggedIn) {
                    onLogin();
                } else {
                    if (!isLoginFormVisible()) {
                        displayLoginForm();
                    }
                }
            }
            function onLogin() {
                console.log("onLogin");
                if (loginForm != null)
                    loginForm.hide();

                if (layout == null)
                    createLayout();
            }
            function resizeTabLayout() {
                console.log("resizeTabLayout");
                if (!tabs) return;
                layout.resizeAll();
            }
            function createLayout() {
                console.log("createLayout");
                $(document.body).append($('#layout').html());
                $('#layout').remove();
                layout = $('body').layout({
                    center__paneSelector: "#main",
                    north__paneSelector: "#header",
                    north__size: 100,
                    spacing_open: 0,
                    north__slidable: false
                });
                tabs = $('#main').tabs({
                    show: function(event, ui) {
                        resizeTabLayout();
                        switch (ui.panel.id) {
                            case "trackhistory":
                                load__TrackHistory();
                                break;
                            case "favourites":
                                load__Favourites();
                                break;
                        }
                    }
                });
                $('#results-tab').hide();
                $('#main').layout({
                    center__paneSelector: ".tabs-panel-container",
                    north__paneSelector: "#tabs",
                    spacing_open: 0,
                    north__slidable: false
                });
            }
            function load__TrackHistory() {
                if (trackHistoryTable != null) {
                    trackHistoryTable.remove();
                    trackHistoryTable = null;
                }
                $('#trackhistory').html("loading..");
                load("/Music/Playlist", null, function(data) {
                    $('#trackhistory').html("");
                    trackHistoryTable = $("<table></table>");
                    var rows = [];
                    for (var i = 0; i < data.History.length; i++) {
                        var historyItem = data.History[i];
                        rows.push($("<tr><td>" + historyItem.Track.Title + "</td></tr>").appendTo(trackHistoryTable));
                        rows[i].find("td").hide();
                    }
                    $(trackHistoryTable).appendTo('#trackhistory');
                    for (var i = 0; i < rows.length; i++) {
                        rows[i].find("td").delay(10*i).fadeIn(100);
                    }
                });
            }
            function load__Favourites() {
                console.log("loading favourites");
            }
            function isLoginFormVisible() {
                console.log("isLoginFormVisible");
                return (loginForm != null && loginForm.is(":visible"));
            }
            function displayLoginForm() {
                console.log("displayLoginForm");
                if (loginForm == null) {
                    loginForm = $('#login-form').show();
                    loginForm.dialog({ width: 350,
                        modal: true,
                        closeOnEscape: false,
                        autoResize: true,
                        open: function() {
                            $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
                        },
                        buttons: {
                            'Login': function() {
                                if (loginButton == null) {
                                    loginButton = loginForm.parent().find("button:contains(Login)");
                                }
                                loginButton.html("Wait..");
                                load("/Account/Logon", $(this).find("form").serialize());
                            }
                        }
                    });
                } else {
                    loginButton.html("Login");
                    loginForm.dialog();
                }
            }
            function removeStaticContent() {
                console.log("removeStaticContent");
                $('#static-content').remove();
            }
            init();
        });
        return this;
    };
})(jQuery);
 
 $(function (){
    $(document.body).spofficeInterface();
 });