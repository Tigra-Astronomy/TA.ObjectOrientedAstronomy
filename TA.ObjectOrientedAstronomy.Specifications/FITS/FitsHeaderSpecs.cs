using System.Linq;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;
using TA.Utils.Core;
using static TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.FitsFormat;

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {
    [Subject(typeof(FitsHeader),"Append")]
    public class When_appending_a_record_to_a_header_with_an_end_record
        {
        Establish context = () =>
            {
            header = new FitsHeader();
            header.AppendHeaderRecord(FitsHeaderRecord.Create(EndKeyword));
            };

        Because of = () =>
            header.AppendRecord(
                FitsHeaderRecord.Create("COMMENT", Maybe<string>.Empty, "This is not the end".AsMaybe()));

        It should_preserve_the_end_record = () => header.HeaderRecords.Last().Keyword.ShouldEqual(EndKeyword);
        static FitsHeader header;
        }
    }