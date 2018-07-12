    
# FITS Reader/Writer #
A set of classes for reading, writing and manipulating **NASA Flexible Image Transport System** (FITS) files.

The aim is ultimately to be able to read and write all of the header and data array formats
that are set out in the latest
[FITS standard documentation](http://fits.gsfc.nasa.gov/fits_documentation.html "NASA FITS Documentation"). 
At the time of writing, this was
[FITS v4.0 July 2016 DRAFT](http://fits.gsfc.nasa.gov/fits_documentation.html "PDF Document").


Initially, we will concentrate on building a `FitsReader` class and  being able to read a
Primary HDU containing a single image data array ("SIF" of Single Image FITS format).

Next we'll add the ability to transform the raw data array into a `Bitmap` so it can be
more easily displayed and manipulated.

Then we will add the ability to modify and create new files and write them out to disk
with a `FitsWriter` class.

Once we can both read and write SIF files, we'll add additional data array types and
the ability to work with multiple Extensions/HDUs.

# Orbit Engine #

## Problem Statement ##

> Calculate the position in space of the Earth relative to the Sun for a given date, time
> Give the answer in both cartesian coordinates (X,Y,Z)
> and sperical coordinates (Latitude, Longitude and Radius).
> 
> Use a reference implementation to verify the results.

## Assumptions ##
- Epoch J2000 is assumed unless otherwise stated.
- We are going to use VSOP87 but may want to use other orbit engines in the future, so we will need to keep things loosely coupled.

# ToDo List #


- ~~Calculate the position in space of Earth using VSOP87 in both spherical and rectangular coordinates~~
- ~~Calculate rectangular coordinates~~
- ~~Calculate spherical coordinates~~
 - ~~Calculate the 6 latitude terms, L0 to L5 (L0 done)~~.
 - ~~Make ComputeL0 general purpose by passing in 'alpha' and the VSOP87 data~~.
 - ~~Compute latitude L in radians as (the sum of the series (Ln*rho^n)) / 100000000.0  
for n from 0 to 5~~.
 - ~~Calculate Longitude (B) in a similar way~~
 - ~~Calculate Radius (R) in AU in a similar way~~
- ~~Orbit Engine - method is hard coded for Earth. How do we get this to work independent of the body being computed?~~
- How to represent the Sun and Earth?
- ~~Read the VSOP87 data from a text file instead of having it hard coded.~~
- ~~Abstract the loading of data away from the user.~~
- ~~Delete the obsolete hard-coded data.~~
- Download VSOP87 data from the web if needed (maybe?).
- Caching strategy, so that each VSOP87 file is loaded only once.
- Position of planets other than Earth
- Position of other planets relative to Earth (instead of Sun)
- Position of Planets in horizon-based coordinates


# License #

This project is licensed under the [Tigra Astronomy MIT License](https://tigra.mit-license.org/ "MIT open source license").
Essentially, "Anyone can do anything at all with the software without restriction, 
but whatever happens it's not our fault".

