using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {
    [Subject(typeof(FitsPropertySerializer))]
    [Ignore("Not implemented")]
    public class When_serializing_a_poco_with_no_special_attributes : with_fits_writer
        {
        }
    }