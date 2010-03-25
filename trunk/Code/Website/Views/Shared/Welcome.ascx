<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%= Html.Encode(ViewData["Message"]) %>
<%= ViewRes.HomeStrings.Content %> 