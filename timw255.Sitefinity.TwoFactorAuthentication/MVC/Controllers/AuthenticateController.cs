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
        public string Index()
        {
            return "Default action";
        }

        [Route("Authenticate/SWT")]
        [HttpGet]
        public ActionResult SWT(string realm, string redirect_uri, string deflate)
        {
            Session["tfa.authState"] = 0;

            var model = new LoginModel();

            model.ProvidersList = new List<SelectListItem>();

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

                        model.ProvidersList.Add(listItem);
                    }
                    finally
                    {
                        membershipDataProvider.SuppressSecurityChecks = false;
                    }
                }
            }

            return View("Login", model);
        }

        [Route("Authenticate/SWT")]
        [HttpPost]
        public ActionResult SWT(string realm, string redirect_uri, string deflate, string wrap_name, string wrap_password, string sf_domain = "Default", string sf_persistent = "false", string is_form = "false")
        {
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
                    //profile = profileManager.GetUserProfile(user, "Telerik.Sitefinity.Security.Model.authyprofile");
                    profile = profileManager.GetUserProfile<SitefinityProfile>(user);

                    string authyId = profile.GetValue<string>("AuthyID");

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

            return Redirect("/");
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
                return Redirect(GetLoginUri(true));
            }
            else
            {
                return Redirect("/TFA/Authenticate/Verify");
            }
        }

        private string GetLoginUri(bool isTwoFactor = false)
        {
            Session["tfa.authState"] = 0;
            string realm = Session["tfa.realm"].ToString();
            string redirect_uri = Session["tfa.redirect_uri"].ToString();
            string deflate = Session["tfa.deflate"].ToString();
            string wrap_name = Session["tfa.wrap_name"].ToString();
            string sf_persistent = Session["tfa.sf_persistent"].ToString();

            Uri u = GetTokenUri(realm, redirect_uri, deflate, wrap_name, sf_persistent, isTwoFactor);

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

        private Uri GetTokenUri(string realm, string redirect_uri, string deflate, string wrap_name, string sf_persistent, bool isTwoFactor)
        {
            string issuer = Request.Url.AbsoluteUri;
            var tokenBuilder = new TokenBuilder();
            return tokenBuilder.ProcessRequest(issuer, realm, redirect_uri, deflate, wrap_name, sf_persistent, isTwoFactor);
        }
    }
}
