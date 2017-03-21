using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Scalemate.Helpers
{
    public class SettingsHelper
    {

        public static Object GetSetting(String settingName, bool isRoaming = false)
        {
            ApplicationDataContainer settings;

            if (!isRoaming)
            {
                settings = ApplicationData.Current.LocalSettings;
            }
            else
            {
                settings = ApplicationData.Current.RoamingSettings;
            }

            return settings.Values[settingName];
        }

        public static void UpdateSetting(String settingName, Object settingValue, bool isRoaming = false)
        {

            ApplicationDataContainer settings;

            if (!isRoaming)
            {
                settings = ApplicationData.Current.LocalSettings;
            }
            else
            {
                settings = ApplicationData.Current.RoamingSettings;
            }

            settings.Values[settingName] = settingValue;

        }

    }
}
