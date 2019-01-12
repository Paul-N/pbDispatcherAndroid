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
    [Register ("PhotoController")]
    partial class PhotoController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AddPhotoButton { get; set; }

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
        UIKit.UICollectionView MachinePhotosCollection { get; set; }

        [Action ("AddPhotoButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AddPhotoButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AddPhotoButton != null) {
                AddPhotoButton.Dispose ();
                AddPhotoButton = null;
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

            if (MachinePhotosCollection != null) {
                MachinePhotosCollection.Dispose ();
                MachinePhotosCollection = null;
            }
        }
    }
}