using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using TestWorkflowExtensionLibrary;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("TestWorkflowExtensionLibrary")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("TestWorkflowExtensionLibrary")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ExtensionLibrary(typeof(Startup), "TestWorkflowExtensionLibrary",
    FriendlyName = "Test Workflow Extension Library",
    LibraryType = NetworkSide.Server)]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c1a67853-21db-47ea-bd92-aaaf976812fb")]

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
