// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: IKeywordLookup.cs  Last modified: 2016-10-02@02:13 by Tim Long

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder
    {
    /// <summary>
    ///     A collection that can perform keyword lookup and return a value.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the collection.</typeparam>
    /// <typeparam name="TValue">The type of the values in the collection.</typeparam>
    public interface IKeywordLookup<in TKey, TValue>
        {
        /// <summary>
        ///     Looks up the specified keyword and returns the value as a <see cref="Maybe{TValue}" />. If no match was
        ///     found, then <see cref="Maybe{T}.Empty" /> is returned.
        /// </summary>
        /// <param name="keyword">The keyword to be looked up.</param>
        /// <returns>
        ///     <see cref="Maybe{T}" /> containing the first matching value; or <see cref="Maybe{T}.Empty" />,
        /// </returns>
        Maybe<TValue> this[TKey keyword] { get; }
        }
    }