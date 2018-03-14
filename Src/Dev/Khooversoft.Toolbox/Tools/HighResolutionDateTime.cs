using System;
using System.Runtime.InteropServices;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// The retrieves the current system date and time with the highest possible level of precision (&lt;1us).
    /// The retrieved information is in Coordinated Universal Time (UTC) format.
    /// </summary>
    public static class HighResolutionDateTime
    {
        static HighResolutionDateTime()
        {
            try
            {
                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                IsAvailable = true;
            }
            catch (EntryPointNotFoundException)
            {
                // Not running Windows 8 or higher.
                IsAvailable = false;
            }
        }

        /// <summary>
        /// Field to indicate high resolution is available
        /// </summary>
        public static bool IsAvailable { get; private set; }

        /// <summary>
        /// Interop services support
        /// </summary>
        /// <param name="filetime"></param>
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        /// <summary>
        /// Get high resolution UTC now
        /// </summary>
        /// <returns>time date offset</returns>
        public static DateTimeOffset GetUtcNow()
        {
            if (!IsAvailable)
            {
                return DateTimeOffset.UtcNow;
            }

            long filetime;
            GetSystemTimePreciseAsFileTime(out filetime);
            return DateTimeOffset.FromFileTime(filetime);
        }

        /// <summary>
        /// Get file timestamp
        /// </summary>
        /// <returns>Current time in file stamp</returns>
        public static long GetTimestamp()
        {
            long filetime;
            GetSystemTimePreciseAsFileTime(out filetime);
            return filetime;
        }
    }
}
