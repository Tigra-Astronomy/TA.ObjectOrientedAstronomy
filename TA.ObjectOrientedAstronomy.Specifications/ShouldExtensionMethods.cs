// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: ShouldExtensionMethods.cs  Last modified: 2015-11-21@16:45 by Tim Long

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Machine.Specifications;
using Machine.Specifications.Utility;
using Machine.Specifications.Utility.Internal;

namespace TA.ObjectOrientedAstronomy.Specifications
{
    public static class ShouldExtensionMethods
    {
        const string CurlyBraceSurround = "{{{0}}}";
        static readonly Regex EscapeNonFormatBraces = new Regex(
            "{([^\\d].*?)}", RegexOptions.Compiled | RegexOptions.Singleline);

        static SpecificationException NewException(string message, params object[] parameters)
        {
            if (parameters.Any())
            {
                return
                    new SpecificationException(
                        string.Format(
                            message.EnsureSafeFormat(),
                            parameters.Select(x => x.ToUsefulString()).Cast<object>().ToArray()));
            }
            return new SpecificationException(message);
        }

        public static void ShouldBeLikeObjectGraph(this object obj, object expected, double tolerance)
        {
            var exceptions = ShouldBeLikeObjectGraphInternal(obj, expected, "", tolerance).ToArray();

            if (exceptions.Any())
            {
                throw NewException(
                    exceptions.Select(e => e.Message)
                        .Aggregate("", (r, m) => r + m + Environment.NewLine + Environment.NewLine)
                        .TrimEnd());
            }
        }

        static IEnumerable<SpecificationException> ShouldBeLikeObjectGraphInternal(
            object obj, object expected, string nodeName, double tolerance)
        {
            ObjectGraphHelper.INode expectedNode = null;
            var nodeType = typeof(ObjectGraphHelper.LiteralNode);

            if (expected != null)
            {
                expectedNode = ObjectGraphHelper.GetGraph(expected);
                nodeType = expectedNode.GetType();
            }

            if (nodeType == typeof(ObjectGraphHelper.LiteralNode))
            {
                try
                {
                    if (obj is double)
                        ((double) obj).ShouldBeCloseTo((double) expected, tolerance);
                    else if (obj is float)
                        ((float) obj).ShouldBeCloseTo((float) expected, (float) tolerance);
                    else
                        obj.ShouldEqual(expected);
                }
                catch (SpecificationException ex)
                {
                    return new[]
                    {NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, ex.Message), nodeName)};
                }

                return Enumerable.Empty<SpecificationException>();
            }
            if (nodeType == typeof(ObjectGraphHelper.SequenceNode))
            {
                if (obj == null)
                {
                    var errorMessage = PrettyPrintingExtensions.FormatErrorMessage(null, expected);
                    return new[]
                    {NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName)};
                }
                var actualNode = ObjectGraphHelper.GetGraph(obj);
                if (actualNode.GetType() != typeof(ObjectGraphHelper.SequenceNode))
                {
                    var errorMessage = string.Format(
                        "  Expected: Array or Sequence{0}  But was:  {1}", Environment.NewLine, obj.GetType());
                    return new[]
                    {NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName)};
                }

                var expectedValues = ((ObjectGraphHelper.SequenceNode) expectedNode).ValueGetters;
                var actualValues = ((ObjectGraphHelper.SequenceNode) actualNode).ValueGetters;

                var expectedCount = expectedValues.Count();
                var actualCount = actualValues.Count();
                if (expectedCount != actualCount)
                {
                    var errorMessage = string.Format(
                        "  Expected: Sequence length of {1}{0}  But was:  {2}", Environment.NewLine, expectedCount,
                        actualCount);
                    return new[]
                    {NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName)};
                }

                return
                    Enumerable.Range(0, expectedCount)
                        .SelectMany(
                            i =>
                                ShouldBeLikeObjectGraphInternal(
                                    actualValues.ElementAt(i)(), expectedValues.ElementAt(i)(),
                                    string.Format("{0}[{1}]", nodeName, i), tolerance));
            }
            if (nodeType == typeof(ObjectGraphHelper.KeyValueNode))
            {
                var actualNode = ObjectGraphHelper.GetGraph(obj);
                if (actualNode.GetType() != typeof(ObjectGraphHelper.KeyValueNode))
                {
                    var errorMessage = string.Format(
                        "  Expected: Class{0}  But was:  {1}", Environment.NewLine, obj.GetType());
                    return new[]
                    {NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName)};
                }

                var expectedKeyValues = ((ObjectGraphHelper.KeyValueNode) expectedNode).KeyValues;
                var actualKeyValues = ((ObjectGraphHelper.KeyValueNode) actualNode).KeyValues;

                return expectedKeyValues.SelectMany(
                    kv =>
                    {
                        var fullNodeName = string.IsNullOrEmpty(nodeName)
                            ? kv.Name
                            : string.Format("{0}.{1}", nodeName, kv.Name);
                        var actualKeyValue = actualKeyValues.SingleOrDefault(k => k.Name == kv.Name);
                        if (actualKeyValue == null)
                        {
                            var errorMessage = string.Format(
                                "  Expected: {1}{0}  But was:  Not Defined", Environment.NewLine,
                                ToUsefulString(kv.ValueGetter()));
                            return new[]
                            {
                                NewException(
                                    string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), fullNodeName)
                            };
                        }

                        return ShouldBeLikeObjectGraphInternal(
                            actualKeyValue.ValueGetter(), kv.ValueGetter(), fullNodeName, tolerance);
                    });
            }
            throw new InvalidOperationException("Unknown node type");
        }

        internal static string ToUsefulString(this object obj)
        {
            if (obj == null)
                return "[null]";
            if (obj.GetType() == typeof(string))
                return "\"" + ((string) obj).Replace("\n", "\\n") + "\"";
            if (obj.GetType().IsValueType)
                return "[" + obj + "]";
            if (obj is IEnumerable)
            {
                var enumerable = ((IEnumerable) obj).Cast<object>();
                return (string) (object) obj.GetType() + (object) ":\r\n" + enumerable.EachToUsefulString();
            }
            var str1 = obj.ToString();
            if (str1 == null || str1.Trim() == "")
                return string.Format("{0}:[]", obj.GetType());
            var str2 = str1.Trim();
            if (str2.Contains("\n"))
                return string.Format("{1}:\r\n[\r\n{0}\r\n]", Tab(str2), obj.GetType());
            if (obj.GetType().ToString() == str2)
                return obj.GetType().ToString();
            return string.Format("{0}:[{1}]", obj.GetType(), str2);
        }

        internal static string EnsureSafeFormat(this string message)
        {
            return EscapeNonFormatBraces.Replace(message, match => string.Format("{{{0}}}", match.Groups[0]));
        }

        static string Tab(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            var strArray = str.Split(new string[2] {"\r\n", "\n"}, StringSplitOptions.None);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("  " + strArray[0]);
            foreach (var str1 in strArray.Skip(1))
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("  " + str1);
            }
            return stringBuilder.ToString();
        }
    }
}