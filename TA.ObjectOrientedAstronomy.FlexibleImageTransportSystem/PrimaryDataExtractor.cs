// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: PrimaryDataExtractor.cs  Last modified: 2016-10-28@11:57 by Daniel Van Noord

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
{
    public static class PrimaryDataExtractor
    {
        /// <summary>
        /// This function returns the array that corresponds to the hdu's raw data byte array.
        /// 
        /// Note: I believe that double works for every data type except for certain values of Int64. This will need to be fixed in the future. 
        /// 
        /// But for the moment I only intend to use this for working with bitmaps
        /// </summary>
        /// <param name="hdu"></param>
        /// <returns></returns>
        public static double[,] ExtractDataArray(FitsHeaderDataUnit hdu)
        {
            var pixelScale = hdu.Header.HeaderRecords.BindProperties<PixelScale>();
            var bitDepth = hdu.MandatoryKeywords.BitsPerPixel;
            // FITS section 3.3.2 lowest numbered axis varies most rapidly
            var xAxis = hdu.MandatoryKeywords.LengthOfAxis[0];
            var yAxis = hdu.MandatoryKeywords.LengthOfAxis[1];

            var returnDataArray = new double[xAxis, yAxis];
            var readPixel = GetPixelReader(hdu.MandatoryKeywords.BitsPerPixel);

            using (var inStream = new MemoryStream(hdu.RawData, writable: false))

            //MiscUtil is licensed under the Apache 2.0 License (see LibraryLicenses folder). This project is NOT a derivative work via the standard Apache 2.0 License exemption.
            using (var reader = new MiscUtil.IO.EndianBinaryReader(MiscUtil.Conversion.BigEndianBitConverter.Big, inStream))
                for (var y = 0; y < yAxis; y++)
                {
                    for (var x = 0; x < xAxis; x++)
                    {
                        var arrayValue = readPixel(reader);

                        returnDataArray[x, y] = pixelScale.ZeroOffset + pixelScale.Scale * arrayValue;
                    }
                }
            return returnDataArray;
        }

        //MiscUtil is licensed under the Apache 2.0 License (see LibraryLicenses folder). This project is NOT a derivative work via the standard Apache 2.0 License exemption.
        //This uses decimal because not all Int64 values can be stored in a double
        private static Func<MiscUtil.IO.EndianBinaryReader, double> GetPixelReader(int bitsPerPixel)
        {
            switch (bitsPerPixel)
            {
                case -32:
                    return reader => reader.ReadSingle();
                case -64:
                    return reader => reader.ReadDouble();
                case 8:
                    return reader => reader.ReadByte();
                case 16:
                    return reader => reader.ReadInt16();
                case 32:
                    return reader => reader.ReadInt32();
                case 64:
                    return reader => reader.ReadInt64();
                default:
                    throw new NotSupportedException(
                        $"BITPIX was {bitsPerPixel} which is not a recognized FITS format");
            }
        }

        private static short ReadIeeeDoublePrecision(BinaryReader reader)
        {
            return (short)reader.ReadDouble();
        }
    }
}
