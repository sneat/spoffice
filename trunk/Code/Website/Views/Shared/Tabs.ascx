<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<ul id="tabs" class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
    <li class="ui-state-default ui-corner-top ui-tabs-selected ui-state-active"><%= Html.ActionLink("Home", "Home", "Index", null, new { rel = "home", title = ViewRes.SiteStrings.SongHistory })%></li>
    <li class="ui-state-default ui-corner-top"><%= Html.ActionLink(ViewRes.SiteStrings.SongHistory, "Playlist", "Music", null, new { rel = "trackhistory", title = ViewRes.SiteStrings.SongHistory })%></li>
    <li class="ui-state-default ui-corner-top"><%= Html.ActionLink(ViewRes.SiteStrings.Favourites, "Index", "Favourites", null, new { rel = "favourites", title = ViewRes.SiteStrings.Favourites })%></li>
    <li class="ui-state-default ui-corner-top"><%= Html.ActionLink(ViewRes.SiteStrings.Account, "Index", "Account", null, new { rel = "myaccount", title = ViewRes.SiteStrings.Account })%></li>
    <li class="ui-state-default ui-corner-top" id="results-tab"><a href="#" rel="results">Search Results</a></li>
</ul>
