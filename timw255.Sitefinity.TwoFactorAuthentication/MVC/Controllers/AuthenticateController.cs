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

            var model = new AuthenticateModel();

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
                    profile = profileManager.GetUserProfile(user, "Telerik.Sitefinity.Security.Model.authyprofile");

                    string authyId = profile.GetValue<string>("AuthyID");

                    if (!String.IsNullOrWhiteSpace(authyId))
                    {
                        Session["tfa.authyId"] = authyId;
                    }
                }

                if (is_form == "false")
                {
                    return new HttpStatusCodeResult(200);
                }
                else
                {
                    return Redirect("/TFA/Authenticate/Verify");
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

            var model = new AuthenticateModel();

            return View("Verify", model);
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
                Session["tfa.authState"] = 0;
                string realm = Session["tfa.realm"].ToString();
                string redirect_uri = Session["tfa.redirect_uri"].ToString();
                string deflate = Session["tfa.deflate"].ToString();
                string wrap_name = Session["tfa.wrap_name"].ToString();
                string sf_persistent = Session["tfa.sf_persistent"].ToString();

                Uri u = GetLoginUri(realm, redirect_uri, deflate, wrap_name, sf_persistent);

                return Redirect(u.AbsoluteUri);
            }
            else
            {
                return Redirect("/TFA/Authenticate/Verify");
            }
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

        private Uri GetLoginUri(string realm, string redirect_uri, string deflate, string wrap_name, string sf_persistent)
        {
            string issuer = Request.Url.AbsoluteUri;
            var tokenBuilder = new TokenBuilder();
            return tokenBuilder.ProcessRequest(issuer, realm, redirect_uri, deflate, wrap_name, sf_persistent);
        }
    }
}
