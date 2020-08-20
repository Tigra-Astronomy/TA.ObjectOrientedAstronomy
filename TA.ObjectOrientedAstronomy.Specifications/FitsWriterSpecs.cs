using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using FakeItEasy;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;

namespace TA.ObjectOrientedAstronomy.Specifications
    {
    public class with_fits_writer_test_context
        {
        protected static readonly TimeSpan writerTimeout = TimeSpan.FromSeconds(1);
        static AggregateException exception;
        protected static LoggingStream outputStream;
        protected static FitsWriter writer;
        //                                        0        1         2         3         4         5         6         7         8
        protected const string ValidFitsRecord = "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
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
    public class When_writing_a_record_into_an_empty_block : with_fits_writer_test_context
        {
        Because of = () => writer.WriteRecord(FitsRecord.FromString(ValidFitsRecord)).Wait(writerTimeout);
        It should_reduce_the_available_block_space_by_the_record_length = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength - FitsFormat.FitsRecordLength);
        It should_write_the_record_into_the_block = () => writer.currentBlock.ToString().ShouldEqual(ValidFitsRecord);
        }

    [Subject(typeof(FitsWriter))]
    public class When_writing_a_record_that_fills_the_current_block : with_fits_writer_test_context
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
    public class When_closing_a_writer_containing_unflushed_data : with_fits_writer_test_context
        {
        Establish context = () => writer.currentBlock.Append(ValidFitsRecord);
        Because of = () => writer.Close();
        It should_clear_the_block = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_dispose_the_stream = () => Catch.Exception(()=>outputStream.WriteByte(0)).ShouldBeOfExactType<ObjectDisposedException>();
        It should_pad_the_block = () => outputStream.OutputByteCount.ShouldEqual(FitsFormat.FitsBlockLength);    
        }

    [Subject(typeof(FitsWriter), "block padding")]
    internal class when_flushing_a_partially_filled_block : with_fits_writer_test_context
        {
        IStream thing;
        Establish context = () => writer.currentBlock.Append(ValidFitsRecord);
        Because of = () => writer.FlushBlock().Wait(writerTimeout);
        It should_write_a_full_block = () => outputStream.Length.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_leave_the_current_block_empty = () => writer.BlockSpaceRemaining.ShouldEqual(FitsFormat.FitsBlockLength);
        It should_pad_with_zeroes = () => outputStream.GetBuffer().Skip(ValidFitsRecord.Length).All(item=>item==0).ShouldBeTrue();
        }
    }