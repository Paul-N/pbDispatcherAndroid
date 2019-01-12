using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class FilterController : UIViewController
    {
        private byte needDataUpdate = 0;
        string filterKind = "";

        private List<FilterObject> filterList = new List<FilterObject>(); 

        public FilterController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            FilterTable.Source = new FilterTableSource(filterList);

            UpdateViewValues();

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateViewValues();
            CheckNewData();

            SetLayout();

            Application.StartUpdateTimer(CheckNewData);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            SetLayout();

            base.WillRotate(toInterfaceOrientation, duration);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout()
        {
            ViewUtils.ExpandWidth(FilterTable, 3);
            ViewUtils.ExpandHeight(FilterTable, 3);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestState)
        {
            //if (requestState == RequestStates.Completed)
            needDataUpdate++;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                Title = filterKind;
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                Title = "Нет авторизации";
            else
                Title = "Нет связи";

            if (((FilterTableSource)FilterTable.Source).signalRowSelected == true)
            {
                ((FilterTableSource)FilterTable.Source).signalRowSelected = false;
                NavigationController?.PopViewController(true);
            }

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }

        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues()
        {
            int selectedIndex = 0;

            filterList.Clear();

            if (DataManager.filterToSelect is City)
            {
                filterKind = "Выберите город";
                foreach (var item in DataManager.cities)
                {
                    filterList.Add(new FilterObject("", item.Value.name, "", item.Value));
                    if ((DataManager.selectedCity != null) && (item.Value.ID == DataManager.selectedCity.ID))
                        selectedIndex = filterList.Count - 1;
                }
            }
            else if (DataManager.filterToSelect is Division)
            {
                filterKind = "Выберите подразделение";
                foreach (var item in DataManager.divisions)
                {
                    if ((DataManager.selectedCity == null) || (item.Value.city.ID == DataManager.selectedCity.ID) || (DataManager.selectedCity.ID == 0) || (item.Value.ID == 0))
                    {
                        filterList.Add(new FilterObject("", item.Value.name + " (" + item.Value.city.name + ")", "", item.Value));
                        if ((DataManager.selectedDivision != null) && (item.Value.ID == DataManager.selectedDivision.ID))
                            selectedIndex = filterList.Count - 1;
                    }
                }
            }
            else if (DataManager.filterToSelect is MachineType)
            {
                filterKind = "Выберите тип оборудования";
                foreach (var item in DataManager.machineTypes)
                {
                    filterList.Add(new FilterObject("", item.Value.name, item.Value.iconName, item.Value));
                    if ((DataManager.selectedMachineType != null) && (item.Value.ID == DataManager.selectedMachineType.ID))
                        selectedIndex = filterList.Count - 1;
                }
            }
            else if (DataManager.filterToSelect is MachineState)
            {
                filterKind = "Выберите рабочее состояние";
                foreach (var item in DataManager.machineStates)
                {
                    filterList.Add(new FilterObject("", item.Value.name, item.Value.iconName, item.Value));
                    if ((DataManager.selectedMachineState != null) && (item.Value.ID == DataManager.selectedMachineState.ID))
                        selectedIndex = filterList.Count - 1;
                }
            }
            else if (DataManager.filterToSelect is MachineServiceState)
            {
                filterKind = "Выберите сервисное состояние";
                foreach (var item in DataManager.machineServiceStates)
                {
                    filterList.Add(new FilterObject("", item.Value.name, item.Value.iconName, item.Value));
                    if ((DataManager.selectedMachineServiceState != null) && (item.Value.ID == DataManager.selectedMachineServiceState.ID))
                        selectedIndex = filterList.Count - 1;
                }
            }

            FilterTable.ReloadData();

            if (filterList.Count != 0)
                FilterTable.SelectRow(NSIndexPath.FromRowSection(selectedIndex, 0), false, UITableViewScrollPosition.Middle);
        }
    }




    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class FilterTableSource : UITableViewSource
    {
        string cellIdentifier = "FilterCell";
        public bool signalRowSelected = false;

        public List<FilterObject> filterList;

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public FilterTableSource(List<FilterObject> filterList)
        {
            this.filterList = filterList;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return filterList.Count;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as FilterTableCell;

            if (indexPath.Row < filterList.Count)
            {
                var item = filterList[(ushort)indexPath.Row];

                cell.LeftTitle = item.leftTitle;
                cell.MainTitle = item.mainTitle;
                cell.Icon = item.iconName;

                cell.UserInteractionEnabled = true;

                cell.SetLayout();

                if (indexPath.Row == 0)
                {
                    cell.MainTitle = "Все";
                    cell.SetMainTitleOnMiddle();
                }
            }

            return cell;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (filterList.Count <= indexPath.Row)
                return;
            
            Object selectedObject = filterList [indexPath.Row].obj;

            if (DataManager.filterToSelect is City)
                DataManager.selectedCity = (City)selectedObject;
            
            else if (DataManager.filterToSelect is Division)
                DataManager.selectedDivision = (Division)selectedObject;
            
            else if (DataManager.filterToSelect is MachineType)
                DataManager.selectedMachineType = (MachineType)selectedObject;
            
            else if (DataManager.filterToSelect is MachineState)
                DataManager.selectedMachineState = (MachineState)selectedObject;
            
            else if (DataManager.filterToSelect is MachineServiceState)
                DataManager.selectedMachineServiceState = (MachineServiceState)selectedObject;

            Application.StoreValues();

            signalRowSelected = true;
        }
    }


}