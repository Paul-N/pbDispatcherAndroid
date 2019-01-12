using Foundation;
using System;
using UIKit;
using DataUtils;
using CoreGraphics;

namespace pbDispatcher.iOS
{
	public partial class CamController : UIViewController
	{
        UIImage image = new UIImage();
        private byte needDataUpdate = 0;
        private CamImage camImage = new CamImage();
        private DateTime lastUpdateTime = DateTime.MinValue;
        private double imageProportion = 108f/192f;

        private bool autoUpdate;

        static public DateTime selectedCamTime = DateTime.MinValue;

        UIScrollView scrollView;
        UIImageView imageView;

        UIPanGestureRecognizer panGesture;

        public CamController(IntPtr handle) : base(handle)
		{
		}

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            camImage.time = DateTime.MinValue;

            bool last = (selectedCamTime == DateTime.MinValue);
            DataManager.SheduleGetCamImageRequest(Application.selectedMachine.camBridgeMac, Application.selectedMachine.camNum, selectedCamTime, DateTime.Now, last, DataUpdateCallback);

            scrollView = new UIScrollView();

            scrollView.Frame = new CoreGraphics.CGRect(0, TimeLabel.Frame.Bottom, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Width * imageProportion);
            scrollView.ContentSize = new CGSize(scrollView.Frame.Width, scrollView.Frame.Height);
            View.AddSubview(scrollView);
            imageView = new UIImageView();
            scrollView.AddSubview(imageView);

            panGesture = new UIPanGestureRecognizer(HandlePanAction);
            View.AddGestureRecognizer(panGesture);

            if (selectedCamTime != DateTime.MinValue)
            {
                DateTime nsRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                CamTimePicker.SetDate(NSDate.FromTimeIntervalSinceReferenceDate((selectedCamTime.ToUniversalTime() - nsRef).TotalSeconds), false);
            }
            CamTimePicker.ValueChanged += CamTimePicker_ValueChanged; 

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            Title = Application.selectedMachine.name;

            base.ViewWillAppear(animated);

            UpdateViewValues();

            SetLayout();

            Application.StartUpdateTimer(CheckNewData);
            Application.canRotate = true;

            if (Application.selectedMachine.type.code == MachineTypeCodes.Cam)
                autoUpdate = true;
            else
                autoUpdate = false;
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
            SetLayout();

            base.WillRotate(toInterfaceOrientation, duration);
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
                scrollView.Frame = new CGRect(0, TimeLabel.Frame.Bottom, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Width * imageProportion);
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

            CamTimePicker.Frame = new CGRect(0, scrollView.Frame.Bottom + 50, UIScreen.MainScreen.Bounds.Width, 150);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestResult)
        {
            if (requestResult is CamImage)
            {
                if (((CamImage)requestResult).data.length != 0) 
                {
                    camImage = (CamImage)requestResult;

                    if (autoUpdate == true)
                        DataManager.SheduleSendCamQueryRequest(Application.selectedMachine.camBridgeMac, Application.selectedMachine.camNum, DataUpdateCallback);

                    needDataUpdate++;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void HandlePanAction()
        {
            if ((scrollView.ZoomScale != scrollView.MinimumZoomScale) || (panGesture.State != UIGestureRecognizerState.Ended))
                return;

            autoUpdate = false;

            if (panGesture.TranslationInView (View).X > 0)
                DataManager.SheduleGetCamImageRequest(Application.selectedMachine.camBridgeMac, Application.selectedMachine.camNum, DateTime.MinValue, camImage.time.AddMinutes(-1), true, DataUpdateCallback);
            else
                DataManager.SheduleGetCamImageRequest(Application.selectedMachine.camBridgeMac, Application.selectedMachine.camNum, camImage.time.AddMinutes(1), DateTime.Now, false, DataUpdateCallback);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        void CamTimePicker_ValueChanged(object sender, EventArgs e)
        {
            autoUpdate = false;

            DateTime nsRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime time = nsRef.AddSeconds(CamTimePicker.Date.SecondsSinceReferenceDate).ToLocalTime();
            DataManager.SheduleGetCamImageRequest(Application.selectedMachine.camBridgeMac, Application.selectedMachine.camNum, time, DateTime.Now, false, DataUpdateCallback);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
            {
                Title = Application.selectedMachine.name;
            }
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                Title = "Нет авторизации";
            else
                Title = "Нет связи";

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
            else if ((DateTime.Now.Subtract(lastUpdateTime).TotalMilliseconds > Settings.updatePeriodMs) && (DataManager.NotAnsweredRequestsCount == 0) && (scrollView.ZoomScale == scrollView.MinimumZoomScale) && (autoUpdate == true))
            {
                DataManager.SheduleGetCamImageRequest(Application.selectedMachine.camBridgeMac, Application.selectedMachine.camNum, camImage.time, DateTime.Now, true, DataUpdateCallback);

                lastUpdateTime = DateTime.Now;
            }

        }

		//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		public void UpdateViewValues()
		{
            if ((camImage.data.length != 0) && (scrollView.ZoomScale == scrollView.MinimumZoomScale))
            {
                image = UIImage.LoadFromData(NSData.FromArray(camImage.data.data));
                imageView.Image = image;
              
                TimeLabel.Text = camImage.time.ToString();

                imageProportion = image.Size.Height / image.Size.Width;

                //DateTime nsRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                //CamTimePicker.SetDate(NSDate.FromTimeIntervalSinceReferenceDate((camImage.time.ToUniversalTime() - nsRef).TotalSeconds), false);

                SetLayout();
            }
		}
	}
}