(function($) {
    $.fn.lazyLoader = function(url, settings) {
        settings = jQuery.extend({
            onData: function(data) { },
            size : 30
        }, settings);
        this.each(function() {

            $(this).scroll(function() {
                console.log('scrolling');
            });
        });
    }
})(jQuery);