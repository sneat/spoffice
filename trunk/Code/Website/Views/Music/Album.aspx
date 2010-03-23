<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AlbumNode>" %>
<%@ Import Namespace="Spoffice.Website.Models.Spotify.MetadataApi" %>
<%@ Import Namespace="ScriptManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= ViewRes.SharedStrings.Album %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% Html.ScriptManager().ScriptInclude("jquery", "~/Scripts/jquery-latest.min.js"); %>
<% Html.ScriptManager().Script( "albumArt", () => {%>
    var albumImage = document.createElement('img');
    albumImage.src = "<%= Page.ResolveUrl(String.Format("~/Music/AlbumImage/{0}", Model.PublicId)) %>";
    albumImage.setAttribute('alt', '<%= Model.Name %>');
    
    var img = new Image();
  
    // wrap our new image in jQuery, then:
    $(img)
        // once the image has loaded, execute this code
        .load(function () {
            // set the image hidden by default    
            $(this).hide();
            $(this).css({
                "position" : "absolute",
                "top" : "20px",
                "right" : "20px"
            });
            
            // with the holding div #loader, apply:
            $('#main')
                // then insert our image
                .append(this);

            // fade our image in to create a nice effect
            $(this).fadeIn("slow");
        })

        // if there was an error loading the image, react accordingly
        .error(function () {
            // notify the user that the image could not be loaded
        })

        // *finally*, set the src attribute of the new image to our image
        .attr('src', '<%= Page.ResolveUrl(String.Format("~/Music/AlbumImage/{0}", Model.PublicId)) %>');
    
<%}); %>

    <h2><%= String.Format("{0}: {1}", ViewRes.SharedStrings.Album, String.Format(ViewRes.MusicStrings.AlbumArtist, Model.Name, Html.ActionLink(Model.Artist.Name, "Artist", new { id = Model.Artist.PublicId }, new { title = ViewRes.MusicStrings.ArtistDetails })))%></h2>
    
    <table>
        <thead>
            <tr>
                <th scope="col"></th>
                <th scope="col"><%= ViewRes.SharedStrings.Track %></th>
                <th scope="col"><%= ViewRes.SharedStrings.Length %></th>
            </tr>
        </thead>
        <tbody>
            <% foreach (TrackNode track in Model.Tracks)
               { %>
            <tr>
                <td><% if (track.Album == null || Model.IsAvailable) Html.RenderPartial("AddToFavourites", track.PublicId);  %></td>
                <td><%= track.Title %></td>
                <td><%= track.Length %></td>
            </tr>
          <% } %>
        </tbody>
    </table>
 
</asp:Content>

