using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.ObjectOrientedAstronomy.Specifications.FundamentalTypes;
using TA.Utils.Core;

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

    [Subject(typeof(FitsWriter))]
    public class When_writing_a_record_into_an_empty_block : with_fits_writer
        {
        Because of = () => writer.WriteRecord(FitsRecord.FromString(ValidFitsRecord)).Wait(writerTimeout);
        It should_reduce_the_available_block_space_by_the_record_length = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength - FitsFormat.FitsRecordLength);
        It should_write_the_record_into_the_block = () => writer.currentBlock.ToString().ShouldEqual(ValidFitsRecord);
        }

    [Subject(typeof(FitsWriter))]
    public class When_writing_a_record_that_fills_the_current_block : with_fits_writer
        {
        Establish context = () =>
            {
            writer.currentBlock.Append('*', (FitsFormat.RecordsPerBlock - 1) * FitsFormat.FitsRecordLength);
            };
        Because of = () => writer.WriteRecord(FitsRecord.FromString(ValidFitsRecord)).Wait(writerTimeout);
        It should_clear_the_block = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_write_the_block_to_the_stream = () => outputStream.Length.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_write_the_block_text_to_the_stream_as_ascii = () =>
            outputStream.OutputBytes.TakeLast(FitsFormat.FitsRecordLength)
                .ShouldEqual(Encoding.ASCII.GetBytes(ValidFitsRecord));
    }

    [Subject(typeof(FitsWriter))]
    public class When_closing_a_writer_containing_unflushed_data : with_fits_writer
        {
        Establish context = () => writer.currentBlock.Append(ValidFitsRecord);
        Because of = () => writer.Close();
        It should_clear_the_block = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_dispose_the_stream = () => Catch.Exception(()=>outputStream.WriteByte(0)).ShouldBeOfExactType<ObjectDisposedException>();
        It should_pad_the_block = () => outputStream.OutputByteCount.ShouldEqual(FitsFormat.FitsBlockLength);    
        }

    [Subject(typeof(FitsWriter), "block padding")]
    internal class when_flushing_a_partially_filled_block : with_fits_writer
        {
        IStream thing;
        Establish context = () => writer.currentBlock.Append(ValidFitsRecord);
        Because of = () => writer.FlushBlock().Wait(writerTimeout);
        It should_write_a_full_block = () => outputStream.Length.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_leave_the_current_block_empty = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_pad_with_zeroes = () => outputStream.GetBuffer().Skip(ValidFitsRecord.Length).All(item=>item==0).ShouldBeTrue();
        }

    [Subject(typeof(FitsWriter), "header records")]
    internal class when_writing_a_header_record : with_fits_writer
        {
        static FitsHeaderRecord record = FitsHeaderRecord.FromRecordText(ValidPrimaryHeaderRecord);
        Because of = () => writer.WriteHeaderRecord(record);
        It should_write_the_record_into_the_block = () => writer.currentBlock.ToString().ShouldEqual(ValidPrimaryHeaderRecord);
        }

    [Subject(typeof(FitsWriter), "header records")]
    internal class when_writing_a_mandatory_record:with_fits_writer
        {
        Establish context;
        Because of = async () =>
            {
            await writer.WriteHeaderRecord(FitsHeaderRecord.Create("SIMPLE", Maybe<string>.From("T"),
                Maybe<string>.From("Single Image FITS")));
            await writer.WriteHeaderRecord(FitsHeaderRecord.Create("NAXIS", Maybe<string>.From("2"),
                Maybe<string>.From("2D data array")));
            await writer.WriteHeaderRecord(FitsHeaderRecord.Create("NAXIS1", Maybe<string>.From("16"),
                Maybe<string>.Empty));
            await writer.WriteHeaderRecord(FitsHeaderRecord.Create("NAXIS2", Maybe<string>.From("16"),
                Maybe<string>.Empty));
            await writer.WriteHeaderRecord(FitsHeaderRecord.Create("BITPIX", Maybe<string>.From("8"),
                Maybe<string>.From("Byte array")));
            };
        It should_update_the_tracking_header = () => writer.headerMandatoryValues.ShouldBeLike(ExpectedHeader);
            static FitsPrimaryHduMandatoryKeywords ExpectedHeader =
            new FitsPrimaryHduMandatoryKeywords()
                {
                BitsPerPixel = 8,
                Simple = true,
                NumberOfAxes = 2,
                LengthOfAxis = new List<int> {16, 16}
                };
            }

    [Subject(typeof(FitsWriter), "Single Image FITS")]
    internal class when_creating_a_single_image_header
        {
        Establish context = () => mandatoryKeywords = new FitsPrimaryHduMandatoryKeywords
            {
            Simple = true,      // This is a Single-Image FITS object
            BitsPerPixel = 8,   // 8 bits per pixel, unsigned integers (byte)
            NumberOfAxes = 2,   // 2-D, i.e. monochrome
            LengthOfAxis = new List<int>{16,16} // 16 pixels in X and Y.
            };
        Because of = () => actual = FitsHeader.CreateSingleImageFitsHeader(mandatoryKeywords).HeaderRecords.ToList();
        It should_be_simple = () => actual[0].Keyword.ShouldEqual("SIMPLE");
        It should_have_2_axes = () => actual.Single(p=>p.Keyword=="NAXIS").Value.Single().ShouldEqual("2") ;
        It should_have_16_pixels_in_x = () => actual.Single(p=>p.Keyword=="NAXIS0").Value.Single().ShouldEqual("16") ;
        It should_have_16_pixels_in_y = () => actual.Single(p=>p.Keyword=="NAXIS1").Value.Single().ShouldEqual("16") ;
        It should_have_8_bits_per_pixel = () => actual.Single(p=>p.Keyword=="BITPIX").Value.Single().ShouldEqual("8") ;
        static FitsPrimaryHduMandatoryKeywords mandatoryKeywords;
        static List<FitsHeaderRecord> actual;
        }

    [Subject(typeof(FitsWriter), "header records")]
    [Ignore("Pending implementation")]
    internal class when_writing_the_end_header_record : with_fits_writer
        {
        Establish context = () => writer.WriteHeaderRecord(FitsHeaderRecord.Create("SIMPLE")).Wait();
        Because of = async () => writer.EndHeader();
        It should_flush_the_header_block;
        It should_start_the_data_block;
        static FitsPrimaryHduMandatoryKeywords mandatoryKeywords;
        }

    [Subject(typeof(FitsWriter), "header records")]
    [Ignore("Pending implementation")]
    internal class when_writing_a_header_record_within_a_data_block : with_fits_writer
        {
        It should_throw;
        }
    }