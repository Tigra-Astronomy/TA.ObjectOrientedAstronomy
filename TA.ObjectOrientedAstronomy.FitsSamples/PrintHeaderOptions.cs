// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: PrintHeaderOptions.cs  Last modified: 2020-12-15@10:58 by Tim Long

using CommandLine;

namespace TA.ObjectOrientedAstronomy.FitsSamples
    {
    [Verb("PrintHeader", true, new[] {"Header", "ListHeader", "ShowHeader"},
        HelpText = "Print the header records to the console window")]
    class PrintHeaderOptions
        {
        [Value(0, Required = true,
            HelpText = "The absolute or relative path to the input file")]
        public string InputFile { get; private set; }
        }
    }