using System;
using System.ServiceProcess;

namespace murray.common.winservice
{
    /// <summary>
    /// Helpful methods related to Windows Services
    /// </summary>
    public static class WinServiceHelper
    {
        /// <summary>
        /// Is the service in any of the running states (including StopPending).
        /// Essential !Stopped
        /// </summary>
        /// <param name="pServiceName"></param>
        /// <param name="pMachineName"></param>
        /// <returns></returns>
        public static bool IsServiceRunning(string pServiceName, string pMachineName = null)
        {
            return !IsServiceStopped(pServiceName, pMachineName);
        }

        /// <summary>
        /// Does this status count as the service running (basically anything besides Stopped)
        /// </summary>
        /// <param name="pStatus"></param>
        /// <returns></returns>
        public static bool IsServiceRunning(ServiceControllerStatus pStatus)
        {
            return pStatus != ServiceControllerStatus.Stopped;
        }

        /// <summary>
        /// Is this service status "Stopped"
        /// </summary>
        /// <param name="pServiceName"></param>
        /// <param name="pMachineName"></param>
        /// <returns></returns>
        public static bool IsServiceStopped(string pServiceName, string pMachineName = null)
        {
            return IsServiceStopped(GetServiceStatus(pServiceName));
        }

        /// <summary>
        /// Is this service status "Stopped"
        /// </summary>
        /// <param name="pStatus"></param>
        /// <returns></returns>
        public static bool IsServiceStopped(ServiceControllerStatus pStatus)
        {
            return pStatus == ServiceControllerStatus.Stopped;
        }

        /// <summary>
        /// Given a service name and optional machine name, get an instance of a service controller for it
        /// </summary>
        /// <param name="pServiceName"></param>
        /// <param name="pMachineName"></param>
        /// <returns></returns>
        public static ServiceController GetServiceController(string pServiceName, string pMachineName = null)
        {
            ServiceController service;
            if (string.IsNullOrWhiteSpace(pMachineName))
                service = new ServiceController(pServiceName);
            else
                service = new ServiceController(pServiceName, pMachineName);
            return service;
        }

        /// <summary>
        /// ServiceControllerStatus
        /// Value	        Meaning
        /// ContinuePending	The service has been paused and is about to continue.
        /// Paused	        The service is paused.
        /// PausePending	The service is in the process of pausing.
        /// Running	        The service is running.
        /// StartPending	The service is in the process of starting.
        /// Stopped	        The service is not running.
        /// StopPending	    The service is in the process of stopping.
        /// 
        /// http://www.blackwasp.co.uk/DetectServiceStatus.aspx
        /// 
        /// </summary>
        /// <param name="pServiceName"></param>
        /// <param name="pMachineName"></param>
        /// <returns></returns>
        public static ServiceControllerStatus GetServiceStatus(string pServiceName, string pMachineName = null)
        {
            return GetServiceStatus(GetServiceController(pServiceName, pMachineName));
        }

        public static ServiceControllerStatus GetServiceStatus(ServiceController pServiceController)
        {
            try
            {
                return pServiceController.Status;
            }
            catch (Exception ex)
            {
                //ConsoleHelper.LogCaughtException(ex);
            }
            return ServiceControllerStatus.Stopped;
        }

        public static bool StartService(string pServiceName, string pMachineName = null, int pTimeoutMilliseconds = 90000)
        {
            return StartService(GetServiceController(pServiceName, pMachineName), pTimeoutMilliseconds);
        }

        public static bool StartService(ServiceController pServiceController, int pTimeoutMilliseconds = 90000)
        {
            if (pServiceController == null)
                return false;
            pTimeoutMilliseconds = Math.Max(pTimeoutMilliseconds, 1000);

            try
            {
                pServiceController.Start();
                pServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(pTimeoutMilliseconds));
                return pServiceController.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

        public static bool StopService(string pServiceName, string pMachineName = null, int pTimeoutMilliseconds = 90000)
        {
            return StopService(GetServiceController(pServiceName, pMachineName), pTimeoutMilliseconds);
        }

        public static bool StopService(ServiceController pServiceController, int pTimeoutMilliseconds = 90000)
        {
            if (pServiceController == null)
                return false;
            pTimeoutMilliseconds = Math.Max(pTimeoutMilliseconds, 1000);

            try
            {
                pServiceController.Stop();
                pServiceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(pTimeoutMilliseconds));
                return pServiceController.Status == ServiceControllerStatus.Stopped;
            }
            catch
            {
                return false;
            }
        }

        public static bool RestartService(string pServiceName, string pMachineName = null, int pTimeoutMilliseconds = 180000)
        {
            return RestartService(GetServiceController(pServiceName, pMachineName), pTimeoutMilliseconds);
        }

        public static bool RestartService(ServiceController pServiceController, int pTimeoutMilliseconds = 180000)
        {
            if (pServiceController == null)
                return false;
            pTimeoutMilliseconds = Math.Max(pTimeoutMilliseconds, 1000);

            try
            {
                int millisec1 = Environment.TickCount; //so we can honor the overall pTimeoutMilliseconds

                var stopped = StopService(pServiceController, pTimeoutMilliseconds);

                // calc the rest of the timeout ms
                var timeout = Math.Max(pTimeoutMilliseconds - (Environment.TickCount - millisec1), pTimeoutMilliseconds / 10); //make sure we have at least a little bit of time to start the service

                return StartService(pServiceController, timeout);
            }
            catch
            {
                return false;
            }
        }



    }
}
