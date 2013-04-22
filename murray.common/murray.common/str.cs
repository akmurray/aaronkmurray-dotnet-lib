using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace murray.common
{
    public static class str
    {
        public static bool IsEmpty(HttpCookie source)
        {
            if (source == null)
                return true;
            return IsEmpty(source.Value);
        }

        /// <summary>
        /// Check for nulls as well as empty spaces
        /// </summary>
        public static bool IsEmpty(object source)
        {
            if (source == null)
                return true;
            return string.IsNullOrWhiteSpace(ToString(source));
        }


        /// <summary>
        /// Postpends a string with a character up to pPlaces number of char places
        /// </summary>
        /// <param name="pValue">value to postpend to (right pad)</param>
        /// <param name="pPlaces">min number of places in the returned string</param>
        /// <param name="pAppendedChar">char to repeat at the end of pValue</param>
        /// <returns>Prepend("BALL", 5, '8') returns "BALL8". Prepend("BALL", 7, '8') returns "BALL888".</returns>
        public static string Postpend(string pValue, int pPlaces, char pAppendedChar)
        {
            if (pValue == null)
                pValue = string.Empty;
            while (pValue.Length < pPlaces)
                pValue = pValue + pAppendedChar;
            return pValue;
        }
        /// <summary>
        /// Postpends a string with a character up to pPlaces number of char places
        /// </summary>
        /// <param name="pValue">value to postpend to (right pad)</param>
        /// <param name="pPlaces">min number of places in the returned string</param>
        /// <param name="pAppendedString">string to repeat at the end of pValue</param>
        /// <returns>Prepend("BALL", 5, '88') returns "BALL88". Prepend("BALL", 7, '8') returns "BALL8888".</returns>
        public static string Postpend(string pValue, int pPlaces, string pAppendedString)
        {
            if (pValue == null)
                pValue = string.Empty;
            while (pValue.Length < pPlaces)
                pValue = pValue + pAppendedString;
            return pValue;
        }


        /// <summary>
        /// Get a formatted & trimmed string safely. Will return empty string instead of null
        /// </summary>
        /// <param name="pFormat"></param>
        /// <param name="pArgs"></param>
        /// <returns></returns>
        public static string FormatSafe(string pFormat, params object[] pArgs)
        {
            if (string.IsNullOrWhiteSpace(pFormat))
                return string.Empty;
            if (pArgs != null && pArgs.Length > 0)
                pFormat = string.Format(pFormat, pArgs); //only call string format if we have args - else it'll throw an exception
            pFormat = pFormat.Trim();
            return pFormat;
        }

        /// <summary>
        /// Safely converts possibly null string to a string. Empty string if necessary
        /// </summary>
        public static string ToString(object value, string pValueIfNull = "", string pValueIfCorrupt = "")
        {
            try
            {
                if (value == null)
                    return pValueIfNull;
                if (value is DateTime || value is string)
                    return value.ToString();

                return value.ToString();
            }
            catch (Exception ex)
            {
                console.LogCaughtException(ex);
                return pValueIfCorrupt;
            }
        }

        /// <summary>
        /// Used when you want to get a bool from a string that may have T/F, Y/N, etc
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pValueIfStringIsNullOrEmpty"></param>
        /// <returns>True for: "Y", "YES", "ON", "1", "T", "TRUE"</returns>
        public static bool ToBool(object pString, bool pValueIfStringIsNullOrEmpty = false)
        {
            var s = ToString(pString).Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(s))
                return pValueIfStringIsNullOrEmpty;

            return s == "1"
                || s == "T"
                || s == "Y"
                || s == "TRUE"
                || s == "YES"
                || s == "YEP"
                || s == "YEA"
                || s == "YEAH"
                || s == "ON"
                || s == "FOSHO"
            ;
        }


        /// <summary>
        /// Converts value obj to integer. Trims all whitespace before checking null/empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pValueIfStringIsNullOrEmpty"></param>
        /// <param name="pValueIfStringIsCorrupt"></param>
        /// <returns></returns>
        public static int ToInt(object value, int pValueIfStringIsNullOrEmpty = 0, int pValueIfStringIsCorrupt = 0)
        {
            if (value == null) //null check first...because we trim the value next
                return pValueIfStringIsNullOrEmpty;
            if (IsEmpty(value))
                return pValueIfStringIsNullOrEmpty;

            long parsedLong = pValueIfStringIsCorrupt;
            try
            {
                var stringValue = value.ToString().Trim().Replace(",", ""); //strip commas from strings like "1,000,000"

                double parsedDouble;
                if (!long.TryParse(stringValue, out parsedLong))
                    if (double.TryParse(stringValue, out parsedDouble))
                        parsedLong = Convert.ToInt64(parsedDouble);

                if (parsedLong == 0 && stringValue != "0")
                    return pValueIfStringIsCorrupt; //then the out value of TryParse was zero, but we didn't pass a zero in

            }
            catch (Exception ex)
            {
                //ConsoleHelper.LogCaughtException(ex);
                //they might have passed in a number like 1234578153891356815318461
                parsedLong = pValueIfStringIsCorrupt;
            }

            if (parsedLong > int.MaxValue)
                return int.MaxValue; //too big

            return (int)parsedLong;
        }


        /// <summary>
        /// Converts value obj to 64 bit integer. Trims all whitespace before checking null/empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pValueIfStringIsNullOrEmpty"></param>
        /// <param name="pValueIfStringIsCorrupt"></param>
        /// <returns></returns>
        public static long ToInt64(object value, long pValueIfStringIsNullOrEmpty = 0, long pValueIfStringIsCorrupt = 0)
        {
            if (value == null) //null check first...because we trim the value next
                return pValueIfStringIsNullOrEmpty;
            if (IsEmpty(value))
                return pValueIfStringIsNullOrEmpty;

            long parsedLong = pValueIfStringIsCorrupt;
            try
            {
                var stringValue = value.ToString().Trim().Replace(",", ""); //strip commas from strings like "1,000,000"

                double parsedDouble;
                if (!long.TryParse(stringValue, out parsedLong))
                    if (double.TryParse(stringValue, out parsedDouble))
                        parsedLong = Convert.ToInt64(parsedDouble);

                if (parsedLong == 0 && stringValue != "0")
                    return pValueIfStringIsCorrupt; //then the out value of TryParse was zero, but we didn't pass a zero in

            }
            catch (Exception ex)
            {
                //ConsoleHelper.LogCaughtException(ex);
                //they might have passed in a number like 1234578153891356815318461456789123432
                parsedLong = pValueIfStringIsCorrupt;
            }

            if (parsedLong > long.MaxValue)
                return long.MaxValue; //too big

            return parsedLong;
        }
    }
}
