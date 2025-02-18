using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelBarApp
{
    /// <summary>
    /// Wrapper class to get ConfigurationSettings
    /// </summary>
    public static class AppConfigurationSettings
    {
        /// <summary>
        /// Level Bar update rate in the view
        /// </summary>
        public static int LevelBarViewUpdateRate => int.Parse(ConfigurationManager.AppSettings["LevelBarViewUpdateRate"]);

        /// <summary>
        /// Time to reset the peak hold
        /// </summary>
        public static int PeakHoldMaintenanceTime => int.Parse(ConfigurationManager.AppSettings["PeakHoldMaintenanceTime"]);
    }
}