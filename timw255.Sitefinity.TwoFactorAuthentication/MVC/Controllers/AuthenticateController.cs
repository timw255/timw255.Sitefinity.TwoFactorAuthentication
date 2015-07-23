using Authy.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;
using timw255.Sitefinity.TwoFactorAuthentication.MVC.Models;
using Telerik.Sitefinity.Model;
using timw255.Sitefinity.TwoFactorAuthentication.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Security.Data;
using Telerik.Sitefinity.Data;

namespace timw255.Sitefinity.TwoFactorAuthentication.MVC.Controllers
{
    [RoutePrefix("TFA")]
    public class AuthenticateController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            return Redirect("/");
        }

        [Route("Authenticate/SWT")]
        [HttpGet]
        public ActionResult SWT(string realm, string redirect_uri, string deflate)
        {
            Session["tfa.authState"] = 0;

            var model = new LoginModel();

            model.ProvidersList = GetProvidersList();

            return View("Login", model);
        }

        [Route("Authenticate/SWT")]
        [HttpPost]
        public ActionResult SWT(string realm, string redirect_uri, string deflate, string wrap_name, string wrap_password, string sf_domain = "Default", string sf_persistent = "false", string is_form = "false")
        {
            var model = new LoginModel();
            UserManager um = new UserManager(sf_domain);
            if (um.ValidateUser(wrap_name, wrap_password))
            {
                Session["tfa.authState"] = 1;
                Session["tfa.realm"] = realm;
                Session["tfa.redirect_uri"] = redirect_uri;
                Session["tfa.deflate"] = deflate;
                Session["tfa.wrap_name"] = wrap_name;
                Session["tfa.sf_persistent"] = sf_persistent;

                UserProfileManager profileManager = UserProfileManager.GetManager();
                UserManager userManager = UserManager.GetManager();

                User user = userManager.GetUser(wrap_name);

                UserProfile profile = null;

                if (user != null)
                {
                    profile = profileManager.GetUserProfile<SitefinityProfile>(user);

                    string authyId = profile.GetValue<string>("AuthyId");

                    bool useTwoFactor = false;

                    if (!String.IsNullOrWhiteSpace(authyId))
                    {
                        useTwoFactor = true;

                        Session["tfa.authyId"] = authyId;
                    }

                    if (is_form == "false")
                    {
                        if (useTwoFactor)
                        {
                            return Json(new { url = "/TFA/Authenticate/Verify" });
                        }

                        return Json(new { url = GetLoginUri() });
                    }
                    else
                    {
                        if (useTwoFactor)
                        {
                            return Redirect("/TFA/Authenticate/Verify");
                        }

                        return Redirect(GetLoginUri());
                    }
                }
            }

            model.ProvidersList = GetProvidersList();

            ModelState.AddModelError("InvalidCredentials", "Incorrect Username/Password Combination");

            return View("Login", model);
        }

        [Route("Authenticate/Verify")]
        [HttpGet]
        public ActionResult Verify()
        {
            if (!IsAuthState(1))
            {
                return Redirect("/");
            }

            return View("Verify");
        }

        [Route("Authenticate/Verify")]
        [HttpPost]
        public ActionResult Verify(string token)
        {
            if (!IsAuthState(1))
            {
                return Redirect("/");
            }

            TwoFactorAuthenticationConfig config = Config.Get<TwoFactorAuthenticationConfig>();

            var authy = new AuthyClient(config.ApiKey, test: false);

            string authyId = Session["tfa.authyId"].ToString();

            VerifyTokenResult result = authy.VerifyToken(authyId, token);

            if (result.Success)
            {
                var loggedInUsers = SecurityManager.GetLoggedInBackendUsers();

                if (loggedInUsers.Where(u => u.UserName == Session["tfa.wrap_name"].ToString()).Count() > 0)
                {

                }

                return Redirect(GetLoginUri());
            }
            else
            {
                return Redirect("/TFA/Authenticate/Verify");
            }
        }

        private List<SelectListItem> GetProvidersList()
        {
            var providersList = new List<SelectListItem>();

            ProvidersCollection<MembershipDataProvider> providersCollection = ManagerBase<MembershipDataProvider>.ProvidersCollection ?? UserManager.GetManager().Providers;
            if (providersCollection.Count > 1)
            {
                string defaultProviderName = ManagerBase<MembershipDataProvider>.GetDefaultProviderName();
                
                foreach (MembershipDataProvider membershipDataProvider in providersCollection)
                {
                    membershipDataProvider.SuppressSecurityChecks = true;
                    try
                    {
                        SelectListItem listItem = new SelectListItem()
                            {
                                Text = membershipDataProvider.Title,
                                Value = membershipDataProvider.Name
                            };

                        if (defaultProviderName == membershipDataProvider.Name)
                        {
                            listItem.Selected = true;
                        }

                        providersList.Add(listItem);
                    }
                    finally
                    {
                        membershipDataProvider.SuppressSecurityChecks = false;
                    }
                }
            }

            return providersList;
        }

        private string GetLoginUri()
        {
            Session["tfa.authState"] = 0;
            string realm = Session["tfa.realm"].ToString();
            string redirect_uri = Session["tfa.redirect_uri"].ToString();
            bool deflate = "true".Equals(Session["tfa.deflate"].ToString(), StringComparison.OrdinalIgnoreCase);
            string wrap_name = Session["tfa.wrap_name"].ToString();
            string sf_persistent = Session["tfa.sf_persistent"].ToString();

            Uri u = GetTokenUri(realm, redirect_uri, deflate, wrap_name, sf_persistent);

            return u.AbsoluteUri;
        }

        private bool IsAuthState(int value)
        {
            if (Session["tfa.authState"] == null)
            {
                return false;
            }

            int authState = ((int)Session["tfa.authState"]);

            if (authState == value)
            {
                return true;
            }

            return false;
        }

        private Uri GetTokenUri(string realm, string redirect_uri, bool deflate, string wrap_name, string sf_persistent)
        {
            string issuer = Request.Url.AbsoluteUri;
            //string issuer = "http://localhost";
            var tokenBuilder = new TokenBuilder();
            return tokenBuilder.ProcessRequest(realm, redirect_uri, deflate, wrap_name, sf_persistent);
        }
    }
}
