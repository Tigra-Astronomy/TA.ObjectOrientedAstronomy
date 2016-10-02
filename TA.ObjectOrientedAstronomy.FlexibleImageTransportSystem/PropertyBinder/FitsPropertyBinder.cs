// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsPropertyBinder.cs  Last modified: 2016-10-02@07:23 by Tim Long

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NLog;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder
    {
    /// <summary>
    ///     Provides a mechanism for binding values in a FITS header to properties in an arbitrary type, based on name
    ///     matching between the properties and the FITS header keywords. The default name matching strategy is to use
    ///     the uppercase property name. The default strategy can be modified somewhat by applying
    ///     <see cref="FitsKeywordAttribute" /> attributes to the properties being bound.
    /// </summary>
    public static class FitsPropertyBinder
        {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Given an instance of type <typeparamref name="T" /> and a collection of key/value pairs as a
        ///     <see cref="IKeywordLookup{String,String}" />, attempts to set each of the properties in the target to
        ///     its equivalent value from the <see cref="IKeywordLookup{TKey,TValue}" />, performing data conversion from
        ///     the source string to the target property type as necessary. Properties are matched with keywords
        ///     in one of several ways:
        ///     <list type="table">
        ///         <item>
        ///             <term>
        ///                 <see cref="FitsKeywordAttribute" />
        ///             </term>
        ///             <description>
        ///                 The property is examined using reflection to see if it has the <see cref="FitsKeywordAttribute" />
        ///                 attribute applied. If found,
        ///                 the name from the attribute is used as the data key name.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>Property name</term>
        ///             <description>The name of the target property is converted to upper case and used as the data key name.</description>
        ///         </item>
        ///         <item>
        ///             <term>Complex property</term>
        ///             <description>
        ///                 A complex property is any property implementing <see cref="IList" />. use of the
        ///                 <see cref="FitsKeywordAttribute" /> is mandatory in this case and multiple attributes may be applied.
        ///                 For each attribute, that data item will be retrieved from the
        ///                 <see cref="IKeywordLookup{TKey,TValue}" /> and added to the
        ///                 collection property.
        ///             </description>
        ///         </item>
        ///     </list>
        ///     When there is no matching key, the property is left uninitialized and will have its default value.
        /// </summary>
        /// <typeparam name="T">The type of the bound target object.</typeparam>
        /// <param name="source">
        ///     The <see cref="IKeywordLookup{String,String}" /> containing the keys and values to be bound, as
        ///     strings.
        /// </param>
        /// <returns>
        ///     A new instance of <typeparamref name="T" />, with its properties initialized from the values in the
        ///     source collection.
        /// </returns>
        public static T BindProperties<T>(this IKeywordLookup<string, string> source) where T : new()
            {
            log.Debug($"Performing property binding for type {typeof(T)}");
            var type = typeof(T);
            var targetProperties = type.GetProperties();
            var target = new T();
            object targetObject = target; // This ensures that if the traget type is a struct, then it is boxed.
            foreach (var property in targetProperties)
                {
                var targetType = property.PropertyType;
                if (typeof(IList).IsAssignableFrom(targetType))
                    {
                    BindCollectionProperty(targetObject, property, source);
                    continue;
                    }
                BindSimpleProperty(source, property, targetObject);
                }
            target = (T) targetObject; // This unboxes the target if it is a struct.
            log.Debug($"Property binding for {typeof(T)} complete");
            return target;
            }

        /// <summary>
        ///     Binds a simple property.
        /// </summary>
        /// <param name="source">The source dictionary containing the key/value pairs to be bound.</param>
        /// <param name="property">The property being bound.</param>
        /// <param name="target">The target object.</param>
        private static void BindSimpleProperty(IKeywordLookup<string, string> source, PropertyInfo property,
            object target)
            {
            try
                {
                var targetType = property.PropertyType;
                var sourceName = DetermineSimplePropertyDataKeyName(property);
                var convertedValue = GetConvertedValue(source, targetType, sourceName);
                log.Debug(
                    $"Binding simple property {property.Name}, effective name {sourceName}, value {convertedValue ?? "(null)"}");
                if (convertedValue != null)
                    property.SetValue(target, convertedValue, null);
                }
            catch (InvalidOperationException ex)
                {
                log.Error($"Type conversion failed for property {property.Name}");
                // We don't fail, the property is just left unset.
                }
            }

        /// <summary>
        ///     Gets the value of the specified Key in the source dictionary, and attempts to convert it to the target type.
        ///     Returns the converted type, of null if the conversion failed for any reason.
        /// </summary>
        /// <param name="source">The source dictionary of key/value pairs.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="sourceName">Key name of the source entry.</param>
        /// <returns>System.Object containing the converted type, or null if the conversion failed, or the key did not exist.</returns>
        private static object GetConvertedValue(IKeywordLookup<string, string> source, Type targetType,
            string sourceName)
            {
            var maybeValue = source[sourceName];
            if (maybeValue.None)
                return null;
            // Need to handle booleans as single characters, because FITS uses 'T' for true and 'F' for false.
            var targetIsBoolean = targetType == typeof(bool);
            if (targetIsBoolean)
                targetType = typeof(string);
            var convertedValue = ConvertStringToSimpleTypeOrNull(maybeValue.Single(), targetType);
            if (targetIsBoolean && convertedValue != null)
                convertedValue = ((string) convertedValue).StartsWith("T");
            return convertedValue;
            }

        /// <summary>
        ///     Deals with properties that implement <see cref="ICollection{T}" />.
        /// </summary>
        /// <param name="target">The target structure boxed.</param>
        /// <param name="property">The property.</param>
        /// <param name="source">The source.</param>
        private static void BindCollectionProperty(object target, PropertyInfo property,
            IKeywordLookup<string, string> source)
            {
            log.Debug($"Binding collection property {property.Name}");
            // Determine the generic type T of the ICollection<T> implementation.
            // Get a list of mapping attributes
            // For each mapping attribute, which represents the Key name
            //      Get the value from the source dictionary based on the mapped key name
            //      Add the value to the collection property
            var propertyType = property.PropertyType;
            var genericTypeArgs = propertyType.GetGenericArguments();
            var targetType = genericTypeArgs[0];
            // Need to create the collection, as the property will have a default value of null.
            var collection = Activator.CreateInstance(propertyType);
            property.SetValue(target, collection, null);
            var targetCollection = collection as IList;
            if (targetCollection == null)
                return; // No point in continuing if we can't save the results anywhere.
            var mappingAttributes = property.GetCustomAttributes(typeof(FitsKeywordAttribute), false);
            // LINQ comprehension query: cast the attributes so we can access the properties, then order by sequence number.
            var mappingAttributesInSequenceOrder =
                from attribute in mappingAttributes.Cast<FitsKeywordAttribute>()
                orderby attribute.Sequence ascending
                select attribute;
            foreach (var mappingAttribute in mappingAttributesInSequenceOrder)
                {
                var sourceKeyName = mappingAttribute.Keyword;
                var convertedItem = GetConvertedValue(source, targetType, sourceKeyName);
                if (convertedItem == null)
                    {
                    log.Debug($"Skipping value {sourceKeyName} (no value found)");
                    continue;
                    }
                log.Debug($"Adding collection value {sourceKeyName} = {convertedItem}");
                targetCollection.Add(convertedItem);
                }
            }

        /// <summary>
        ///     Determines the name of the data key for a given simple property.
        ///     This will be determined by the presence of a <see cref="FitsKeywordAttribute" /> attribute, if it is
        ///     present. If no attribute is present, then the name of the property itself will be used.
        /// </summary>
        /// <param name="property">The property being examined.</param>
        /// <returns>System.String.</returns>
        private static string DetermineSimplePropertyDataKeyName(PropertyInfo property)
            {
            var propertyNameUpperCase = property.Name.ToUpperInvariant();
            var attributes = property.GetCustomAttributes(typeof(FitsKeywordAttribute), false);
            if (attributes.Length == 0)
                return propertyNameUpperCase;
            var nameAttribute = attributes[0] as FitsKeywordAttribute;
            if (nameAttribute == null)
                return propertyNameUpperCase;
            return nameAttribute.Keyword;
            }

        /// <summary>
        ///     Converts a string-encoded value to a simple type or null.
        ///     This technique is borrowed in part from the ASP.net MVC ModelBinder subsystem.
        /// </summary>
        /// <param name="value">The string encoded value.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>A new System.Object containing the converted value, or null if conversion failed.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private static object ConvertStringToSimpleTypeOrNull(string value, Type destinationType)
            {
            if (value == null || destinationType.IsInstanceOfType(value))
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
                throw new InvalidOperationException(message, ex);
                }
            }
        }
    }