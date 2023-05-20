// This file is part of the TA.Dpoint project
// 
// Copyright © 2015-2021 Tigra Astronomy, all rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so. The Software comes with no warranty of any kind.
// You make use of the Software entirely at your own risk and assume all liability arising from your use thereof.
// 
// File: Should.cs  Last modified: 2021-02-21@16:48 by Tim Long

using System;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.Observatory;

namespace TA.ObjectOrientedAstronomy.Specifications.TestHelpers
{
    /// <summary>Test assertion helpers</summary>
    static class Should
    {
        /// <summary>Verifies that the supplied action throws an exception.</summary>
        /// <param name="performTest">The test action.</param>
        /// <returns>The thrown exception.</returns>
        public static Exception Throw(Action performTest)
        {
            return Throw<Exception>(performTest);
        }

        /// <summary>
        /// Verifies that the supplied action throws an exception of the specified type
        /// (or any derived type).
        /// </summary>
        /// <typeparam name="TException">The type of the expected exception.</typeparam>
        /// <param name="performTest">The test action.</param>
        /// <returns>The thrown exception, if thrown correctly.</returns>
        /// <exception cref="SpecificationException">
        /// Thrown (causing test failure) if the action did not throw an exception,
        /// or if the test action threw an exception of an unexpected type.
        /// </exception>
        public static TException Throw<TException>(Action performTest) where TException : Exception
        {
            try
            {
                performTest();
            }
            catch (TException e)
            {
                return e;
            }
            catch (Exception e)
            {
                throw new SpecificationException($"Should throw {typeof(TException).Name} but threw {e.GetType().Name}",
                    e);
            }
            throw new SpecificationException($"Should throw {typeof(TException).Name}, but did not throw.");
        }

        public static void ShouldBeCloseTo(this DomePosition actual, DomePosition expected)
        {
            actual.Azimuth.Value.ShouldBeCloseTo(expected.Azimuth.Value, 0.0005);
            actual.Elevation.Value.ShouldBeCloseTo(expected.Elevation.Value, 0.0005);
        }
    }
}