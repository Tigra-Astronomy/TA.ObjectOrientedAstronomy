# Object Oriented Astronomy Libraries #

Please see the [Wiki pages on Github][wiki] for the latest project information.

## Fundamental Types ##

Classes for storing and manipulating various fundamental types, such as coordinates, time/date in various timescales.

## FITS Reader/Writer ##

A set of classes for reading, writing and manipulating **NASA Flexible Image Transport System** (FITS) files.

The aim is ultimately to be able to read and write all of the header and data array formats
that are set out in the latest
[FITS standard documentation](http://fits.gsfc.nasa.gov/fits_documentation.html "NASA FITS Documentation"). 
At the time of writing, this was
[FITS v4.0 July 2016 DRAFT](http://fits.gsfc.nasa.gov/fits_documentation.html "PDF Document").

Initially, we have concentrated on building a `FitsReader` class and  being able to read a
Primary HDU containing a single image data array ("SIF" of Single Image FITS format). We have also implemented methods for converting the FITS image data to a Windows Bitmap for display. A demo application, the **Simple FITS Viewer**, can display
a FITS image and the header data.

We plan to add the ability to modify and create new files and write them out to disk
with a `FitsWriter` class in a furture version.

Once we can both read and write SIF files, we'll add additional data array types and
the ability to work with multiple Extensions/HDUs.

## Orbit Engines ##

Currently, this library contains an implementation of the VSOP87a algorithm for computing the position of the major planets, the Sun and the Earth-Moon barycentre.

In time, we plan to provide additional orbit engines, such as VSOP2000 and SGP4 (Standard General Perterbation model for working with minor planet and satellite orbits)

### ToDo List ###

- How to represent the Sun and Earth?
- Download VSOP87 data directly from the web on demand instead of requiring local copies of the data.
- Create a caching strategy to avoid unnecessary downloads and so that each VSOP87 file is read in only once per session.
- Position of planets other than Earth
- Position of other planets relative to Earth (instead of Sun)
- Position of Planets in horizon-based coordinates
- Implement VSOP2000
- Implement SGP4

## License ##

This project is licensed under the [Tigra Astronomy MIT License][license].
Essentially, "Anyone can do anything at all with the software without restriction, 
but whatever happens it's not our fault".

[wiki]: https://github.com/Tigra-Astronomy/TA.ObjectOrientedAstronomy/wiki "Project Wiki"
[license]: https://tigra.mit-license.org/ "Tigra open source license"
