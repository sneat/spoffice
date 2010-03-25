<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="ScriptManager" %>

<% Html.ScriptManager().ScriptInclude("jquery", "~/Scripts/jquery-latest.min.js"); %>
<% Html.ScriptManager().ScriptInclude("jixed", "~/Scripts/jixedbar-0.0.2.js"); %>
<% Html.ScriptManager().ScriptInclude("musicinfo", "~/Scripts/MusicInfoUpdater.js"); %>
<% Html.ScriptManager().Script( "nowPlaying", () => {%>
/*$(document).ready(function (){
	$('#now_playing .album img').error(function (){
		$(this).attr("src", "/Content/images/blank_album_small.png");
	});
    	function number_format( number, decimals, dec_point, thousands_sep ) {
		var n = number, c = isNaN(decimals = Math.abs(decimals)) ? 2 : decimals;
		var d = dec_point == undefined ? "," : dec_point;
		var t = thousands_sep == undefined ? "." : thousands_sep, s = n < 0 ? "-" : "";
		var i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "", j = (j = i.length) > 3 ? j % 3 : 0;

		return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
	}
        function size_format (filesize) {
		if (filesize >= 1073741824) {
		     filesize = number_format(filesize / 1073741824, 2, '.', '') + ' Gb';
        
		} else { 
			if (filesize >= 1048576) {
	     			filesize = number_format(filesize / 1048576, 2, '.', '') + ' Mb';
	   		} else { 
				if (filesize >= 1024) {
	    				filesize = number_format(filesize / 1024, 0) + ' Kb';
	  			} else {
	    				filesize = number_format(filesize, 0) + ' bytes';
				};
	 		};
		};
	 	return filesize;
	};
	function updateProgress (){
			$('#progress_bar').css("width", updater.getProgress()+"%");
			$('#totalbytestotal').html(updater.getTotalBytes());
			$('#tb_formatted').html(size_format(updater.getTotalBytes()));
			setTimeout(updateProgress, 100);
	}
	function updateListing (){
			$('#now_playing .titlePlaying').
						html(updater.track.title);

			$('#now_playing .artistPlaying a').
						html(updater.track.artist.name).		
						attr("href", "/Music/Artist/"+updater.track.artist.id);

			$('#now_playing .album').
						attr("href", "/Music/Album/"+updater.track.album.id);
						
			$('#now_playing .album img').
						attr('src', '/Music/TrackImage/' + updater.track.id)
	}
	var updater = $.MusicInfoUpdater.init("/music/current", {
		onTrackChange : function (){
			updateListing();
		},
		onStateChange : function (state){
		},
		onReady : function (){
			updateListing();
			updateProgress();
			$('#now_playing').show();
		}
	});
});
*/
    
<%}); %>
<% Html.ScriptManager().Script( "vote", () => {%>
   /* var cacheImage = document.createElement('img');
    cacheImage.src = "<%= ResolveUrl("~/Content/images/ajax-loader.gif") %>";
    cacheImage.setAttribute('alt', '<%= ViewRes.FavouritesStrings.PleaseWait %>');
    
    $('#voteFor').click(function(e) {
        e.preventDefault();
        $(this).find('img').replaceWith(cacheImage);
        $(document.createElement('p')).addClass('result').appendTo("body");
        $.post('/Music/Vote/', {
                id: $('#now_playing').attr('track'),
                'value': 'for'
            }, function(data) {
                if (data)
                {
                    if (data.Status == 'OK')
                    {
                        $('p.result').addClass('success').text('<%= ViewRes.SharedStrings.SuccessfullyVotedFor %>').jixedbar();
                        $('#now_playing div.voting').slideUp('fast');
                    }
                    else
                    {
                        $('p.result').addClass('error').text(data.Message).jixedbar();
                        $('#now_playing div.voting').slideUp('fast');
                    }
                }
                else
                {
                    $('p.result').addClass('error').text('<%= ViewRes.SharedStrings.ErrorVotingFor %>').jixedbar();
                    $('#now_playing div.voting').slideUp('fast');
                }
        }, 'json');
    });
    
    $('#voteAgainst').click(function(e) {
        e.preventDefault();
        $(this).find('img').replaceWith(cacheImage);
        $(document.createElement('p')).addClass('result').appendTo("body");
        $.post('/Music/Vote/', {
                id: $('#now_playing').attr('track'),
                'value': 'against'
            }, function(data) {
                if (data)
                {
                    if (data.Status == 'OK')
                    {
                        $('p.result').addClass('success').text('<%= ViewRes.SharedStrings.SuccessfullyVotedAgainst %>').jixedbar();
                        $('#now_playing div.voting').slideUp('fast');
                    }
                    else
                    {
                        $('p.result').addClass('error').text(data.Message).jixedbar();
                        $('#now_playing div.voting').slideUp('fast');
                    }
                }
                else
                {
                    $('p.result').addClass('error').text('<%= ViewRes.SharedStrings.ErrorVotingAgainst %>').jixedbar();
                    $('#now_playing div.voting').slideUp('fast');
                }
        }, 'json');
    });*/
    
<%}); %>

<div id="now_playing">
    <div class="title"><%= ViewRes.SiteStrings.NowPlaying %></div>
    <a href="#" class="album"><img runat="server" src="~/Content/images/blank_album_small.png" alt="Album cover" width="64" height="64" /></a>
    <div class="songDetails">
        <div class="titlePlaying">Title</div>
        <div class="artistPlaying"><%= Html.ActionLink("Artist", "Artist", new { id = "asdf" }) %></div>
        <div class="timePlaying">
            <div id="progress"><div id="progress_bar"></div></div>
        </div>
    </div>
    <div class="voting">
        <a href="<%= ResolveUrl("~/Music/Vote/For") %>" title="<%= ViewRes.SharedStrings.VoteFor %>" id="voteFor"><img runat="server" src="~/Content/images/thumb_up.png" alt="I like it" /><%= ViewRes.SharedStrings.VoteFor %></a>
        <a href="<%= ResolveUrl("~/Music/Vote/Against") %>" title="<%= ViewRes.SharedStrings.VoteAgainst %>" id="voteAgainst"><img runat="server" src="~/Content/images/thumb_down.png" alt="I like it" /><%= ViewRes.SharedStrings.VoteAgainst %></a>
    </div>
</div>