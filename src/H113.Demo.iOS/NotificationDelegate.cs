using System;
using UserNotifications;

namespace H113.Demo.iOS
{
    public class NotificationDelegate : UNUserNotificationCenterDelegate
    {
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center,
            UNNotificationResponse response,
            Action completionHandler)
        {
            completionHandler();
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge |
                              UNNotificationPresentationOptions.Sound);
        }
    }
}