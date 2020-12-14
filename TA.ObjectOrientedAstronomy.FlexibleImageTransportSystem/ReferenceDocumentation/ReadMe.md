# Object Oriented FITS from Tigra Astronomy

## Introduction

The *[NASA Flexible Image Transport System][FITS]* (FITS) is a standard for encoding image data and other scientific observations.
FITS is the *de facto* file standard for astronomical observation data used in amateur and professional astronomy.

Object Oriented FITS (OOFITS) forms part of the Object Oriented Astronomy library.
It is a blank-slate implementation of the NASA FITS file format for .NET applications.
It is written in C# and is [open source][repo] under the [MIT License][license].
We have used modern object-oriented design practices and test-driven development.

### What's the Point?

Many existing FITS IO libraries began life as FORTRAN, were ported to C and/or C++ and then maybe had a thin .NET wrapper made for them.
While there is nothing inherently wrong with those libraries, non-object-oriented code does not usually transfer well to the object oriented world.
Ports of such code tends to produce clunky interfaces that force the user into a "last century programming paradigm".
Often they force the developer to focus on the implementation details, which is the opposite of what a good object-oriented design should do.
The point of OOFITS, in essence, is to produce a library that feels more "natural" for use in object oriented systems.
We hope it is the library that would have been designed if C# had been available from the outset, instead of FORTRAN 77.

OOFITS focuses on **Single Image FITS** (aka "SIF") files which are ubiquitous within amateur astronomy.
FITS files may also contain data tables, multiple images or other types of data.
While OOFITS is capable of reading and writing such data at a low level, it does not currently provide application-level features for manipulating them.
This is an area for possible future improvement, although the use of such features is vanishingly rare in amateur astronomy.

### Concepts

OOFITS aims to release the developer from having any knowledge of the FITS format.
The goal is to provide an API and utilities that "nudge the user into the pit of success" by making it easy to do the right thing.

#### Stream-based I/O

OOFITS builds on the .NET concept of Stream I/O and provides `FitsReader` and `FitsWriter` classes.
Thus, FITS data can be read and written to any type of stream (`FileStream`, `MemoryStream`, `NetworkStream`, etc.).
The highest and probably most useful level of access is the *FITS Header Data Unit*, represented by `FitsHeaderDataUnit`, which essentially contains an entire Single Image FITS file.
`FitsReader` can read an entire FITS file and return a `FitsHeaderDataUnit`.
A new file can be created by populating a `FitsHeaderDataUnit` and writing that out using `FitsWriter`.

#### Asynchronous

OOFITS was built from the ground up with asynchrony in mind.
I/O bound operations are a perfect use-case for asynchronous code and we make full use of C#'s `async` and `await` features.
Most of the methods of the `FitsReader` and `FitsWriter` class are asynchronous and return `Task<T>` so you can `await` them.

#### Property Binder

OOFITS provides a powerful property binding engine, which lets users easily populate plain CLR objects from FITS header metadata and vice versa.
This lets you define data transfer objects that are meaningful within your application and not have to worry about the FITS concepts of header records and keywords.
Conversions between .NET types and FITS header types are handled for you.
In many cases, FITS files can be read and written with no undesrtanding of the internal structure of a FITS file.

#### SBFITSEXT 1.0 Compliant

`SBFITSEXT` refers to a paper titled "[A Set of FITS Standard Extensions for Amateur Astronomical Processing Software Packages][SBFITSEXT]".
The paper was penned by Matt Longmire of Santa Barabara Instruments Group (SBIG) in March 2003, after a discussion between
various equipment vendors, software developers and interested users.
The discussion was initiated by Tim Long of Tigra Astronomy after he became frustrated at an inability to freely move FITS images between various software.
The [NASA FITS standard][FITS] describes the overall file format and some standard header records, but leaves open the exact content of most of the header metadata.
Therefore, prior to SBFITSEXT, the various software was all using different keywords and formats and there was no easy interoperability between software.
That is still true today, but most developers qickly adopted SBFITEXT so that at least there is a core of widely accepted header records that almost all applications understand and create.
Therefore is is usually at least possible to open, view, manipulate and save images across different software.

SBFITSEXT has not been updated since its original publication but remains in widespread use.

OOFITS therefore contains support for the SBFITSEXT standard and aims to help developers produce conforming image files.
`SbFitsExtHeader` is a data transfer object (DTO) with properties corresponding to header items described in SBFITSEXT.
`SbFitsExt` class has static helper methods for creating conforming image files.

