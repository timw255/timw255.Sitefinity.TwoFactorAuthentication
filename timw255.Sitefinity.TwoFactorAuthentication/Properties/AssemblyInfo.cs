using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using timw255.Sitefinity.TwoFactorAuthentication;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("timw255.Sitefinity.TwoFactorAuthentication")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("timw255.Sitefinity.TwoFactorAuthentication")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Registers TwoFactorAuthenticationInstaller.PreApplicationStart() to be executed prior to the application start
[assembly: PreApplicationStartMethod(typeof(TwoFactorAuthenticationInstaller), "PreApplicationStart")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("72f929f7-0e96-41ef-a15f-deba35aa64f0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: ControllerContainer]
[assembly: WebResource("timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget.js", "application/x-javascript")]