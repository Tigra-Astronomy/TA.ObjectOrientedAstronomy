// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: BitmapConverter.cs  Last modified: 2016-10-17@21:26 by Tim Long

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
                        return SingleImagePlaneToRgbBitmap(hdu);
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

        //ToDo: Clean Code violation - method too long
        private static Bitmap SingleImagePlaneToRgbBitmap(FitsHeaderDataUnit hdu)
            {
            var pixelScale = hdu.Header.HeaderRecords.BindProperties<PixelScale>();
            var bitDepth = hdu.MandatoryKeywords.BitsPerPixel;
            // FITS section 3.3.2 lowest numbered axis varies most rapidly
            var xAxis = hdu.MandatoryKeywords.LengthOfAxis[0];
            var yAxis = hdu.MandatoryKeywords.LengthOfAxis[1];
            var readPixel = GetPixelReader(hdu.MandatoryKeywords.BitsPerPixel);

            var bytesPerPixel = 6; // 48 bits per pixel, 16, 16, 16 RGB
            var pixelData = new byte[yAxis * xAxis * bytesPerPixel];
            var blackPixel = ushort.MinValue;
            var whitePixel = ushort.MaxValue;
            using (var outStream = new MemoryStream(pixelData, writable: true))
            using (var writer = new BinaryWriter(outStream))
            using (var inStream = new MemoryStream(hdu.RawData, writable: false))
            using (var reader = new BinaryReader(inStream, Encoding.ASCII))
                for (var y = 0; y < yAxis; y++)
                    {
                    for (var x = 0; x < xAxis; x++)
                        {
                        /*
                         * Pixels are transformed from raw data to displayable pixels as follows:
                         * - The raw value is read from the file in a manner consistent with BITPIX.
                         * - The original physical sensor value (in ADUs) is recovered by applying BZERO and BSCALE. 16 and 32-bit integer array
                         *   values are always signed integers, whereas sensor data is typically unsigned. Therefore the data
                         *   is typically stored with BSCALE=1.0 and BZERO=-32767, but any values are possible.
                         * - The sensor value is constrained by the Black Point (CBLACK) and White Point (CWHITE).
                         * - The constrained value is "stretched" to cover the full luminosity spectrum of the display
                         * - finally the luminance value is cast to an Int16/short and written to each colour channel of the screen pixel.
                         */
                        var arrayValue = readPixel(reader);
                        var physicalValue = pixelScale.ZeroOffset + pixelScale.Scale * arrayValue;
                        var constrainedValue = physicalValue.Constrain(pixelScale.BlackPoint, pixelScale.WhitePoint);
                        // ToDo: A more sophisticated stretching algorithm is needed that takes account of CSTRETCH
                        var displayValue = constrainedValue.MapToRange(pixelScale.BlackPoint, pixelScale.WhitePoint,
                            short.MinValue, short.MaxValue);
                        var luminance = (short) displayValue;
                        writer.Write(luminance); // Red channel
                        writer.Write(luminance); // Green channel
                        writer.Write(luminance); // Blue channel
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