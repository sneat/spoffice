(function($) {
    function MusicInfoUpdater() { }
    $.extend(MusicInfoUpdater.prototype, {
        init: function(url, options) {

            settings = jQuery.extend({
                onTrackChange: function() { },
                onStateChange: function() { },
                onReady: function() { },
                loadMethod: function() { }
            }, options);

            this.loaded = false;

            this.onTrackChange = settings.onTrackChange;
            this.onStageChange = settings.onStateChange;
            this.onReady = settings.onReady;
            this.loadMethod = settings.loadMethod;

            this.url = url;

            this.callSpeed = 5000;

            this.position = 0;
            this.length = 0;
            this.state = 0;
            this.buffersize = 0;
            this.tracksize = 0;
            this.totalbytes = 0;
            this.track = null;
            this.upcoming = [];

            this.position_speed = 1;
            this.download_speed = 1;
            this.totalbyte_speed = 1;

            this.requestTimeout = null;

            this.lastCheck = null;

            this.load();


            return this;
        },
        load: function() {
            var self = this;
            this.loadMethod(this.url, null, function(data) {
                self._parseJson(data);
            });
        },
        _parseJson: function(data) {
            var self = this;

            var checkTime = new Date().getTime();
            if (this.lastCheck != null && (this.track != null && this.track.Id == data.Tracks[0].Id)) {
                var timeDifference = (checkTime - this.lastCheck);
                this.position_speed = (data.PlayerPosition - this.position) / timeDifference;
                this.totalbyte_speed = (data.TotalBytes - this.totalbytes) / timeDifference;
                //this.buffer_speed = (data.DownloadedTrackBytes - this.buffersize) / timeDifference;
            }

            this.lastCheck = checkTime;
            this.position = data.PlayerPosition;
            this.length = data.Tracks[0].Length;
            //this.buffersize = data.DownloadedTrackBytes;
            //this.tracksize = data.TotalTrackBytes;
            this.totalbytes = data.TotalBytes;

            var currentTrack = data.Tracks[0];
            if (this.track == null) {
                this.refreshTracks(data);
            } else if (this.track.Id != currentTrack.Id) {
                var oldtrack = this.track;
                this.refreshTracks(data);
                this.onTrackChange(oldtrack);
            }

            var timeleft = this.length - this.positon;

            clearTimeout(this.requestTimeout);
            var nextRequest = timeleft > 0 ? Math.min(timeleft, this.callSpeed) : this.callSpeed;
            this.requestTimeout = setTimeout(function() { self.load() }, nextRequest);
            if (!this.loaded) {
                this.loaded = true;
                this.onReady();
            }
        },
        refreshTracks: function(data) {
            this.track = data.Tracks[0];
            for (var i = 1; i < data.Tracks.length; i++) {
                this.upcoming[i - 1] = data.Tracks[i];
            }
        },
        getPosition: function() {
            var difference = new Date().getTime() - this.lastCheck;
            return Math.floor(Math.min(this.position + (difference * this.position_speed), this.length));
        },
        getBuffer: function() {
            var difference = new Date().getTime() - this.lastCheck;
            return Math.floor(Math.min(this.buffersize + (difference * this.buffer_speed), this.totalbytes));
        },
        getTotalBytes: function() {
            var difference = new Date().getTime() - this.lastCheck;
            return Math.floor(this.totalbytes + (difference * this.totalbyte_speed));
        },
        getLength: function() {
            return this.length;
        },
        getProgress: function() {
            return (100 / this.getLength()) * this.getPosition();
        }
    });
    $.MusicInfoUpdater = new MusicInfoUpdater();
})(jQuery);
