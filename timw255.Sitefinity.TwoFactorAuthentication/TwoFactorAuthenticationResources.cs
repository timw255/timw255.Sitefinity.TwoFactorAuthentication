using System;
using System.Linq;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace timw255.Sitefinity.TwoFactorAuthentication
{
    [ObjectInfo("TwoFactorAuthenticationResources", ResourceClassId = "TwoFactorAuthenticationResources", Title = "TwoFactorAuthenticationResourcesTitle", TitlePlural = "TwoFactorAuthenticationResourcesTitlePlural", Description = "TwoFactorAuthenticationResourcesDescription")]
    public class TwoFactorAuthenticationResources : Resource
    {
        #region Construction
        /// <summary>
        /// Initializes new instance of <see cref="TwoFactorAuthenticationResources"/> class with the default <see cref="ResourceDataProvider"/>.
        /// </summary>
        public TwoFactorAuthenticationResources()
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="TwoFactorAuthenticationResources"/> class with the provided <see cref="ResourceDataProvider"/>.
        /// </summary>
        /// <param name="dataProvider"><see cref="ResourceDataProvider"/></param>
        public TwoFactorAuthenticationResources(ResourceDataProvider dataProvider)
            : base(dataProvider)
        {
        }
        #endregion

        #region Class Description
        /// <summary>
        /// TwoFactorAuthentication Resources
        /// </summary>
        [ResourceEntry("TwoFactorAuthenticationResourcesTitle",
            Value = "TwoFactorAuthentication module labels",
            Description = "The title of this class.",
            LastModified = "2015/07/20")]
        public string TwoFactorAuthenticationResourcesTitle
        {
            get
            {
                return this["TwoFactorAuthenticationResourcesTitle"];
            }
        }

        /// <summary>
        /// TwoFactorAuthentication Resources Title plural
        /// </summary>
        [ResourceEntry("TwoFactorAuthenticationResourcesTitlePlural",
            Value = "TwoFactorAuthentication module labels",
            Description = "The title plural of this class.",
            LastModified = "2015/07/20")]
        public string TwoFactorAuthenticationResourcesTitlePlural
        {
            get
            {
                return this["TwoFactorAuthenticationResourcesTitlePlural"];
            }
        }

        /// <summary>
        /// Contains localizable resources for TwoFactorAuthentication module.
        /// </summary>
        [ResourceEntry("TwoFactorAuthenticationResourcesDescription",
            Value = "Contains localizable resources for TwoFactorAuthentication module.",
            Description = "The description of this class.",
            LastModified = "2015/07/20")]
        public string TwoFactorAuthenticationResourcesDescription
        {
            get
            {
                return this["TwoFactorAuthenticationResourcesDescription"];
            }
        }
        #endregion
    }
}