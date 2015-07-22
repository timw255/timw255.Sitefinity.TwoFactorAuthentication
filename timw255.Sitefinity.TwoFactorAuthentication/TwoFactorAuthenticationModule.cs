using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Routing;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Fluent.Modules;
using Telerik.Sitefinity.Fluent.Modules.Toolboxes;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;
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