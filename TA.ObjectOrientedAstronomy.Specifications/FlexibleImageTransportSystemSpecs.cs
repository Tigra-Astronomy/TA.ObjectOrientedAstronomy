﻿// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FlexibleImageTransportSystemSpecs.cs  Last modified: 2016-10-02@08:10 by Tim Long

using System;
using System.IO;
using System.Linq;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;
using TA.ObjectOrientedAstronomy.Specifications.Builders;

// ReSharper disable ComplexConditionExpression

namespace TA.ObjectOrientedAstronomy.Specifications
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
            () => FitsReader = ContextBuilder.FromString(new string(' ', Constants.FitsBlockLength - 1)).Build();
        Because of = () => Exception = Catch.Exception(() =>
            {
            var block = FitsReader.ReadBlock().WaitFoResult();
            var first = block.First();
            });
        It should_throw = () => Exception.ShouldBeOfExactType<IOException>();
        static Exception Exception;
        }

    [Subject(typeof(FitsReader), "read block")]
    class when_reading_a_block_from_a_stream_containing_at_least_a_block_of_data : with_fits_reader
        {
        Establish context =
            () => FitsReader = ContextBuilder.FromString(new string(' ', Constants.FitsBlockLength + 1)).Build();
        Because of = () => block = FitsReader.ReadBlock().WaitFoResult();
        It should_read_a_block_of_the_correct_size = () => block.Length.ShouldEqual(Constants.FitsBlockLength);
        static byte[] block;
        }

    [Subject(typeof(FitsReader), "Read a record")]
    class when_reading_a_record_from_a_fits_file : with_fits_reader
        {
        Establish context = () => FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
        Because of = () => record = FitsReader.ReadRecord().WaitFoResult();
        It should_populate_the_record_with_exactly_80_characters = () => record.Text.ShouldEqual(ExpectedRecord);
        It should_read_an_entire_block =
            () => FitsReader.BlockBytesRemaining.ShouldEqual(Constants.FitsBlockLength - Constants.FitsRecordLength);
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
        It should_parse_the_expected_value = () => record.Value.ShouldEqual("T");
        It should_parse_the_expected_comment = () => record.Comment.ShouldEqual("Standard FITS format");
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
            () => header.HeaderRecords.Last().Keyword.ShouldEqual(Constants.EndKeyword);
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
        Because of = () => MandatoryKeywords = header.BindProperties<FitsPrimaryHduMandatoryKeywords>();
        It should_be_a_simple_fits = () => MandatoryKeywords.Simple.ShouldBeTrue();
        It should_have_2_axes = () => MandatoryKeywords.NumberOfAxes.ShouldEqual(2);
        It should_populate_the_list_of_axes_with_2_entries = () => MandatoryKeywords.LengthOfAxis.Count.ShouldEqual(2);
        It should_have_the_expected_pixel_depth = () => MandatoryKeywords.BitsPerPixel.ShouldEqual(-32);
        static FitsHeader header;
        static FitsPrimaryHduMandatoryKeywords MandatoryKeywords;
        }

    [Subject(typeof(FitsReader))]
    class when_reading_the_complete_primary_header_data_unit : with_fits_reader
        {
        Establish context = () => FitsReader = ContextBuilder.FromEmbeddedResource("FOSy19g0309t_c2f.fits").Build();
        Because of = () => hdu = FitsReader.ReadPrimaryHeaderDataUnit().WaitFoResult();
        It should_compute_the_correct_data_array_bit_length = () => hdu.DataArrayLengthBits.ShouldEqual(32 * 2 * 2064);
        It should_compute_the_correct_data_array_byte_length = () => hdu.DataArrayLengthBytes.ShouldEqual(hdu.DataArrayLengthBits / 8);
        It should_populate_the_data_array = () => hdu.RawData.Length.ShouldEqual(hdu.DataArrayLengthBytes);
        It should_set_the_data_type_to_image = () => hdu.DataType.ShouldEqual(FitsDataType.Image);
        static FitsHeaderDataUnit hdu;
        }
    }