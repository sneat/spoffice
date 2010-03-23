<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArtistNode>" %>
<%@ Import Namespace="Spoffice.Website.Models.Spotify.MetadataApi" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= ViewRes.SharedStrings.Artist %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= ViewRes.SharedStrings.Artist %>: <%= Html.Encode(Model.Name) %></h2>     
    
    <table>
        <thead>
            <tr>
                <th scope="col"><%= ViewRes.SharedStrings.Album %></th>
            </tr>
        </thead>
        <tbody>
            <% foreach (AlbumNode album in (IEnumerable)Model.Albums)
            { %>
            <tr>
                <td><%= album.IsAvailable ? Html.ActionLink(album.Name, "Album", new { id = album.PublicId }) : album.Name%></td>
            </tr>
          <% } %>
        </tbody>
    </table>
 

</asp:Content>

