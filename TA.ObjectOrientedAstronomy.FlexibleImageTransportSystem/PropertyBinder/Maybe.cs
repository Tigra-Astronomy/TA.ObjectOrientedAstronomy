// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: Maybe.cs  Last modified: 2016-10-02@02:45 by Tim Long

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder
    {
    /// <summary>
    ///     Represents an object that may or may not have a value (strictly, a collection of zero or one elements). Use
    ///     LINQ expression <c>maybe.Any()</c> to determine if there is a value. Use LINQ expression
    ///     <c>maybe.Single()</c> to retrieve the value.
    /// </summary>
    /// <typeparam name="T">The type of the item in the collection.</typeparam>
    /// <remarks>
    ///     This type almost completely eliminates any need to return <c>null</c> or deal with possibly null references,
    ///     which makes code cleaner and more clearly expresses the intent of 'no value' versus 'error'.  The value of a
    ///     Maybe cannot be <c>null</c>, because <c>null</c> really means 'no value' and that is better expressed by
    ///     using
    ///     <see cref="Empty" />.
    /// </remarks>
    public class Maybe<T> : IEnumerable<T>
        {
        private readonly IEnumerable<T> values;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Maybe{T}" /> with no value.
        /// </summary>
        private Maybe()
            {
            values = new T[0];
            }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Maybe{T}" /> with a value.
        /// </summary>
        /// <param name="value">
        ///     The value of the maybe. Note that passing <c>null</c> as the value is valid and will not result in an empty Maybe.
        /// </param>
        private Maybe(T value)
            {
            values = new[] {value};
            }

        /// <summary>
        ///     Gets an instance that does not contain a value.
        /// </summary>
        /// <value>The empty instance.</value>
        public static Maybe<T> Empty { get; } = new Maybe<T>();

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Maybe{T}" /> is empty (has no value).
        /// </summary>
        /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
        public bool None => ReferenceEquals(this, Empty) || !values.Any();

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
        ///     collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
            {
            return values.GetEnumerator();
            }

        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
            {
            if (Equals(Empty))
                return "{no value}";
            return this.Single().ToString();
            }

        /// <summary>
        ///     Creates a <see cref="Maybe{T}" /> from a possibly null value. If the value is in fact <c>null</c>, then a
        ///     reference to the singleton <see cref="Maybe{T}.Empty" /> will result.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value being wrapped - this can usually be inferred by the compiler.
        /// </typeparam>
        /// <param name="value">The (potentially null) value being wrapped.</param>
        /// <returns>
        ///     Returns a populated <see cref="Maybe{T}" /> containing the value, or a reference to
        ///     <see cref="Maybe{T}.Empty" />.
        /// </returns>
        [NotNull]
        public static Maybe<TValue> FromValue<TValue>([CanBeNull] TValue value)
            {
            return value == null ? Maybe<TValue>.Empty : new Maybe<TValue>(value);
            }
        }
    }