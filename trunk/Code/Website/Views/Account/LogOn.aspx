<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewRes.SiteStrings.LogOn %>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= ViewRes.SiteStrings.LogOn %></h2>
    <p>
        <%= ViewRes.AccountStrings.LogOnHint.Replace("{0}", Html.ActionLink(ViewRes.SiteStrings.Register, "Register"))%>
    </p>
    <%= Html.ValidationSummary(ViewRes.ValidationStrings.LogOnUnsuccessful) %>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend><%= ViewRes.AccountStrings.AccountInformation %></legend>
                <p>
                    <label for="username"><%= ViewRes.AccountStrings.LabelUsername %></label>
                    <%= Html.TextBox("username") %>
                    <%= Html.ValidationMessage("username") %>
                </p>
                <p>
                    <label for="password"><%= ViewRes.AccountStrings.LabelPassword %></label>
                    <%= Html.Password("password") %>
                    <%= Html.ValidationMessage("password") %>
                </p>
                <p>
                    <%= Html.CheckBox("rememberMe") %> <label class="inline" for="rememberMe"><%= ViewRes.AccountStrings.LabelRememberMe %></label>
                </p>
                <p>
                    <input type="submit" value="<%= ViewRes.SiteStrings.LogOn %>" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
