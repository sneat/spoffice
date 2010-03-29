(function($) {
	/**
	 * spofficeInterface plugin
	 * @param {Object} [settings] The config settings
	 */
    $.fn.spofficeInterface = function(settings) {
        var config = {
	        'foo': 'bar'
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
            var oldTrackHistoryTop = 0;
            var artistaccordion;
            var albumaccordion;
            var loadingTrackHistory = false;
            var loadingFavourites = false;
            var favouritesTable;
            var oldFavouritesTop = 0;
            var favourites = null;
            var icons = {
                header: "ui-icon-folder-collapsed",
                headerSelected: "ui-icon-folder-open"
            };

	        /**
	         * Called when the class initially loads
	         */
            function init() {
                removeStaticContent();
                getLoginStatus();
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
                var loadingdiv = $('#loading');
                if (loadingdiv.is(":visible")) {
                    loadingdiv.fadeOut();
                };
                if (data.LoggedIn) {
	                // Save favourites data for later use
                    favourites = data.Favourites;
                    onLogin();
                } else {
                    if (!isLoginFormVisible()) {
                        displayLoginForm();
                    }
                }
            }

	        /**
	         * Closes the login form and runs {@link createLayout}
	         */
            function onLogin() {
                if (loginForm != null)
                    loginForm.dialog("close");

                if (layout == null)
                    createLayout();
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
	         * TODO Use config settings to determine layout options
	         * @see <a href="http://layout.jquery-dev.net/">jQuery Layout</a>
	         */
            function createLayout() {
                $(document.body).append($('#layout').html());
                $('#layout').remove();
	            /**
	             * Create base layout consisting of a center and north pane
	             * TODO Use config settings to determine layout options
	             */
                layout = $('body').layout({
                    center__paneSelector: "#main",
                    north__paneSelector: "#header",
                    north__size: 100,
                    spacing_open: 0,
                    north__slidable: false
                });
	            /**
	             * Create the tab interface and configure the events that each tag represents
	             */
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
	            /**
	             * Add the tab interface to the center pane of the base layout
	             * TODO Use config settings to determine layout options
	             */
                $('#main').layout({
                    center__paneSelector: ".tabs-panel-container",
                    north__paneSelector: "#tabs",
                    spacing_open: 0,
                    north__slidable: false
                });
	            /**
	             * Set up the central tab panel
	             * Configure east and west panels to use accordion
	             * TODO Use config settings to determine layout options
	             */
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

	        /**
	         * Called when TrackHistory tab is clicked
	         * Gets the track history in blocks from the database. If the user scrolls to near the bottom of the page,
	         * the next block of track histories is requested and appended to the table.
	         * TODO Use config settings to determine how close to the bottom of the page the user for a new request
	         * TODO Ensure that requests stop once we have the entire track history list
	         * @see addTrackHistoryRows
	         */
            function load__TrackHistory() {
                if (trackHistoryTable == null) {
                    var amount = 0;
                    loadingTrackHistory = true;
                    $('#trackhistory').html("loading..").scroll(function() {
                        if (trackHistoryTable != null) {
                            var th = $('#trackhistory');
                            var thc = th[0];
	                        /**
	                         * Calculates whether the user has scrolled to within {@link configscrollDistance} of the
	                         * bottom of the page and loads the next block of data if it's not already loading some.
	                         * scrollHeight = the total scrollable height of the trackhistory container
	                         * scrollTop = the current vertical position of the scroll bar in the trackhistory container
	                         * outerHeight = the height of the visible portion of the trackhistory container
	                         * TODO Use config variable for distance to bottom
	                         * TODO Fix oldTrackHistoryTop as it's meant to prevent extra calls to load more data
	                         */
                            if (!loadingTrackHistory && (thc.scrollHeight - (th.scrollTop() + th.outerHeight()) < 150)) {
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
	                    // Calculate the amount of data to load when the trackHistory container is scrolled
                        var self = $(this);
                        amount = Math.ceil((self.outerHeight() / 20) * 1.2);
                    });
	                // Load the initial block of track history data
                    load("/Music/Playlist/?amount=" + amount, null, function(data) {
                        $('#trackhistory').empty();
                        trackHistoryTable = $("<table></table>");
                        addTrackHistoryRows(data);
                        loadingTrackHistory = false;
                    });
                }
            }

	        /**
	         * Adds track history rows to the trackhistory container, fading them in nicely
	         * @param {Object[]} data The rows of data to be added
	         * @param {Object} data.History The trackHistory object
	         * @param {Object} data.History.Track The Track object
	         * @param {Object} data.History.Track.Artist The track Artist object
	         * @param {Object} data.History.Track.Album The track Album object
	         */
            function addTrackHistoryRows(data) {
                var rows = [];
	            var rowcount = data.History.length;
                for (var i = 0; i < rowcount; i++) {
                    var historyItem = data.History[i];
                    var row = $("<tr />");
	                // Create the row containing the track history data
                    row.append(createTrackTd(historyItem.Track));
                    row.append(createTrackLengthTd(historyItem.Track));
                    row.append(createArtistTd(historyItem.Track.Artist));
                    row.append(createAlbumTd(historyItem.Track.Album));
	                // Add the row to the trackHistoryTable variable
                    rows.push(row.appendTo(trackHistoryTable));
                    rows[i].find("td").hide();
                }
	            // Append the trackHistoryTable to the trackhistory container
                $(trackHistoryTable).appendTo('#trackhistory');
	            var rowsCount = rows.length;
                for (var i = 0; i < rowsCount; i++) {
                    rows[i].find("td").delay(10 * i).fadeIn(100);
                }
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
                if ($.inArray(track.PublicId, favourites) > -1) {
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
                    var index = $.inArray(id, favourites);
                    if (index > -1) {
	                    // Track is already a Favourite, therefore we want to remove it
                        load("/Favourites/Remove/" + id, null, function(data) {
                            if (data.StatusCode == "OK") {
                                favourites.splice(index, 1);
                                $(document.body).find("a[trackid=" + id + "]").find(".ui-icon")
                                    .removeClass("ui-icon-circle-minus").addClass("ui-icon-circle-plus");
                            }
                        });
                    } else {
	                    // Track is not already a Favourite, therefore we want to add it
                        load("/Favourites/Add/" + id, null, function(data) {
                            if (data.StatusCode == "OK") {
                                favourites.push(id);
                                $(document.body).find("a[trackid=" + id + "]").find(".ui-icon")
                                    .removeClass("ui-icon-circle-plus").addClass("ui-icon-circle-minus");
                            }
                        });
                    }
                }).hover(
					function () {
						$(this).toggleClass("ui-state-highlight", true);
					},
					function () {
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
					function () {
						$(this).toggleClass("ui-state-highlight", true);
					},
					function () {
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
					function () {
						$(this).toggleClass("ui-state-highlight", true);
					},
					function () {
						$(this).toggleClass("ui-state-highlight", false);
					}
				);
            }

	        /**
	         * Display Album details in the album panel using jQuery UI Accordion
	         * @param {String} id The Album Guid
	         * @see <a href="http://docs.jquery.com/UI/Accordion">jQuery UI Accordion</a>
	         * TODO Allow panel to be selected via config
	         */
            function displayAlbum(id) {
                centrallayout.open("east");
                load("/Music/Album/" + id, null, function(data) {
                    var accordionLength = albumaccordion.find("h3").length;
                    if (albumaccordion.accordion != null) {
						// Remove the accordion functionality, makes initialising it easier later on
                        albumaccordion.accordion("destroy");
                    }
                    if (accordionLength > 4) {
	                    /**
	                     * We only want a max of {@link configAccordionAmount}
	                     * TODO Make this use config
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
	         * TODO Allow panel to be selected via config
	         * TODO Add album cover thumbnail
	         */
            function displayArtist(id) {
                centrallayout.open("west");
                load("/Music/Artist/" + id, null, function(data) {
                    var accordionLength = artistaccordion.find("h3").length;
                    if (artistaccordion.accordion != null) {
						// Remove the accordion functionality, makes initialising it easier later on
                        artistaccordion.accordion("destroy");
                    }
                    if (accordionLength > 4) {
	                    /**
	                     * We only want a max of {@link configAccordionAmount}
	                     * TODO Make this use config
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
	         * TODO Use config settings to determine how close to the bottom of the page the user for a new request
	         * TODO Ensure that requests stop once we have the entire track favourites list
	         * @see addTrackHistoryRows
	         */
            function load__Favourites() {
                if (favouritesTable == null) {
                    var amount = 0;
                    loadingFavourites = true;
                    $('#favourites').html("loading..").scroll(function() {
                        if (favouritesTable != null) {
                            var th = $('#favourites');
                            var thc = th[0];
	                        /**
	                         * Calculates whether the user has scrolled to within {@link configscrollDistance} of the
	                         * bottom of the page and loads the next block of data if it's not already loading some.
	                         * scrollHeight = the total scrollable height of the trackhistory container
	                         * scrollTop = the current vertical position of the scroll bar in the trackhistory container
	                         * outerHeight = the height of the visible portion of the trackhistory container
	                         * TODO Use config variable for distance to bottom
	                         * TODO Fix oldFavouritesTop as it's meant to prevent extra calls to load more data
	                         */
                            if ((thc.scrollHeight - th.scrollTop() < th.outerHeight() + 150) && !loadingFavourites) {
                                var rowcount = favouritesTable.find("tr").length;
                                if (oldFavouritesTop != rowcount) {
                                    oldFavouritesTop = rowcount;
                                    loadingFavourites = true;
                                    load("/Favourites/?from=" + rowcount + "&amount=" + amount, null, function(data) {
                                        addTrackHistoryRows(data);
                                        loadingFavourites = false;
                                    });
                                }
                            }
                        }
                    }).each(function() {
	                    // Calculate the amount of data to load when the trackHistory container is scrolled
                        var self = $(this);
                        amount = Math.ceil((self.outerHeight() / 20) * 1.2);
                    });
	                // Load the initial block of track favourites data
                    load("/Favourites/?amount=" + amount, null, function(data) {
                        $('#trackhistory').empty();
                        trackHistoryTable = $("<table></table>");
                        addFavouritesRows(data);
                        loadingTrackHistory = false;
                    });
                }
            }

	        /**
	         * Adds track favourites rows to the trackhistory container, fading them in nicely
	         * @param {Object[]} data The rows of data to be added
	         * @param {Object} data.History The trackFavourites object
	         * @param {Object} data.History.Track The Track object
	         * @param {Object} data.History.Track.Artist The track Artist object
	         * @param {Object} data.History.Track.Album The track Album object
	         */
            function addFavouritesRows(data) {
                var rows = [];
	            var rowCount = data.History.length;
                for (var i = 0; i < rowCount; i++) {
                    var historyItem = data.History[i];
                    var row = $("<tr />");
	                // Create the row containing the track favourites data
                    row.append(createTrackTd(historyItem.Track));
                    row.append(createTrackLengthTd(historyItem.Track));
                    row.append(createArtistTd(historyItem.Track.Artist));
                    row.append(createAlbumTd(historyItem.Track.Album));
	                // Add the row to the trackHistoryTable variable
                    rows.push(row.appendTo(trackHistoryTable));
                    rows[i].find("td").hide();
                }
	            // Append the trackHistoryTable to the trackhistory container
                $(trackHistoryTable).appendTo('#trackhistory');
	            var rowsCount = rows.length;
                for (var i = 0; i < rowsCount; i++) {
                    rows[i].find("td").delay(10 * i).fadeIn(100);
                }
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
                    loginForm = $('#login-form').show();

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
                        buttons: {
                            'Login': function() {
                                submitLoginForm($(this).find("form"));
                            }
                        }
                    });
                } else {
	                // Reset the login button and create the dialog window again
                    loginButton.html("Login");
                    loginForm.dialog();
                }
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
                loginButton.html("Wait..");
                load("/Account/Logon", form.serialize());
            }

	        /**
	         * Removes the static content container
	         */
            function removeStaticContent() {
                $('#static-content').remove();
            }

	        // Run the initialisation function
            init();
        });
        return this;
    };
})(jQuery);

/**
 * When the DOM is ready, run the spofficeInterface on the document body
 */
 $(function (){
    $(document.body).spofficeInterface();
 });