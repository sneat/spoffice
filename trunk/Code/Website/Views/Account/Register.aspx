<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewRes.SiteStrings.Register %>
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= ViewRes.AccountStrings.RegisterTitle %></h2>
    <p>
        <%= ViewRes.AccountStrings.RegisterTip %>
    </p>
    <p>
        <%= ViewRes.AccountStrings.RegisterTip2.Replace("{0}", Html.Encode(ViewData["PasswordLength"])) %>
    </p>
    <%= Html.ValidationSummary(ViewRes.ValidationStrings.RegisterUnsuccessful) %>

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
                    <label for="email"><%= ViewRes.AccountStrings.LabelEmail%></label>
                    <%= Html.TextBox("email") %>
                    <%= Html.ValidationMessage("email") %>
                </p>
                <p>
                    <label for="password"><%= ViewRes.AccountStrings.LabelPassword%></label>
                    <%= Html.Password("password") %>
                    <%= Html.ValidationMessage("password") %>
                </p>
                <p>
                    <label for="confirmPassword"><%= ViewRes.AccountStrings.LabelConfirmPassword %></label>
                    <%= Html.Password("confirmPassword") %>
                    <%= Html.ValidationMessage("confirmPassword") %>
                </p>
                <p>
                    <input type="submit" value="<%= ViewRes.SiteStrings.Register %>" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
