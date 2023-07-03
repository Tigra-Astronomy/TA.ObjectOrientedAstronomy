// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: when_constructing_a_fits_header_record.cs  Last modified: 2020-08-22@14:49 by Tim Long

using System;
using System.Linq;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.Utils.Core;

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {
    /*
     * The keyword name shall be a left justified, eight-character,
     * space-filled, ASCII string with no embedded spaces. All digits
     * 0 through 9 (decimal ASCII codes 48 to 57, or hexadecimal 30
     * to 39) and upper case Latin alphabetic characters ’A’ through
     * ’Z’ (decimal 65 to 90 or hexadecimal 41 to 5A) are permitted;
     * lower-case characters shall not be used. The underscore (’ ’,
     * decimal 95 or hexadecimal 5F) and hyphen (’-’, decimal 45
     * or hexadecimal 2D) are also permitted. No other characters
     * are permitted.3 For indexed keyword names that have a single
     * positive integer index counter appended to the root name,
     * the counter shall not have leading zeros (e.g., NAXIS1, not
     * NAXIS001). Note that keyword names that begin with (or
     * consist solely of) any combination of hyphens, underscores, and
     * digits are legal.
     */
    [Subject(typeof(FitsHeaderRecord), "keyword")]
    internal class when_creating_a_fully_populated_fits_header_record
        {
        const string Keyword = "keyWoRd";
        const string Value = "'Value'";
        const string Comment = "Comment";

        Because of = () =>
            {
            record = FitsHeaderRecord.Create(Keyword, Value.AsMaybe(), Comment.AsMaybe());
            };
        It should_contain_the_keyword = () => record.Keyword.ShouldEqual(Keyword.ToUpper());
        It should_contain_the_value = () => record.Value.Single().ShouldEqual(Value);
        It should_contain_the_comment = () => record.Comment.Single().ShouldEqual(Comment);
        It should_be_a_valid_fits_record = () => record.Text.Length.ShouldEqual(FitsFormat.FitsRecordLength);
        It should_have_the_expected_text = () => record.Text.ShouldEqual(ExpectedText);
        static FitsHeaderRecord record;
        //                           0        1         2         3         4         5         6         7         8
        //                           12345678901234567890123456789012345678901234567890123456789012345678901234567890
        const string ExpectedText = "KEYWORD = 'Value' / Comment                                                     ";
        }

    [Subject(typeof(FitsHeaderRecord), "invalid keyword")]
    internal class when_creating_a_header_record_and_the_keyword_has_embedded_spaces
        {
        Establish context;
        Because of = () => exception = Catch.Exception(()=>FitsHeaderRecord.Create("KEY WORD",Maybe<string>.Empty, Maybe<string>.Empty));
        It should_throw = () => exception.ShouldBeOfExactType<FitsFormatException>();
        static Exception exception;
        }

    [Subject(typeof(FitsHeaderRecord), "invalid keyword")]
    internal class when_creating_a_header_record_and_the_keyword_has_invalid_characters
        {
        Establish context;
        Because of = () => exception = Catch.Exception(()=>FitsHeaderRecord.Create("KEY$WORD",Maybe<string>.Empty, Maybe<string>.Empty));
        It should_throw = () => exception.ShouldBeOfExactType<FitsFormatException>();
        static Exception exception;
        }

    [Subject(typeof(FitsHeaderRecord), "invalid keyword")]
    internal class when_creating_a_header_record_and_the_keyword_is_too_long
        {
        Because of = () => exception = Catch.Exception(()=>FitsHeaderRecord.Create("KEYWORDTOOLONG",Maybe<string>.Empty, Maybe<string>.Empty));
        It should_throw = () => exception.ShouldBeOfExactType<FitsFormatException>();
        static Exception exception;
        }

    [Subject(typeof(FitsHeaderRecord), "Validation")]
    internal class when_creating_a_header_record_with_only_a_keyword
        {
        Because of = () => record=FitsHeaderRecord.Create("SIMPLE",Maybe<string>.Empty, Maybe<string>.Empty);
        It should_contain_the_keyword = () => record.Keyword.ShouldEqual("SIMPLE");
        It should_not_have_a_value = () => record.Value.Any().ShouldBeFalse();
        It should_not_have_a_comment = () => record.Comment.Any().ShouldBeFalse();
        It should_format_correctly = () => record.Text.ShouldEqual(ExpectedText);
        static FitsHeaderRecord record;
        //                           0        1         2         3         4         5         6         7         8
        //                           12345678901234567890123456789012345678901234567890123456789012345678901234567890
        const string ExpectedText = "SIMPLE                                                                          ";
    }

    [Subject(typeof(FitsHeaderRecord), "Validation")]
    internal class when_creating_a_header_record_with_a_keyword_and_a_comment
        {
        Because of = () => record=FitsHeaderRecord.Create(ExpectedKeyword,Maybe<string>.Empty, ExpectedComment.AsMaybe());
        It should_contain_the_keyword = () => record.Keyword.ShouldEqual(ExpectedKeyword);
        It should_not_have_a_value = () => record.Value.Any().ShouldBeFalse();
        It should_contain_the_expected_comment = () => record.Comment.Single().ShouldEqual(ExpectedComment);
        It should_format_correctly = () => record.Text.ShouldEqual(ExpectedText);
        static FitsHeaderRecord record;
        //                           0        1         2         3         4         5         6         7         8
        //                           12345678901234567890123456789012345678901234567890123456789012345678901234567890
        const string ExpectedText = "SIMPLE  / Uses simple format                                                    ";
        const string ExpectedKeyword = "SIMPLE";
        const string ExpectedComment = "Uses simple format";
        }
}