// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: FitsDemoApp.cs  Last modified: 2020-12-15@10:58 by Tim Long

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem;

namespace TA.ObjectOrientedAstronomy.FitsSamples
    {
    internal static class FitsDemo
        {
        public static async Task PrintHeaderRecords(string inputFile)
            {
            var stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var reader = new FitsReader(stream);
            var header = await reader.ReadPrimaryHeader();
            header.HeaderRecords.ToList()
                .ForEach(Console.WriteLine);
            }

        public static async Task Copy(string source, string destination)
            {
            FitsHeaderDataUnit hdu;
            await using (var inStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            await using (var reader = new FitsReader(inStream))
                hdu = await reader.ReadPrimaryHeaderDataUnit();
            await using (var outStream = new FileStream(destination, FileMode.CreateNew, FileAccess.Write))
            await using (var writer = new FitsWriter(outStream))
                {
                hdu.Header.AppendHistory($"Duplicated by {Assembly.GetExecutingAssembly().GetName().Name}");
                await writer.WriteHeaderDataUnit(hdu);
                }

            }

        internal static async Task ShowInformation(string inputFile)
            {
            string fullFileName = Path.GetFullPath(inputFile);
            FitsHeaderDataUnit hdu;
            await using (var inStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
            await using (var reader = new FitsReader(inStream))
                hdu = await reader.ReadPrimaryHeaderDataUnit();
            var imageData = PrimaryDataExtractor.ExtractDataArray(hdu);
            double highestPixel = 0.0;
            double lowestPixel = double.MaxValue;
            int pixelCount = 0;
            double pixelSum = 0;
            // keep a list of how many times each value occurs. This is a list of Key-Value pairs,
            // where pixel.Key is the pixel value, and pixel.Value is the number of times that value is found.
            // Note: using keys of type double may be a naive implementation, but it appears to work.
            SortedList<double, int> valueFrequency = new System.Collections.Generic.SortedList<double, int>();

            foreach (var pixel in imageData)
                {
                ++pixelCount;
                pixelSum += pixel;
                highestPixel = System.Math.Max(highestPixel, pixel);
                lowestPixel = System.Math.Min(lowestPixel, pixel);
                // Keep a tally of the number of times each value occurs
                if (valueFrequency.ContainsKey(pixel))
                    {
                    valueFrequency[pixel] += 1;
                    }
                else
                    {
                    valueFrequency.Add(pixel, 1);
                    }
                }
            var pixelMean = pixelSum / pixelCount;
            var mostFrequentOccurence = valueFrequency.Values.Max();
            var mostFrequentValue = valueFrequency.First(p => p.Value == mostFrequentOccurence);
            var pixelMode = mostFrequentValue.Key;
            var distinctValues = valueFrequency.Keys.Count;

            // Find the median pixel value by traversing the value frequency list.
            // The list is sorted by value, so we just need to find the half-way point and that's the median value.
            // The "half way point" is half the pixel count.
            var medianPixelPosition = pixelCount / 2; // The index of the median value in a sorted list of pixel values
            int traverseCount = 0;
            double pixelMedian = double.NaN;
            foreach (var pixelValueFrequencyPair in valueFrequency)
                {
                pixelMedian = pixelValueFrequencyPair.Key; // the pixel value
                traverseCount += pixelValueFrequencyPair.Value; // The number of occurrences of this value
                // If we have traversed to or past the median pixel position, then we have found the median value.
                if (traverseCount >= medianPixelPosition)
                    break;
                }
            Console.WriteLine($"Information for: {fullFileName}");
            Console.WriteLine($"Pixel count:     {pixelCount}");
            Console.WriteLine($"Distinct values: {distinctValues}");
            Console.WriteLine($"Maximum pixel:   {highestPixel:F3}");
            Console.WriteLine($"Minimum pixel:   {lowestPixel:F3}");
            Console.WriteLine($"Sum of pixels:   {pixelSum:F3}");
            Console.WriteLine($"Mean of pixels:  {pixelMean:F3}");
            Console.WriteLine($"Median pixel:    {pixelMedian:F3}");
            Console.WriteLine($"Mode of pixels:  {pixelMode:F3} (occurs {valueFrequency[pixelMode]} times)");
            }
        }
    }