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
    [Register ("CountersController")]
    partial class CountersController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DivisionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel InventoryIDLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView MachineIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MachineNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton OkButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Setting1Field { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Setting1Label { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Setting2Field { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Setting2Label { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Setting3Field { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Setting3Label { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SettingsTotalLabel { get; set; }

        [Action ("ChancelButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChancelButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("OkButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OkButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChancelButton != null) {
                ChancelButton.Dispose ();
                ChancelButton = null;
            }

            if (DivisionLabel != null) {
                DivisionLabel.Dispose ();
                DivisionLabel = null;
            }

            if (InventoryIDLabel != null) {
                InventoryIDLabel.Dispose ();
                InventoryIDLabel = null;
            }

            if (MachineIcon != null) {
                MachineIcon.Dispose ();
                MachineIcon = null;
            }

            if (MachineNameLabel != null) {
                MachineNameLabel.Dispose ();
                MachineNameLabel = null;
            }

            if (OkButton != null) {
                OkButton.Dispose ();
                OkButton = null;
            }

            if (Setting1Field != null) {
                Setting1Field.Dispose ();
                Setting1Field = null;
            }

            if (Setting1Label != null) {
                Setting1Label.Dispose ();
                Setting1Label = null;
            }

            if (Setting2Field != null) {
                Setting2Field.Dispose ();
                Setting2Field = null;
            }

            if (Setting2Label != null) {
                Setting2Label.Dispose ();
                Setting2Label = null;
            }

            if (Setting3Field != null) {
                Setting3Field.Dispose ();
                Setting3Field = null;
            }

            if (Setting3Label != null) {
                Setting3Label.Dispose ();
                Setting3Label = null;
            }

            if (SettingsTotalLabel != null) {
                SettingsTotalLabel.Dispose ();
                SettingsTotalLabel = null;
            }
        }
    }
}