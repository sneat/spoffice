(function($){
	function Album (){
		this.id = '';
		this.name = '';
		this.uri = '';
	}
	function Artist (){
		this.id = '';
		this.name = '';
		this.uri = '';
	}
	function Track (){
		this.id = '';
		this.title = '';
		this.uri = '';
		this.album = new Album();
		this.artist = new Artist();
	}
	function MusicInfoUpdater (){}
	$.extend(MusicInfoUpdater.prototype, {
		init: function(url, options) {

			  settings = jQuery.extend({
			  	onTrackChange : function (){},
			  	onStateChange : function (){},
			  	onReady : function (){}
			  }, options);

			  this.loaded = false;

			  this.onTrackChange = settings.onTrackChange;
			  this.onStageChange = settings.onStateChange;
			  this.onReady = settings.onReady;

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
		load : function (){
			var self = this;
			$.getJSON(this.url, function (data) { self._parseJson(data); });
		},
		_parseJson : function (data){
			var self = this;

			var checkTime = new Date().getTime();
			if (this.lastCheck != null && (this.track != null && this.track.id == data.tracks[0].PublicId)){
				var timeDifference = (checkTime - this.lastCheck);
				this.position_speed = (data.PlayerPosition - this.position) / timeDifference;
				this.totalbyte_speed = (data.TotalBytes - this.totalbytes) / timeDifference;
				this.buffer_speed = (data.DownloadedTrackBytes - this.buffersize) / timeDifference;
			}

			this.lastCheck = checkTime;
			this.position = data.PlayerPosition;
			this.length = data.TrackLength;
			this.buffersize = data.DownloadedTrackBytes;
			this.tracksize = data.TotalTrackBytes;
			this.totalbytes = data.TotalBytes;

			var currentTrack = data.tracks[0];
			if (this.track == null){
				this.refreshTracks(data);
			}else if (this.track.id != currentTrack.PublicId){
				this.refreshTracks(data);
				this.onTrackChange();
			}



			var timeleft = this.length - this.positon;

			clearTimeout(this.requestTimeout);
			var nextRequest = timeleft > 0?Math.min(timeleft, this.callSpeed) : this.callSpeed;
			this.requestTimeout = setTimeout(function () { self.load() }, nextRequest);
			if (!this.loaded){
				this.loaded = true;
				this.onReady();
			}
		},
		refreshTracks : function (data){
			if (this.track == null) this.track = new Track();
			this.createTrack(data.tracks[0], this.track);
			for (var i=1; i<data.tracks.length; i++){
				if (this.upcoming[i-1] == null) this.upcoming[i-1] = new Track();
				this.createTrack(data.tracks[i], this.upcoming[i-1]);
			}
		},
		createTrack : function (data, track){
			track.artist.id = data.Artist.PublicId;
			track.artist.name = data.Artist.Name;
			track.artist.uri = data.Artist.URI;

			track.album.id = data.Album.PublicId;
			track.album.name = data.Album.Name;
			track.album.uri = data.Album.URI;

			track.id = data.PublicId;
			track.title = data.Title;
			track.uri = data.URI;
		},
		getPosition : function (){
			var difference = new Date().getTime() - this.lastCheck;
			return Math.floor(Math.min(this.position + (difference * this.position_speed), this.length));
		},
		getBuffer : function (){
			var difference = new Date().getTime() - this.lastCheck;
			return Math.floor(Math.min(this.buffersize + (difference * this.buffer_speed), this.totalbytes));
		},
		getTotalBytes : function (){
			var difference = new Date().getTime() - this.lastCheck;
			return Math.floor(this.totalbytes + (difference * this.totalbyte_speed));
		},
		getLength : function (){
			return this.length;
		},
		getProgress : function (){
			return  (100 / this.getLength()) * this.getPosition();
		}
	});
	$.MusicInfoUpdater = new MusicInfoUpdater();
})(jQuery);
