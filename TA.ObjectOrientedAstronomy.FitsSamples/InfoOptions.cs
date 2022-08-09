// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: CopyOptions.cs  Last modified: 2020-12-15@11:22 by Tim Long

using CommandLine;

namespace TA.ObjectOrientedAstronomy.FitsSamples
    {
    [Verb("Info", false, new[] { "Statistics" }, HelpText = "Prints information about the FITS file.")]
    internal class InfoOptions
        {
        [Value(0, Required = true, HelpText = "The absolute or relative path to the input file")]
        public string InputFile { get; private set; }
        }
    }