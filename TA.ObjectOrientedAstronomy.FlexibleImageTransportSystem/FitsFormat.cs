// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsFormat.cs  Last modified: 2016-10-12@23:53 by Tim Long

using System.Collections.Generic;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public static class FitsFormat
        {
        public const string EndKeyword = "END";
        public const short FitsRecordLength = 80;
        public const short FitsBlockLength = 2880;
        public const string FitsValidKeywordPattern = @"^(?<Keyword>[0-9A-Z-_]{1,8})$";
        public const string FitsHeaderRecordPattern =
            @"^( {80})|( */(?<Comment>.{1,69}))|(?<Keyword>[0-9A-Z-_]{1,8})([ ]*)(= ( *(?<Value>.{0,69}) *)(/ *(?<Comment>.{0,69}))|(?<Comment>.{0,69}))$";
        public const char CommentCharacter = '/';
        public const string EmptyRecord =
            "                                                                                ";
        public const char EqualsCharacter = '=';
        public const int KeywordLength = 8;
        public const string KeywordPermittedCharacters = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ-_";
        public const int RecordsPerBlock = 36;
        public const int ValueFieldPosition = 10;
        public const string ValueIndicator = "= "; // Equals, Space
        public const int ValueIndicatorPosition = 8;
        public const string BooleanTrue = "T";
        public const string BooleanFalse = "F";
        public const char PadCharacter = ' '; // Space, ASCII 0x20, decimal 32
        public static IEnumerable<string> CommentaryKeywords = new[] {"COMMENT", "HISTORY"};
        }
    }