Note: Despite the title, SBFITEXT does not actually describe "extensions" to the FITS standard.
Within FITS, an "extension" has a specific defined meaning that differs from the SBFITSEXT usage.
In that sense, the title of the paper was poorly chosen.
What SBFITSEXT actually proposes is a set agreed header ketwords and the the interpretation of their content.
It also lists the minimum headers that must be present in order for a FITS file to be considered *SBFITSEXT compliant*.
Thus, it provides a guideline for interoperability through common naming and does not describe any "FITS Extensions".

### Use Cases

Three main use cases were considered.

1. Create: generate a new Single Image FITS file from raw image data, ensuring that it conforms to SBFITEXT. Single-plane and multi-plane image data will be supported.
1. Read: Load a FITS file and extract its data array and header metadata into an easily usable form.
1. Update: Load a FITS file, manipulate its data aray and/or header metadata in some way and then re-save it, preserving as far as possible the order of existing header records.

#### Create

The ability to create FITS files is a work in progress.
It is anticipated that users will:

1. Create a data transfer object (DTO) containing the FITS header metadata, preferably using `SbFitsExtHeader` or a derived class.

1. Create a data array from raw image sensor data. We will need to provide support for:

   - Writing pixel values into the correct FITS format for 8-bit, 16-bit or floating point values.
   - Ensure that all data is stored in Big Endian format
   - Ensure that axes (image planes) are laid out correctly within the data array.

1. Create a `FitsHeaderDataUnit` object from the data array and the header metadata DTO created above.

1. Create a `FileStream` or other suitable stream.

1. Create a `FitsWriter` object using the stream instance.

1. await `FitsWriter.WriteHeaderDataUnit()`, passing in the instance of `FitsHeaderDataUnit`

#### Read

Reading a FITS file is fairly simple:

1. Open a stream over the FITS data.
1. Create a `FitsReader` from the stream.  
`var reader = new FitsReader(stream);`
1. Read the file into a `FitsHeaderDataUnit` object.  
`var hdu = reader.ReadHeaderDataUnit();`
1. Optionally, extract the header data into a data transfer object.  
  `var dto = new SbFitsExtHeader();`  
  `dto.BindProperties(hdu.Header);`
1. The raw data is in Big Endian order (which may or may not match your system's architecture) and values may be stored as 8-bit unsigned integer, 16-bit signed integer, 32-bit signed integer, 64-bit signed integer or IEEE-754 single precision (32-bit) or double precision (64-bit) floating point values.
This provides infinite capacity for mischief, but we have provided a helper method to sort it all out:   
`var imageData = PrimaryDataExtractor.ExtractDataArray(hdu);`

#### Update

OOFITS assumes that FITS files are *immutable* and does not directly provide a way to update a file 'in place'.
OOFITS understands only streams and assumes that all data manipulation will occur in memory.

In theory, an output stream could be opened in such a way that it replaces content in an existing file.
However, this is not recommended because of the way FITS files are formatted into records and blocks.
It is best to perform updates in-memory and write out a complete new file.

If it is necessary to update or overwite an existing file,
then we recommend that a new temporary file is first written, then the old file deleted and the temporary file renamed.
Note that OOFITS understands only streams (not files) and file management is explicitly outside the scope of OOFITS.

It is desirable to maintain the relative order of header records.
For example, `HISTORY` records are usually intended to contain a chronological log of operations carried out on the file so they must be kept together and in order.
Therefore, when reading header data, OOFITS maintains the records in an ordered collection within `FitsHeaderDataUnit`.
This collection may be amended in any way necessary before writing out to a stream using `FitsWriter`.

When 'updating' a FITS file, typically it is only the header metadata that is affected.
Typically, some analysis is performed on the data array (e.g. plate solving and adding world coordinates to the metadata).
In this case, headers can be manipulated using the ordered collection contained within `FitsHeaderDataUnit`.
When the HDU is written out, headers will be written in a deterministic order.

Updating is a work in progress.

Tim Long  
Tigra Astronomy  
December 2020

[FITS]: https://fits.gsfc.nasa.gov/fits_home.html "NASA Flexible Image Transport System"
[SBFITSEXT]: https://diffractionlimited.com/wp-content/uploads/2016/11/sbfitsext_1r0.pdf "A Set of FITS Standard Extensions for Amateur Astronomical Processing Software Packages"
[repo]: https://github.com/Tigra-Astronomy/TA.ObjectOrientedAstronomy "Object Oriented Astronomy from Tigra Astronomy"
[license]: https://tigra.mit-license.org "Tigra Astronomy Open Source Software License (MIT)"