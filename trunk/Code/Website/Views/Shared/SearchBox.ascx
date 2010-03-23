<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="ScriptManager" %>
<% Html.ScriptManager().ScriptInclude("defaultvalue", "~/Scripts/jquery.defaultvalue.js"); %>
<% Html.ScriptManager().Script( () => {%>
    jQuery(function($) {
        $('#id').defaultValue('<%= ViewRes.MusicStrings.SearchByTrack %>');
    });
<%}); %>

<div id="search">
    <%  using (Html.BeginForm("Search", "Music", FormMethod.Post)){ %>
        <%=Html.TextBox("id", ViewRes.MusicStrings.SearchByTrack )%>
        <input type="submit" value="<%= ViewRes.MusicStrings.Search %>" />
    <% } %>
</div>