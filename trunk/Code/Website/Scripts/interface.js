(function($) {
    /**
    * spofficeInterface plugin
    * @param {Object} [settings] The config settings
    */
    $.fn.spofficeInterface = function(settings) {
        var config = {
            favouritesDiv: '#favourites',
            loadingDiv: '#loading',
            loginDiv: '#login-form',
            layoutDiv: '#layout',
            resultsDiv: '#results-tab',
            staticDiv: '#static-content',
            artistAccordionDiv: '#artistaccordion',
            albumAccordionDiv: '#albumaccordion',
            trackHistoryDiv: '#trackhistory',
            searchDiv: '#searchresults',
            progressBar: '#progress',

            searchForm: '#search-form',
            switcher: '#switcher',
            resultsTab: '#results-tab',

            footerDistance: 150,

            artistLocation: 'west',
            albumLocation: 'east',

            accordionLength: 5,

            baseCenterPaneSelector: '#main',
            baseNorthPaneSelector: '#header',
            baseNorthSize: 100,
            baseSpacingOpen: 0,
            baseNorthSlidable: false,
            tabsCenterPaneSelector: '.tabs-panel-container',
            tabsNorthPaneSelector: '#tabs',
            tabsSpacingOpen: 0,
            tabsNorthSlidable: false,
            mainCenterPaneSelector: '#middle',
            mainWestPaneSelector: '#left',
            mainEastPaneSelector: '#right',
            mainWestClosed: true,
            mainEastClosed: true,
            mainWestSize: 326,
            mainEastSize: 300,
            mainSpacingOpen: 4,
            mainSpacingClosed: 4
        };
        if (settings) $.extend(config, settings);
        this.each(function() {
            if (this.nodeName.toLowerCase() != "body") return;
            var loginForm;
            var loginButton;
            var centrallayout;
            var layout;
            var tabs;
            var trackHistoryTable;
            var searchTable;
            var oldTrackHistoryTop = 0;
            var artistaccordion;
            var albumaccordion;
            var loadingTrackHistory = false;
            var loadingFavourites = false;
            var favouritesTable;
            var oldFavouritesTop = 0;
            var favourites = null;
            var registerForm = null;
            var registerButton = null;
            var progressBar;
            var updater;
            var language;
            var icons = {
                header: "ui-icon-folder-collapsed",
                headerSelected: "ui-icon-folder-open"
            };

            /**
            * Called when the class initially loads
            */
            function init() {
                load("/Home/Localization", null, function(data) {
                    language = data.Language;
                    removeStaticContent();
                    getLoginStatus();
                });
            }

            function loadLanguage(lang, callback) {
                load("/Home/Localization/" + lang, null, function(data) {
                    language = data.Language;
                    switchLanguage();
                });
            }

            function switchLanguage() {
                console.log("switching language");
            }

            /**
            * Requests the current user Login Status
            */
            function getLoginStatus() {
                load("/Account/Logon");
            }

            /**
            * Requests the specified data from the URL
            * @param {String} url
            * @param {String|Object} [data] Data to be sent to the server
            * @param {Function} [callback] Function to call on success
            * @see <a href="http://api.jquery.com/jQuery.ajax/">jQuery AJAX Documentation</a>
            */
            function load(url, data, callback) {
                $.ajaxSetup({ async: true, callback: null, timeout: 20000 });
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

            /**
            * Checks whether the user is logged in
            * @param {Object} data The user object
            * @returns {Boolean} Whether user is logged in
            */
            function isLoginData(data) {
                return (data.LoggedIn != null);
            }

            /**
            * Deals with the response of {@link isLoginData}
            * Runs either {@link displayLoginForm} or {@link onLogin}
            * @param {Object} data The user object
            */
            function dealWithLoginData(data) {
                var loadingdiv = $(config.loadingDiv);
                if (loadingdiv.is(":visible")) {
                    loadingdiv.fadeOut();
                };
                if (data.LoggedIn) {
                    onLogin();
                } else {
                    if (!isLoginFormVisible()) {
                        displayLoginForm();
                    } else {
                        loginButton.find(".ui-button-text").html("Login");
                        if (data.ErrorMessages != null && data.ErrorMessages.length > 0) {
                            var messages = [];
                            for (var i = 0; i < data.ErrorMessages.length; i++) {
                                var error = data.ErrorMessages[i];
                                loginForm.find("input[ref=" + error.Field + "]").addClass("ui-state-error");
                                for (var a = 0; a < error.Message.length; a++) {
                                    messages.push({ label: "Error", message: error.Message[0] });
                                }
                            }
                            var errorMsg = createErrorMessage(messages).hide();
                            loginForm.find("fieldset").prepend(errorMsg);
                            errorMsg.slideDown();
                        }
                    }
                }
            }

            /**
            * Creates an error message. This does not append to the HTML
            * @param {String} label The label for the message
            * @param {String} The text for the message
            */
            function createErrorMessage(messages) {
                var outerdiv = $('<div class="ui-state-error ui-corner-all" />');
                for (var i = 0; i < messages.length; i++) {
                    outerdiv.append('<p><span class="ui-icon ui-icon-alert"></span><strong>' + messages[i].label + '</strong> ' + messages[i].message + '</p>');
                }
                return outerdiv;
            }

            /**
            * Closes the login form and runs {@link createLayout}
            */
            function onLogin() {

                load("/Favourites/", null, function(data) {

                    favourites = data.Favourites;

                    if (loginForm != null)
                        loginForm.dialog("close");

                    if (layout == null)
                        createLayout();

                });
            }

            /**
            * Resizes the layout if the layout has tabs
            */
            function resizeTabLayout() {
                if (!tabs) return;
                layout.resizeAll();
            }

            /**
            * Creates the interface layout
            * @see <a href="http://layout.jquery-dev.net/">jQuery Layout</a>
            */
            function createLayout() {
                $(document.body).append($(config.layoutDiv).html());
                $('#layout').remove();

                /**
                * Create base layout consisting of a center and north pane
                */
                layout = $('body').layout({
                    center__paneSelector: config.baseCenterPaneSelector,
                    north__paneSelector: config.baseNorthPaneSelector,
                    north__size: config.baseNorthSize,
                    spacing_open: config.baseSpacingOpen,
                    north__slidable: config.baseNorthSlidable
                });
                /**
                * Create the tab interface and configure the events that each tag represents
                */
                tabs = $(config.baseCenterPaneSelector).tabs({
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
                $(config.resultsDiv).hide();
                /**
                * Add the tab interface to the center pane of the base layout
                */
                $(config.baseCenterPaneSelector).layout({
                    center__paneSelector: config.tabsCenterPaneSelector,
                    north__paneSelector: config.tabsNorthPaneSelector,
                    spacing_open: config.tabsSpacingOpen,
                    north__slidable: config.tabsNorthSlidable
                });
                /**
                * Set up the central tab panel
                * Configure east and west panels to use accordion
                */
                centrallayout = $(config.tabsCenterPaneSelector).layout({
                    center__paneSelector: config.mainCenterPaneSelector,
                    east__paneSelector: config.mainEastPaneSelector,
                    west__paneSelector: config.mainWestPaneSelector,
                    east__initClosed: config.mainEastClosed,
                    west__initClosed: config.mainWestClosed,
                    east__size: config.mainEastSize,
                    west__size: config.mainWestSize,
                    spacing_open: config.mainSpacingOpen,
                    spacing_closed: config.mainSpacingClosed,
                    west__onresize: function() { artistaccordion.accordion("resize"); },
                    east__onresize: function() { albumaccordion.accordion("resize"); }
                });

                artistaccordion = $(config.artistAccordionDiv);
                albumaccordion = $(config.albumAccordionDiv);
                centrallayout.hide(config.artistLocation);
                centrallayout.hide(config.albumLocation);

                $(config.switcher).themes();

                $(config.searchForm).submit(search).find('input[type=submit]').button();
                progressBar = $(config.progressBar).progressbar({
                    value: 0
                });

                updater = $.MusicInfoUpdater.init("/Music/Current", {
                    onTrackChange: function() {
                        updateNowPlaying();
                        progressBar.progressbar("value", 0);
                    },
                    onStateChange: function(state) {
                    },
                    onReady: function() {
                        updateNowPlaying();
                        setInterval(function() {
                            progressBar.progressbar("value", updater.getProgress());
                        }, 200);
                    },
                    loadMethod: load
                });
            }

            function updateNowPlaying() {
                var td = createTrackTd(updater.track, true);
                $('#current_track').empty();
                $('#current_track').append(td.children());
                //$('#current_track').html(updater.track.title);
                //$('#current_artist').html(updater.track.artist.name);
            }

            /**
            * Called when TrackHistory tab is clicked
            * Gets the track history in blocks from the database. If the user scrolls to near the bottom of the page,
            * the next block of track histories is requested and appended to the table.
            * TODO Ensure that requests stop once we have the entire track history list
            * @see addTrackHistoryRows
            */
            function load__TrackHistory() {
                if (trackHistoryTable == null) {
                    var trackHistoryDiv = $(config.trackHistoryDiv);
                    trackHistoryDiv.empty();
                    trackHistoryTable = $("<table></table>").appendTo(trackHistoryDiv);
                    setupElementAsScrolling("/Music/Playlist", trackHistoryDiv, trackHistoryTable, 20, "History", function(data) {
                        var now = new Date().getTime();
                        var rowcount = data.History.length;
                        for (var i = 0; i < rowcount; i++) {
                            var item = data.History[i];
                            var row = $("<tr />");
                            // Create the row containing the track history data
                            var time = new Date(item.Timestamp).getTime();
                            row.append(createTrackTd(item.Track));
                            row.append(createTrackLengthTd(item.Track));
                            row.append(createArtistTd(item.Track.Artist));
                            row.append(createAlbumTd(item.Track.Album));
                            row.appendTo(trackHistoryTable);
                        }
                        $(trackHistoryTable).appendTo(trackHistoryDiv);
                    });
                }
            }

            /**
            * Search for a track and list the results
            */
            function search() {
                var search_value = $(this).find("input[type=text]").val();
                load("/Music/Search/" + search_value, null, function(data) {
                    $(config.resultsTab).show().find("a").html("Search Results: " + search_value);
                    tabs.tabs("select", 4);
                    if (searchTable == null) {
                        var searchDiv = $(config.searchDiv);
                        searchDiv.empty();
                        searchTable = $("<table></table>").appendTo(searchDiv);
                    }
                    searchTable.empty();
                    for (var i = 0; i < data.Tracks.length; i++) {
                        var item = data.Tracks[i];
                        var row = $("<tr />");
                        row.append(createTrackTd(item));
                        row.append(createTrackLengthTd(item));
                        row.append(createArtistTd(item.Artist));
                        row.append(createAlbumTd(item.Album));
                        row.appendTo(searchTable);
                    }
                });
                return false;
            }

            /**
            * Check to see if a track id is in the favourites already
            * @param (string) the ID of the track
            */
            function isInFavourites(trackid) {
                if (favourites != null) {
                    for (var i = 0; i < favourites.length; i++) {
                        if (favourites[i].Track.PublicId == trackid) {
                            return i;
                        }
                    }
                }
                return -1;
            }

            /**
            * Creates the table cells showing the Track information
            * Includes a link to Add/Remove the track from Favourites
            * @param {Object} track The Track object
            * @param {Boolean} showartist Whether to show the name of the Artist
            * @see addTrackHistoryRows
            * @see displayAlbum
            */
            function createTrackTd(track, showartist) {
                var link = $('<a href="javascript:void(0);" class="track" />');
                var td = $('<td />').append(link);
                /**
                * Checks whether the Track ID is in the favourites array
                * @see dealWithLoginData
                */
                if (isInFavourites(track.PublicId) > -1) {
                    link.html('<span class="ui-icon-circle-minus ui-icon"></span>');
                } else {
                    link.html('<span class="ui-icon-circle-plus ui-icon"></span>');
                }
                link.append('<span class="track-title">' + track.Title + '</span>');
                if (track.Artist != null && track.Artist.Name != null) {
                    link.attr("title", track.Artist.Name);
                    if (showartist) {
                        createArtistLink(track.Artist).addClass("track-artist").addClass("ui-priority-secondary")
                            .appendTo(td);
                    }
                }
                // Configure click and hover events
                link.attr("trackid", track.PublicId).click(function() {
                    var id = $(this).attr("trackid");
                    var index = isInFavourites(id)
                    if (index > -1) {
                        // Track is already a Favourite, therefore we want to remove it
                        load("/Favourites/Remove/" + id, null, function(data) {
                            if (data.StatusCode == "OK") {
                                favourites.splice(index, 1);
                                if (favouritesTable != null) {
                                    favouritesTable.find("a[trackid=" + id + "]").parents("tr").fadeOut("slow", function() { $(this).remove(); }); ;
                                }
                                $(document.body).find("a[trackid=" + id + "]").find(".ui-icon")
                                    .removeClass("ui-icon-circle-minus").addClass("ui-icon-circle-plus");
                            }
                        });
                    } else {
                        // Track is not already a Favourite, therefore we want to add it
                        load("/Favourites/Add/" + id, null, function(data) {
                            if (data.StatusCode == "OK") {
                                favourites.push(data.Favourite);
                                $(document.body).find("a[trackid=" + id + "]").find(".ui-icon")
                                    .removeClass("ui-icon-circle-plus").addClass("ui-icon-circle-minus");
                            }
                        });
                    }
                }).hover(
					function() {
					    $(this).toggleClass("ui-state-highlight", true);
					},
					function() {
					    $(this).toggleClass("ui-state-highlight", false);
					}
				);
                return td;
            }

            /**
            * Creates the table cell containing the Track Length information
            * @param {Object} track The Track object
            */
            function createTrackLengthTd(track) {
                var link = $('<span class="track-length"><span class="ui-icon-clock ui-icon"></span>' + track.FormattedLength + '</span>');
                return $("<td />").append(link);
            }

            /**
            * Creates the table cell containing the Album information
            * @param {Object} album The Album object
            */
            function createAlbumTd(album) {
                return $("<td />").append(createAlbumLink(album));
            }

            /**
            * Creates the table cell containing the Album link
            * @param {Object} album The Album object
            */
            function createAlbumLink(album) {
                return $('<a href="javascript:void(0);"><span class="ui-icon-newwin ui-icon"></span><span class="album-title">' + album.Name + '</span></a>').click(function() {
                    displayAlbum(album.PublicId);
                }).hover(
					function() {
					    $(this).toggleClass("ui-state-highlight", true);
					},
					function() {
					    $(this).toggleClass("ui-state-highlight", false);
					}
				);
            }

            /**
            * Creates the table cell containing the Artist information
            * @param {Object} artist The Artist object
            */
            function createArtistTd(artist) {
                return $("<td />").append(createArtistLink(artist));
            }

            /**
            * Creates the table cell containing the Artist link
            * @param {Object} artist The Artist object
            */
            function createArtistLink(artist) {
                return $('<a href="javascript:void(0);"><span class="ui-icon-newwin ui-icon"></span><span class="artist-title">' + artist.Name + '</span></a>').click(function() {
                    displayArtist(artist.PublicId);
                }).hover(
					function() {
					    $(this).toggleClass("ui-state-highlight", true);
					},
					function() {
					    $(this).toggleClass("ui-state-highlight", false);
					}
				);
            }

            /**
            * Display Album details in the album panel using jQuery UI Accordion
            * @param {String} id The Album Guid
            * @see <a href="http://docs.jquery.com/UI/Accordion">jQuery UI Accordion</a>
            */
            function displayAlbum(id) {
                centrallayout.open(config.albumLocation);
                load("/Music/Album/" + id, null, function(data) {
                    var accordionLength = albumaccordion.find("h3").length;
                    if (albumaccordion.accordion != null) {
                        // Remove the accordion functionality, makes initialising it easier later on
                        albumaccordion.accordion("destroy");
                    }
                    if (accordionLength >= config.accordionLength) {
                        /**
                        * We only want a max of {@link config.accordionLength}
                        */
                        albumaccordion.find('h3:first').remove();
                        albumaccordion.find('div:first').remove();
                        accordionLength--;
                    }
                    // Add the new album to display
                    albumaccordion.append('<h3><a href="#">' + data.Artist.Name + ' - ' + data.Name + '</a></h3><div></div>');
                    // Initialise the accordion
                    albumaccordion.accordion({
                        fillSpace: true,
                        icons: icons,
                        active: accordionLength < 0 ? 0 : accordionLength
                    });
                    var table = $('<table class="ui-helper-reset" cellspacing="0" />');
                    var trackCount = data.Tracks.length;
                    for (i = 0; i < trackCount; i++) {
                        /**
                        * Show each track from the album, include the Artist name if it varies from the Album artist
                        * (eg. a compilation album)
                        */
                        var row = $('<tr />');
                        row.append(createTrackTd(data.Tracks[i], data.Tracks[i].Artist.Name != data.Artist.Name));
                        table.append(row);
                    }
                    // Add the Album Cover image, but make sure we don't insert a broken image
                    var img = $('<img>').error(function() {
                        $(this).remove();
                    }).attr('src', '/Music/AlbumImage/' + id);
                    albumaccordion.find('.ui-accordion-content-active').append(img).append(table);
                });
            }

            /**
            * Display Artist details in the artist panel using jQuery UI Accordion
            * @param {String} id The Artist Guid
            * @see <a href="http://docs.jquery.com/UI/Accordion">jQuery UI Accordion</a>
            * TODO Add album cover thumbnail
            */
            function displayArtist(id) {
                centrallayout.open(config.artistLocation);
                load("/Music/Artist/" + id, null, function(data) {
                    var accordionLength = artistaccordion.find("h3").length;
                    if (artistaccordion.accordion != null) {
                        // Remove the accordion functionality, makes initialising it easier later on
                        artistaccordion.accordion("destroy");
                    }
                    if (accordionLength >= config.accordionLength) {
                        /**
                        * We only want a max of {@link config.accordionLength}
                        */
                        artistaccordion.find('h3:first').remove();
                        artistaccordion.find('div:first').remove();
                        accordionLength--;
                    }
                    // Add the new artist to display
                    artistaccordion.append('<h3><a href="#">' + data.Name + ' Albums</a></h3><div></div>');
                    // Initialise the accordion
                    artistaccordion.accordion({
                        fillSpace: true,
                        icons: icons,
                        active: accordionLength < 0 ? 0 : accordionLength
                    });
                    var table = $('<table class="ui-helper-reset" cellspacing="0" />');
                    var albumCount = data.Albums.length;
                    for (i = 0; i < albumCount; i++) {
                        // Show each album for the artist
                        var row = $('<tr />');
                        row.append(createAlbumTd(data.Albums[i]));
                        table.append(row);
                    }
                    artistaccordion.find('.ui-accordion-content-active').append(table);
                });
            }

            /**
            * Called when Favourites tab is clicked
            * Gets the track favourites in blocks from the database. If the user scrolls to near the bottom of the
            * page, the next block of track favourites is requested and appended to the table.
            * TODO Ensure that requests stop once we have the entire track favourites list
            * @see addTrackHistoryRows
            */
            function load__Favourites() {
                if (favouritesTable == null) {
                    var favouritesDiv = $(config.favouritesDiv);
                    favouritesDiv.empty();
                    favouritesTable = $("<table></table>").appendTo(favouritesDiv);
                }
                favouritesTable.empty();
                for (var i = 0; i < favourites.length; i++) {
                    var item = favourites[i];
                    var row = $("<tr />");
                    row.append(createTrackTd(item.Track));
                    row.append(createArtistTd(item.Track.Artist));
                    row.append(createAlbumTd(item.Track.Album));
                    row.appendTo(favouritesTable);
                }
            }

            function setupElementAsScrolling(url, container, table, rowheight, arrayName, callback) {
                var loadingRows = true;
                var default_amount = Math.ceil((container.outerHeight() / rowheight) * 1.2);
                var amount = default_amount;
                var from = 0;
                var gotAll = false;
                container.html("loading..").scroll(function() {
                    if (!loadingRows && (this.scrollHeight - (container.scrollTop() + container.outerHeight()) < config.footerDistance)) {
                        from = table.find("tr").length;
                        loadingRows = true;
                        load(url + "?from=" + from + "&amount=" + amount, null, function(data) {
                            if (data[arrayName].length < amount) {
                                gotAll = true;
                            }
                            loadingRows = false;
                            callback(data);
                        });
                    }
                }).each(function() {
                    load(url + "?from=" + from + "&amount=" + amount, null, function(data) {
                        container.empty();
                        if (data[arrayName].length < amount) {
                            gotAll = true;
                        }
                        loadingRows = false;
                        callback(data);
                    });
                });
            }


            /**
            * @returns {Boolean} Whether the login form exists and is visible
            */
            function isLoginFormVisible() {
                return (loginForm != null && loginForm.is(":visible"));
            }

            /**
            * Displays the login form in a draggable dialog window
            */
            function displayLoginForm() {
                if (loginForm == null) {
                    loginForm = $(config.loginDiv).show();

                    $(loginForm).find("form").submit(function() {
                        // Bind to the submit event of the form
                        submitLoginForm($(this));
                        return false;
                    });
                    // Create the dialog window
                    var buttons = {};
                    buttons[language.Login] = function() {
                        submitLoginForm($(this).find("form"));
                    };
                    buttons[language.Register] = function() {
                        displayRegisterForm();
                    };
                    loginForm.dialog({
                        width: 350,
                        modal: true,
                        closeOnEscape: false,
                        autoResize: true,
                        open: function() {
                            $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
                        },
                        buttons: buttons
                    });
                } else {
                    // Reset the login button and create the dialog window again
                    loginButton.find(".ui-button-text").html("Login");
                    loginForm.dialog();
                }
            }

            function formatMillisecondsTimeSpan(millis) {
                var s = function(n) { return n == 1 ? '' : 's' };
                var seconds = millis / 1000;
                if (seconds < 0) {
                    return 'just now';
                }
                if (seconds < 60) {
                    var n = seconds;
                    return n + ' second' + s(n) + ' ago';
                }
                if (seconds < 60 * 60) {
                    var n = Math.floor(seconds / 60);
                    return n + ' minute' + s(n) + ' ago';
                }
                if (seconds < 60 * 60 * 24) {
                    var n = Math.floor(seconds / 60 / 60);
                    return n + ' hour' + s(n) + ' ago';
                }
                if (seconds < 60 * 60 * 24 * 7) {
                    var n = Math.floor(seconds / 60 / 60 / 24);
                    return n + ' day' + s(n) + ' ago';
                }
                if (seconds < 60 * 60 * 24 * 31) {
                    var n = Math.floor(seconds / 60 / 60 / 24 / 7);
                    return n + ' week' + s(n) + ' ago';
                }
                if (seconds < 60 * 60 * 24 * 365) {
                    var n = Math.floor(seconds / 60 / 60 / 24 / 31);
                    return n + ' month' + s(n) + ' ago';
                }
                var n = Math.floor(seconds / 60 / 60 / 24 / 365);
                return n + ' year' + s(n) + ' ago';
            }

            /**
            * Handles the submitting of the login form
            * @param {Object} form The login form data
            */
            function submitLoginForm(form) {
                if (loginButton == null) {
                    // Find the login button
                    loginButton = loginForm.parent().find("button:contains(Login)");
                }
                loginButton.find(".ui-button-text").html("Wait..");
                form.find('div.ui-state-error').slideUp();
                form.find('input.ui-state-error').removeClass("ui-state-error");
                load("/Account/Logon", form.serialize());
            }

            function displayRegisterForm() {
                if (registerForm == null) {
                    registerForm = $('#register-form');
                    $(registerForm).find("form").submit(function() {
                        submitLoginForm($(this));
                        return false;
                    });
                    registerForm.show();
                    registerForm.dialog({
                        width: 350,
                        modal: false,
                        closeOnEscape: false,
                        autoResize: true,
                        buttons: {
                            'Register': function() {
                                submitRegisterForm(registerForm.find("form"));
                            }
                        }
                    });
                } else {
                    registerForm.dialog("open");
                }
            }

            function submitRegisterForm(form) {
                if (registerButton == null) {
                    // Find the login button
                    registerButton = registerForm.parent().find("button:contains(Register)");
                }
                registerButton.find(".ui-button-text").html("Wait..");
                form.find('div.ui-state-error').slideUp();
                form.find('input.ui-state-error').removeClass("ui-state-error");
                load("/Account/Register", form.serialize(), function(data) {
                    registerButton.find(".ui-button-text").html("Register");
                    if (!data.Success) {
                        if (data.ErrorMessages != null && data.ErrorMessages.length > 0) {
                            var messages = [];
                            for (var i = 0; i < data.ErrorMessages.length; i++) {
                                var error = data.ErrorMessages[i];
                                for (var a = 0; a < error.Message.length; a++) {
                                    registerForm.find("input[ref=" + error.Field + "]").addClass("ui-state-error");
                                    messages.push({ label: 'Error', message: error.Message[a] });
                                }
                            }
                            var errorMsg = createErrorMessage(messages).hide();
                            registerForm.find("fieldset").prepend(errorMsg);
                            errorMsg.slideDown();
                        }
                    }
                });
            }

            /**
            * Removes the static content container
            */
            function removeStaticContent() {
                $(config.staticDiv).remove();
            }

            // Run the initialisation function
            init();
        });
        return this;
    };
})(jQuery);

var first = true;

// IE doesn't update the display immediately, so reload the page
function reloadIE(id, display, url) {
    if (!first && $.browser.msie) {
        window.location.href = window.location.href;
    }
    first = false;
}

/**
 * When the DOM is ready, run the spofficeInterface on the document body
 */
$(function() {
    $('#themes').click(function() {
        var offset = $(this).offset();
        $('#switcher').css('left', offset.left).
            css('top', offset.top + $(this).outerHeight());
        $('#switcher,#themes_label').toggle();
    });
    $.themes.init({ themeBase: 'http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/themes/',
        icons: 'Content/themes/img/themes.gif',
        previews: 'Content/themes/img/themes-preview.gif',
        onSelect: reloadIE
    });
    $('#switcher').themes();

    $(document.body).spofficeInterface();

});