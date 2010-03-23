<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CoolTunes.Models.Favourite>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= ViewRes.FavouritesStrings.Remove %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= ViewRes.FavouritesStrings.Remove %></h2>
    
    <% using (Html.BeginForm()){%>
        
        <fieldset>
            <%= Html.AntiForgeryToken() %>
            
            <p><%= String.Format(ViewRes.FavouritesStrings.ConfirmRemoveFavourite, Html.Encode(Model.Track.Title)) %></p>
            <input type="submit" value="<%= ViewRes.FavouritesStrings.Confirm %>" />
        </fieldset>
    <%} %>

</asp:Content>
