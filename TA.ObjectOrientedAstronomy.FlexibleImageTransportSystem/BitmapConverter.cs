﻿// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2018 Tigra Astronomy, all rights reserved.
// 
// File: BitmapConverter.cs  Last modified: 2018-07-11@23:34 by Tim Long

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

//MiscUtil is licensed under the Apache 2.0 License (see LibraryLicenses folder). This project is NOT a derivative work via the standard Apache 2.0 License exemption.

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public static class BitmapConverter
        {
        /// <summary>
        ///     Converts the data array in this HDU to a Windows bitmap.
        /// </summary>
        /// <param name="hdu">The hdu.</param>
        /// <param name="histogram">
        ///     The histogram stretch configuration. Optional. If this parameter is provided, then it is used to stretch
        ///     the histogram of the displayed image. If not provided, then the FITS header will be examined for CBLACK
        ///     and CWHITE keywords and those will be used if present. If none of the above is found, then the default
        ///     is to use the minimum and maximum data values obtained from the data array.
        /// </param>
        /// <returns>Bitmap.</returns>
        /// <exception cref="NotSupportedException">Thrown if the image is neither greyscale or RGB</exception>
        public static Bitmap ToWindowsBitmap(this FitsHeaderDataUnit hdu, HistogramStretch histogram = null)
            {
            var bitmapKind = DetermineBitmapKind(hdu);
            switch (bitmapKind)
                {
                    case ImageKind.Monocrome:
                        return SingleImagePlaneToRgbBitmap(hdu, histogram);
                    case ImageKind.RGB:
                        throw new NotSupportedException(
                            "Currently only monochrome images are supported. Feel free to clone the code and add this feature.");
                    default:
                        throw new NotSupportedException(
                            "Unknown image type - only single layer (monochrome) and triple layer (RGB) images are supported");
                }
            }

        private static Bitmap TripleImagePlaneToRgbBitmap(FitsHeaderDataUnit hdu)
            {
            // ToDo
            return null;
            }

        private static Bitmap SingleImagePlaneToRgbBitmap(FitsHeaderDataUnit hdu,
            HistogramStretch histogramStretch = null)
            {
            // FITS section 3.3.2 lowest numbered axis varies most rapidly
            var xAxis = hdu.MandatoryKeywords.LengthOfAxis[0];
            var yAxis = hdu.MandatoryKeywords.LengthOfAxis[1];

            var bytesPerPixel = 3; // 24 bits per pixel, 8, 8, 8 RGB
            var pixelData = new byte[yAxis * xAxis * bytesPerPixel];

            var dataArray = PrimaryDataExtractor.ExtractDataArray(hdu);

            /*
             * Establish the range of values for the histogram stretch.
             * Use the values passed in via histogramStretch if they were provided.
             * Otherwise, if there is Blacnlpoint and WhitePoint information in the FITS header, use that.
             * Otherwise, using the Minimum and Maximum data values obtained from the data array.
             */

            var histogram = histogramStretch ?? hdu.Header.HeaderRecords.BindProperties<HistogramStretch>();
            if (histogramStretch == null)
                {
                if (!hdu.Header.HeaderRecords.Any(p => p.Keyword == "CBLACK"))
                    histogram.BlackPoint = dataArray.Cast<double>().Min();
                if (!hdu.Header.HeaderRecords.Any(p => p.Keyword == "CWHITE"))
                    histogram.WhitePoint = dataArray.Cast<double>().Max();
                }

            using (var outStream = new MemoryStream(pixelData, writable: true))
            using (var writer = new BinaryWriter(outStream))
                {
                for (var y = 0; y < yAxis; y++)
                for (var x = 0; x < xAxis; x++)
                    {
                    //ToDo: only linear histogram stretch currently supported. In future support other histogram shapes.
                    var displayValue = (byte) dataArray[x, y]
                        .Constrain(histogram.BlackPoint, histogram.WhitePoint)
                        .MapToRange(histogram.BlackPoint, histogram.WhitePoint, byte.MinValue, byte.MaxValue);
                    writer.Write(displayValue); // Red channel
                    writer.Write(displayValue); // Green channel
                    writer.Write(displayValue); // Blue channel
                    }
                }
            var bitmap = ByteToImage24bpp(xAxis, yAxis, pixelData);
            return bitmap;
            }

        private static Bitmap ByteToImage24bpp(int width, int height, byte[] pixels)
            {
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            byte bytesPerPixel = 3;
            var boundingRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(boundingRectangle,
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            /*
             * copy line by line, compensating for the difference between
             * width * bytesPerPixel and bitmapData.Stride
             */
            for (var y = 0; y < height; y++)
                Marshal.Copy(pixels, y * width * bytesPerPixel, bitmapData.Scan0 + bitmapData.Stride * y,
                    width * bytesPerPixel);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
            }

        private static Bitmap ByteToImage48bpp(int width, int height, byte[] pixels)
            {
            var bitmap = new Bitmap(width, height, PixelFormat.Format48bppRgb);
            byte bytesPerPixel = 6;
            var boundingRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(boundingRectangle,
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            /*
             * copy line by line, compensating for the difference between
             * width * bytesPerPixel and bitmapData.Stride
             */
            for (var y = 0; y < height; y++)
                Marshal.Copy(pixels, y * width * bytesPerPixel, bitmapData.Scan0 + bitmapData.Stride * y,
                    width * bytesPerPixel);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
            }

        private static ImageKind DetermineBitmapKind(FitsHeaderDataUnit hdu)
            {
            switch (hdu.MandatoryKeywords.NumberOfAxes)
                {
                    case 2:
                        return ImageKind.Monocrome;
                    case 3:
                        return hdu.MandatoryKeywords.LengthOfAxis[2] == 3 ? ImageKind.RGB : ImageKind.Unknown;
                    default:
                        return ImageKind.Unknown;
                }
            }
        }
    }