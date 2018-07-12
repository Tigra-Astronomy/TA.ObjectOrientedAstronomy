
# Individual Sample FITS files #

Most of the following sample FITS files were obtained from the MAST data archive. 

Telescope/Instrument   | Filename                 | Description
-----------------------|------------------        |------------
HST WFPC II            | WFPC2u5780205r_c0fx.fits | WFPC II 800 x 800 x 4 primary array data cube containing the 4 CCD images, plus a table extension containing world coordinate parameters. This sample file has been trimmed to 200 x 200 x 4 pixels to save disk space.
HST WFPC II            | WFPC2ASSNu5780205bx.fits | WFPC II 1600 x 1600 primary array mosaic constructed from the 4 individual CCD chips. (Image has been trimmed to 100 x 100 pixels to save disk space).
HST FOC                | FOCx38i0101t_c0f.fits    | FOC (1024 x 1024) primary array image, plus a table extension containing world coordinate parameters.
HST FOS                | FOSy19g0309t_c2f.fits    | FOS 2 x 2064 primary array spectrum containing the flux and wavelength arrays, plus a small table extension.
HST HRS                | HRSz0yd020fm_c2f.fits    | HRS 2000 x 4 primary array spectrum, plus a small table extension.
HST NICMOS             | NICMOSn4hk12010_mos.fits | NICMOS null primary array plus 5 image extensions (270 x 263) containing the science, error, data quality, samples, and time images 
HST FGS                | FGSf64y0106m_a1f.fits    | FGS file with a 89688 x 7 2-dimensional primary array and 1 table extension. The primary array contains a time series of 7 astrometric quantities.
Astro UIT              | UITfuv2582gc.fits        | Astro1 Ultraviolet Imaging Telescope (512 x 512) primary array image
IUE LWP                | IUElwp25637mxlo.fits     | IUE spectrum contained in vector columns of a binary table.
EUVE                   | EUVEngc4151imgx.fits     | EUVE sky image and 2D spectra, contained in multiple image extensions, with associated binary table extensions. The 1st image extension has been rebinned by a factor of 4 to save disk space.
Random Groups          | DDTSUVDATA.fits          | This example file illustrates the Random Groups FITS file format which has the keywords NAXIS1 = 0 and GROUPS = T. This format has been used almost exclusively for applications in radio interferometry and should not be used for applications outside of this field. It is recommended that new applications should use FITS binary tables to store this type of data. This example file comes from the Classic AIPS 'DDT' (Dirty Dozen Test) benchmarking suite. 

Samples obtained from [NASA/GSFC FITS Support Office][1], downloaded 2016-09-28

[1]: http://fits.gsfc.nasa.gov/fits_samples.html