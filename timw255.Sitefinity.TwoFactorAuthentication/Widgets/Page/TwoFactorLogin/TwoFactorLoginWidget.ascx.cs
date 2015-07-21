using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Sitefinity.Web.UI.PublicControls;

namespace timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin
{
    public class TwoFactorLoginWidget : LoginWidget
    {
        public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            IEnumerable<ScriptDescriptor> scriptDescriptors = base.GetScriptDescriptors();

            ScriptControlDescriptor descriptor = (ScriptControlDescriptor)scriptDescriptors.Single();

            descriptor.Type = typeof(TwoFactorLoginWidget).FullName;

            return new ScriptControlDescriptor[] { descriptor };
        }

        public override IEnumerable<ScriptReference> GetScriptReferences()
        {
            return new List<ScriptReference>()
            {
                new ScriptReference("timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget.js", typeof(TwoFactorLoginWidget).Assembly.FullName)
            };
        }
    }
}