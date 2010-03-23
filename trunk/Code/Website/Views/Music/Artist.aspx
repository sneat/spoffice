<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArtistNode>" %>
<%@ Import Namespace="Spoffice.Website.Models.Spotify.MetadataApi" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= ViewRes.SharedStrings.Artist %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= ViewRes.SharedStrings.Artist %>: <%= Html.Encode(Model.Name) %>    <br />
    <%= ViewRes.SharedStrings.Album %><br />
      <% foreach (AlbumNode album in (IEnumerable)Model.Albums)
            { %>
            
                <%= album.IsAvailable ? Html.ActionLink(album.Name, "Album", new { id = album.PublicId }) : album.Name%><br />
            
          <% } %>

</asp:Content>

