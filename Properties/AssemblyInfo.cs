using CommandLine.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("DotNetOMIDemo")]
[assembly: AssemblyDescription("A simple demo of using the SAS® Open Metadata Interface (OMI) from the .NET platform.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DotNetOMIDemo")]
[assembly: AssemblyCopyright("Copyright © 2012 Paul Homes")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d333c55b-481f-43b8-8ad9-d162088577bd")]

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
[assembly: AssemblyInformationalVersionAttribute("1.0")]

// from CommandLineParser.Text
[assembly: AssemblyLicense(
  "DotNetOMIDemo is licensed under the terms of the MIT License <http://opensource.org/licenses/MIT>.")]
[assembly: AssemblyUsage(
  "Usage: DotNetOMIDemo.exe --host=<hostname> --port=<port> --user=<user> --password=<password>",
  "       DotNetOMIDemo.exe --host localhost --port=8561 --user='sasadm@saspw' --password='secret'")]