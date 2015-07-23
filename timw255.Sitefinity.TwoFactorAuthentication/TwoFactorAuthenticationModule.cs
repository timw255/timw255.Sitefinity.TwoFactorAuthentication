using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Routing;
using System.Web.UI;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Metadata;
using Telerik.Sitefinity.Fluent.Definitions;
using Telerik.Sitefinity.Fluent.DynamicData;
using Telerik.Sitefinity.Fluent.Modules;
using Telerik.Sitefinity.Fluent.Modules.Toolboxes;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Metadata.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.ModuleEditor.Web.Services.Model;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Modules.UserProfiles;
using Telerik.Sitefinity.Modules.UserProfiles.Configuration;
using Telerik.Sitefinity.Modules.UserProfiles.Web.Services;
using Telerik.Sitefinity.Modules.UserProfiles.Web.Services.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ContentUI.Config;
using Telerik.Sitefinity.Web.UI.ContentUI.Views.Backend.Master.Config;
using Telerik.Sitefinity.Web.UI.Fields.Config;
using Telerik.Sitefinity.Web.UI.Fields.Enums;
using timw255.Sitefinity.TwoFactorAuthentication.Configuration;
using timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin;

namespace timw255.Sitefinity.TwoFactorAuthentication
{
    /// <summary>
    /// Custom Sitefinity module 
    /// </summary>
    public class TwoFactorAuthenticationModule : ModuleBase
    {
        #region Properties
        /// <summary>
        /// Gets the landing page id for the module.
        /// </summary>
        /// <value>The landing page id.</value>
        public override Guid LandingPageId
        {
            get
            {
                return SiteInitializer.DashboardPageNodeId;
            }
        }

        /// <summary>
        /// Gets the CLR types of all data managers provided by this module.
        /// </summary>
        /// <value>An array of <see cref="T:System.Type" /> objects.</value>
        public override Type[] Managers
        {
            get
            {
                return new Type[0];
            }
        }
        #endregion

        #region Module Initialization
        /// <summary>
        /// Initializes the service with specified settings.
        /// This method is called every time the module is initializing (on application startup by default)
        /// </summary>
        /// <param name="settings">The settings.</param>
        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);

            Config.RegisterSection<TwoFactorAuthenticationConfig>();

