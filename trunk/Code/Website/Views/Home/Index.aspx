<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MembershipUserCollection>" %>

<asp:Content ID="homeTab" ContentPlaceHolderID="homeTab" runat="server">   
    <%= Res.Strings.Welcome %>
    <a href="Music/Vote" title="Vote for track" id="voteFor" rel="for">Vote for track</a>
    <a href="Music/Vote" title="Vote against track" id="voteAgainst" rel="against">Vote against track</a>
</asp:Content>
