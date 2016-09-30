// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: Constants.cs  Last modified: 2016-09-29@18:04 by Tim Long

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public static class Constants
        {
        public const string EndKeyword = "END";
        public const short FitsRecordLength = 80;
        public const short FitsBlockLength = 2880;
        public const string FitsValidKeywordPattern = @"^(?<Keyword>[0-9A-Z-_]{1,8})$";
        public const string FitsHeaderRecordPattern =
            @"^( {80})|( */(?<Comment>.{1,69}))|(?<Keyword>[0-9A-Z-_]{1,8})([ ]*)(= ( *(?<Value>.{0,69}) *)(/ *(?<Comment>.{0,69}))|(?<Comment>.{0,69}))$";
        }
    }