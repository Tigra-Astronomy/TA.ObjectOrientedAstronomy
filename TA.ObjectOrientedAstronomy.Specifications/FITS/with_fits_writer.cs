// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: with_fits_writer.cs  Last modified: 2020-12-12@10:13 by Tim Long

using System;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.ObjectOrientedAstronomy.Specifications.FundamentalTypes;
using TA.ObjectOrientedAstronomy.Specifications.TestHelpers;

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {
    public class with_fits_writer
        {
        protected static readonly TimeSpan writerTimeout = TimeSpan.FromSeconds(1);
        static AggregateException exception;
        protected static LoggingStream outputStream;
        protected static FitsWriter writer;
        //                                                 0        1         2         3         4         5         6         7         8
        protected const string ValidFitsRecord          = "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
        protected const string ValidPrimaryHeaderRecord = "SWOWNER = 'Tigra Automatic Observatory' / Licensed software owner               ";
        Establish context = () =>
            {
            outputStream = new LoggingStream();
            writer = new FitsWriter(outputStream);
            };
        Cleanup after = () =>
            {
            writer = null; // [Sentinel]
            outputStream.Dispose();
            outputStream = null; // [Sentinel]
            };
        }
    }