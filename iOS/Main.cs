using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Foundation;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
	public class Application
	{
        static Thread clientThread;
        public static Machine selectedMachine;
        public static MachinePhoto selectedMachinePhoto;
        public static bool canRotate = false;

        static private NSTimer updateTimer;

        // This is the main entry point of the application.
        static void Main (string [] args)
		{
            
            DataManager.storedCityID = (ushort)NSUserDefaults.StandardUserDefaults.IntForKey("SelectedCity");
            DataManager.storedDivisionID = (ushort)NSUserDefaults.StandardUserDefaults.IntForKey("SelectedDivision");
            DataManager.storedMachineTypeID = (ushort)NSUserDefaults.StandardUserDefaults.IntForKey("SelectedMachineType");
            DataManager.storedMachineStateID = (ushort)NSUserDefaults.StandardUserDefaults.IntForKey("SelectedMachineState");
            DataManager.storedMachineServiceStateID = (ushort)NSUserDefaults.StandardUserDefaults.IntForKey("SelectedMachineServiceState");
            DataManager.onlyMyMachines = (bool)NSUserDefaults.StandardUserDefaults.BoolForKey("onlyMyMachines");

            Client.userToken = NSUserDefaults.StandardUserDefaults.StringForKey("UserToken");
            Client.currentServerNum = (int)NSUserDefaults.StandardUserDefaults.IntForKey("CurrentServer");

            clientThread = new Thread(DataManager.Init);
            clientThread.IsBackground = true;
            clientThread.Start();

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}

        //-----------------------------------------------------------------------------------------------------------------------
        static public void StoreValues()
        {
            if (DataManager.selectedCity != null)
                NSUserDefaults.StandardUserDefaults.SetInt((int)DataManager.selectedCity.ID, "SelectedCity");
            if (DataManager.selectedDivision != null)
                NSUserDefaults.StandardUserDefaults.SetInt((int)DataManager.selectedDivision.ID, "SelectedDivision");
            if (DataManager.selectedMachineType != null)
                NSUserDefaults.StandardUserDefaults.SetInt((int)DataManager.selectedMachineType.ID, "SelectedMachineType");
            if (DataManager.selectedMachineState != null)
                NSUserDefaults.StandardUserDefaults.SetInt((int)DataManager.selectedMachineState.ID, "SelectedMachineState");
            if (DataManager.selectedMachineServiceState != null)
                NSUserDefaults.StandardUserDefaults.SetInt((int)DataManager.selectedMachineServiceState.ID, "SelectedMachineServiceState");

            NSUserDefaults.StandardUserDefaults.SetBool(DataManager.onlyMyMachines, "onlyMyMachines");

            if(Client.userToken != null)
                NSUserDefaults.StandardUserDefaults.SetString(Client.userToken, "UserToken");
            NSUserDefaults.StandardUserDefaults.SetInt(DataManager.CurrentServerNum, "CurrentServer");

            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public delegate void UpdateTimerCallback();
        static private UpdateTimerCallback lastUpdateTimerCallback;
        //-----------------------------------------------------------------------------------------------------------------------
        static public void StartUpdateTimer(UpdateTimerCallback updateTimerCallback)
        {
            lastUpdateTimerCallback = updateTimerCallback;
            StopUpdateTimer();
            updateTimer = NSTimer.CreateRepeatingScheduledTimer(0.1, delegate
            {
                lastUpdateTimerCallback?.Invoke();
            });
        }
        //-----------------------------------------------------------------------------------------------------------------------
        static public void RestartUpdateTimer()
        {
            StopUpdateTimer();
            updateTimer = NSTimer.CreateRepeatingScheduledTimer(0.1, delegate
            {
                lastUpdateTimerCallback?.Invoke();
            });
        }
        //-----------------------------------------------------------------------------------------------------------------------
        static public void StopUpdateTimer()
        {
            updateTimer?.Invalidate();
            updateTimer?.Dispose();
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------
    static public class ViewUtils
    {
        //-----------------------------------------------------------------------------------------------------------------------
        static public void ExpandWidth(UIView view, double rightBorder)
        {
            view.Frame = new CoreGraphics.CGRect(view.Frame.X, view.Frame.Y, UIScreen.MainScreen.Bounds.Width - view.Frame.X - rightBorder, view.Frame.Height);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        static public void ExpandWidthToSize(CoreGraphics.CGSize toSize, UIView view, double rightBorder)
        {
            view.Frame = new CoreGraphics.CGRect(view.Frame.X, view.Frame.Y, toSize.Width - view.Frame.X - rightBorder, view.Frame.Height);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        static public void ExpandHeight(UIView view, double bottomBorder)
        {
            view.Frame = new CoreGraphics.CGRect(view.Frame.X, view.Frame.Y, view.Frame.Width, UIScreen.MainScreen.Bounds.Height - view.Frame.Y - bottomBorder);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        static public void ExpandHeightToSize(CoreGraphics.CGSize toSize, UIView view, double bottomBorder)
        {
            view.Frame = new CoreGraphics.CGRect(view.Frame.X, view.Frame.Y, view.Frame.Width, toSize.Height - view.Frame.Y - bottomBorder);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        static public void ExpandWidthProportion(UIView view, double rightBorder, double proportion)
        {
            view.Frame = new CoreGraphics.CGRect(view.Frame.X, view.Frame.Y, UIScreen.MainScreen.Bounds.Width - view.Frame.X - rightBorder, (UIScreen.MainScreen.Bounds.Width - view.Frame.X - rightBorder) * proportion);

            if ((view.Frame.Y + view.Frame.Height) > UIScreen.MainScreen.Bounds.Height)
                view.Frame = new CoreGraphics.CGRect(view.Frame.X, view.Frame.Y, (UIScreen.MainScreen.Bounds.Height - view.Frame.Y) / proportion, UIScreen.MainScreen.Bounds.Height - view.Frame.Y);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        static public void ExpandFullScreenProportion(UIView view, double proportion, double bottomBorder = 0)
        {
            view.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Width * proportion);

            double maxHeight = UIScreen.MainScreen.Bounds.Height - bottomBorder;

            if (view.Frame.Height > maxHeight)
            {
                view.Frame = new CoreGraphics.CGRect(0, 0, maxHeight / proportion, maxHeight);

                view.Frame = new CoreGraphics.CGRect((UIScreen.MainScreen.Bounds.Width - view.Frame.Width) / 2, 0, view.Frame.Width, view.Frame.Height);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------
        static public string FormTitle (string defaultTitle)
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                return defaultTitle;
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                return "Нет авторизации";
            else
                return "Нет связи";
        }

    }
}
