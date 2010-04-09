<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="myAccount" runat="server">
    <h2><%= ViewRes.AccountStrings.ChangePassword %></h2>
    <p>
        <%= ViewRes.AccountStrings.ChangePasswordTip %>
    </p>
    <p>
        <%=ViewRes.AccountStrings.ChangePasswordTip2.Replace("{0}", Html.Encode(Html.Encode(ViewData["PasswordLength"])))%>
    </p>
    <%= Html.ValidationSummary(ViewRes.ValidationStrings.PasswordChangeUnsuccessful)%>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend><%= ViewRes.AccountStrings.AccountInformation %></legend>
                <p>
                    <label for="currentPassword"><%= ViewRes.AccountStrings.LabelCurrentPassword %></label>
                    <%= Html.Password("currentPassword") %>
                    <%= Html.ValidationMessage("currentPassword") %>
                </p>
                <p>
                    <label for="newPassword"><%= ViewRes.AccountStrings.LabelNewPassword %></label>
                    <%= Html.Password("newPassword") %>
                    <%= Html.ValidationMessage("newPassword") %>
                </p>
                <p>
                    <label for="confirmPassword"><%= ViewRes.AccountStrings.LabelNewPasswordConfirm %></label>
                    <%= Html.Password("confirmPassword") %>
                    <%= Html.ValidationMessage("confirmPassword") %>
                </p>
                <p>
                    <input type="submit" value="<%= ViewRes.SharedStrings.SubmitButton %>" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
