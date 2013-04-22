using System.Linq;
using System.Text;

namespace murray.common.extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends a NewLine after the format string. Safe formatting for null/empty pArgs.
        /// </summary>
        /// <returns></returns>
        public static void AppendLineFormat(this StringBuilder pStringBuilder, string pFormat, params object[] pArgs)
        {
            if (pArgs != null && pArgs.Any())
            {
                pStringBuilder.AppendFormat(pFormat, pArgs);
                pStringBuilder.AppendLine();
            }
            else
                pStringBuilder.AppendLine(pFormat);
        }
    }
}
