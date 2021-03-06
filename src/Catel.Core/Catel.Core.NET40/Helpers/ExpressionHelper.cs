﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Logging;

    /// <summary>
    /// Expression helper class that allows easy parsing of expressions.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Cache for the property names.
        /// </summary>
        private static readonly Dictionary<string, string> _propertyNameCache = new Dictionary<string, string>();

        /// <summary>
        /// Gets the name of the property from the expression.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="ignoreCache">if set to <c>true</c>, the cache will be ignored and the value will be determined again, even when the item is already in the cache.</param>
        /// <returns>
        /// The name of the property parsed from the expression or <c>null</c> if the property cannot be found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> propertyExpression, bool ignoreCache = false)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            string propertyExpressionAsString = propertyExpression.ToString();

            if (!ignoreCache)
            {
                if (_propertyNameCache.ContainsKey(propertyExpressionAsString))
                {
                    return _propertyNameCache[propertyExpressionAsString];
                }
            }

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                Log.Warning("Failed to retrieve the body of the expression (value is null)");
                return null;
            }

            string propertyName = body.Member.Name;
            if (!_propertyNameCache.ContainsKey(propertyExpressionAsString))
            {
                _propertyNameCache.Add(propertyExpressionAsString, propertyName);
            }

            return propertyName;
        }

        /// <summary>
        /// Gets the owner of the expression. For example if the expression <c>() => MyProperty</c>, the owner of the
        /// property will be returned.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The owner of the expression or <c>null</c> if the owner cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        public static object GetOwner<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                Log.Warning("Failed to retrieve the body of the expression (value is null)");
                return null;
            }

            var constantExpression = body.Expression as ConstantExpression;

            //if ((constantExpression == null) && (body.Expression is MemberExpression))
            //{
            //    constantExpression = ((MemberExpression) body.Expression).Expression as ConstantExpression;
            //}

            return (constantExpression != null) ? constantExpression.Value : null;
        }
    }
}
