// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.NavigationExtensions.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

using Prism.Navigation;

namespace HLI.Forms.Prism.Extensions
{
    /// <summary>
    ///     Extensions for Prism <see cref="NavigationParameters" />
    /// </summary>
    public static class NavigationExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Try to get the specified parameter as Guid
        /// </summary>
        /// <param name="parameters">Prism navigation parameters</param>
        /// <param name="parameterKey">The key name</param>
        /// <returns>Parsed object as Guid or <c>null</c></returns>
        public static Guid? GetParameterAsGuid(this NavigationParameters parameters, string parameterKey)
        {
            var match = parameters?.FirstOrDefault(pair => pair.Key == parameterKey);
            if (match?.Value != null)
            {
                Guid result;
                Guid.TryParse(match.Value.ToString(), out result);
                return result;
            }

            return null;
        }

        /// <summary>
        ///     Try to get the specified parameter as int
        /// </summary>
        /// <param name="parameters">Prism navigation parameters</param>
        /// <param name="parameterKey">The key name</param>
        /// <returns>Parsed object as int or <c>null</c></returns>
        public static int? GetParameterAsInt(this NavigationParameters parameters, string parameterKey)
        {
            var match = parameters?.FirstOrDefault(pair => pair.Key == parameterKey);
            if (match?.Value != null)
            {
                int result;
                int.TryParse(match.Value.ToString(), out result);
                return result;
            }

            return null;
        }

        /// <summary>
        ///     Try to get the specified parameter as supplied type
        /// </summary>
        /// <typeparam name="TParam">Expected type of the parameter</typeparam>
        /// <param name="parameters">Prism navigation parameters</param>
        /// <param name="parameterKey">The key name</param>
        /// <returns>Parsed object as <see cref="TParam" /> or <c>null</c></returns>
        public static TParam GetParameterAsType<TParam>(this NavigationParameters parameters, string parameterKey) where TParam : class, new()
        {
            var match = parameters?.FirstOrDefault(pair => pair.Key == parameterKey);
            return match?.Value as TParam;
        }

        /// <summary>
        ///     Joins strings to an uri format
        /// </summary>
        /// <param name="pages">Page names</param>
        /// <returns>Pages names joined by forward slash (/)</returns>
        public static string PageNamesToUri(params string[] pages)
        {
            var uri = pages.Aggregate((p1, p2) => $"{p1}/{p2}");
            return uri;
        }

        /// <summary>
        ///     Joins strings to an uri format
        /// </summary>
        /// <param name="service">this</param>
        /// <param name="pages">Page names</param>
        /// <returns>Pages names joined by forward slash (/)</returns>
        public static string PageNamesToUri(this INavigationService service, params string[] pages)
        {
            return PageNamesToUri(pages);
        }

        #endregion
    }
}