            App.WorkWith()
                .Module(settings.Name)
                    .Initialize()
                    .Localization<TwoFactorAuthenticationResources>();
        }

        /// <summary>
        /// Installs this module in Sitefinity system for the first time.
        /// </summary>
        /// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
        public override void Install(SiteInitializer initializer)
        {
            this.InstallVirtualPaths(initializer);
            this.InstallPageWidgets(initializer);
            this.InstallUserProfileTypes(initializer);
        }

        /// <summary>
        /// Upgrades this module from the specified version.
        /// This method is called instead of the Install method when the module is already installed with a previous version.
        /// </summary>
        /// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
        /// <param name="upgradeFrom">The version this module us upgrading from.</param>
        public override void Upgrade(SiteInitializer initializer, Version upgradeFrom)
        {
        }

        /// <summary>
        /// Uninstalls the specified initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public override void Uninstall(SiteInitializer initializer)
        {
            base.Uninstall(initializer);
        }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Gets the module configuration.
        /// </summary>
        protected override ConfigSection GetModuleConfig()
        {
            return Config.Get<TwoFactorAuthenticationConfig>();
        }
        #endregion

        #region Virtual paths
        /// <summary>
        /// Installs module virtual paths.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallVirtualPaths(SiteInitializer initializer)
        {
            var virtualPaths = initializer.Context.GetConfig<VirtualPathSettingsConfig>().VirtualPaths;
            var moduleVirtualPath = TwoFactorAuthenticationModule.ModuleVirtualPath + "*";
            if (!virtualPaths.ContainsKey(moduleVirtualPath))
            {
                virtualPaths.Add(new VirtualPathElement(virtualPaths)
                {
                    VirtualPath = moduleVirtualPath,
                    ResolverName = "EmbeddedResourceResolver",
                    ResourceLocation = typeof(TwoFactorAuthenticationModule).Assembly.GetName().Name
                });
            }
        }
        #endregion

        #region Install backend pages
        /// <summary>
        /// Installs the backend pages.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallBackendPages(SiteInitializer initializer)
        {
        }
        #endregion

        #region Widgets
        /// <summary>
        /// Installs the form widgets.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallFormWidgets(SiteInitializer initializer)
        {
        }

        /// <summary>
        /// Installs the layout widgets.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallLayoutWidgets(SiteInitializer initializer)
        {
        }

        /// <summary>
        /// Installs the page widgets.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallPageWidgets(SiteInitializer initializer)
        {
            string modulePageWidgetSectionName = "TwoFactorAuthentication";
            string modulePageWidgetSectionTitle = "Two Factor Authentication";
            string modulePageWidgetSectionDescription = "Two Factor Authentication";

            initializer.Installer
                .Toolbox(CommonToolbox.PageWidgets)
                    .LoadOrAddSection(modulePageWidgetSectionName)
                        .SetTitle(modulePageWidgetSectionTitle)
                        .SetDescription(modulePageWidgetSectionDescription)
                        .LoadOrAddWidget<TwoFactorLoginWidget>("TwoFactorLogin")
                            .SetTitle("Two Factor Login")
                            .SetDescription("Two Factor Login Widget")
                            .LocalizeUsing<TwoFactorAuthenticationResources>()
                            .SetCssClass("sfLoginIcn")
                        .Done()
                    .Done()
                .Done();
        }
        #endregion

        #region Custom Profile
        /// <summary>
        /// Installs the profile type.
        /// </summary>
        private void InstallUserProfileTypes(SiteInitializer initializer)
        {
            MetadataManager managerInTransaction = initializer.GetManagerInTransaction<MetadataManager>();
            MetaType metaType = managerInTransaction.GetMetaType(typeof(SitefinityProfile));
            if (metaType != null)
            {
                MetaField fullName = (
                    from f in metaType.Fields
                    where f.FieldName == "AuthyId"
                    select f).FirstOrDefault<MetaField>();
                if (fullName == null)
                {
                    fullName = managerInTransaction.CreateMetafield("AuthyId");
                    fullName.Title = "AuthyId";
                    fullName.ClrType = typeof(string).FullName;
                    fullName.ColumnName = "authy_id";
                    fullName.Required = false;
                    fullName.Hidden = false;
                    fullName.SetMinValue(0);
                    IList<MetaFieldAttribute> metaAttributes = fullName.MetaAttributes;
                    MetaFieldAttribute metaFieldAttribute = new MetaFieldAttribute()
                    {
                        Name = "UserFriendlyDataType",
                        Value = UserFriendlyDataType.ShortText.ToString()
                    };
                    metaAttributes.Add(metaFieldAttribute);
                    IList<MetaFieldAttribute> metaFieldAttributes = fullName.MetaAttributes;
                    MetaFieldAttribute metaFieldAttribute1 = new MetaFieldAttribute()
                    {
                        Name = "IsCommonProperty",
                        Value = "true"
                    };
                    metaFieldAttributes.Add(metaFieldAttribute1);
                    metaType.Fields.Add(fullName);
                }
            }
            this.InstallAuthyProfileConfiguration(initializer);
        }

        private void InstallAuthyProfileConfiguration(SiteInitializer initializer)
        {
            Type type = typeof(SitefinityProfile);
            List<DetailFormViewElement> detailFormViewElements = new List<DetailFormViewElement>();
            foreach (ContentViewControlElement value in initializer.Context.GetConfig<ContentViewConfig>().ContentViewControls.Values)
            {
                if (value.ContentType != type)
                {
                    continue;
                }
                detailFormViewElements.AddRange(CustomFieldsContext.GetViews(value.ViewsConfig.Values));
            }
            foreach (DetailFormViewElement detailFormViewElement in detailFormViewElements)
            {
                ContentViewSectionElement contentViewSectionElement = detailFormViewElement.Sections.Values.First<ContentViewSectionElement>();
                if (!contentViewSectionElement.Fields.ContainsKey("AuthyId"))
                {
                    TextFieldDefinitionElement textFieldDefinitionElement = new TextFieldDefinitionElement(contentViewSectionElement.Fields)
                    {
                        ID = "authyIdField",
                        FieldName = "AuthyId",
                        DataFieldName = "AuthyId",
                        DisplayMode = new FieldDisplayMode?(FieldDisplayMode.Write),
                        Title = "AuthyId",
                        WrapperTag = HtmlTextWriterTag.Li,
                        ResourceClassId = typeof(TwoFactorAuthenticationResources).Name,
                        Hidden = new bool?(false)
                    };
                    contentViewSectionElement.Fields.Add(textFieldDefinitionElement);
                }
            }
        }
        #endregion

        #region Upgrade methods
        #endregion

        #region Private members & constants
        public const string ModuleName = "Two Factor Authentication";
        internal const string ModuleTitle = "Two Factor Authentication";
        internal const string ModuleDescription = "This is a custom module that adds two-factor authentication to Telerik Sitefinity.";
        internal const string ModuleVirtualPath = "~/TwoFactorAuthentication/";
        #endregion
    }
}