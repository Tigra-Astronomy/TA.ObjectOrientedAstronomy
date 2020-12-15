using System;
using System.Diagnostics;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;

namespace TA.ObjectOrientedAstronomy.Specifications.FITS
    {
    [Subject(typeof(FitsReader))]
    internal class When_reading_a_fits_file : with_fits_reader
        {
        It should_read_the_file = () =>
            {
            var reader = ContextBuilder
                .FromEmbeddedResource(@"FITS-SIMPLE-Minimal-Monochrome-16x16-all-pixels-zero.fits")
                .Build();
            var hdu = reader.ReadPrimaryHeaderDataUnit();
            };
        }
    }