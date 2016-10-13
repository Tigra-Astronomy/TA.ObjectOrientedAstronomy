// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsPropertyBinder.cs  Last modified: 2016-10-13@00:59 by Tim Long

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using NLog;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder
    {
    /// <summary>
    ///     Provides a mechanism for binding values in FITS header metadata to properties in an arbitrary type, based on
    ///     name matching between the properties and the keywords in the metadata. The default name matching strategy is
    ///     to use the uppercase property name. The default strategy can be modified somewhat by applying
    ///     <see cref="FitsKeywordAttribute" /> attributes to the properties being bound.
    /// </summary>
    public static class FitsPropertyBinder
        {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Given an <see cref="IEnumerable{FitsHeaderRecord}" /> containing FITS header metadata,
        ///     attempts to create a data transfer object of type <typeparamref name="TOut" />
        ///     with its properties set to values obtained from the FITS metadata.
        ///     Properties are matched with keywords in the FITS header records in one of several ways:
        ///     <list type="number">
        ///         <item>
        ///             <term>
        ///                 using the <see cref="FitsKeywordAttribute.Keyword" /> value specified in a
        ///                 <see cref="FitsKeywordAttribute" /> attribute applied to the property.
        ///             </term>
        ///             <description>
        ///                 The property is examined using reflection to see if it has been decorated with one or more
        ///                 <see cref="FitsKeywordAttribute" /> attributes.
        ///                 If found, the Keyword value from each attribute is tried in turn to search the FITS header records
        ///                 for a match. The value from the first matching header record is used to set the property value.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>Property name</term>
        ///             <description>
        ///                 The uppercase name of the target property is used to search the FITS header records for a match. The
        ///                 value
        ///                 from the first matching header record is used to set the property value.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>Property implementing <see cref="IList" /> (complex property)</term>
        ///             <description>
        ///                 In the case of a 'complex' property that implements <see cref="IList" />, use of the
        ///                 <see cref="FitsKeywordAttribute" /> is mandatory. Multiple attributes may be applied, and for each
        ///                 attribute, values from all matching header records will be added to the collection property.
        ///             </description>
        ///         </item>
        ///     </list>
        ///     If no matching records are found, the property is left uninitialized and will have its default value.
        /// </summary>
        /// <typeparam name="TOut">The type of the target data transfer object.</typeparam>
        /// <param name="headerRecords">An enumerable collection of <see cref="FitsHeaderRecord" />.</param>
        /// <returns>
        ///     A new instance of <typeparamref name="TOut" />, with its properties initialized from the values in the
        ///     FITS header records.
        /// </returns>
        public static TOut BindProperties<TOut>(this IEnumerable<FitsHeaderRecord> headerRecords) where TOut : new()
            {
            var type = typeof(TOut);
            var targetProperties = type.GetProperties();
            var target = new TOut();
            object targetObject = target; // This ensures that if the target type is a struct, then it is boxed.
            Log.Info("Beginning property binding into type {0}", type.Name);
            foreach (var property in targetProperties)
                BindProperty(headerRecords, property, targetObject);
            target = (TOut) targetObject; // This unboxes the target if it is a struct.
            return target;
            }

        /// <summary>
        ///     Binds a single property.
        /// </summary>
        /// <param name="headerRecords">The source dictionary containing the key/value pairs to be bound.</param>
        /// <param name="property">The property being bound.</param>
        /// <param name="target">The target object.</param>
        private static void BindProperty(IEnumerable<FitsHeaderRecord> headerRecords, PropertyInfo property,
            object target)
            {
            Contract.Requires(headerRecords != null);
            Contract.Requires(property != null);
            Contract.Requires(target != null);
            try
                {
                var targetType = PropertyTypeOrUnderlyingCollectionType(property);
                var sourceKeywords = GetKeywordNamesFromPropertyNameOrAttributes(property);
                Log.Debug("Binding property {0} of type {1} using source keywords {2}",
                    property.Name,
                    targetType.Name,
                    sourceKeywords);
                var sourceRecords = from record in headerRecords
                                    where sourceKeywords.Contains(record.Keyword,
                                        StringComparer.InvariantCultureIgnoreCase)
                                    select record;
                var deserializedValues = DeserializedValues(sourceRecords, targetType).ToList();
                Log.Debug("Obtained the following values: {0}", deserializedValues);
                if (typeof(IList).IsAssignableFrom(property.PropertyType))
                    {
                    // Collection properties can accept the entire collection of values.
                    PopulateTargetCollection(target, property, deserializedValues);
                    return;
                    }
                PopulateTargetProperty(target, property, deserializedValues);
                }

            catch (InvalidOperationException)
                {
                // We don't fail, the property is just left unset. But we should log it as a warning.
                Log.Warn("Type conversion failed for {0} property", property.Name);
                }
            }

        /// <summary>
        ///     Gets the type of a 'simple' or collection property.
        ///     If the property is assignable to <c>IList</c> or <c>IList{T}</c>, then it is a collection.
        ///     For generic collections, the type is the generic type paramter of the collection.
        ///     For non-generic collections, the type is <c>object</c>.
        ///     For simple properties, the type is the property type.
        /// </summary>
        /// <param name="property">An instance of <see cref="PropertyInfo" />.</param>
        /// <returns>The underlying type of the property.</returns>
        private static Type PropertyTypeOrUnderlyingCollectionType(PropertyInfo property)
            {
            Contract.Requires(property != null);
            var propertyType = property.PropertyType;
            if (typeof(IList).IsAssignableFrom(propertyType))
                {
                var genericTypeArgs = propertyType.GetGenericArguments();
                if (genericTypeArgs.Length < 1)
                    return typeof(object);
                var targetType = genericTypeArgs[0];
                return targetType;
                }
            return property.PropertyType;
            }

        private static void PopulateTargetCollection(object target, PropertyInfo property,
            IEnumerable<object> convertedValues)
            {
            Contract.Requires(target != null);
            Contract.Requires(property != null);
            Contract.Requires(convertedValues != null);
            var propertyType = property.PropertyType;
            // Need to create the collection, as the property will have a default value of null.
            var collection = Activator.CreateInstance(propertyType);
            property.SetValue(target, collection, null);
            var targetCollection = collection as IList;
            if (targetCollection == null)
                {
                Log.Warn("Unable to populate collection property {0} because it could not be downcast to IList",
                    property.Name);
                return; // No point in continuing if we can't save the results anywhere.
                }
            foreach (var value in convertedValues)
                targetCollection.Add(value);
            }

        private static void PopulateTargetProperty(object target, PropertyInfo property,
            IEnumerable<object> convertedValues)
            {
            Contract.Requires(convertedValues != null);
            if (!convertedValues.Any())
                return; // If there are no values, make no attempt to set the target property.
            property.SetValue(target, convertedValues.First());
            }

        /// <summary>
        ///     Deserializes values from a collection of <see cref="FitsHeaderRecord" />
        ///     converting each value to type <paramref name="targetType" />
        ///     and returns the successfully converted values in an <see cref="IEnumerable{object}" />.
        /// </summary>
        /// <param name="headerRecords">The source dictionary of key/value pairs.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="keyword">Keyword to search for in the header records.</param>
        /// <returns>System.Object containing the converted type, or null if the conversion failed, or the key did not exist.</returns>
        private static IEnumerable<object> DeserializedValues(IEnumerable<FitsHeaderRecord> headerRecords,
            Type targetType)
            {
            Contract.Requires(headerRecords != null);
            var valueStrings = from record in headerRecords
                               select string.IsNullOrEmpty(record.Value) ? record.Comment : record.Value;
            return valueStrings.Select(valueString => DeserializeToType(valueString, targetType))
                .Where(deserializedValue => deserializedValue != null);
            }

        [CanBeNull]
        private static object DeserializeToType(string valueString, Type targetType)
            {
            Contract.Requires(!string.IsNullOrEmpty(valueString));
            Contract.Requires(targetType != null);
            // Need special handling for boolean, as FITS serializes them as 'T' or 'F'.
            if (targetType == typeof(bool))
                return valueString == FitsFormat.BooleanTrue; // Any other value is taken to mean False.
            // Special handling for strings, which FITS encodes with single quotes
            if (targetType == typeof(string))
                return valueString.Trim('\''); // Remove single quotes
            var convertedValue = ConvertStringToSimpleTypeOrNull(valueString, targetType);
            return convertedValue;
            }

        /// <summary>
        ///     Retrieves candidate FITS keywords for a given property.
        ///     This will be determined by the presence of one or more <see cref="FitsKeywordAttribute" /> attributes, if any are
        ///     present. If no attribute is present, then the name of the property itself will be used.
        /// </summary>
        /// <param name="property">The property being examined.</param>
        /// <returns>A collection of candidate keywords.</returns>
        private static IEnumerable<string> GetKeywordNamesFromPropertyNameOrAttributes(PropertyInfo property)
            {
            Contract.Requires(property != null);
            var attributes = property.GetCustomAttributes(typeof(FitsKeywordAttribute), false);
            if (attributes.Length == 0)
                return new List<string> {property.Name};
            return attributes.Cast<FitsKeywordAttribute>().OrderBy(p => p.Sequence).Select(p => p.Keyword);
            }

        /// <summary>
        ///     Converts a string-encoded value to a simple type or null.
        ///     This technique is borrowed in part from the ASP.net MVC ModelBinder subsystem.
        /// </summary>
        /// <param name="value">The string encoded value.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>A new System.Object containing the converted value, or null if conversion failed.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [CanBeNull]
        private static object ConvertStringToSimpleTypeOrNull(string value, Type destinationType)
            {
            Contract.Requires(value != null);
            Contract.Requires(destinationType != null);
            if (destinationType.IsInstanceOfType(value))
                return value;
            // In case of a Nullable object, we extract the underlying type and try to convert it.
            var underlyingType = Nullable.GetUnderlyingType(destinationType);
            if (underlyingType != null)
                destinationType = underlyingType;
            // look for a type converter that can convert from string to the specified type.
            var converter = TypeDescriptor.GetConverter(destinationType);
            var canConvertFrom = converter.CanConvertFrom(value.GetType());
            if (!canConvertFrom)
                converter = TypeDescriptor.GetConverter(value.GetType());
            var culture = CultureInfo.InvariantCulture;
            try
                {
                var convertedValue = canConvertFrom
                    ? converter.ConvertFrom(null /* context */, culture, value)
                    : converter.ConvertTo(null /* context */, culture, value, destinationType);
                return convertedValue;
                }
            catch (Exception ex)
                {
                var message = string.Format(CultureInfo.CurrentCulture,
                    "Conversion from string to type '{0}' failed. See the inner exception for more information.",
                    destinationType.FullName);
                Log.Error(ex, $"Conversion from string to type {destinationType.FullName} failed");
                throw new InvalidOperationException(message, ex);
                }
            }
        }
    }