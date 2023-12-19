using System.Globalization;
using System.Reflection;
using System.Resources;

namespace OGA.AppSettings.Writeable.JSONConfig
{

    internal static class Resources
    {
        /// <summary>
        /// File path must be a non-empty string.
        /// </summary>
        internal static string Error_InvalidFilePath
        {
            get => "File path must be a non-empty string.";
        }

        /// <summary>
        /// File path must be a non-empty string.
        /// </summary>
        internal static string FormatError_InvalidFilePath()
            => "File path must be a non-empty string.";

        /// <summary>
        /// Could not parse the JSON file. Error on line number '{0}': '{1}'.
        /// </summary>
        internal static string Error_JSONParseError
        {
            get => "Could not parse the JSON file. Error on line number '{0}': '{1}'.";
        }

        /// <summary>
        /// Could not parse the JSON file. Error on line number '{0}': '{1}'.
        /// </summary>
        internal static string FormatError_JSONParseError(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, "Could not parse the JSON file. Error on line number '{0}': '{1}'.", p0, p1);

        /// <summary>
        /// A duplicate key '{0}' was found.
        /// </summary>
        internal static string Error_KeyIsDuplicated
        {
            get => "A duplicate key '{0}' was found.";
        }

        /// <summary>
        /// A duplicate key '{0}' was found.
        /// </summary>
        internal static string FormatError_KeyIsDuplicated(object p0)
            => string.Format(CultureInfo.CurrentCulture, "A duplicate key '{0}' was found.", p0);

        /// <summary>
        /// Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.
        /// </summary>
        internal static string Error_UnsupportedJSONToken
        {
            get => "Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.";
        }

        /// <summary>
        /// Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.
        /// </summary>
        internal static string FormatError_UnsupportedJSONToken(object p0, object p1, object p2, object p3)
            => string.Format(CultureInfo.CurrentCulture, "Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.", p0, p1, p2, p3);

        internal static string FormatError_UnsupportedJSONTokenType(object p0)
            => string.Format(CultureInfo.CurrentCulture, "Unsupported JSON token type '{0}' was found.", p0);
    }
}