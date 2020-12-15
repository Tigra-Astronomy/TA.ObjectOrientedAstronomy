// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: FitsDemoApp.cs  Last modified: 2020-12-15@10:58 by Tim Long

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;

namespace TA.ObjectOrientedAstronomy.FitsSamples
    {
    internal static class FitsDemo
        {
        public static async Task PrintHeaderRecords(string inputFile)
            {
            var stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var reader = new FitsReader(stream);
            var header = await reader.ReadPrimaryHeader();
            header.HeaderRecords.ToList()
                .ForEach(Console.WriteLine);
            }

        public static async Task Copy(string source, string destination)
            {
            }
        }
    }