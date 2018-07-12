// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Networks., all rights reserved.
// 
// File: TaskExtensions.cs  Last modified: 2016-09-28@21:47 by Tim Long

using System;
using System.Linq;
using System.Threading.Tasks;

namespace TA.ObjectOrientedAstronomy.Specifications
    {
    public static class TaskExtensions
        {
        /// <summary>
        ///     Waits for an asynchronous operation to complete and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="asyncOperation">The asynchronous operation.</param>
        /// <returns>TResult.</returns>
        public static TResult WaitFoResult<TResult>(this Task<TResult> asyncOperation)
            {
            try
                {
                asyncOperation.Wait();
                }
            catch (AggregateException exceptions)
                {
                throw exceptions.InnerExceptions.First();
                }
            return asyncOperation.Result;
            }
        }
    }