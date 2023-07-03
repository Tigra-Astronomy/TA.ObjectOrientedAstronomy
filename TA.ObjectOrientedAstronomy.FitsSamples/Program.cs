// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: Program.cs  Last modified: 2020-12-15@10:58 by Tim Long

using System;
using System.Threading.Tasks;
using CommandLine;

namespace TA.ObjectOrientedAstronomy.FitsSamples
    {
    class Program
        {
        static async Task Main(string[] args)
            {
            var commandLineParser = new Parser(
                with =>
                    {
                    with.CaseSensitive = false;
                    with.IgnoreUnknownArguments = false;
                    with.HelpWriter = Console.Out;
                    with.AutoVersion = true;
                    with.AutoHelp = true;
                    });

            await commandLineParser.ParseArguments<PrintHeaderOptions, CopyOptions, InfoOptions>(args)
                .MapResult(
                    (PrintHeaderOptions opt)  => FitsDemo.PrintHeaderRecords(opt.InputFile),
                    (CopyOptions copy)=>FitsDemo.Copy(copy.InputFile, copy.OutputFile),
                    (InfoOptions info)=>FitsDemo.ShowInformation(info.InputFile),
                    errors => Task.CompletedTask
                );

            //await parserResult.WithParsedAsync<PrintHeaderOptions>(options => FitsDemo.PrintHeaderRecords(options.InputFile))
            //    await parserResult.WithParsedAsync()
            //    ;
            }
        }
    }