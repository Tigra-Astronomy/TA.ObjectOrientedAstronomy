// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: BitmapConverter.cs  Last modified: 2016-10-07@05:21 by Tim Long

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
    public static class BitmapConverter
        {
        /// <summary>
        ///     Converts the data array in this HDU to a Windows bitmap.
        /// </summary>
        /// <returns>Bitmap.</returns>
        /// <exception cref="NotSupportedException">Thrown if the image is neither greyscale or RGB</exception>
        public static Bitmap ToWindowsBitmap(this FitsHeaderDataUnit hdu)
            {
            var bitmapKind = DetermineBitmapKind(hdu);
            switch (bitmapKind)
                {
                    case ImageKind.Monocrome:
                        return ImageDataToRgbBitmap(hdu);
                    case ImageKind.RGB:
                        return ToRgbBitmap(hdu);
                    default:
                        throw new NotSupportedException(
                            "Unknown image type - only single layer (monochrome) and triple layer (RGB) images are supported");
                }
            }

        private static Bitmap ToRgbBitmap(FitsHeaderDataUnit hdu)
            {
            return null;
            }

        private static Bitmap ImageDataToRgbBitmap(FitsHeaderDataUnit hdu)
            {
            var pixelScale = hdu.Header.HeaderRecords.BindProperties<PixelScale>();
            var bitDepth = hdu.MandatoryKeywords.BitsPerPixel;
            // FITS section 3.3.2 lowest numbered axis varies most rapidly
            var xAxis = hdu.MandatoryKeywords.LengthOfAxis[0];
            var yAxis = hdu.MandatoryKeywords.LengthOfAxis[1];
            var readPixel = GetPixelReader(hdu.MandatoryKeywords.BitsPerPixel);

            var bytesPerPixel = 6; // 48 bits per pixel, 16, 16, 16 RGB
            var pixelData = new byte[yAxis * xAxis * bytesPerPixel];
            using (var outStream = new MemoryStream(pixelData, writable: true))
            using (var writer = new BinaryWriter(outStream))
            using (var inStream = new MemoryStream(hdu.RawData, writable: false))
            using (var reader = new BinaryReader(inStream, Encoding.ASCII))
                for (var y = 0; y < yAxis; y++)
                    {
                    for (var x = 0; x < xAxis; x++)
                        {
                        var physicalValue = readPixel(reader);
                        var scaledValue = pixelScale.ZeroOffset + pixelScale.Scale * physicalValue;
                        var luminosity = (short) scaledValue.Constrain(0, short.MaxValue);
                        writer.Write(luminosity); // Red channel
                        writer.Write(luminosity); // Green channel
                        writer.Write(luminosity); // Blue channel
                        }
                    }
            var bitmap = ByteToImage48bpp(xAxis, yAxis, pixelData);
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

        private static Func<BinaryReader, double> GetPixelReader(int bitsPerPixel)
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
            return (short) reader.ReadDouble();
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

        private class PixelScale
            {
            [FitsKeyword("BZERO")]
            [UsedImplicitly]
            public double ZeroOffset { get; set; }

            [FitsKeyword("BSCALE")]
            [UsedImplicitly]
            public double Scale { get; set; } = 1.0;

            [FitsKeyword("CBLACK")]
            [UsedImplicitly]
            public double BlackPoint { get; set; }

            [FitsKeyword("CWHITE")]
            [UsedImplicitly]
            public double WhitePoint { get; set; } = short.MaxValue;
            }
        }
    }