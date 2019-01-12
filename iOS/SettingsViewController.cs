using Foundation;
using System;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class SettingsViewController : UIViewController
    {
        public NSTimer timer;

        public SettingsViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad ()
		{
            FilterTable.Source = new FilterTableSource();

            UpdateViewValues();

			Title = "Настройки";

            timer = NSTimer.CreateRepeatingScheduledTimer(0.1, delegate {
                CheckNewData();
            });

            //UserTokenField.Frame = new CoreGraphics.CGRect(UserTokenField.Frame.X, UserTokenField.Frame.Y, UIScreen.MainScreen.Bounds.Width - UserTokenField.Frame.X - 10, UserTokenField.Frame.Height);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            DataManager.currentView = ViewTypes.GeneralSettings;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            DataManager.currentView = ViewTypes.None;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            //Console.WriteLine("UpdateView");

            if (DataManager.currentView != ViewTypes.GeneralSettings)
                return;

            if (DataManager.updateState == UpdateStates.Completed)
            {
                UpdateViewValues();

                DataManager.updateState = UpdateStates.None;
            }

        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues ()
		{
            FilterTable.ReloadData();

            int index = DataManager.nodesFilters.FindIndex((NodeFilter obj) => obj.group == DataManager.selectedNodesFilter);

            if (index >= 0)
                FilterTable.SelectRow(NSIndexPath.FromRowSection(index, 0), false, UITableViewScrollPosition.Middle);

            ServerButton.SetTitle (DataManager.ServerName, UIControlState.Normal);
            UserButton.SetTitle   (DataManager.UserName, UIControlState.Normal);
        }


        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
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
            return UIInterfaceOrientationMask.Portrait;
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            return UIInterfaceOrientation.Portrait;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ServerButtonTouch(UIButton sender)
        {
            DataManager.CurrentServer++;

            NSUserDefaults.StandardUserDefaults.SetInt(DataManager.CurrentServer, "CurrentServer");
            NSUserDefaults.StandardUserDefaults.Synchronize();

            ServerButton.SetTitle("Загрузка...", UIControlState.Normal);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void UserButtonTouch(UIButton sender)
        {
            TableViewController.mainViewController.NavigationController.PushViewController(TableViewController.mainViewController.Storyboard.InstantiateViewController("UserSettingsController"), true);
        }
    }




    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class FilterTableSource : UITableViewSource
    {
        string cellIdentifier = "FilterCell";

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return DataManager.nodesFilters.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as FilterTableCell;

            if (indexPath.Row < DataManager.nodesFilters.Count)
            {
                var item = DataManager.nodesFilters[indexPath.Row];

                cell.Name = item.name;

                cell.UserInteractionEnabled = true;
            }

            return cell;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            DataManager.selectedNodesFilter = DataManager.nodesFilters[indexPath.Row].group;

            NSUserDefaults.StandardUserDefaults.SetInt (DataManager.selectedNodesFilter, "SelectedNodesFilter");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }
    }


}