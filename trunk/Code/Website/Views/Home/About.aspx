<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewRes.AboutStrings.Title %>
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%= ViewRes.AboutStrings.Title %>
    <%= ViewRes.AboutStrings.Content %>
</asp:Content>
