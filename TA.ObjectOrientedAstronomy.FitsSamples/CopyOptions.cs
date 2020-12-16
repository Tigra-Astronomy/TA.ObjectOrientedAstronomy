// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: CopyOptions.cs  Last modified: 2020-12-15@11:22 by Tim Long

using CommandLine;

namespace TA.ObjectOrientedAstronomy.FitsSamples
    {
    [Verb("Copy", false, new []{"Duplicate"},
        HelpText = "Creates a duplicate copy of a FITS with an additional HISTORY record to record the action.")]
    internal class CopyOptions
        {
        [Value(0, Required = true,
            HelpText = "The absolute or relative path to the input file")]
        public string InputFile { get; private set; }

        [Value(1,Required = true, HelpText = "The absolute or relative path to the output file.")]
        public string OutputFile { get; private set; }
        }
    }