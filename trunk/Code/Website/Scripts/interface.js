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
            var timeouts = {};
            var z = 100;
            var icons = {
                header: "ui-icon-folder-collapsed",
                headerSelected: "ui-icon-folder-open"
            };
            var THEME_COOKIE_NAME = 'themeID';
            var themes = {  // The definitions of the available themes
                'blacktie': { display: 'Black Tie', icon: 0, preview: 0,
                    url: 'black-tie/ui.all.css'
                },
                'blitzer': { display: 'Blitzer', icon: 1, preview: 1,
                    url: 'blitzer/ui.all.css'
                },
                'cupertino': { display: 'Cupertino', icon: 2, preview: 2,
                    url: 'cupertino/ui.all.css'
                },
                'darkhive': { display: 'Dark Hive', icon: 17, preview: 17,
                    url: 'dark-hive/ui.all.css'
                },
                'dotluv': { display: 'Dot Luv', icon: 3, preview: 3,
                    url: 'dot-luv/ui.all.css'
                },
                'eggplant': { display: 'Eggplant', icon: 18, preview: 18,
                    url: 'eggplant/ui.all.css'
                },
                'excitebike': { display: 'Excite Bike', icon: 4, preview: 4,
                    url: 'excite-bike/ui.all.css'
                },
                'flick': { display: 'Flick', icon: 19, preview: 19,
                    url: 'flick/ui.all.css'
                },
                'hotsneaks': { display: 'Hot Sneaks', icon: 5, preview: 5,
                    url: 'hot-sneaks/ui.all.css'
                },
                'humanity': { display: 'Humanity', icon: 6, preview: 6,
                    url: 'humanity/ui.all.css'
                },
                'lefrog': { display: 'Le Frog', icon: 20, preview: 20,
                    url: 'le-frog/ui.all.css'
                },
                'mintchoc': { display: 'Mint Choc', icon: 7, preview: 7,
                    url: 'mint-choc/ui.all.css'
                },
                'overcast': { display: 'Overcast', icon: 21, preview: 21,
                    url: 'overcast/ui.all.css'
                },
                'peppergrinder': { display: 'Pepper Grinder', icon: 22, preview: 22,
                    url: 'pepper-grinder/ui.all.css'
                },
                'redmond': { display: 'Redmond', icon: 8, preview: 8,
                    url: 'redmond/ui.all.css'
                },
                'smoothness': { display: 'Smoothness', icon: 9, preview: 9,
                    url: 'smoothness/ui.all.css'
                },
                'southstreet': { display: 'South Street', icon: 10, preview: 10,
                    url: 'south-street/ui.all.css'
                },
                'start': { display: 'Start', icon: 11, preview: 11,
                    url: 'start/ui.all.css'
                },
                'sunny': { display: 'Sunny', icon: 23, preview: 23,
                    url: 'sunny/ui.all.css'
                },
                'swankypurse': { display: 'Swanky Purse', icon: 12, preview: 12,
                    url: 'swanky-purse/ui.all.css'
                },
                'trontastic': { display: 'Trontastic', icon: 13, preview: 13,
                    url: 'trontastic/ui.all.css'
                },
                'uidarkness': { display: 'UI Darkess', icon: 14, preview: 14,
                    url: 'ui-darkness/ui.all.css'
                },
                'uilightness': { display: 'UI Lightness', icon: 15, preview: 15,
                    url: 'ui-lightness/ui.all.css'
                },
                'vader': { display: 'Vader', icon: 16, preview: 16,
                    url: 'vader/ui.all.css'
                }
            };

            /**
            * Called when the class initially loads
            */
            function init() {
                load("/Home/Localization", null, function(data) {

                    $.each(themes, function(key, val) {
                        $('<li role="menuitem"><a href="javascript:void(0);" class="ui-state-default ui-corner-all" tabindex="-1" id="theme_' + key + '" >' +
                            '<span class="theme_preview" title="' + val.display + '" style="background-position: -' + (val.icon * 23) + 'px 0px;"></span>' + val.display + '</a></li>').
                        click(function(e) {
                            e.preventDefault();
                            switchTheme(key, val);
                        }).mouseenter(function() {
                            $(this).toggleClass("hover", true).find("a").toggleClass("ui-state-hover", true);
                        }).mouseleave(function() {
                            $(this).toggleClass("hover", false).find("a").toggleClass("ui-state-hover", false);
                        }).appendTo($('#available-themes ul'));
                    });

                    language = data.Language; // Sets the language
                    $.each(data.AvailableLanguages, function(key, val) {
                        $('<li role="menuitem"><a href="javascript:void(0);" class="ui-state-default ui-corner-all" tabindex="-1" id="language_' + key + '"><img src="/Content/flags/' + key + '.gif" />' + val + '</a></li>').
                        click(function(e) {
                            e.preventDefault();
                            loadLanguage(key);
                        }).mouseenter(function() {
                            $(this).toggleClass("hover", true).find("a").toggleClass("ui-state-hover", true);
                        }).mouseleave(function() {
                            $(this).toggleClass("hover", false).find("a").toggleClass("ui-state-hover", false);
                        }).appendTo($('#available-languages ul'));
                    });
                    $('a#language_' + data.CurrentCulture).toggleClass("ui-state-highlight", true);
                    $('#languagelabel, #themelabel').mouseenter(function() {
                        var me = $(this);
                        clearTimeout(timeouts[me.attr("rel")]);
                        me.toggleClass("ui-state-hover", true);
                        var related = $(me.attr("href"));
                        var offset = me.offset();
                        related.css({
                            'position': 'absolute',
                            'top': offset.top + me.outerHeight() + 2,
                            'left': offset.left,
                            'z-index': z++
                        })
                        related.slideDown();
                    }).mouseleave(function() {
                        var me = $(this);
                        clearTimeout(timeouts[me.attr("rel")]);
                        timeouts[me.attr("rel")] = setTimeout(function() {
                            me.toggleClass("ui-state-hover", false);
                            var related = $(me.attr("href"));
                            related.slideUp();
                        }, 500);
                    });
                    $('#available-languages, #available-themes').mouseenter(function() {
                        me = $(this);
                        clearTimeout(timeouts[me.attr("rel")]);
                    }).mouseleave(function() {
                        me = $(this);
                        clearTimeout(timeouts[me.attr("rel")]);
                        timeouts[me.attr("rel")] = setTimeout(function() {
                            $('#nav-right a[rel=' + me.attr("rel") + ']').toggleClass("ui-state-hover", false);
                            me.slideUp();
                        }, 500);
                    });
                    $('#languagelabel span.lbl').html(language.Language);
                    $('#themelabel span.lbl').html(language.Theme);

                    removeStaticContent();
                    getLoginStatus();
                });
            }

            /**
            * Handles switching themes
            */
            function switchTheme(key, theme) {
                var existing = $('#switched_stylesheet');
                if (existing.length == 0) {
                    existing = $('<link rel="stylesheet" type="text/css" id="switched_stylesheet" />').appendTo("head");
                } else {
                    $('link.theme_css').attr("href", existing.attr("href"));
                }
                existing.attr("href", 'http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/themes/' + theme.url);
                $('#available-themes a').toggleClass("ui-state-highlight", false);
                $('#available-themes a#theme_' + key).toggleClass("ui-state-highlight", true);
                setTimeout(function() {
                    $('#themelabel').each(function() {
                        var me = $(this);
                        var related = $(me.attr("href"));
                        var offset = me.offset();
                        related.css({
                            'position': 'absolute',
                            'top': offset.top + me.outerHeight(),
                            'left': offset.left,
                            'z-index': 100
                        });
                    });
                }, 500);
            }

            /**
            * Handles switching languages
            */
            function loadLanguage(lang) {
                load("/Home/Localization/" + lang, null, function(data) {
                    language = data.Language;
                    $.each(data.AvailableLanguages, function(key, val) {
                        $('#language_' + key).toggleClass("ui-state-highlight", false).find("span").html(val);
                    });
                    switchLanguage();
                    $('#languagelabel,#themelabel').each(function() {
                        var me = $(this);
                        var related = $(me.attr("href"));
                        var offset = me.offset();
                        related.css({
                            'position': 'absolute',
                            'top': offset.top + me.outerHeight(),
                            'left': offset.left,
                            'z-index': 100
                        });
                    });
                    $('a#language_' + data.CurrentCulture).toggleClass("ui-state-highlight", true);
                });
            }

            /**
            * Changes all of the visible text to the current language
            */
            function switchLanguage() {
                if (loginForm != null) {
                    $('#lblLoginUsername').html(language.Username);
                    $('#lblLoginPassword').html(language.Password);
                    $('#lblLoginRememberMe').html(language.RememberMe);
                    $('#lblLoginRememberMe').html(language.RememberMe);
                    loginForm.dialog("option", "buttons", createLoginButtons());
                    loginForm.dialog("option", "title", language.Login);
                }
                if (registerForm != null) {
                    $('#lblRegisterUsername').html(language.Username);
                    $('#lblRegisterPassword').html(language.Password);
                    $('#lblRegisterEmail').html(language.Email);
                    $('#lblRegisterConfirmPassword').html(language.ConfirmPassword);
                    registerForm.dialog("option", "buttons", createRegisterButtons());
                    registerForm.dialog("option", "title", language.Register);
                }
                $('#home_tab').html(language.Home);
                $('#trackhistory_tab').html(language.TrackHistory);
                $('#favourites_tab').html(language.ManageFavourites);
                $('#myaccount_tab').html(language.MyAccount);
                $('.lblAlbums').html(language.Albums);
                $('#btnSearch').val(language.Search);
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
                        loginButton.find(".ui-button-text").html(language.Login);
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
                    var label = (messages[i].label) ? '<strong>' + messages[i].label + '</strong> ' : '';
                    outerdiv.append('<p><span class="ui-icon ui-icon-alert"></span>' + label + messages[i].message + '</p>');
                }
                return outerdiv;
            }

            /**
            * Closes the login form and runs {@link createLayout}
            */
            function onLogin() {

                load("/Favourites/", null, function(data) {

                    if (registerForm != null) {
                        registerForm.dialog("close");
                    }

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

                /**
                * Initialise the themes switcher
                */
                $(config.switcher).themes();

                progressBar = $(config.progressBar).progressbar({
                    value: 0
                });

                /**
                * Initialise the now playing update
                */
                updater = $.MusicInfoUpdater.init("/Music/Current", {
                    onTrackChange: function(oldtrack) {
                        if (trackHistoryTable != null) {
                            trackHistoryTable.prepend(createTrackHistoryRow(oldtrack));
                        }
                        updateNowPlaying();
                        progressBar.progressbar("value", 0);
                    },
                    onStateChange: function(state) {
                        // What? Do something?
                    },
                    onReady: function() {
                        updateNowPlaying();
                        setInterval(function() {
                            progressBar.progressbar("value", updater.getProgress());
                        }, 200);
                    },
                    loadMethod: load
                });

                /**
                * Set up the search box to call 'search' on submit, give it a pretty UI button and set it's language
                */
                $(config.searchForm).submit(search).find('input[type=submit]').button();
                $('#btnSearch').val(language.Search);
            }

            /**
            * Update the now playing box
            */
            function updateNowPlaying() {
                var td = createTrackTd(updater.track, true);
                $('#current_track').empty();
                $('#current_track').append(td.children());
                $('#voteFor, #voteAgainst').unbind().click(function(e) {
                    e.preventDefault();
                    // console.log(updater.track.Id);
                    //console.log($(this).attr('rel'));
                    load($(this).attr('href'), { id: updater.track.Id, value: $(this).attr('rel') }, function(data) {
                        //console.log(data);
                    });
                });
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
                            createTrackHistoryRow(item.Track).appendTo(trackHistoryTable);
                        }
                        $(trackHistoryTable).appendTo(trackHistoryDiv);
                    });
                }
            }

            function createTrackHistoryRow(Track) {
                var row = $("<tr />");

                row.append(createTrackTd(Track));
                row.append(createTrackLengthTd(Track));
                row.append(createArtistTd(Track.Artist));
                row.append(createAlbumTd(Track.Album));
                return row;
            }

            /**
            * Search for a track and list the results
            */
            function search() {
                var search_value = $(this).find("input[type=text]").val();
                load("/Music/Search/" + search_value, null, function(data) {
                    $(config.resultsTab).show().find("a").html('<span id="lblSearchResults">' + language.SearchResults + '</span>: ' + search_value);
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
                        console.log(favourites[i].Track.Id);
                        if (favourites[i].Track.Id == trackid) {
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
                if (isInFavourites(track.Id) > -1) {
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
                if (track.Album != null && !track.Album.IsAvailable) {
                    link.addClass("ui-state-disabled");
                }
                // Configure click and hover events
                link.attr("trackid", track.Id);

                if (track.Album == null || track.Album.IsAvailable) {
                    link.click(function() {
                        var id = $(this).attr("trackid");
                        var index = isInFavourites(id);
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
                }
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
                var link = $('<a href="javascript:void(0);"><span class="ui-icon-newwin ui-icon"></span><span class="album-title">' + album.Name + '</span></a>');
                if (album.IsAvailable) {
                    link.click(function() {
                        displayAlbum(album.Id);
                    }).hover(
					    function() {
					        $(this).toggleClass("ui-state-highlight", true);
					    },
					    function() {
					        $(this).toggleClass("ui-state-highlight", false);
					    }
				    );
                } else {
                    link.addClass("ui-state-disabled")
                }
                return link;
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
                    displayArtist(artist.Id);
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
                    artistaccordion.append('<h3><a href="#">' + data.Name + ' <span class="lblAlbums">' + language.Albums + '</span></a></h3><div></div>');
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

            /**
            * Sets up a table as a scrolling version of pagination
            * Used to load a chunk of data to fill the screen. Loads the next chunk when the screen is scrolled to the bottom
            */
            function setupElementAsScrolling(url, container, table, rowheight, arrayName, callback) {
                var loadingRows = true;
                var default_amount = Math.ceil((container.outerHeight() / rowheight) * 1.2);
                var amount = default_amount;
                var from = 0;
                var gotAll = false;
                container.html('<p>' + language.Loading + '</p>').scroll(function() {
                    // Called when the screen is scrolled
                    if (!loadingRows && (this.scrollHeight - (container.scrollTop() + container.outerHeight()) < config.footerDistance)) {
                        table.append('<tr id="__row_load"><td colspan="4">' + language.Loading + '</td></tr>'); // Add a loading message
                        from = table.find("tr").length;
                        loadingRows = true;
                        load(url + "?from=" + from + "&amount=" + amount, null, function(data) {
                            if (data[arrayName].length < amount) {
                                gotAll = true;
                            }
                            $('#__row_load').remove();
                            loadingRows = false;
                            callback(data);
                        });
                    }
                }).each(function() {
                    // Called on the initial load
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
            * @returns {Boolean} The buttons with the appropriate language labels
            */
            function createLoginButtons() {
                var buttons = {};
                buttons[language.Login] = function() {
                    submitLoginForm($(this).find("form"));
                };
                buttons[language.Register] = function() {
                    displayRegisterForm();
                };
                return buttons;
            }

            /**
            * @returns {Boolean} The buttons with the appropriate language labels
            */
            function createRegisterButtons() {
                var buttons = {};
                buttons[language.Register] = function() {
                    submitRegisterForm(registerForm.find("form"));
                }
                return buttons;
            }

            /**
            * Displays the login form in a draggable dialog window
            */
            function displayLoginForm() {
                if (loginForm == null) {
                    loginForm = $(config.loginDiv).show();

                    $('#lblLoginUsername').html(language.Username);
                    $('#lblLoginPassword').html(language.Password);
                    $('#lblLoginRememberMe').html(language.RememberMe);

                    $(loginForm).find("form").submit(function() {
                        // Bind to the submit event of the form
                        submitLoginForm($(this));
                        return false;
                    });
                    // Create the dialog window

                    loginForm.dialog({
                        width: 350,
                        modal: true,
                        closeOnEscape: false,
                        autoResize: true,
                        open: function() {
                            $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
                        },
                        buttons: createLoginButtons()
                    });
                } else {
                    // Reset the login button and create the dialog window again
                    loginButton.find(".ui-button-text").html(language.Login);
                    loginForm.dialog("open");
                }
                loginForm.dialog("option", "title", language.Login);
            }

            /**
            * @returns {String} The formatted timespan
            * TODO Add localisation
            */
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
                    loginButton = loginForm.parent().find("button:contains(" + language.Login + ")");
                }
                loginButton.find(".ui-button-text").html(language.Wait);
                form.find('div.ui-state-error').slideUp();
                form.find('input.ui-state-error').removeClass("ui-state-error");
                load("/Account/Logon", form.serialize());
            }

            /**
            * Displays the register form
            */
            function displayRegisterForm() {
                if (loginForm != null) {
                    loginForm.dialog("close");
                }
                if (registerForm == null) {
                    registerForm = $('#register-form');

                    $('#lblRegisterUsername').html(language.Username);
                    $('#lblRegisterEmail').html(language.Email);
                    $('#lblRegisterPassword').html(language.Password);
                    $('#lblRegisterConfirmPassword').html(language.ConfirmPassword);

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
                        open: function() {
                            $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
                        },
                        buttons: createRegisterButtons()
                    });
                } else {
                    registerForm.dialog("open");
                }
                registerForm.dialog("option", "title", language.Register);
            }

            /**
            * Called when the registration form is submitted
            */
            function submitRegisterForm(form) {
                if (registerButton == null) {
                    // Find the login button
                    registerButton = registerForm.parent().find("button:contains(" + language.Register + ")");
                }
                registerButton.find(".ui-button-text").html(language.Wait);
                form.find('div.ui-state-error').slideUp();
                form.find('input.ui-state-error').removeClass("ui-state-error");
                load("/Account/Register", form.serialize(), function(data) {
                    registerButton.find(".ui-button-text").html(language.Register);
                    if (!data.Success) {
                        if (data.ErrorMessages != null && data.ErrorMessages.length > 0) {
                            var messages = [];
                            for (var i = 0; i < data.ErrorMessages.length; i++) {
                                var error = data.ErrorMessages[i];
                                for (var a = 0; a < error.Message.length; a++) {
                                    registerForm.find("input[ref=" + error.Field + "]").addClass("ui-state-error");
                                    messages.push({ label: language.Error, message: error.Message[a] });
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
    $(document.body).spofficeInterface();
});