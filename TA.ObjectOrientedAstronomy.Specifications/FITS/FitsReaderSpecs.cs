// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FlexibleImageTransportSystemSpecs.cs  Last modified: 2016-11-07@19:12 by Tim Long

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;
using TA.ObjectOrientedAstronomy.Specifications.Builders;
using TA.ObjectOrientedAstronomy.Specifications.TestHelpers;

// ReSharper disable ComplexConditionExpression

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {

    #region  Context base classes
    class with_fits_reader
        {
        Establish context = () => ContextBuilder = new FitsStreamBuilder();
        Cleanup after = () =>
            {
            FitsReader = null;
            ContextBuilder = null;
            };

        protected static FitsStreamBuilder ContextBuilder { get; private set; }

        protected static FitsReader FitsReader { get; set; }
        }
    #endregion

    [Subject(typeof(FitsReader), "read block")]
    class when_reading_a_block_from_a_stream_containing_less_than_a_block_of_data : with_fits_reader
        {
        Establish context =
            () => FitsReader = ContextBuilder.FromString(new string(' ', FitsFormat.FitsBlockLength - 1)).Build();
        Because of = () => Exception = Catch.Exception(() =>
            {
            var block = FitsReader.ReadBlock().WaitFoResult();
            var first = block.First();
            });
        It should_throw = () => Exception.ShouldBeOfExactType<FitsIncompleteBlockException>();
        static Exception Exception;
        }

    [Subject(typeof(FitsReader), "read block")]
    class when_reading_a_block_from_a_stream_containing_at_least_a_block_of_data : with_fits_reader
        {
        Establish context =
            () => FitsReader = ContextBuilder.FromString(new string(' ', FitsFormat.FitsBlockLength + 1)).Build();
        Because of = () => block = FitsReader.ReadBlock().WaitFoResult();
        It should_read_a_block_of_the_correct_size = () => block.Length.ShouldEqual(FitsFormat.FitsBlockLength);
        static byte[] block;
        }

    [Subject(typeof(FitsReader), "Read a record")]
    class when_reading_a_record_from_a_fits_file : with_fits_reader
        {
        Establish context = () => FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
        Because of = () => record = FitsReader.ReadRecord().WaitFoResult();
        It should_populate_the_record_with_exactly_80_characters = () => record.Text.ShouldEqual(ExpectedRecord);
        It should_read_an_entire_block =
            () => FitsReader.BlockBytesRemaining.ShouldEqual(FitsFormat.FitsBlockLength - FitsFormat.FitsRecordLength);
        static FitsRecord record;
        //   0        1         2         3         4         5         6         7         8
        //   12345678901234567890123456789012345678901234567890123456789012345678901234567890
        const string ExpectedRecord =
            "SIMPLE  =                    T / Standard FITS format                           ";
        }

    [Subject(typeof(FitsReader), "Read a record")]
    class when_reading_a_header_record_from_a_fits_file : with_fits_reader
        {
        Establish context = () => FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
        Because of = () => record = FitsReader.ReadHeaderRecord().WaitFoResult();
        It should_parse_the_expected_keyword = () => record.Keyword.ShouldEqual("SIMPLE");
        It should_parse_the_expected_value = () => record.Value.Single().ShouldEqual("T");
        It should_parse_the_expected_comment = () => record.Comment.Single().ShouldEqual("Standard FITS format");
        static FitsHeaderRecord record;
        //   0        1         2         3         4         5         6         7         8
        //   12345678901234567890123456789012345678901234567890123456789012345678901234567890
        const string ExpectedRecord =
            "SIMPLE  =                    T / Standard FITS format                           ";
        }

    [Subject(typeof(FitsReader), "Read a record")]
    class when_reading_a_header_record_from_a_fits_file_with_blank_records : with_fits_reader
        {
        Establish context = () =>
            {
            var fitsData = new FitsBuilder()
                .AppendRecord("SIMPLE  =                    T / Standard FITS format                           ")
                .AppendEmptyRecord()
                .AppendRecord("NAXIS   =                    0 / No data table                                  ")
                .AppendRecord("END                                                                             ")
                .Build();
            FitsReader = ContextBuilder.FromString(fitsData).Build();
            };
        Because of = () => header = FitsReader.ReadPrimaryHeader().WaitFoResult();
        It should_include_the_blank_record = () => header.HeaderRecords.Count().ShouldEqual(4);
        static FitsHeader header;
        //   0        1         2         3         4         5         6         7         8
        //   12345678901234567890123456789012345678901234567890123456789012345678901234567890
        const string ExpectedRecord =
            "SIMPLE  =                    T / Standard FITS format                           ";
        }

    [Subject(typeof(FitsReader), "Read primary HDU headers")]
    class when_reading_the_primary_header : with_fits_reader
        {
        Establish context = () => FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
        Because of = () => header = FitsReader.ReadPrimaryHeader().WaitFoResult();
        It should_read_the_expected_number_of_records = () => header.HeaderRecords.Count().ShouldEqual(164);
        It should_have_end_as_the_last_keyword =
            () => header.HeaderRecords.Last().Keyword.ShouldEqual(FitsFormat.EndKeyword);
        static FitsHeader header;
        }

    [Subject(typeof(FitsPropertyBinder))]
    class when_binding_to_primary_hdu_mandatory_keywords : with_fits_reader
        {
        Establish context = () =>
            {
            FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
            header = FitsReader.ReadPrimaryHeader().WaitFoResult();
            };
        Because of = () => MandatoryKeywords = header.HeaderRecords.BindProperties<FitsPrimaryHduMandatoryKeywords>();
        It should_be_a_simple_fits = () => MandatoryKeywords.Simple.ShouldBeTrue();
        It should_have_2_axes = () => MandatoryKeywords.NumberOfAxes.ShouldEqual(2);
        It should_populate_the_list_of_axes_with_2_entries = () => MandatoryKeywords.LengthOfAxis.Count.ShouldEqual(2);
        It should_have_the_expected_pixel_depth = () => MandatoryKeywords.BitsPerPixel.ShouldEqual(-32);
        static FitsHeader header;
        static FitsPrimaryHduMandatoryKeywords MandatoryKeywords;
        }

    [Subject(typeof(FitsPropertyBinder), "Multiple appearance keywords")]
    class when_binding_to_keywords_with_multiple_occurrences : with_fits_reader
        {
        Establish context = () =>
            {
            FitsReader = ContextBuilder.FromEmbeddedResource("WFPC2ASSNu5780205bx.fits").Build();
            header = FitsReader.ReadPrimaryHeader().WaitFoResult();
            };
        Because of = () => History = header.HeaderRecords.BindProperties<HistoryLog>();
        It should_retrieve_all_history_records = () => History.ModificationHistory.Count.ShouldBeGreaterThan(1);
        static FitsHeader header;
        static HistoryLog History;

        class HistoryLog
            {
            [FitsKeyword("HISTORY")]
            public List<string> ModificationHistory { get; set; }
            }
        }

    [Subject(typeof(FitsReader), "Read primary HDU")]
    class when_reading_the_complete_primary_header_data_unit : with_fits_reader
        {
        Establish context = () => FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
        Because of = () => hdu = FitsReader.ReadPrimaryHeaderDataUnit().WaitFoResult();
        It should_compute_the_correct_data_array_bit_length = () => hdu.DataArrayLengthBits.ShouldEqual(32 * 2 * 2064);
        It should_compute_the_correct_data_array_byte_length =
            () => hdu.DataArrayLengthBytes.ShouldEqual(hdu.DataArrayLengthBits / 8);
        It should_populate_the_data_array = () => hdu.RawData.Length.ShouldEqual(hdu.DataArrayLengthBytes);
        It should_set_the_data_type_to_image = () => hdu.DataType.ShouldEqual(FitsDataType.Image);
        static FitsHeaderDataUnit hdu;
        }

    [Subject(typeof(FitsHeaderRecord), "Commentary Keywords")]
    internal class when_parsing_a_history_record 
        {
        //                           00000000011111111112222222222333333333344444444445555555555666666666677777777778
        //                           12345678901234567890123456789012345678901234567890123456789012345678901234567890 
        const string sourceRecord = "HISTORY The cat sat on the mat                                                  ";
        Because of = () => header = FitsHeaderRecord.FromRecordText(sourceRecord);
        It should_start_the_comment_immediately_after_the_keyword = () => header.Comment.Single().ShouldStartWith("The");
        It should_have_no_value = () => header.Value.Any().ShouldBeFalse();
        static FitsHeaderRecord header;
        }

    [Subject(typeof(FitsHeaderRecord), "Commentary Keywords")]
    internal class when_parsing_a_comment_record 
        {
        //                           00000000011111111112222222222333333333344444444445555555555666666666677777777778
        //                           12345678901234567890123456789012345678901234567890123456789012345678901234567890 
        const string sourceRecord = "COMMENT      The cat sat on the mat                                             ";
        Because of = () => header = FitsHeaderRecord.FromRecordText(sourceRecord);
        It should_start_the_comment_immediately_after_the_keyword_and_not_remove_leading_white_space 
            = () => header.Comment.Single().ShouldStartWith("     The");
        It should_have_no_value = () => header.Value.Any().ShouldBeFalse();
        static FitsHeaderRecord header;
        }

    [Subject(typeof(FitsReader), "disposal")]
    internal class when_calling_a_disposed_reader : with_fits_reader
        {
        Establish context = () =>
            {
            FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
            FitsReader.Dispose();
            };
        Because of = () => exception = Catch.Exception(() => FitsReader.ReadPrimaryHeader().Wait()).First();
        It should_throw_object_disposed = () => exception.ShouldBeOfExactType<ObjectDisposedException>();
        static Exception exception;
        }
    }