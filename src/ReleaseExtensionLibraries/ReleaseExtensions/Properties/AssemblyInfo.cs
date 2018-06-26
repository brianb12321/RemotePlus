using System.Reflection;
using System.Runtime.InteropServices;
using RemotePlusLibrary.Extension.ExtensionLoader;

// RemotePlus server extension attributes.
[assembly: ExtensionLibrary(typeof(ReleaseExtensions.Startup), "ReleaseExtensions",
    FriendlyName = "Release Extensions",
    LibraryType = ExtensionLibraryType.Both,
    Guid = "DAA60EA7-76D1-4144-8CD8-FC1DE9C8A5C2")]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ReleaseExtensions")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ReleaseExtensions")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4a312e8c-fe94-4996-b2b4-55c9854075c8")]

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
