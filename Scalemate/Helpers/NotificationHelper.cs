using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Scalemate.Helpers
{
    public class NotificationHelper
    {

        public static void ShowNotification()
        {
            // template to load for showing Toast Notification
            var xmlToastTemplate = "<toast>" +
                                     "<visual>" +
                                       "<binding template =\"ToastGeneric\">" +
                                         "<text>Your images have been exported.</text>" +
                                         "<text>" +
                                           "Tap Open Folder to view them." +
                                         "</text>" +
                                       "</binding>" +
                                     "</visual>" +
                                     "<actions>" +
                                         "<action activationType=\"background\" arguments=\"action=rsvpEvent&amp;eventId=63851\" content=\"Open Folder\" />" +
                                         "<action activationType = \"system\" arguments = \"dismiss\" content = \"\" />" +
                                     "</actions>" +
                                   "</toast>";

            // load the template as XML document
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlToastTemplate);

            // create the toast notification and show to user
            var toastNotification = new ToastNotification(xmlDocument);
            toastNotification.NotificationMirroring = NotificationMirroring.Disabled;
            toastNotification.Group = "toast";
            var notification = ToastNotificationManager.CreateToastNotifier();
            notification.Show(toastNotification);
        }

    }
}
