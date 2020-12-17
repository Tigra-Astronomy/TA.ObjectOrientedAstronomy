// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsStreamBuilder.cs  Last modified: 2016-09-29@01:30 by Tim Long

using System.IO;
using System.Reflection;
using System.Text;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;

namespace TA.ObjectOrientedAstronomy.Specifications.TestHelpers
    {
    class FitsStreamBuilder
        {
        public Stream FitsStream;

        public FitsStreamBuilder FromEmbeddedResource(string name)
            {
            var assembly = Assembly.GetExecutingAssembly();
            var fullyQualifiedName = $"{assembly.GetName().Name}.SampleData.Fits.{name}";
            FitsStream = assembly.GetManifestResourceStream(fullyQualifiedName);
            return this;
            }

        public FitsReader Build()
            {
            return new FitsReader(FitsStream);
            }

        public FitsStreamBuilder FromString(string data)
            {
            var buffer = Encoding.ASCII.GetBytes(data);
            var stream = new MemoryStream(buffer, writable: false);
            FitsStream = stream;
            return this;
            }
        }
    }