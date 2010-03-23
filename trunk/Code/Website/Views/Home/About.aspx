<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewRes.AboutStrings.Title %>
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= ViewRes.AboutStrings.Title %></h2>
    <p>
        <%= ViewRes.AboutStrings.Content %>
    </p>
</asp:Content>
