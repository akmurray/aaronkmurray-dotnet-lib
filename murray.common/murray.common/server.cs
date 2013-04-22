using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace murray.common
{
    public static class server
    {

        private static string _CurrentMachineName = null;

        /// <summary>
        /// Get the current MachineName (cached value)
        /// Gets the name of the server using HttpContext.Current.Server.MachineName if possible, or Environment.MachineName.
        /// Returns an empty string if no server name could be found.
        /// </summary>
        public static string GetMachineName(string pDefaultIfNotFound = "unknown")
        {
            if (_CurrentMachineName != null)
                return _CurrentMachineName;
            _CurrentMachineName = FetchMachineName();
            if (string.IsNullOrWhiteSpace(_CurrentMachineName))
                _CurrentMachineName = pDefaultIfNotFound;
            return _CurrentMachineName;
        }


        /// <summary>
        /// Returns null if not found or rights are limited and name is unable to be fetched
        /// http://connect.microsoft.com/VisualStudio/feedback/details/96799/httpserverutility-machinename-property-is-protected-differently-and-much-less-than-environment-machinename
        /// </summary>
        /// <returns></returns>
        public static string FetchMachineName()
        {
            string name = null;
            try
            {
                if (HttpContext.Current != null)
                    name = HttpContext.Current.Server.MachineName;
                if (name == null)
                    name = Environment.MachineName;
            }
            catch (Exception ex)
            {
                //ConsoleHelper.LogUnhandledException(ex);
            }
            return name;
        }

    }
}
