using Foundation;
using System;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class FiltersListController : UIViewController
    {
        private byte needDataUpdate = 0;

        public FiltersListController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            Title = "Настройки";
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            UpdateViewValues();

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateViewValues();
            UpdateUserValue();

            SetLayout();

            Application.StartUpdateTimer(CheckNewData);

            /*DataManager.SheduleGetMessageSubjectsRequest(null);
            DataManager.SheduleGetMessagesRequest(0, DataUpdateCallback);
            DataManager.SheduleSendMessageRequest(Settings.adminUserID, 4, null, "Как использовать фильтры2?", null);*/
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
            ViewUtils.ExpandWidth(CityButton, 3);
            ViewUtils.ExpandWidth(DivisionButton, 3);
            ViewUtils.ExpandWidth(TypeButton, 3);
            ViewUtils.ExpandWidth(StateButton, 3);
            ViewUtils.ExpandWidth(ServiceStateButton, 3);
            ViewUtils.ExpandWidth(UserButton, 3);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                Title = "Настройки";
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                Title = "Нет авторизации";
            else
                Title = "Нет связи";

            UpdateUserValue();

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateUserValue()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                UserButton.SetTitle(DataManager.UserName, UIControlState.Normal);
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                UserButton.SetTitle("Нет авторизации", UIControlState.Normal);
            else
                UserButton.SetTitle("Нет связи", UIControlState.Normal);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues()
        {
            if (DataManager.selectedCity != null)
                CityButton.SetTitle(DataManager.selectedCity.name, UIControlState.Normal);
            if (DataManager.selectedDivision != null)
                DivisionButton.SetTitle(DataManager.selectedDivision.name, UIControlState.Normal);
            if (DataManager.selectedMachineType != null)
            {
                TypeButton.SetTitle(DataManager.selectedMachineType.name, UIControlState.Normal);
                TypeImageButton.SetBackgroundImage(UIImage.FromFile(DataManager.selectedMachineType.iconName), UIControlState.Normal);
            }
            if (DataManager.selectedMachineState != null)
            {
                StateButton.SetTitle(DataManager.selectedMachineState.name, UIControlState.Normal);
                StateImageButton.SetBackgroundImage(UIImage.FromFile(DataManager.selectedMachineState.iconName), UIControlState.Normal);
            }
            if (DataManager.selectedMachineServiceState != null)
            {
                ServiceStateButton.SetTitle(DataManager.selectedMachineServiceState.name, UIControlState.Normal);
                ServiceStateImageButton.SetBackgroundImage(UIImage.FromFile(DataManager.selectedMachineServiceState.iconName), UIControlState.Normal);
            }

            OnlyMyButton.SetState(DataManager.onlyMyMachines, false);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void UserButton_TouchUpInside(UIButton sender)
        {
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("UserSettingsController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void CityButton_TouchUpInside(UIButton sender)
        {
            DataManager.filterToSelect = new City();
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("FilterController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void DivisionButton_TouchUpInside(UIButton sender)
        {
            DataManager.filterToSelect = new Division();
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("FilterController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void TypeButton_TouchUpInside(UIButton sender)
        {
            DataManager.filterToSelect = new MachineType();
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("FilterController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void StateButton_TouchUpInside(UIButton sender)
        {
            DataManager.filterToSelect = new MachineState();
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("FilterController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ServiceStateButton_TouchUpInside(UIButton sender)
        {
            DataManager.filterToSelect = new MachineServiceState();
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("FilterController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void onlMySwitchChanged(UISwitch sender)
        {
            DataManager.onlyMyMachines = OnlyMyButton.On;
            Application.StoreValues();
        }
    }
}