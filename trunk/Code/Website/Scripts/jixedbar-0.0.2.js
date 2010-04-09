/*
 * jixedbar - Fixed bar.
 * http://code.google.com/p/jixedbar/
 * 
 * Version 0.0.2 - December 18, 2009
 * Modified for use with spoffice - April 9, 2010
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
            success: "",
            error: "",
            zIndex: "2"
        };
        var options = $.extend(defaults, options);
        var ie6 = (navigator.appName == "Microsoft Internet Explorer" && parseInt(navigator.appVersion) == 4 && navigator.appVersion.indexOf("MSIE 6.0") != -1);

        this.each(function() {
            var obj = $(this);

            // set html and body style for jixedbar to work
            $("html").css({ "overflow": "hidden", "height": "100%" });

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
            $(document.createElement('div')).attr("id", "__jx_tooltip_con__").addClass("ui-widget").appendTo("body"); // create div element and append in html body
            $("#__jx_tooltip_con__").css({
                "height": "auto",
                "margin-left": "0px",
                "width": "100%", // use entire width
                "overflow": constants['constOverflow'],
                "position": constants['constPosition'],
                "bottom": constants['constBottom'],
                "z-index": options.zIndex
            });

            // create the wrapper and icon containers
            var __wrapper = $(document.createElement('div')).addClass("ui-corner-all __jixed").appendTo("#__jx_tooltip_con__");
            var __icon = $('<span class="ui-icon" style="float: left; margin-right: .3em;"></span>');

            // calculate and adjust bar to center
            marginLeft = ($(window).width() - __wrapper.width()) / 2 + $(window).scrollLeft();
            __wrapper.css({ 'margin-left': marginLeft });

            // add styles and message depending on whether it's a success message or error message.
            if (options.success != '') {
                __wrapper.addClass('ui-state-highlight');
                __icon.addClass("ui-icon-info");
                $(this).html(options.success);
            } else if (options.error != '') {
                __wrapper.addClass('ui-state-error');
                __icon.addClass("ui-icon-alert");
                $(this).html(options.error);
            } else {
                __icon = '';
            }

            $(this).prepend(__icon);

            // add the message to the page
            __wrapper.append(this).delay(3000).fadeOut("slow", function() { $("#__jx_tooltip_con__").remove(); });

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