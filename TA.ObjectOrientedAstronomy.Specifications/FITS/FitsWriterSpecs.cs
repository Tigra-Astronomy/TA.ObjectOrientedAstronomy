using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.ObjectOrientedAstronomy.Specifications.TestHelpers;
using TA.Utils.Core;

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {
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
        It should_dispose_the_stream = () => outputStream.Disposed.ShouldBeTrue();
        It should_pad_the_block = () => outputStream.OutputByteCount.ShouldEqual(FitsFormat.FitsBlockLength);    
        }

    [Subject(typeof(FitsWriter), "block padding")]
    internal class when_flushing_a_partially_filled_header_block : with_fits_writer
        {
        Establish context = () => writer.currentBlock.Append(ValidFitsRecord);
        Because of = () => writer.FlushBlock().Wait(writerTimeout);
        It should_write_a_full_block = () => outputStream.Length.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_leave_the_current_block_empty = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_pad_with_spaces = () => outputStream.GetBuffer().Skip(ValidFitsRecord.Length).All(item=>item==FitsFormat.PadCharacter).ShouldBeTrue();
        }

    [Subject(typeof(FitsWriter), "block padding")]
    internal class when_flushing_a_partially_filled_data_block : with_fits_writer
        {
        Establish context = () =>
            {
            writer.currentBlock.Append(ValidFitsRecord);
            writer.EndHeader().Wait();
            writer.WriteDataByte(0xFF).Wait();
            };
        Because of = () => writer.FlushBlock().Wait(writerTimeout);
        It should_write_a_full_block = () => outputStream.Length.ShouldEqual(FitsFormat.FitsBlockLength * 2);
        It should_leave_the_current_block_empty = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_pad_with_zeroes = () => outputStream.OutputBytes.Skip(FitsFormat.FitsBlockLength+1).All(item=>item==0x0).ShouldBeTrue();
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
    internal class when_writing_the_end_header_record : with_fits_writer
        {
        Establish context = () => writer.WriteHeaderRecord(FitsHeaderRecord.Create("SIMPLE")).Wait();
        Because of = async () => writer.EndHeader().Wait();
        It should_flush_the_header_block = () => outputStream.Length.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_add_the_end_record = () => writer.headerRecords.HeaderRecords.Last().Keyword.ShouldEqual(FitsFormat.EndKeyword);
        It should_start_the_data_block = () => writer.state.ShouldEqual(WriterState.PrimaryData);
        static FitsPrimaryHduMandatoryKeywords mandatoryKeywords;
        }

    [Subject(typeof(FitsWriter), "header records")]
    internal class when_writing_a_header_record_within_a_data_block : with_fits_writer
        {
        Establish context = () => writer.WriteHeaderRecord(FitsHeaderRecord.Create("SIMPLE"))
            .ContinueWith((task) => writer.EndHeader())
            .Wait();
        Because of = () => Exception = Catch.Exception(() => { writer.WriteHeaderRecord(record).Wait(); })
            .First();
        It should_be_invalid_fits_format = () => Exception.ShouldBeOfExactType<FitsFormatException>();
        static Exception Exception;
        static FitsHeaderRecord record = FitsHeaderRecord.Create("INVALID");
        }

    // Full end-to-end test of creating a Simple FITS image as defined in SBFITSEXT 1.0
    [Subject(typeof(FitsWriter), "SBFITSEXT 1.0")]
    internal class when_round_tripping_a_fits_file : with_fits_writer
        {
        Establish context = () =>
            {
            fitsStreamBuilder = new FitsStreamBuilder();
            fitsReader = fitsStreamBuilder
                .FromEmbeddedResource(@"FITS-SIMPLE-Minimal-Monochrome-16x16-all-pizels-zero.fits")
                .Build();
            specimenPdu = fitsReader.ReadPrimaryHeaderDataUnit().WaitFoResult();
            var inputStream = fitsStreamBuilder.FitsStream;
            inputStream.Seek(0L, SeekOrigin.Begin);
            var length = inputStream.Length;
            expected = new byte[length];
            var bytesRead = inputStream.ReadAsync(expected, 0, (int) length)
                .WaitFoResult();
            };
        Because of = () =>
            {
            writer.WriteProtocolDataUnit(specimenPdu).Wait();
            writer.Close();
            actual = outputStream.OutputBytes;
            };
        It should_write_a_binary_equal_output_stream = () => actual.SequenceEqual(expected);
        static FitsReader fitsReader;
        static FitsStreamBuilder fitsStreamBuilder;
        static byte[] expected;
        static byte[] actual;
        static FitsHeaderDataUnit specimenPdu;
        }
    }