<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<h2></h2>

<div class="ui-widget">
    <%= Html.RouteLink(Res.Strings.AccountInformationLink, new { controller = "Account", action = "ChangeInformation" }, new { id = "changeInformationLink", @class = "ui-priority-primary" })%>
    <%= Html.RouteLink(Res.Strings.Logout, new {controller = "Account", action = "LogOff"}, new { @class = "ui-priority-secondary" } ) %>
</div>

<div id="ChangeInformationDialog" title="<%= Res.Strings.AccountInformationTitle %>">
<%= Html.ValidationSummary(Res.Strings.ChangeAccountInformationErrors)%>

<% using (Html.BeginForm("ChangeInformation", "Account", FormMethod.Post, new { id = "ChangeInformationForm" })) { %>
    <fieldset>
        <legend><%= Res.Strings.AccountInformation %></legend>
        <div class="form-row">
            <label for="email"><%= Res.Strings.AccountInformationEmail%>:</label>
            <%= Html.TextBox("email", null, new { @class = "textfield ui-widget-content ui-corner-all"})%>
            <span class="hint ui-corner-all"><%= Res.Strings.ChangeEmailHint %></span>
        </div>
        <div class="form-row">
            <label for="currentPassword"><%= Res.Strings.ChangePasswordCurrentPassword %>:</label>
            <%= Html.Password("currentPassword", null, new { @class = "textfield ui-widget-content ui-corner-all" })%>
        </div>
        <div class="form-row">
            <label for="newPassword"><%= Res.Strings.ChangePasswordNewPassword %>:</label>
            <%= Html.Password("newPassword", null, new { @class = "textfield ui-widget-content ui-corner-all" })%>
            <span class="hint ui-corner-all"><%= Res.Strings.ChangePasswordHint %></span>
        </div>
        <div class="form-row">
            <label for="confirmPassword"><%= Res.Strings.ChangePasswordConfirmPassword %>:</label>
            <%= Html.Password("confirmPassword", null, new { @class = "textfield ui-widget-content ui-corner-all" })%>
        </div>
    </fieldset>
<% } %>
</div>