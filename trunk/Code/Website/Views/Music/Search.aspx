<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TrackList>" %>
<%@ Import Namespace="Spoffice.Website.Models.Spotify.MetadataApi" %>
<%@ Import Namespace="Spoffice.Website.Helpers" %>
<%@ Import Namespace="Spoffice.Website.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= ViewRes.MusicStrings.SearchSongs %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% if (ViewData["search_term"] == "")
   { %>
    <h3><%= ViewRes.MusicStrings.EnterSearchTerm %></h3>
<% }
   else
   { %>
   <%= ViewRes.MusicStrings.SearchedFor.Replace("{0}", Html.Encode(ViewData["search_term"]))%>
    
    <table summary="<%= ViewRes.MusicStrings.SearchSummary %>">
        <thead>
            <tr>
                <th scope="col"></th>
                <th scope="col"><%= ViewRes.MusicStrings.Title%></th>
                <th scope="col"><%= ViewRes.SharedStrings.Artist%></th>
                <th scope="col"><%= ViewRes.SharedStrings.Album%></th>
                <th scope="col"><%= ViewRes.SharedStrings.Length%></th>
            </tr>
        </thead>
        <tbody>
            <% foreach (TrackNode track in Model.Tracks)
               {
               %>
            <tr>
                <td>
                    <% 
                        if (track.Album != null && track.Album.IsAvailable)
                        {
                        Html.RenderPartial("AddToFavourites", track.PublicId);
                        }
                   %>
                </td>
                <td><%= track.Title%></td>
                <td><%= Html.ActionLink(track.Artist.Name, "Artist",
                                            new { id = track.Artist.PublicId },
                                            new { title = ViewRes.MusicStrings.ArtistDetails })%></td>
                <td><%= Html.ActionLink(String.Format("{0} {1}", track.Album.Name, ""), "Album", new { id = track.Album.PublicId }, new { title = ViewRes.MusicStrings.AlbumDetails })%></td>
                <td><%= StringFormatter.MillisecondsToString(track.Length)%></td>
            </tr>
          <%
        } %>
        </tbody>
    </table>
<% } %>
</asp:Content>
