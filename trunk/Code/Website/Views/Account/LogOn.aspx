<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewRes.SiteStrings.LogOn %>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= ViewRes.SiteStrings.LogOn %></h2>
    <% Html.RenderPartial("LoginForm"); %>
</asp:Content>
