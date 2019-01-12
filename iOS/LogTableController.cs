using Foundation;
using System;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class LogTableController : UITableViewController
    {
        public NSTimer timer;
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;

        Node node;

        public LogTableController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            node = DataManager.nodes[DataManager.selectedSensor];

            LogTable.Source = new LogTableSource();

            UpdateViewValues();

            timer = NSTimer.CreateRepeatingScheduledTimer(0.1, delegate {
                CheckNewData();
            });
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            DataManager.currentView = ViewTypes.NodeLog;

            UpdateViewValues();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            DataManager.currentView = ViewTypes.None;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(RequestStates requestState)
        {
            if (requestState == RequestStates.Completed)
                needDataUpdate++;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.currentView != ViewTypes.NodeLog)
                return;

            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                Title = DataManager.nodes[DataManager.selectedSensor].nodeName;
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                Title = "Нет авторизации";
            else
                Title = "Нет связи";

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
            else if (DateTime.Now.Subtract(lastUpdateTime).TotalMilliseconds > Settings.updatePeriodMs)
            {
                DataManager.SheduleNodeLogRequest(node.nodeID, DataUpdateCallback);
                lastUpdateTime = DateTime.Now;
            }

        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateViewValues()
        {
            LogTable.ReloadData();
        }

    }





    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class LogTableSource : UITableViewSource
    {
        string cellIdentifier = "LogTableCell";

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return DataManager.packets.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as LogTableCell;

            if (indexPath.Row < DataManager.packets.Count)
            {
                var item = DataManager.packets[indexPath.Row];

                cell.Time = item.time.ToString("dd.MM.yyyy  HH:mm:ss");

                cell.AckTime = "";
                if (item.ackTime != DateTime.MinValue)
                    cell.AckTime = item.ackTime.ToString("dd.MM.yyyy  HH:mm:ss");

                cell.PacketHeader = item.header;
                cell.PacketDescription = item.description;

                if (item.directionToNode == false)
                {
                    cell.SeqNumber = item.nodeSeqNumber.ToString();
                    cell.BackgroundColor = UIColor.White;
                }
                else
                {
                    cell.SeqNumber = "";

                    if (item.inProcess == false)
                        cell.BackgroundColor = UIColor.Yellow;
                    else
                    { 
                        cell.BackgroundColor = UIColor.FromRGB(255, 255, 224);
                        cell.SeqNumber = item.ack.ToString();
                    }
                        
                }



                cell.UserInteractionEnabled = true;
            }

            return cell;
        }
    }
}