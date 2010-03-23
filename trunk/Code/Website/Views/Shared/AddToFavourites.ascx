<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="ScriptManager" %>
<%@ Import Namespace="Spoffice.Website.Models" %>
<%@ Import Namespace="Spoffice.Website.Models.Spotify.MetadataApi" %>
<% Html.ScriptManager().ScriptInclude("jquery", "~/Scripts/jquery-latest.min.js"); %>
<% if (!(ViewData["favourites"] as List<Favourite>).Select(t=> FeedNode.ConvertPrivateToPublic(t.Track.Id)).Contains(Model))
{
%>
<%= Html.ActionLink(ViewRes.MusicStrings.AddFavourites, "Add", "Favourites", new { id = Model }, new { title = ViewRes.MusicStrings.AddFavourites, id = Model })%>
<%
}else {    
%>
<%= Html.ActionLink(ViewRes.MusicStrings.RemoveFavourites, "Remove", "Favourites", new { id = Model }, new { title = ViewRes.MusicStrings.AddFavourites, id = Model })%>
<% 
}
%>