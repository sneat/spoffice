using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Spoffice.Website.Helpers;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Controllers
{

    [HandleError]
    public class AccountController : BaseController
    {

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.

        public AccountController()
            : this(null, null)
        {
        }

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
            _MembershipService = new AccountMembershipService();
        }

        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        public AccountMembershipService _MembershipService
        {
            get;
            private set;
        }
 
        public ActionResult LogOn()
        {
            return MultiformatView(typeof(LoggedInStatusOutput), new LoggedInStatusOutput { LoggedIn = Request.IsAuthenticated });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {
            LoggedInStatusOutput status = new LoggedInStatusOutput { LoggedIn = false };
            if (!ValidateLogOn(userName, password))
            {
                status.Errors = ModelState;
                return MultiformatView(typeof(LoggedInStatusOutput), status);
            }

            FormsAuth.SignIn(userName, rememberMe);
            status.LoggedIn = true;

            return MultiformatView(typeof(LoggedInStatusOutput), status);
        }

        public ActionResult AccountInfo()
        {
            return MultiformatView(typeof(AccountInformationOutput), new AccountInformationOutput { 
                Username = Membership.GetUser().UserName, 
                Name = User.Identity.Name, 
                Email = Membership.GetUser().Email,
                MyAccountHeading = String.Format(System.Globalization.CultureInfo.CurrentCulture,
                         Res.Strings.MyAccountHeading,
                                 User.Identity.Name)
            });
        }

        public ActionResult LogOff()
        {
            FormsAuth.SignOut();
            return MultiformatView(typeof(LoggedInStatusOutput), new LoggedInStatusOutput(), Redirect("~/"));
        }

        public ActionResult Register()
        {
            return MultiformatView(typeof(LoggedInStatusOutput), new LoggedInStatusOutput());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(string userName, string email, string password, string confirmPassword)
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword))
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(userName, password, email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuth.SignIn(userName, false /* createPersistentCookie */);
                    return MultiformatView(typeof(LoggedInStatusOutput), new LoggedInStatusOutput{ LoggedIn = true });
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return MultiformatView(typeof(RegisterStatusOutput), new RegisterStatusOutput {  Success = false, Errors = ModelState});
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangeInformation(string email, string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(email) && String.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("_FORM", Res.Strings.ChangeAccountNoInfo);
            }
            if (!String.IsNullOrEmpty(email))
            {
                ValidateInformation(email);
            }
            if (!String.IsNullOrEmpty(newPassword))
            {
                ValidateChangePassword(currentPassword, newPassword, confirmPassword);
            }

            // Perform changes if we don't have errors
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(email))
                {
                    try
                    {
                        if (!_MembershipService.UpdateUser(User.Identity.Name, email))
                        {
                            ModelState.AddModelError("email", Res.Strings.ChangePasswordFailed);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("_FORM", Res.Strings.ChangePasswordFailed);
                    }
                }
                if (!String.IsNullOrEmpty(newPassword))
                {
                    try
                    {
                        if (!MembershipService.ChangePassword(User.Identity.Name, currentPassword, newPassword))
                        {
                            ModelState.AddModelError("_FORM", Res.Strings.ChangePasswordFailed);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("_FORM", Res.Strings.ChangePasswordFailed);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                return MultiformatView(typeof(AccountInformationOutput), new AccountInformationOutput { Success = true, Email = email });
            }
            else
            {
                return MultiformatView(typeof(AccountInformationOutput), new AccountInformationOutput { Success = false, Email = email, Errors = ModelState });
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException(Res.Strings.WindowsAuthNotSupported);
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", Res.Strings.ChangePasswordCurrentMissing);
            }
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         Res.Strings.ChangePasswordMinLength,
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", Res.Strings.ChangePasswordNoMatch);
            }

            return ModelState.IsValid;
        }

        private bool ValidateInformation(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("email", Res.Strings.AccountInvalidEmail);
            }
            if (!ModelState.IsValid)
            {
                // Don't try to look up whether the email address is in use if it's not a valid email
                string _user = Membership.GetUserNameByEmail(email);
                if (!String.IsNullOrEmpty(_user) && _user != User.Identity.Name)
                {
                    ModelState.AddModelError("email", Res.Strings.AccountDuplicateEmail);
                }
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", Res.Strings.ChangePasswordNoUsername);
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", Res.Strings.ChangePasswordNoPassword);
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", Res.Strings.ChangePasswordFailed);
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", Res.Strings.ChangePasswordNoUsername);
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", Res.Strings.ChangePasswordNoEmail);
            }
            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         Res.Strings.ChangePasswordMinLength,
                         MembershipService.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", Res.Strings.ChangePasswordNoMatch);
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return Res.Strings.AccountDuplicateUserName;

                case MembershipCreateStatus.DuplicateEmail:
                    return Res.Strings.AccountDuplicateEmail;

                case MembershipCreateStatus.InvalidPassword:
                    return Res.Strings.AccountInvalidPassword;

                case MembershipCreateStatus.InvalidEmail:
                    return Res.Strings.AccountInvalidEmail;

                case MembershipCreateStatus.InvalidAnswer:
                    return Res.Strings.AccountInvalidAnswer;

                case MembershipCreateStatus.InvalidQuestion:
                    return Res.Strings.AccountInvalidQuestion;

                case MembershipCreateStatus.InvalidUserName:
                    return Res.Strings.AccountInvalidUserName;

                case MembershipCreateStatus.ProviderError:
                    return Res.Strings.AccountProviderError;

                case MembershipCreateStatus.UserRejected:
                    return Res.Strings.AccountUserRejected;

                default:
                    return Res.Strings.AccountUnknownError;
            }
        }
        #endregion
    }

    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool UpdateUser(string userName, string email)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            currentUser.Email = email;
            try
            {
                Membership.UpdateUser(currentUser);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            return currentUser.ChangePassword(oldPassword, newPassword);
        }
    }
}
