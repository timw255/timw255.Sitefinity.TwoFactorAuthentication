using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Routing;
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
using Telerik.Sitefinity.Web.UI.Fields.Enums;
using timw255.Sitefinity.TwoFactorAuthentication.Configuration;
using timw255.Sitefinity.TwoFactorAuthentication.Security;
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
            this.InstallUserProfileTypes();
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

            this.UninstallUserProfileTypes();
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
        private void InstallUserProfileTypes()
        {
            var metaDataConfig = Config.Get<MetadataConfig>();
            string metaDataProviderName = metaDataConfig.DefaultProvider;

            var userProfileTypes = UserProfilesHelper.GetUserProfileTypes(metaDataProviderName);
            var authyUserProfileType = userProfileTypes.Where(t => t.DynamicTypeName == typeof(AuthyProfile).FullName).FirstOrDefault();

            if (authyUserProfileType == null)
            {
                var profileTypeData = UserProfileTypeViewModel.GetBlankItem(typeof(AuthyProfile), "") as UserProfileTypeViewModel;

                profileTypeData.DynamicTypeName = typeof(AuthyProfile).FullName;
                profileTypeData.Id = Guid.Empty;
                profileTypeData.MembershipProvidersUsage = MembershipProvidersUsage.AllProviders;
                profileTypeData.Name = "AuthyProfile";
                profileTypeData.Title = "Authy profile";

                ConfigManager manager = ConfigManager.GetManager();
                UserProfilesConfig section = manager.GetSection<UserProfilesConfig>();
                string str = string.Concat(typeof(AuthyProfile).Namespace, ".", profileTypeData.Name);
                if (section.ProfileTypesSettings.ContainsKey(str))
                {
                    throw new ArgumentException(Res.Get<UserProfilesResources>().ErrorProfileTypeAlreadyExists);
                }
                string title = profileTypeData.Title;
                MetadataManager metadataManager = MetadataManager.GetManager();
                if ((
                    from td in metadataManager.GetMetaTypeDescriptions()
                    where td.UserFriendlyName == title
                    select td).Count<MetaTypeDescription>() > 0)
                {
                    throw new ArgumentException(Res.Get<UserProfilesResources>().ErrorProfileTypeTitleAlreadyExists);
                }
                string name = profileTypeData.Name;
                MetaType fullName = metadataManager.CreateMetaType(typeof(AuthyProfile).Namespace, profileTypeData.Name);
                fullName.BaseClassName = typeof(UserProfile).FullName;
                fullName.IsDynamic = true;
                fullName.DatabaseInheritance = DatabaseInheritanceType.vertical;
                MetaTypeDescription metaTypeDescription = metadataManager.CreateMetaTypeDescription(fullName.Id);
                UpdateUserProfileType(fullName, metaTypeDescription, section, profileTypeData);
                metadataManager.SaveChanges(true);
                manager.SaveSection(section);
                string fullTypeName = fullName.FullTypeName;
                profileTypeData.DynamicTypeName = fullTypeName;
                string contentViewDefinitionName = UserProfilesHelper.GetContentViewDefinitionName(profileTypeData.Name);

                ContentViewConfig contentViewConfig = manager.GetSection<ContentViewConfig>();
                ConfigElementDictionary<string, ContentViewControlElement> contentViewControls = contentViewConfig.ContentViewControls;
                ContentViewControlDefinitionFacade contentViewControlDefinitionFacade = App.WorkWith().Module().DefineContainer(contentViewControls, contentViewDefinitionName).SetContentTypeName(fullTypeName);
                ContentViewControlElement contentViewControlElement = contentViewControlDefinitionFacade.Get();

                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.BackendCreate, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.BackendEdit, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.BackendView, FieldDisplayMode.Read);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.FrontendCreate, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.FrontendEdit, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.FrontendView, FieldDisplayMode.Read);

                contentViewControls.Add(contentViewControlElement);
                manager.SaveSection(contentViewConfig);

                SystemManager.RestartApplication(OperationReason.KnownKeys.StaticModulesUpdate);
            }
        }

        private void UninstallUserProfileTypes()
        {
            var metaDataConfig = Config.Get<MetadataConfig>();
            string metaDataProviderName = metaDataConfig.DefaultProvider;

            var userProfileTypes = UserProfilesHelper.GetUserProfileTypes(metaDataConfig.DefaultProvider);
            var authyUserProfileType = userProfileTypes.Where(t => t.DynamicTypeName == typeof(AuthyProfile).FullName).FirstOrDefault();

            if (authyUserProfileType != null)
            {
                Telerik.Sitefinity.Fluent.AppSettings appSetting = App.Prepare();
                if (!string.IsNullOrEmpty(metaDataProviderName))
                {
                    appSetting.MetadataProviderName = metaDataProviderName;
                }
                DynamicTypeDescriptionFacade dynamicTypeDescriptionFacade = appSetting.WorkWith().DynamicData().TypeDescription(authyUserProfileType.Id);
                DynamicTypeFacade dynamicTypeFacade = dynamicTypeDescriptionFacade.DynamicType();
                Type clrType = dynamicTypeFacade.Get().ClrType;
                if (clrType == typeof(SitefinityProfile))
                {
                    throw new InvalidOperationException(Res.Get<UserProfilesResources>().ErrorDeleteBuiltInProfileType);
                }
                UserProfileManager userProfileManager = UserProfilesHelper.GetUserProfileManager(clrType, null);
                userProfileManager.DeleteProfilesForProfileType(clrType);
                userProfileManager.SaveChanges();
                dynamicTypeDescriptionFacade.Delete();
                dynamicTypeFacade.Delete();

                ConfigManager manager = ConfigManager.GetManager();
                UserProfilesConfig section = manager.GetSection<UserProfilesConfig>();
                section.ProfileTypesSettings.Remove(dynamicTypeFacade.Get().FullTypeName);
                ContentViewConfig contentViewConfig = manager.GetSection<ContentViewConfig>();
                string contentViewDefinitionName = UserProfilesHelper.GetContentViewDefinitionName(clrType);
                contentViewConfig.ContentViewControls.Remove(contentViewDefinitionName);
                dynamicTypeFacade.SaveChanges(true);
                manager.SaveSection(section);
                manager.SaveSection(contentViewConfig);

                SystemManager.RestartApplication(OperationReason.KnownKeys.StaticModulesUpdate);
            }
        }

        private void UpdateUserProfileType(MetaType metaType, MetaTypeDescription typeDescription, UserProfilesConfig profilesConfig, UserProfileTypeViewModel profileTypeData)
        {
            string fullTypeName = metaType.FullTypeName;
            typeDescription.UserFriendlyName = profileTypeData.Title;
            UpdateConfiguration(profilesConfig, fullTypeName, profileTypeData);
        }

        private void UpdateConfiguration(UserProfilesConfig profilesConfig, string metaTypeFullName, UserProfileTypeViewModel profileTypeData)
        {
            ProfileTypeSettings profileTypeSettings = UserProfilesHelper.GetProfileTypeSettings(profilesConfig, metaTypeFullName, true);
            profileTypeSettings.ProfileProvider = profileTypeData.ProfileProviderName;
            profileTypeSettings.UseAllMembershipProviders = new bool?(profileTypeData.MembershipProvidersUsage == MembershipProvidersUsage.AllProviders);
            profileTypeSettings.MembershipProviders.Clear();
            if (profileTypeData.MembershipProvidersUsage == MembershipProvidersUsage.SpecifiedProviders)
            {
                ProviderViewModel[] selectedMembershipProviders = profileTypeData.SelectedMembershipProviders;
                for (int i = 0; i < (int)selectedMembershipProviders.Length; i++)
                {
                    ProviderViewModel providerViewModel = selectedMembershipProviders[i];
                    ConfigElementList<MembershipProviderElement> membershipProviders = profileTypeSettings.MembershipProviders;
                    MembershipProviderElement membershipProviderElement = new MembershipProviderElement(profileTypeSettings.MembershipProviders)
                    {
                        ProviderName = providerViewModel.ProviderName
                    };
                    membershipProviders.Add(membershipProviderElement);
                }
            }
        }

        private void InsertDetailView(ContentViewControlDefinitionFacade fluentContentView, ProfileTypeViewKind viewKind, FieldDisplayMode displayMode)
        {
            ContentViewSectionElement contentViewSectionElement;
            DetailViewDefinitionFacade detailViewDefinitionFacade = fluentContentView.AddDetailView(UserProfilesHelper.GetContentViewName(viewKind)).SetTitle(UserProfilesHelper.GetContentViewTitle(viewKind)).HideTopToolbar().SetDisplayMode(displayMode).LocalizeUsing<UserProfilesResources>().DoNotRenderTranslationView().DoNotUseWorkflow();
            DetailFormViewElement detailFormViewElement = detailViewDefinitionFacade.Get();
            string str = CustomFieldsContext.customFieldsSectionName;
            if (!detailFormViewElement.Sections.TryGetValue(str, out contentViewSectionElement))
            {
                SectionDefinitionFacade<DetailViewDefinitionFacade> sectionDefinitionFacade = detailViewDefinitionFacade.AddExpandableSection(str).SetDisplayMode(detailFormViewElement.DisplayMode);
                contentViewSectionElement = sectionDefinitionFacade.Get();
            }
            fluentContentView.Get().ViewsConfig.Add(detailFormViewElement);
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