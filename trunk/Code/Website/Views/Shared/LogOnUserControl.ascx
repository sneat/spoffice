<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        <%= ViewRes.SiteStrings.Welcome %> <b><%= Html.Encode(Page.User.Identity.Name) %></b>!
        [ <%= Html.ActionLink(ViewRes.SiteStrings.LogOff, "LogOff", "Account", null, new { title = ViewRes.SiteStrings.LogOff })%> ]
<%
    }
    else {
%> 
        [ <%= Html.ActionLink(ViewRes.SiteStrings.LogOn, "LogOn", "Account", null, new { title = ViewRes.SiteStrings.LogOn })%> ]
<%
    }
%>
