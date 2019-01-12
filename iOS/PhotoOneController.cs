using Foundation;
using System;
using UIKit;
using DataUtils;
using CoreGraphics;

namespace pbDispatcher.iOS
{
    public partial class PhotoOneController : UIViewController
    {
        DateTime lastUpdateTime = DateTime.MinValue;
        string titleStr = "";

        UIImage image;
        UIScrollView scrollView;
        UIImageView imageView;
        private double imageProportion = 108f / 192f;

        public PhotoOneController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            titleStr = Application.selectedMachine.GetNameStr();

            if ((Application.selectedMachinePhoto.imageData != null) && (Application.selectedMachinePhoto.imageData.length != 0))
            { 
                image = UIImage.LoadFromData(NSData.FromArray(Application.selectedMachinePhoto.imageData.data));

                imageProportion = image.Size.Height / image.Size.Width;
            }

            scrollView = new UIScrollView();

            scrollView.Frame = new CGRect(0, 57, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Width * imageProportion);
            scrollView.ContentSize = new CGSize(scrollView.Frame.Width, scrollView.Frame.Height);
            View.AddSubview(scrollView);
            imageView = new UIImageView();
            scrollView.AddSubview(imageView);

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = titleStr;

            SetLayout();

            Application.canRotate = true;

            Application.StartUpdateTimer(CheckNewData);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            Application.canRotate = false;
            NavigationController.SetNavigationBarHidden(false, false);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            //SetLayout();

            //base.WillRotate(toInterfaceOrientation, duration);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout()
        {
            //TODO: remove new
            //imageView.Image = image;
            imageView.Dispose();
            imageView = new UIImageView(image);

            if (UIScreen.MainScreen.Bounds.Width > UIScreen.MainScreen.Bounds.Height)
            {
                ViewUtils.ExpandFullScreenProportion(scrollView, imageProportion);
                NavigationController.SetNavigationBarHidden(true, false);
            }
            else
            {
                scrollView.Frame = new CGRect(0, 57, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Width * imageProportion - 57 - 65);
                NavigationController.SetNavigationBarHidden(false, false);
            }

            scrollView.AddSubview(imageView);

            scrollView.ContentSize = new CGSize(scrollView.Frame.Width, scrollView.Frame.Height);

            scrollView.MaximumZoomScale = 5f;
            if (imageView.Image.Size.Width == 0)
                scrollView.MinimumZoomScale = scrollView.MaximumZoomScale;
            else
                scrollView.MinimumZoomScale = scrollView.Frame.Width / imageView.Image.Size.Width;
            scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };

            scrollView.SetZoomScale(scrollView.MinimumZoomScale, false);

            DeletePhotoButton.Frame = new CGRect((UIScreen.MainScreen.Bounds.Width - DeletePhotoButton.Frame.Width) / 2, UIScreen.MainScreen.Bounds.Height - 60, DeletePhotoButton.Frame.Width, DeletePhotoButton.Frame.Height);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            Title = ViewUtils.FormTitle(titleStr);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void DeletePhotoButton_TouchUpInside(UIButton sender)
        {
            DataManager.SheduleDeleteMachinePhotoRequest(Application.selectedMachine.ID, Application.selectedMachinePhoto.photoID, null);
            NavigationController?.PopViewController(true);
        }
    }
}