﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueConverterBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data.Converters
{
    using System;
    using System.Globalization;

#if NETFX_CORE
    using global::Windows.UI.Xaml.Data;
#else
    using System.ComponentModel;
    using System.Windows.Data;
#endif

#if NET || SL5
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Base class for value converters which makes it compatible between .NET and WinRT.
    /// </summary>
#if NET || SL5
    public abstract class ValueConverterBase : MarkupExtension, IValueConverter
#else
    public abstract class ValueConverterBase : IValueConverter
#endif
    {
        /// <summary>
        /// Gets the current culture.
        /// </summary>
        /// <value>The current culture.</value>
        protected CultureInfo CurrentCulture { get; private set; }

        /// <summary>
        /// Gets or sets the linked value converter. This way it is possible to chain up several converters.
        /// </summary>
        /// <value>The link.</value>
        public IValueConverter Link { get; set; }

        /// <summary>
        /// Gets or sets an optional <see cref="System.Type"/> value to pass to the <see cref="Convert(object,System.Type,object)"/> method of the chained converter if the <see cref="Link"/>
        /// property is set.
        /// </summary>
        /// <remarks>
        /// Normally this value is ignored as it is in most implementations of <see cref="IValueConverter.Convert"/>.
        /// </remarks>
#if !NETFX_CORE
        [TypeConverter(typeof(StringToTypeConverter))]
#endif
        public Type OverrideType { get; set; }

        /// <summary>
        /// Gets or sets an optional <see cref="System.Type"/> value to pass to the <see cref="ConvertBack(object,System.Type,object)"/> method of this instance if the <see cref="Link"/>
        /// property is set.
        /// </summary>
        /// <remarks>
        /// Normally this value is ignored as it is in most implementations of <see cref="IValueConverter.ConvertBack"/>.
        /// </remarks>
#if !NETFX_CORE
        [TypeConverter(typeof(StringToTypeConverter))]
#endif
        public Type BackOverrideType { get; set; }

#if NETFX_CORE
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, new CultureInfo(language));
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertBack(value, targetType, parameter, new CultureInfo(language));
        }
#endif

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CurrentCulture = culture;

            object returnValue = value;

            if (Link != null)
            {
#if NETFX_CORE
                var cultureToUse = culture.Name;
#else
                var cultureToUse = culture;
#endif

                // Linked converter is set, this is not the last in the chain
                // call the linked converter, i.e. the next in the chain
                returnValue = Link.Convert(returnValue, OverrideType ?? targetType, parameter, cultureToUse);
            }

            returnValue = Convert(returnValue, targetType, parameter);

            return returnValue;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CurrentCulture = culture;

            object returnValue = value;

            // Call ConvertBack first because we are doing this in reverse order
            returnValue = ConvertBack(returnValue, targetType, parameter);

            if (Link != null)
            {
#if NETFX_CORE
                var cultureToUse = culture.Name;
#else
                var cultureToUse = culture;
#endif

                returnValue = Link.ConvertBack(returnValue, BackOverrideType ?? targetType, parameter, cultureToUse);
            }

            return returnValue;
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected abstract object Convert(object value, Type targetType, object parameter);

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the source object.</returns>
        /// <remarks>
        /// By default, this method returns <see cref="ConverterHelper.DoNothingBindingValue"/>. This method only has
        /// to be overridden when it is actually used.
        /// </remarks>
        protected virtual object ConvertBack(object value, Type targetType, object parameter)
        {
            return ConverterHelper.DoNothingBindingValue;
        }

#if NET || SL5
        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
#endif
    }
}