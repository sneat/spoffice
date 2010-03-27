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
            var oldTrackHistoryTop = 0;
            var artistaccordion;
            var albumaccordion;
            var loadingTrackHistory = false;
            var favourites = null;
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
                    favourites = data.Favourites;
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
                    east__size: 326,
                    west__size: 300,
                    spacing_open: 4,
                    spacing_closed: 4,
                    west__onresize: function() { artistaccordion.accordion("resize"); },
                    east__onresize: function() { albumaccordion.accordion("resize"); }
                });

                artistaccordion = $('#artistaccordion');
                albumaccordion = $('#albumaccordion');
                centrallayout.hide("west");
                centrallayout.hide("east");
            }
            function load__TrackHistory() {
                if (trackHistoryTable == null) {
                    var amount = 0;
                    loadingTrackHistory = true;
                    $('#trackhistory').html("loading..").scroll(function() {
                        if (trackHistoryTable != null) {
                            var th = $('#trackhistory');
                            var thc = th[0];
                            if ((thc.scrollHeight - th.scrollTop() < th.outerHeight() + 150) && !loadingTrackHistory) {
                                var rowcount = trackHistoryTable.find("tr").length;
                                if (oldTrackHistoryTop != rowcount) {
                                    oldTrackHistoryTop = rowcount;
                                    loadingTrackHistory = true;

                                    load("/Music/Playlist/?from=" + rowcount + "&amount=" + amount, null, function(data) {
                                        addTrackHistoryRows(data);
                                        loadingTrackHistory = false;
                                    });
                                }
                            }
                        }
                    }).each(function() {
                        var self = $(this);
                        amount = Math.ceil((self.outerHeight() / 20) * 1.2);
                    });
                    load("/Music/Playlist/?amount=" + amount, null, function(data) {
                        $('#trackhistory').html("");
                        trackHistoryTable = $("<table></table>");
                        addTrackHistoryRows(data);
                        loadingTrackHistory = false;
                    });
                }
            }
            function addTrackHistoryRows(data) {
                var rows = [];
                for (var i = 0; i < data.History.length; i++) {
                    var historyItem = data.History[i];
                    var row = $("<tr />");
                    row.append(createTrackTd(historyItem.Track));
                    row.append(createTrackLengthTd(historyItem.Track));
                    row.append(createArtistTd(historyItem.Track.Artist));
                    row.append(createAlbumTd(historyItem.Track.Album));
                    rows.push(row.appendTo(trackHistoryTable));
                    rows[i].find("td").hide();
                }
                $(trackHistoryTable).appendTo('#trackhistory');
                for (var i = 0; i < rows.length; i++) {
                    rows[i].find("td").delay(10 * i).fadeIn(100);
                }
            }
            function createTrackTd(track, showartist) {
                var link = $('<a href="javascript:void(0);" class="track" />');
                var td = $('<td />').append(link);
                if ($.inArray(track.PublicId, favourites) > -1) {
                    link.html('<span class="ui-icon-circle-minus ui-icon"></span>');
                } else {
                    link.html('<span class="ui-icon-circle-plus ui-icon"></span>');
                }
                link.append('<span class="track-title">' + track.Title + '</span>');
                if (track.Artist != null && track.Artist.Name != null) {
                    link.attr("title", track.Artist.Name);
                    if (showartist) {
                        createArtistLink(track.Artist).addClass("track-artist").addClass("ui-priority-secondary").appendTo(td);
                    }
                }
                link.attr("trackid", track.PublicId).click(function() {
                    var id = $(this).attr("trackid");
                    var index = $.inArray(id, favourites);
                    if (index > -1) {
                        load("/Favourites/Remove/" + id, null, function(data) {
                            if (data.StatusCode == "OK") {
                                favourites.splice(index, 1);
                                $(document.body).find("a[trackid=" + id + "]").find(".ui-icon").removeClass("ui-icon-circle-minus").addClass("ui-icon-circle-plus");
                            }
                        });
                    } else {
                        load("/Favourites/Add/" + id, null, function(data) {
                            if (data.StatusCode == "OK") {
                                favourites.push(id);
                                $(document.body).find("a[trackid=" + id + "]").find(".ui-icon").removeClass("ui-icon-circle-plus").addClass("ui-icon-circle-minus");
                            }
                        });
                    }
                }).mouseenter(function() {
                    $(this).toggleClass("ui-state-highlight", true);
                }).mouseleave(function() {
                    $(this).toggleClass("ui-state-highlight", false);
                });
                return td;
            }
            function createTrackLengthTd(track) {
                var link = $('<span class="track-length"><span class="ui-icon-clock ui-icon"></span>' + track.FormattedLength + '</span>');
                return $("<td />").append(link);
            }
            function createAlbumLink(album) {
                return $('<a href="javascript:void(0);"><span class="ui-icon-newwin ui-icon"></span><span class="album-title">' + album.Name + '</span></a>').click(function() {
                    displayAlbum(album.PublicId);
                }).mouseenter(function() {
                    $(this).toggleClass("ui-state-highlight", true);
                }).mouseleave(function() {
                    $(this).toggleClass("ui-state-highlight", false);
                });
            }
            function createAlbumTd(album) {
                return $("<td />").append(createAlbumLink(album));
            }
            function createArtistLink(artist) {
                return $('<a href="javascript:void(0);"><span class="ui-icon-newwin ui-icon"></span><span class="artist-title">' + artist.Name + '</span></a>').click(function() {
                    displayArtist(artist.PublicId);
                }).mouseenter(function() {
                    $(this).toggleClass("ui-state-highlight", true);
                }).mouseleave(function() {
                    $(this).toggleClass("ui-state-highlight", false);
                });
            }
            function createArtistTd(artist) {
                return $("<td />").append(createArtistLink(artist));
            }
            function displayAlbum(id) {
                centrallayout.open("east");
                load("/Music/Album/" + id, null, function(data) {
                    var accordionLength = albumaccordion.find("h3").length;
                    if (albumaccordion.accordion != null) {
                        albumaccordion.accordion("destroy");
                    }
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
                    var img = $('<img>').error(function() {
                        $(this).remove();
                    }).attr('src', '/Music/AlbumImage/' + id);
                    albumaccordion.find('.ui-accordion-content-active').append(img).append(table);
                });
            }
            function displayArtist(id) {
                centrallayout.open("west");
                load("/Music/Artist/" + id, null, function(data) {
                    var accordionLength = artistaccordion.find("h3").length;
                    if (artistaccordion.accordion != null) {
                        artistaccordion.accordion("destroy");
                    }
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

                    $(loginForm).find("form").submit(function() {
                        submitForm($(this));
                        return false;
                    });
                    loginForm.dialog({ width: 350,
                        modal: true,
                        closeOnEscape: false,
                        autoResize: true,
                        open: function() {
                            $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
                        },
                        buttons: {
                            'Login': function() {
                                submitForm($(this).find("form"));
                            }
                        }
                    });
                } else {
                    loginButton.html("Login");
                    loginForm.dialog();
                }
            }
            function submitForm(form) {
                if (loginButton == null) {
                    loginButton = loginForm.parent().find("button:contains(Login)");
                }
                loginButton.html("Wait..");
                load("/Account/Logon", form.serialize());
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