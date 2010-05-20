<%@Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewRes.AccountStrings.ChangePassword %>
</asp:Content>

<asp:Content ID="changePasswordSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= ViewRes.AccountStrings.ChangePassword %></h2>
    <p>
        <%= ViewRes.ValidationStrings.PasswordChangeSuccessful %>
    </p>
</asp:Content>
