// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace pbDispatcher.iOS
{
    [Register ("UserSettingsController")]
    partial class UserSettingsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangeUserButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField LoginField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField NotificationsFromHoursField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField NotificationsFromMinutesField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField NotificationsToHoursField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField NotificationsToMinutesField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch SendNodesOfflineNotificationsSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ServerButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserNameLabel { get; set; }

        [Action ("ChangeUserButtonTouch:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChangeUserButtonTouch (UIKit.UIButton sender);

        [Action ("ServerButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ServerButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChangeUserButton != null) {
                ChangeUserButton.Dispose ();
                ChangeUserButton = null;
            }

            if (LoginField != null) {
                LoginField.Dispose ();
                LoginField = null;
            }

            if (NotificationsFromHoursField != null) {
                NotificationsFromHoursField.Dispose ();
                NotificationsFromHoursField = null;
            }

            if (NotificationsFromMinutesField != null) {
                NotificationsFromMinutesField.Dispose ();
                NotificationsFromMinutesField = null;
            }

            if (NotificationsToHoursField != null) {
                NotificationsToHoursField.Dispose ();
                NotificationsToHoursField = null;
            }

            if (NotificationsToMinutesField != null) {
                NotificationsToMinutesField.Dispose ();
                NotificationsToMinutesField = null;
            }

            if (PasswordField != null) {
                PasswordField.Dispose ();
                PasswordField = null;
            }

            if (SendNodesOfflineNotificationsSwitch != null) {
                SendNodesOfflineNotificationsSwitch.Dispose ();
                SendNodesOfflineNotificationsSwitch = null;
            }

            if (ServerButton != null) {
                ServerButton.Dispose ();
                ServerButton = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }
        }
    }
}