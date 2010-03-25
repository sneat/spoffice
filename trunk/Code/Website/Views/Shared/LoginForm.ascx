<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="login-form" title="Login">
    <p>
        <%= ViewRes.AccountStrings.LogOnHint.Replace("{0}", Html.ActionLink(ViewRes.SiteStrings.Register, "Register", "Account"))%>
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
</div>
