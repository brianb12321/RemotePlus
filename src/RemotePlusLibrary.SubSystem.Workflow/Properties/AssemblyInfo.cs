using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Workflow;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("RemotePlusLibrary.SubSystem.Workflow")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("RemotePlusLibrary.SubSystem.Workflow")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ExtensionLibrary(typeof(Startup), "RemotePlusLibrary.SubSystem.Workflow",
    FriendlyName = "RemotePlus Workflow System.",
    LibraryType = NetworkSide.Server | NetworkSide.Client | NetworkSide.Proxy)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]


// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a38779d7-8a28-4f55-a803-a20f518e9339")]

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
