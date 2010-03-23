<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Favourite>>" %>
<%@ Import Namespace="Spoffice.Website.Models" %>
<%@ Import Namespace="Spoffice.Website.Models.Spotify.MetadataApi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= ViewRes.SiteStrings.Favourites %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= ViewRes.SiteStrings.Favourites %></h2>
    <% 
        if (ViewData.Model.Count() > 0)
        {

            //ids =  parse XML and pull out track IDs
            //go through all the artists, albums..etc. and put them in the database if they dont already exist
            //now do a query where trackids IN (ids)
            // list all
            
                  %>
    <table summary="<%= ViewRes.MusicStrings.SearchSummary %>">
        <thead>
            <tr>
                <th scope="col"></th>
                <th scope="col"><%= ViewRes.MusicStrings.Title%></th>
                <th scope="col"><%= ViewRes.SharedStrings.Artist%></th>
            </tr>
        </thead>
        <tbody>
         <% foreach (Favourite favourite in ViewData.Model)
            { %>
            <tr>
                <td class="actions"><% Html.RenderPartial("AddToFavourites", TrackNode.ConvertPrivateToPublic(favourite.Track.Id)); %></td>
                <td><%= favourite.Track.Title %></td>
                <td><%= favourite.Track.Artist.Name %></td>
            </tr>
            
         <% 
        } %>
        </tbody>
    </table>
    <% }
        else
        { %>
        <p><%= ViewRes.FavouritesStrings.NoFavourites %></p>
        <% } %>
</asp:Content>
