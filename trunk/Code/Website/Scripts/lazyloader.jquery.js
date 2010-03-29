(function($) {
    $.fn.lazyLoader = function(url, settings) {
        settings = jQuery.extend({
            onData: function(data) { }
        }, options);
        this.each(function() {
            $(this).scroll(function() {
                console.log('scrolling');
            });
        });
    }
})(jQuery);