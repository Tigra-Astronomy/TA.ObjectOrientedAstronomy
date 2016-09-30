// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Networks., all rights reserved.
// 
// File: AssemblyInfo.cs  Last modified: 2016-09-29@01:16 by Tim Long

using System;
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

[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0-uncontrolled")]

// For unit test
[assembly: InternalsVisibleTo("TA.ObjectOrientedAstronomy.Specifications")]