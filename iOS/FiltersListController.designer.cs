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
    [Register ("FiltersListController")]
    partial class FiltersListController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CityButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DivisionButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch OnlyMyButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ServiceStateButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ServiceStateImageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StateButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StateImageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TypeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TypeImageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton UserButton { get; set; }

        [Action ("CityButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CityButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("DivisionButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DivisionButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("onlMySwitchChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void onlMySwitchChanged (UIKit.UISwitch sender);

        [Action ("ServiceStateButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ServiceStateButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("StateButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void StateButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("TypeButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TypeButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("UserButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UserButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CityButton != null) {
                CityButton.Dispose ();
                CityButton = null;
            }

            if (DivisionButton != null) {
                DivisionButton.Dispose ();
                DivisionButton = null;
            }

            if (OnlyMyButton != null) {
                OnlyMyButton.Dispose ();
                OnlyMyButton = null;
            }

            if (ServiceStateButton != null) {
                ServiceStateButton.Dispose ();
                ServiceStateButton = null;
            }

            if (ServiceStateImageButton != null) {
                ServiceStateImageButton.Dispose ();
                ServiceStateImageButton = null;
            }

            if (StateButton != null) {
                StateButton.Dispose ();
                StateButton = null;
            }

            if (StateImageButton != null) {
                StateImageButton.Dispose ();
                StateImageButton = null;
            }

            if (TypeButton != null) {
                TypeButton.Dispose ();
                TypeButton = null;
            }

            if (TypeImageButton != null) {
                TypeImageButton.Dispose ();
                TypeImageButton = null;
            }

            if (UserButton != null) {
                UserButton.Dispose ();
                UserButton = null;
            }
        }
    }
}