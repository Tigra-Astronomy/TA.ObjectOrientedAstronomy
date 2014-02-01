    
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


- Calculate the position in space of Earth using VSOP87 in both spherical and rectangular coordinates
- Calculate rectangular coordinates
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
- Add an overload of the VSOP87 computation to accept parameters for the target body, coordinate system and reference frame.
- Delete the obsolete hard-coded data.
- Download VSOP87 data from the web if needed (maybe?).



