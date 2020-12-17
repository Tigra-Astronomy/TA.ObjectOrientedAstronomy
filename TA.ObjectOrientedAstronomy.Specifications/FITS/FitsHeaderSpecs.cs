using System.Linq;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.Utils.Core;
using static TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.FitsFormat;

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {
    [Subject(typeof(FitsHeader),"Append")]
    public class When_appending_a_record_to_a_header
        {
        Establish context = () =>
            {
            header = new FitsHeader();
            header.AppendHeaderRecord(FitsHeaderRecord.Create(EndKeyword));
            };

        Because of = () =>
            header.AppendHeaderRecord(
                FitsHeaderRecord.Create("COMMENT", Maybe<string>.Empty, "This is not the end".AsMaybe()));

        It should_go_at_the_end = () => header.HeaderRecords.Last().Keyword.ShouldEqual("COMMENT");
        static FitsHeader header;
        }

    [Subject(typeof(FitsHeader), "END records")]
    internal class when_reading_a_header
        {
        Establish context;
        Because of;
        It should_remove_all_end_records;
        }

    [Subject(typeof(FitsHeader), "END records")]
    internal class when_writing_a_header
        {
        Establish context;
        Because of;
        It should_append_a_single_end_record;
        }
    }