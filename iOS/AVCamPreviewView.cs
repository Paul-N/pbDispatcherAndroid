using System;
using AVFoundation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace pbDispatcher.iOS
{
    public partial class AVCamPreviewView : UIView
    {
        public AVCamPreviewView(IntPtr handle) : base(handle)
        {
        }

        [Export("layerClass")]
        public static Class LayerClass()
        {
            return new Class(typeof(AVCaptureVideoPreviewLayer));
        }

        public AVCaptureVideoPreviewLayer VideoPreviewLayer
        {
            get
            {
                return (Layer as AVCaptureVideoPreviewLayer);
            }
        }

        public AVCaptureSession Session
        {
            get
            {
                var videoPreviewLayer = VideoPreviewLayer;
                return videoPreviewLayer == null ? null : videoPreviewLayer.Session;
            }
            set
            {
                var videoPreviewLayer = VideoPreviewLayer;
                if (videoPreviewLayer != null)
                    videoPreviewLayer.Session = value;
            }
        }
    }
}