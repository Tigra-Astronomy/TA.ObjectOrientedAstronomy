// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: GlobalAssemblyInfo.cs  Last modified: 2016-11-13@19:44 by Tim Long

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG

[assembly: AssemblyConfiguration("Uncontrolled Debug")]
#else
[assembly: AssemblyConfiguration("Uncontrolled Release")]
#endif

[assembly: AssemblyCompany("Tigra Astronomy")]
[assembly: AssemblyProduct("Object Oriented Astronomy")]
[assembly: AssemblyCopyright("Copyright © 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("0.0.*")]
[assembly: AssemblyFileVersion("0.0.0.0")]
[assembly: AssemblyInformationalVersion("0.0.3-integration")]

// For unit test

[assembly: InternalsVisibleTo("TA.ObjectOrientedAstronomy.Specifications")]