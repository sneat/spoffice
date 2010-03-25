<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MembershipUserCollection>" %>
<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewRes.SharedStrings.Title %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">   
    <% Html.RenderPartial("Welcome"); %>
</asp:Content>
