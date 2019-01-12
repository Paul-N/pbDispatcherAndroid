using Foundation;
using System;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class MyNavigationController : UINavigationController
    {
        public MyNavigationController (IntPtr handle) : base (handle)
        {
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (Application.canRotate == true)
                return true;
            else
                return toInterfaceOrientation == UIInterfaceOrientation.Portrait;
        }

        public override bool ShouldAutomaticallyForwardRotationMethods
        {
            get
            {
                return false;
            }
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }



        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (Application.canRotate == true)
                return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.Landscape;
            else
                return UIInterfaceOrientationMask.Portrait;
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            return UIInterfaceOrientation.Portrait;
        }

    }
}