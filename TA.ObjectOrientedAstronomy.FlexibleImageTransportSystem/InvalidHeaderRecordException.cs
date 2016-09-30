// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: InvalidHeaderRecordException.cs  Last modified: 2016-09-30@01:17 by Tim Long

using System;
using System.Runtime.Serialization;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    [Serializable]
    public class InvalidHeaderRecordException : Exception
        {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidHeaderRecordException() : base("Invalid FITS header record") {}

        public InvalidHeaderRecordException(string message, string record = null) : base(message)
            {
            Record = record ?? string.Empty;
            }

        protected InvalidHeaderRecordException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}

        public string Record { get; set; }
        }
    }