using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Appl;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class FilterListActivity : BaseActivity
    {
        private const float UpdateInterval = 100f;
        private TimerHolder _timerHolder;
        
        private string _filterKind = string.Empty;
        private readonly List<FilterObject> _filterList = new List<FilterObject>();

        private ListView _listView;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _timerHolder = new TimerHolder(UpdateInterval, CheckNewData);
            
            SetContentView(Resource.Layout.activity_filters_list);
            InitActionBar();
            
            _listView = FindViewById<ListView>(Resource.Id.lvFilters);
            _listView.ChoiceMode = ChoiceMode.Single;
            _listView.ItemClick += OnListItemClick;
        }
        
        protected override void InitDataUpdating()
        {
            _timerHolder.Start();
        }

        protected override void OnStart()
        {
            base.OnStart();

            _timerHolder.Start();
        }

        protected override async void OnResume()
        {
            base.OnResume();

            await Task.Delay(100);

            UpdateViewValues();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _timerHolder.Stop();
        }
        
        private void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                UpdateTitle(_filterKind);
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                UpdateTitle(Resource.String.no_authorization);
            else
                UpdateTitle(Resource.String.no_connection);            
        }
        
        private void UpdateViewValues()
        {
            var selectedIndex = 0;

            _filterList.Clear();

            var filterType = AppSession.SelectedFilterType;

            if (filterType == eFilterType.City)
            {
                _filterKind = "Выберите город";
                foreach (var item in DataManager.cities)
                {
                    _filterList.Add(new FilterObject("", item.Value.name, "", item.Value));
                    if ((DataManager.selectedCity != null) && (item.Value.ID == DataManager.selectedCity.ID))
                        selectedIndex = _filterList.Count - 1;
                }
            }
            else if (filterType == eFilterType.Division)
            {
                _filterKind = "Выберите подразделение";
                foreach (var item in DataManager.divisions)
                {
                    if ((DataManager.selectedCity == null) || (item.Value.city.ID == DataManager.selectedCity.ID) || (DataManager.selectedCity.ID == 0) || (item.Value.ID == 0))
                    {
                        _filterList.Add(new FilterObject("", item.Value.name + " (" + item.Value.city.name + ")", "", item.Value));
                        if ((DataManager.selectedDivision != null) && (item.Value.ID == DataManager.selectedDivision.ID))
                            selectedIndex = _filterList.Count - 1;
                    }
                }
            }
            else if (filterType == eFilterType.FacilityType)
            {
                _filterKind = "Выберите тип оборудования";
                foreach (var item in DataManager.machineTypes)
                {
                    _filterList.Add(new FilterObject("", item.Value.name, item.Value.iconName, item.Value));
                    if ((DataManager.selectedMachineType != null) && (item.Value.ID == DataManager.selectedMachineType.ID))
                        selectedIndex = _filterList.Count - 1;
                }
            }
            else if (filterType == eFilterType.WorkState)
            {
                _filterKind = "Выберите рабочее состояние";
                foreach (var item in DataManager.machineStates)
                {
                    _filterList.Add(new FilterObject("", item.Value.name, item.Value.iconName, item.Value));
                    if ((DataManager.selectedMachineState != null) && (item.Value.ID == DataManager.selectedMachineState.ID))
                        selectedIndex = _filterList.Count - 1;
                }
            }
            else if (filterType == eFilterType.ServiceState)
            {
                _filterKind = "Выберите сервисное состояние";
                foreach (var item in DataManager.machineServiceStates)
                {
                    _filterList.Add(new FilterObject("", item.Value.name, item.Value.iconName, item.Value));
                    if ((DataManager.selectedMachineServiceState != null) && (item.Value.ID == DataManager.selectedMachineServiceState.ID))
                        selectedIndex = _filterList.Count - 1;
                }
            }

            _listView.Adapter = new FilterListAdapter(this, _filterList.Select(f => new Filter()
            {
                ImageName = f.iconName,
                Description = f.mainTitle
            }).ToList());           

            if (_filterList.Count != 0)
            {
                _listView.RequestFocusFromTouch();
                _listView.SetSelection(selectedIndex);               
            }                
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (_filterList.Count <= e.Position) return;
            
            var selectedObject = _filterList[e.Position].obj;

            if (selectedObject is City city)
                DataManager.selectedCity = city;
            
            else if (selectedObject is Division division)
                DataManager.selectedDivision = division;
            
            else if (selectedObject is MachineType type)
                DataManager.selectedMachineType = type;
            
            else if (selectedObject is MachineState state)
                DataManager.selectedMachineState = state;
            
            else if (selectedObject is MachineServiceState serviceState)
                DataManager.selectedMachineServiceState = serviceState;
            
            DataUtils.StoreValues();
           
            OnBackPressed();                       
        }
    }
}