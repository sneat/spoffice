<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<User>" %>
<%@ Import Namespace="Spoffice.Website.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ViewRes.AccountStrings.Title.Replace("{0}", Html.Encode(Page.User.Identity.Name))%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="form1" runat="server">

    <h2><%=ViewRes.AccountStrings.Title.Replace("{0}", Html.Encode(Page.User.Identity.Name))%></h2>
    
    <p><%=Html.ActionLink(ViewRes.AccountStrings.ChangePassword, "ChangePassword") %></p>
    
    <p><%= ViewRes.AccountStrings.Content %></p>

    </form>

</asp:Content>

