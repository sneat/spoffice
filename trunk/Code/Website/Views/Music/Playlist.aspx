<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TrackHistoryList>" %>
<%@ Import Namespace="Spoffice.Website.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Playlist
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Playlist</h2>

    <table summary="<%= ViewRes.MusicStrings.SearchSummary %>">
        <thead>
            <tr>
                <th scope="col">Played</th>
                <th scope="col"><%= ViewRes.MusicStrings.Title%></th>
                <th scope="col"><%= ViewRes.SharedStrings.Artist%></th>
            </tr>
        </thead>
        <tbody>
         <% foreach (TrackHistory trackHistory in Model.TrackHistories)
            { %>
            <tr>
                <td><%= trackHistory.Datetime %></td>
                <td><%= trackHistory.Track.Title%></td>
                <td><%= trackHistory.Track.Artist.Name%></td>
            </tr>
            
         <% 
        } %>
        </tbody>
    </table>

</asp:Content>

