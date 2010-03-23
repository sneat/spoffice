/*
 * jixedbar - Fixed bar.
 * http://code.google.com/p/jixedbar/
 * 
 * Version 0.0.2 - December 18, 2009
 * 
 * Copyright (c) 2009 Ryan Yonzon, http://ryan.rawswift.com/
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 */

(function($) {

    // jixedbar plugin
    $.fn.jixedbar = function(options) {
        var constants = {
            constOverflow: "hidden",
            constPosition: "absolute",
            constBottom: "0px"
        };
        var defaults = {
            hoverOpaque: false,
            hoverOpaqueEffect: { enter: { speed: "fast", opacity: 1.0 }, leave: { speed: "fast", opacity: 0.80} },
            roundedCorners: false, // only works in FF
            roundedButtons: true // only works in FF
        };
        var options = $.extend(defaults, options);
        var ie6 = (navigator.appName == "Microsoft Internet Explorer" && parseInt(navigator.appVersion) == 4 && navigator.appVersion.indexOf("MSIE 6.0") != -1);

        this.each(function() {
            var obj = $(this);
            var $screen = jQuery(this);
            var fullScreen = $screen.width(); // get screen width
            var centerScreen = (fullScreen / 2) * (1); // get screen center

            // set html and body style for jixedbar to work
            $("html").css({ "overflow": "hidden", "height": "100%" });
            $("body").css({ "margin": "0px", "overflow": "auto", "height": "100%" });

            // initialize bar
            $(this).css({
                "overflow": constants['constOverflow'],
                "bottom": constants['constBottom']
            });

            // add bar style (theme)
            $(this).addClass("jx-bar");

            // calculate and adjust bar to center
            marginLeft = ($(window).width() - $(this).width()) / 2 + $(window).scrollLeft();
            $(this).css({ 'margin-left': marginLeft });

            // fix image vertical alignment and border
            $("img", obj).css({
                "vertical-align": "bottom",
                "border": "#ffffff solid 0px" // no border
            });

            // check for alt attribute and set it as button text
            $(this).find("img").each(function() {
                altName = "&nbsp;" + $(this).attr('alt');
                $(this).parent().append(altName);
            });

            // create tooltip container
            $("<div />").attr("id", "__jx_tooltip_con__").appendTo("body"); // create div element and append in html body
            $("#__jx_tooltip_con__").css({
                "height": "auto",
                "margin-left": "0px",
                "width": "100%", // use entire width
                "overflow": constants['constOverflow'],
                "position": constants['constPosition'],
                "bottom": constants['constBottom']
            });
            $("#__jx_tooltip_con__").append($(this)).delay(3000).fadeOut("slow", function() { $("#__jx_tooltip_con__").remove(); }); ;

            // fix PNG transparency problem in IE6
            if ($.browser.msie && ie6) {
                $(this).find('li').each(function() {
                    $(this).find('img').each(function() {
                        imgPath = $(this).attr("src");
                        altName = $(this).attr("alt");
                        srcText = $(this).parent().html();
                        $(this).parent().html( // wrap with span element
							'<span style="cursor:pointer;display:inline-block;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src=\'' + imgPath + '\');">' + srcText + '</span>' + altName
						);
                    });
                    $(this).find('img').each(function() {
                        $(this).attr("style", "filter:progid:DXImageTransform.Microsoft.Alpha(opacity=0);"); // show image
                    })
                });
            }

            /**
            * To-do:
            * 	1. Element click event:
            * 		$("li", obj).click(function() {
            * 			// event handler
            * 		});
            */

        });

        return this;

    };

})(jQuery);