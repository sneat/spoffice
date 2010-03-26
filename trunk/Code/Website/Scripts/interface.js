(function($) {
    $.fn.spofficeInterface = function(settings) {
        var config = { 'foo': 'bar' };
        if (settings) $.extend(config, settings);
        this.each(function() {
            if (this.nodeName.toLowerCase() != "body") return;
            var loginForm;
            var loginButton;
            var centrallayout;
            var layout;
            var tabs;
            var trackHistoryTable;
            var artistaccordion;
            var albumaccordion;
            var icons = {
                header: "ui-icon-folder-collapsed",
                headerSelected: "ui-icon-folder-open"
            };
            function init() {
                removeStaticContent();
                getLoginStatus();
            }
            function getLoginStatus() {
                load("/Account/Logon");
            }
            function load(url, data, callback) {
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
                return (data.LoggedIn != null);
            }
            function dealWithLoginData(data) {
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
                if (loginForm != null)
                    loginForm.dialog("close");

                if (layout == null)
                    createLayout();
            }
            function resizeTabLayout() {
                if (!tabs) return;
                layout.resizeAll();
            }
            function createLayout() {
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
                centrallayout = $('.tabs-panel-container').layout({
                    center__paneSelector: "#middle",
                    west__paneSelector: "#left",
                    east__paneSelector: "#right",
                    east__initClosed: true,
                    west__initClosed: true,
                    east__size: 300,
                    west__size: 300,
                    spacing_open: 4,
                    spacing_closed: 4,
                    west__onresize: function() { artistaccordion.accordion("resize"); },
                    east__onresize: function() { albumaccordion.accordion("resize"); }
                });

                artistaccordion = $('#artistaccordion');
                albumaccordion = $('#albumaccordion');
                artistaccordion.accordion();
                albumaccordion.accordion();
                centrallayout.hide("west");
                centrallayout.hide("east");
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
                        var row = $("<tr />");
                        row.append(createTrackTd(historyItem.Track));
                        row.append(createAlbumTd(historyItem.Track.Album));
                        row.append(createArtistTd(historyItem.Track.Artist));
                        rows.push(row.appendTo(trackHistoryTable));
                        rows[i].find("td").hide();
                    }
                    $(trackHistoryTable).appendTo('#trackhistory');
                    for (var i = 0; i < rows.length; i++) {
                        rows[i].find("td").delay(10 * i).fadeIn(100);
                    }
                });
            }
            function createTrackTd(track, showartist) {
                var link = $('<a href="javascript:void(0);" class="track ui-state-default" />').html('<span class="ui-icon-circle-plus ui-icon"></span><span class="track-title">' + track.Title + '</span>');
                if (track.Artist != null && track.Artist.Name != null) {
                    link.attr("title", track.Artist.Name);
                    if (showartist) {
                        $('<span class="track-artist ui-priority-secondary">'+track.Artist.Name+'</span>').click(function (){
                            displayArtist(track.Artist.PublicId);
                        }).appendTo(link);
                    }
                }
                link.click(function() {
                    console.log("track clicked");
                }).mouseenter(function() {
                    $(this).toggleClass("ui-state-highlight", true);
                }).mouseleave(function() {
                    $(this).toggleClass("ui-state-highlight", false);
                });
                return $("<td />").append(link);
            }
            function createAlbumTd(album) {
                var link = $('<a href="javascript:void(0);" class="ui-state-default">' + album.Name + '</a>').click(function() {
                    displayAlbum(album.PublicId);
                });
                return $("<td />").append(link);
            }
            function createArtistTd(artist) {
                var link = $('<a href="javascript:void(0);" class="ui-state-default">' + artist.Name + '</a>').click(function() {
                    displayArtist(artist.PublicId);
                });
                return $("<td />").append(link);
            }
            function displayAlbum(id) {
                centrallayout.open("east");
                load("/Music/Album/" + id, null, function(data) {
                    var accordionLength = albumaccordion.find("h3").length;
                    albumaccordion.accordion("destroy");
                    if (accordionLength > 4) {
                        albumaccordion.find('h3:first').remove();
                        albumaccordion.find('div:first').remove();
                        accordionLength--;
                    }
                    albumaccordion.append('<h3><a href="#">' + data.Artist.Name + ' - ' + data.Name + '</a></h3><div></div>');
                    albumaccordion.accordion({
                        fillSpace: true,
                        icons: icons,
                        active: accordionLength < 0 ? 0 : accordionLength
                    });
                    var table = $('<table class="ui-helper-reset" cellspacing="0" />');
                    for (i = 0; i < data.Tracks.length; i++) {
                        var row = $('<tr />');
                        row.append(createTrackTd(data.Tracks[i], data.Tracks[i].Artist.Name != data.Artist.Name));
                        table.append(row);
                    }
                    albumaccordion.find('.ui-accordion-content-active').append(table);
                });
            }
            function displayArtist(id) {
                centrallayout.open("west");
                load("/Music/Artist/" + id, null, function(data) {
                    var accordionLength = artistaccordion.find("h3").length;
                    artistaccordion.accordion("destroy");
                    if (accordionLength > 4) {
                        artistaccordion.find('h3:first').remove();
                        artistaccordion.find('div:first').remove();
                        accordionLength--;
                    }
                    artistaccordion.append('<h3><a href="#">' + data.Name + ' Albums</a></h3><div></div>');
                    artistaccordion.accordion({
                        fillSpace: true,
                        icons: icons,
                        active: accordionLength < 0 ? 0 : accordionLength
                    });
                    var table = $('<table class="ui-helper-reset" cellspacing="0" />');
                    for (i = 0; i < data.Albums.length; i++) {
                        var row = $('<tr />');
                        row.append(createAlbumTd(data.Albums[i]));
                        table.append(row);
                    }
                    artistaccordion.find('.ui-accordion-content-active').append(table);
                });
            }
            function load__Favourites() {
            }
            function isLoginFormVisible() {
                return (loginForm != null && loginForm.is(":visible"));
            }
            function displayLoginForm() {
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