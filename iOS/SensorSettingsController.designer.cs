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
    [Register ("SensorSettingsController")]
    partial class SensorSettingsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField BorderFromField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField BorderToField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CountersButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DivisionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField HoursField { get; set; }

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
        UIKit.UITextField MinutesField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton OkButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PhotoButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField SecondsField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView SensorIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SensorsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ValueSymbolFromLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ValueSymbolToLabel { get; set; }

        [Action ("ChancelButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChancelButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("CountersButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CountersButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("OkButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OkButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("PhotoButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PhotoButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SensorsButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SensorsButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BorderFromField != null) {
                BorderFromField.Dispose ();
                BorderFromField = null;
            }

            if (BorderToField != null) {
                BorderToField.Dispose ();
                BorderToField = null;
            }

            if (ChancelButton != null) {
                ChancelButton.Dispose ();
                ChancelButton = null;
            }

            if (CountersButton != null) {
                CountersButton.Dispose ();
                CountersButton = null;
            }

            if (DivisionLabel != null) {
                DivisionLabel.Dispose ();
                DivisionLabel = null;
            }

            if (HoursField != null) {
                HoursField.Dispose ();
                HoursField = null;
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

            if (MinutesField != null) {
                MinutesField.Dispose ();
                MinutesField = null;
            }

            if (OkButton != null) {
                OkButton.Dispose ();
                OkButton = null;
            }

            if (PhotoButton != null) {
                PhotoButton.Dispose ();
                PhotoButton = null;
            }

            if (SecondsField != null) {
                SecondsField.Dispose ();
                SecondsField = null;
            }

            if (SensorIcon != null) {
                SensorIcon.Dispose ();
                SensorIcon = null;
            }

            if (SensorsButton != null) {
                SensorsButton.Dispose ();
                SensorsButton = null;
            }

            if (ValueSymbolFromLabel != null) {
                ValueSymbolFromLabel.Dispose ();
                ValueSymbolFromLabel = null;
            }

            if (ValueSymbolToLabel != null) {
                ValueSymbolToLabel.Dispose ();
                ValueSymbolToLabel = null;
            }
        }
    }
}