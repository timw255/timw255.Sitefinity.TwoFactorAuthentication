using System;
using System.Configuration;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;

namespace timw255.Sitefinity.TwoFactorAuthentication.Configuration
{
    [ObjectInfo(Title = "TwoFactorAuthentication", Description = "Configuration for the Two-Factor Authentication module")]
    public class TwoFactorAuthenticationConfig : ConfigSection
    {
        [ObjectInfo(Title = "Api Key", Description = "Api Key for Production")]
        [ConfigurationProperty("apiKey", DefaultValue = "")]
        public string ApiKey
        {
            get
            {
                return (string)this["apiKey"];
            }
            set
            {
                this["apiKey"] = value;
            }
        }
    }
}