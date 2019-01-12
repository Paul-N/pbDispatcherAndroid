using System;
using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using UIKit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ObjCRuntime;

using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class AddPhotoController : UIViewController
    {
        AVCaptureSession session;
        AVCamSetupResult setupResult;
        AVCaptureDeviceInput videoDeviceInput;
        AVCapturePhotoOutput photoOutput;
        Dictionary<long, AVCamPhotoCaptureDelegate> inProgressPhotoCaptureDelegates;

        public AddPhotoController(IntPtr handle) : base(handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create the AVCaptureSession.
            session = new AVCaptureSession();

            // Set up the preview view.
            PreviewView.Session = session;

            setupResult = AVCamSetupResult.Success;

            switch (AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video))
            {
                case AVAuthorizationStatus.Authorized:
                    // The user has previously granted access to the camera.
                    break;

                case AVAuthorizationStatus.NotDetermined:

                    /*
                        The user has not yet been presented with the option to grant
                        video access. We suspend the session queue to delay session
                        setup until the access request has completed.
                        
                        Note that audio access will be implicitly requested when we
                        create an AVCaptureDeviceInput for audio during session setup.
                    */
                    AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, (bool granted) => {
                        if (!granted)
                        {
                            setupResult = AVCamSetupResult.CameraNotAuthorized;
                        }
                    });
                    break;
                default:
                    {
                        // The user has previously denied access.
                        setupResult = AVCamSetupResult.CameraNotAuthorized;
                        break;
                    }
            }

            ConfigureSession();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            switch (setupResult)
            {
                case AVCamSetupResult.Success:
                    session.StartRunning();
                    break;
            }

            Application.canRotate = true;

            SetLayout(CGSize.Empty);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            Application.canRotate = false;
            NavigationController.SetNavigationBarHidden(false, false);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout(CGSize toSize)
        {
            if (toSize == CGSize.Empty)
            {
                toSize.Height = UIScreen.MainScreen.Bounds.Height;
                toSize.Width = UIScreen.MainScreen.Bounds.Width;
            }

            if (toSize.Width > toSize.Height)
            {
                NavigationController.SetNavigationBarHidden(true, false);

                PreviewView.Frame = new CGRect(0, 0, toSize.Width - 70, toSize.Height);

                CapturePhotoButton.Frame = new CGRect(PreviewView.Frame.Right + 5, toSize.Height / 2 - 30, 60, 60);
            }
            else
            {
                NavigationController.SetNavigationBarHidden(false, false);

                PreviewView.Frame = new CGRect(0, 57, toSize.Width, toSize.Height - 57 - 70);

                CapturePhotoButton.Frame = new CGRect(toSize.Width / 2 - 30, PreviewView.Frame.Bottom + 5, 60, 60);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        void ConfigureSession()
        {
            if (setupResult != AVCamSetupResult.Success)
            {
                return;
            }

            NSError error = null;

            session.BeginConfiguration();

            /*
                We do not create an AVCaptureMovieFileOutput when setting up the session because the
                AVCaptureMovieFileOutput does not support movie recording with AVCaptureSessionPresetPhoto.
            */
            session.SessionPreset = AVCaptureSession.PresetPhoto;

            // Add video input.

            // Choose the back dual camera if available, otherwise default to a wide angle camera.
            var videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualCamera, AVMediaType.Video, AVCaptureDevicePosition.Back);
            if (videoDevice == null)
            {
                // If the back dual camera is not available, default to the back wide angle camera.
                videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Back);

                // In some cases where users break their phones, the back wide angle camera is not available. In this case, we should default to the front wide angle camera.
                if (videoDevice == null)
                {
                    videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Front);
                }
            }
            var lVideoDeviceInput = AVCaptureDeviceInput.FromDevice(videoDevice, out error);
            if (lVideoDeviceInput == null)
            {
                Console.WriteLine($"Could not create video device input: {error}");
                setupResult = AVCamSetupResult.SessionConfigurationFailed;
                session.CommitConfiguration();
                return;
            }
            if (session.CanAddInput(lVideoDeviceInput))
            {
                session.AddInput(lVideoDeviceInput);
                videoDeviceInput = lVideoDeviceInput;

                //DispatchQueue.MainQueue.DispatchAsync(() => {
                    /*
                        Why are we dispatching this to the main queue?
                        Because AVCaptureVideoPreviewLayer is the backing layer for AVCamPreviewView and UIView
                        can only be manipulated on the main thread.
                        Note: As an exception to the above rule, it is not necessary to serialize video orientation changes
                        on the AVCaptureVideoPreviewLayer’s connection with other session manipulation.

                        Use the status bar orientation as the initial video orientation. Subsequent orientation changes are
                        handled by -[AVCamCameraViewController viewWillTransitionToSize:withTransitionCoordinator:].
                    */
                    var statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                    var initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
                    if (statusBarOrientation != UIInterfaceOrientation.Unknown)
                    {
                        initialVideoOrientation = (AVCaptureVideoOrientation)statusBarOrientation;
                    }

                    PreviewView.VideoPreviewLayer.Connection.VideoOrientation = initialVideoOrientation;
                //});
            }
            else
            {
                Console.WriteLine(@"Could not add video device input to the session");
                setupResult = AVCamSetupResult.SessionConfigurationFailed;
                session.CommitConfiguration();
                return;
            }

            // Add photo output.
            var lPhotoOutput = new AVCapturePhotoOutput();
            if (session.CanAddOutput(lPhotoOutput))
            {
                session.AddOutput(lPhotoOutput);
                photoOutput = lPhotoOutput;

                photoOutput.IsHighResolutionCaptureEnabled = true;
                photoOutput.IsLivePhotoCaptureEnabled = false;
                //photoOutput.IsDepthDataDeliveryEnabled(photoOutput.IsDepthDataDeliverySupported());

                //livePhotoMode = photoOutput.IsLivePhotoCaptureSupported ? AVCamLivePhotoMode.On : AVCamLivePhotoMode.Off;
                //depthDataDeliveryMode = photoOutput.IsDepthDataDeliverySupported() ? AVCamDepthDataDeliveryMode.On : AVCamDepthDataDeliveryMode.Off;


                inProgressPhotoCaptureDelegates = new Dictionary<long, AVCamPhotoCaptureDelegate>();
                //inProgressLivePhotoCapturesCount = 0;
            }
            else
            {
                Console.WriteLine(@"Could not add photo output to the session");
                setupResult = AVCamSetupResult.SessionConfigurationFailed;
                session.CommitConfiguration();
                return;
            }

            //backgroundRecordingId = UIApplication.BackgroundTaskInvalid;

            session.CommitConfiguration();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.All;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillTransitionToSize(CoreGraphics.CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            var deviceOrientation = UIDevice.CurrentDevice.Orientation;

            if (deviceOrientation.IsPortrait() || deviceOrientation.IsLandscape())
            {
                PreviewView.VideoPreviewLayer.Connection.VideoOrientation = (AVCaptureVideoOrientation)deviceOrientation;

                SetLayout(toSize);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        partial void CapturePhotoButton_TouchUpInside(UIButton sender)
        {
            var videoPreviewLayerVideoOrientation = PreviewView.VideoPreviewLayer.Connection.VideoOrientation;

            // Update the photo output's connection to match the video orientation of the video preview layer.
            var photoOutputConnection = photoOutput.ConnectionFromMediaType (AVMediaType.Video);
            photoOutputConnection.VideoOrientation = videoPreviewLayerVideoOrientation;

            AVCapturePhotoSettings photoSettings;
            // Capture HEIF photo when supported, with flash set to auto and high resolution photo enabled.

            if ( photoOutput.AvailablePhotoCodecTypes.Where (codec => codec == AVVideo2.CodecHEVC).Any ())
            {
                photoSettings = AVCapturePhotoSettings.FromFormat (new NSDictionary<NSString, NSObject> (AVVideo.CodecKey, AVVideo2.CodecHEVC));
            }
            else
            {
                photoSettings = AVCapturePhotoSettings.Create ();
            }

            if (videoDeviceInput.Device.FlashAvailable)
            {
                photoSettings.FlashMode = AVCaptureFlashMode.Auto;
            }
            photoSettings.IsHighResolutionPhotoEnabled = true;
            if (photoSettings.AvailablePreviewPhotoPixelFormatTypes.Count () > 0)
            {
                photoSettings.PreviewPhotoFormat = new NSDictionary<NSString, NSObject> (CoreVideo.CVPixelBuffer.PixelFormatTypeKey, photoSettings.AvailablePreviewPhotoPixelFormatTypes.First ());
            }

            photoSettings.IsDepthDataDeliveryEnabled (false);

            // Use a separate object for the photo capture delegate to isolate each capture life cycle.
            var photoCaptureDelegate = new AVCamPhotoCaptureDelegate (photoSettings, () => {
                    PreviewView.VideoPreviewLayer.Opacity = 0.0f;
                    UIView.Animate (0.25, () => {
                        PreviewView.VideoPreviewLayer.Opacity = 1.0f;
                } );
            }, (bool capturing ) => {
                /*
                    Because Live Photo captures can overlap, we need to keep track of the
                    number of in progress Live Photo captures to ensure that the
                    Live Photo label stays visible during these captures.
                */
                   /* if (capturing)
                    {
                        inProgressLivePhotoCapturesCount++;
                    }
                    else
                    {
                        inProgressLivePhotoCapturesCount--;
                    }

                    var lInProgressLivePhotoCapturesCount = inProgressLivePhotoCapturesCount;

                        if (lInProgressLivePhotoCapturesCount > 0)
                        {
                            CapturingLivePhotoLabel.Hidden = false;
                        }
                        else if (lInProgressLivePhotoCapturesCount == 0)
                        {
                            CapturingLivePhotoLabel.Hidden = true;
                        }
                        else
                        {
                            Console.WriteLine (@"Error: In progress live photo capture count is less than 0");
                        }*/
            }, (AVCamPhotoCaptureDelegate lPhotoCaptureDelegate ) => {
                // When the capture is complete, remove a reference to the photo capture delegate so it can be deallocated.
                    inProgressPhotoCaptureDelegates[lPhotoCaptureDelegate.RequestedPhotoSettings.UniqueID] = null;
            });
    
            /*
                The Photo Output keeps a weak reference to the photo capture delegate so
                we store it in an array to maintain a strong reference to this object
                until the capture is completed.
            */
            inProgressPhotoCaptureDelegates[photoCaptureDelegate.RequestedPhotoSettings.UniqueID] = photoCaptureDelegate;
            photoOutput.CapturePhoto (photoSettings, photoCaptureDelegate);
        }
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class AVCamPhotoCaptureDelegate : NSObject, IAVCapturePhotoCaptureDelegate
    {
        public AVCapturePhotoSettings RequestedPhotoSettings { get; set; }
        Action WillCapturePhotoAnimation { get; set; }
        Action<bool> LivePhotoCaptureHandler { get; set; }
        Action<AVCamPhotoCaptureDelegate> CompletionHandler { get; set; }
        NSData PhotoData { get; set; }
        NSUrl LivePhotoCompanionMovieUrl { get; set; }

        public AVCamPhotoCaptureDelegate (AVCapturePhotoSettings requestedPhotoSettings, Action willCapturePhotoAnimation, Action<bool> livePhotoCaptureHandler, Action<AVCamPhotoCaptureDelegate> completionHandler)
        {
            RequestedPhotoSettings = requestedPhotoSettings;
            WillCapturePhotoAnimation = willCapturePhotoAnimation;
            LivePhotoCaptureHandler = livePhotoCaptureHandler;
            CompletionHandler = completionHandler;
        }

        void DidFinish ()
        {
            if (LivePhotoCompanionMovieUrl != null && NSFileManager.DefaultManager.FileExists (LivePhotoCompanionMovieUrl.Path))
            {
                NSError error;
                NSFileManager.DefaultManager.Remove (LivePhotoCompanionMovieUrl.Path, out error);

                if (error != null)
                    Console.WriteLine ($"Could not remove file at url: {LivePhotoCompanionMovieUrl.Path}");
            }

            CompletionHandler (this);
        }

        [Export ("captureOutput:willBeginCaptureForResolvedSettings:")]
        public virtual void WillBeginCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
        {
            if ((resolvedSettings.LivePhotoMovieDimensions.Width > 0) && (resolvedSettings.LivePhotoMovieDimensions.Height > 0))
            {
                LivePhotoCaptureHandler (true);
            }
        }

        [Export ("captureOutput:willCapturePhotoForResolvedSettings:")]
        public virtual void WillCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
        {
            WillCapturePhotoAnimation ();
        }

        [Export ("captureOutput:didFinishProcessingPhoto:error:")]
        public virtual void DidFinishProcessingPhoto (AVCapturePhotoOutput captureOutput, AVCapturePhoto photo, NSError error)
        {
            if (error != null)
            {
                Console.WriteLine ($"Error capturing photo: {error}", error);
                return;
            }

            PhotoData = photo.FileDataRepresentation ();

            DataManager.SheduleAddMachinePhotoRequest(Application.selectedMachine.ID, PhotoData.ToArray(), null);
        }

        [Export ("captureOutput:didFinishRecordingLivePhotoMovieForEventualFileAtURL:resolvedSettings:")]
        public virtual void DidFinishRecordingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, AVCaptureResolvedPhotoSettings resolvedSettings)
        {
            LivePhotoCaptureHandler (false);
        }

        [Export ("captureOutput:didFinishProcessingLivePhotoToMovieFileAtURL:duration:photoDisplayTime:resolvedSettings:error:")]
        public virtual void DidFinishProcessingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, CMTime duration, CMTime photoDisplayTime, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error)
        {
            if (error != null)
            {
                Console.WriteLine ($"Error processing live photo companion movie: {error}", error);
                return;
            }

            LivePhotoCompanionMovieUrl = outputFileUrl;
        }

        [Export ("captureOutput:didFinishCaptureForResolvedSettings:error:")]
        public virtual void DidFinishCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error)
        {
            if (error != null)
            {
                Console.WriteLine ($"Error capturing photo: {error}", error);
                DidFinish ();
                return;
            }

            if (PhotoData == null)
            {
                Console.WriteLine ("No photo data resource");
                DidFinish ();
                return;
            }

            DidFinish ();
        }
    }



    //-----------------------------------------------------------------------------------------------------------------------------------------------
    enum AVCamSetupResult
    {
        Success,
        CameraNotAuthorized,
        SessionConfigurationFailed
    };
    public unsafe static partial class AVCapturePhoto_Ext
    {
        [CompilerGenerated]
        static readonly IntPtr class_ptr = Class.GetHandle("AVCapturePhoto");

        [Export("fileDataRepresentation")]
        [CompilerGenerated]
        public static NSData FileDataRepresentation(this AVCapturePhoto This)
        {
            return Runtime.GetNSObject<NSData>(global::ApiDefinition.Messaging.IntPtr_objc_msgSend(This.Handle, Selector.GetHandle("fileDataRepresentation")));
        }

    } /* class AVCapturePhoto_Ext */

    public unsafe static partial class AVCapturePhotoOutput_AVCapturePhotoOutputDepthDataDeliverySupport
    {
        [CompilerGenerated]
        static readonly IntPtr class_ptr = Class.GetHandle("AVCapturePhotoOutput");

        [Export("isDepthDataDeliveryEnabled")]
        [CompilerGenerated]
        public static bool IsDepthDataDeliveryEnabled(this AVCapturePhotoOutput This)
        {
            return global::ApiDefinition.Messaging.bool_objc_msgSend(This.Handle, Selector.GetHandle("isDepthDataDeliveryEnabled"));
        }

        [Export("setDepthDataDeliveryEnabled:")]
        [CompilerGenerated]
        public static void IsDepthDataDeliveryEnabled(this AVCapturePhotoOutput This, bool enabled)
        {
            global::ApiDefinition.Messaging.void_objc_msgSend_bool(This.Handle, Selector.GetHandle("setDepthDataDeliveryEnabled:"), enabled);
        }

        [Export("isDepthDataDeliverySupported")]
        [CompilerGenerated]
        public static bool IsDepthDataDeliverySupported(this AVCapturePhotoOutput This)
        {
            return global::ApiDefinition.Messaging.bool_objc_msgSend(This.Handle, Selector.GetHandle("isDepthDataDeliverySupported"));
        }

    } /* class AVCapturePhotoOutput_AVCapturePhotoOutputDepthDataDeliverySupport */

    public unsafe static partial class AVCapturePhotoSettings_AVCapturePhotoSettings
    {
        [CompilerGenerated]
        static readonly IntPtr class_ptr = Class.GetHandle("AVCapturePhotoSettings");

        [Export("isDepthDataDeliveryEnabled")]
        [CompilerGenerated]
        public static bool IsDepthDataDeliveryEnabled(this AVCapturePhotoSettings This)
        {
            return global::ApiDefinition.Messaging.bool_objc_msgSend(This.Handle, Selector.GetHandle("isDepthDataDeliveryEnabled"));
        }

        [Export("setDepthDataDeliveryEnabled:")]
        [CompilerGenerated]
        public static void IsDepthDataDeliveryEnabled(this AVCapturePhotoSettings This, bool enabled)
        {
            global::ApiDefinition.Messaging.void_objc_msgSend_bool(This.Handle, Selector.GetHandle("setDepthDataDeliveryEnabled:"), enabled);
        }

    } /* class AVCapturePhotoSettings_AVCapturePhotoSettings */

    public unsafe static partial class AVCapturePhotoSettings_AVCapturePhotoSettingsConversions
    {
        [CompilerGenerated]
        static readonly IntPtr class_ptr = Class.GetHandle("AVCapturePhotoSettings");

        [Export("processedFileType")]
        [CompilerGenerated]
        public static string ProcessedFileType(this AVCapturePhotoSettings This)
        {
            return NSString.FromHandle(global::ApiDefinition.Messaging.IntPtr_objc_msgSend(This.Handle, Selector.GetHandle("processedFileType")));
        }

    } /* class AVCapturePhotoSettings_AVCapturePhotoSettingsConversions */

    public unsafe static partial class AVVideo2
    {
        [CompilerGenerated]
        static NSString _CodecHEVC;
        [Field("AVVideoCodecTypeHEVC", "__Internal")]
        public static NSString CodecHEVC
        {
            get
            {
                if (_CodecHEVC == null)
                    _CodecHEVC = Dlfcn.GetStringConstant(Libraries.__Internal.Handle, "AVVideoCodecTypeHEVC");
                return _CodecHEVC;
            }
        }
    } /* class AVVideo2 */

    public unsafe static partial class NSString_NSStringExt
    {

        [CompilerGenerated]
        static readonly IntPtr class_ptr = Class.GetHandle("NSString");

        [Export("boolValue")]
        [CompilerGenerated]
        public static bool BoolValue(this NSString This)
        {
            return global::ApiDefinition.Messaging.bool_objc_msgSend(This.Handle, Selector.GetHandle("boolValue"));
        }

    } /* class NSString_NSStringExt */
}

namespace ApiDefinition
{
    partial class Messaging
    {
        static internal System.Reflection.Assembly this_assembly = typeof(Messaging).Assembly;

        const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
        public extern static IntPtr IntPtr_objc_msgSend(IntPtr receiever, IntPtr selector);
        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
        public extern static IntPtr IntPtr_objc_msgSendSuper(IntPtr receiever, IntPtr selector);
        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
        public extern static IntPtr IntPtr_objc_msgSend_IntPtr(IntPtr receiever, IntPtr selector, IntPtr arg1);
        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
        public extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr(IntPtr receiever, IntPtr selector, IntPtr arg1);
        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
        public extern static bool bool_objc_msgSend(IntPtr receiver, IntPtr selector);
        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
        public extern static bool bool_objc_msgSendSuper(IntPtr receiver, IntPtr selector);
        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
        public extern static void void_objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg1);
        [DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
        public extern static void void_objc_msgSendSuper_bool(IntPtr receiver, IntPtr selector, bool arg1);
    }
}

namespace ObjCRuntime
{
    [CompilerGenerated]
    static partial class Libraries
    {
        static public class __Internal
        {
            static public readonly IntPtr Handle = Dlfcn.dlopen(null, 0);
        }
    }
}