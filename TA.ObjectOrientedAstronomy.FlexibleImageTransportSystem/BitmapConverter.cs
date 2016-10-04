// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: BitmapConverter.cs  Last modified: 2016-10-04@00:58 by Tim Long

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
                        return ToMonochromeBitmap(hdu);
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

        private static Bitmap ToMonochromeBitmap(FitsHeaderDataUnit hdu)
            {
            var bitDepth = hdu.MandatoryKeywords.BitsPerPixel;
            // FITS section 3.3.2 lowest numbered axis varies most rapidly
            var xAxis = hdu.MandatoryKeywords.LengthOfAxis[0];
            var yAxis = hdu.MandatoryKeywords.LengthOfAxis[1];
            var pixelReader = GetMonochromePixelReader(hdu.MandatoryKeywords.BitsPerPixel);
            var imageBytes = new byte[xAxis * yAxis * sizeof(short)];
            using (var outStream = new MemoryStream(imageBytes, writable: true))
            using (var writer = new BinaryWriter(outStream))
            using (var inStream = new MemoryStream(hdu.RawData, writable: false))
            using (var reader = new BinaryReader(inStream, Encoding.ASCII))
                for (var y = 0; y < yAxis; y++)
                    {
                    for (var x = 0; x < xAxis; x++)
                        {
                        writer.Write(pixelReader(reader));
                        }
                    }
            var bitmap = CreateMonochromeBitmapFromBytes(imageBytes, xAxis, yAxis);
            return bitmap;
            }

        private static Func<BinaryReader, short> GetMonochromePixelReader(int bitsPerPixel)
            {
            switch (bitsPerPixel)
                {
                    case -32:
                        return reader => (short) reader.ReadSingle();
                    case -64:
                        return reader => (short) reader.ReadDouble();
                    case 8:
                        return reader => (short) reader.ReadByte();
                    case 16:
                        return reader => reader.ReadInt16();
                    case 32:
                        return reader => (short) reader.ReadInt32();
                    case 64:
                        return reader => (short) reader.ReadInt64();
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

        private static Bitmap CreateMonochromeBitmapFromBytes(byte[] pixelValues, int width, int height)
            {
            //Create an image that will hold the image data
            var pic = new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
            //Get a reference to the images pixel data
            var dimension = new Rectangle(0, 0, pic.Width, pic.Height);
            var picData = pic.LockBits(dimension, ImageLockMode.ReadWrite, pic.PixelFormat);
            var pixelStartAddress = picData.Scan0;
            //Copy the pixel data into the bitmap structure
            Marshal.Copy(pixelValues, 0, pixelStartAddress, pixelValues.Length);
            pic.UnlockBits(picData);
            return pic;
            }
        }
    }