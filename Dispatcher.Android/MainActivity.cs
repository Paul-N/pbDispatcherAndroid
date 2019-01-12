using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Appl;
using Dispatcher.Android.Extensions;
using Dispatcher.Android.Helpers;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private const float UpdateInterval = 100f;
        private TimerHolder _timerHolder;

        private byte _needDataUpdate;
        private DateTime _lastUpdateTime = DateTime.MinValue;
        
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private MachinesAdapter _adapter;
        private ImageView _pingIndicator;
        private TextView _tvTitle;
        private CustomScrollListener _scrollListener;
        
        private readonly List<Machine> _machines = new List<Machine>();
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            DataUtils.Init();
            _timerHolder = new TimerHolder(UpdateInterval, CheckNewData);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FindViewById<TextView>(Resource.Id.btnSettings)
                .Click += (sender, args) => StartSettingsActivity();
            
            _pingIndicator = FindViewById<ImageView>(Resource.Id.ivPing);
            _tvTitle = FindViewById<TextView>(Resource.Id.tvTitle);
            
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(_layoutManager);
            
            _adapter = new MachinesAdapter(_machines);
            _recyclerView.SetAdapter(_adapter);
            _adapter.ItemClicked += StartSelectedMachineActivity;
            
            _scrollListener = new CustomScrollListener();
            _recyclerView.AddOnScrollListener(_scrollListener);

            InitDataUpdating();
        }

        protected override void OnStart()
        {
            base.OnStart();            

            if (DataManager.LoginImpossible)
            {
                StartUserActivity();
                return;
            }

            if (_recyclerView.GetAdapter() == null)
            {
                _machines.Clear();
                _adapter = new MachinesAdapter(_machines);
                _recyclerView.SetAdapter(_adapter);
                _adapter.ItemClicked += StartSelectedMachineActivity;

                DataManager.SheduleGetMachinesRequest(DataUpdateCallback);
                _lastUpdateTime = DateTime.Now;
            }

            _timerHolder.Start();
        }        

        protected override void OnStop()
        {
            base.OnStop();
            
            _timerHolder.Stop();            
        }

        private void InitDataUpdating()
        {
            DataManager.SheduleGetMachinesRequest(DataUpdateCallback);
            _lastUpdateTime = DateTime.Now;

            _timerHolder.Start();
        }

        private void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
            {
                if (DataManager.selectedDivision != null)                
                    UpdateTitle(DataManager.selectedDivision.name);                
                else                
                    UpdateTitle(Resource.String.all);                

                if (DataManager.Ping < 200)
                    UpdatePingIndicatorImage("GreenCircle");
                else if (DataManager.Ping < 500)
                    UpdatePingIndicatorImage("YelowCircle");                
                else
                    UpdatePingIndicatorImage("RedCircle");                
            }
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
            {
                UpdateTitle(Resource.String.no_authorization);
                UpdatePingIndicatorImage("GreyCircle");                
            }
            else
            {
                UpdateTitle(Resource.String.no_connection);
                UpdatePingIndicatorImage("GreyCircle");
            }

            if (_needDataUpdate > 0)
            {
                _needDataUpdate--;

                UpdateViewValues();
                RunOnUiThread(() => _pingIndicator.Visibility = ViewStates.Visible);
            }
            else if (DateTime.Now.Subtract(_lastUpdateTime).TotalMilliseconds > Settings.updatePeriodMs &&
                     DataManager.machineTypes.Count != 0 &&
                     DataManager.NotAnsweredRequestsCount == 0)
            {
                DataManager.SheduleGetMachinesRequest(DataUpdateCallback);

                _lastUpdateTime = DateTime.Now;
                RunOnUiThread(() => _pingIndicator.Visibility = ViewStates.Invisible);
            }
        }

        private void UpdatePingIndicatorImage(string name)
        {
            RunOnUiThread(()=> _pingIndicator.SetImageBitmap(ResourcesHelper.GetImageFromResources(name)));
        }

        private void UpdateTitle(int resourceId)
        {
            RunOnUiThread(() => _tvTitle.SetText(resourceId));
        }

        private void UpdateTitle(string title)
        {
            RunOnUiThread(() => _tvTitle.Text = title);
        }

        private void UpdateViewValues()
        {
            RunOnUiThread(FillList);
        }

        private void DataUpdateCallback(object requestState)
        {
            _needDataUpdate++;
        }       

        private void StartSelectedMachineActivity(int position)
        {
            if (DataManager.machines.Count <= position) return;

            _timerHolder.Stop();
            
            AppSession.SelectedMachine = DataManager.machines[position];
            
            var intent = new Intent(this, typeof(MachineDetailsActivity));
            StartActivity(intent);           
        }

        private void StartSettingsActivity()
        {
            _recyclerView.SetAdapter(null);
            _adapter.ItemClicked -= StartSelectedMachineActivity;
            _adapter.Dispose();
            _adapter = null;

            var intent = new Intent(this, typeof(SettingsActivity));
            StartActivity(intent);
        }

        private void StartUserActivity()
        {
            var intent = new Intent();
            intent.SetClass(BaseContext, typeof(UserSettingsActivity));
            intent.SetFlags(ActivityFlags.ReorderToFront);
            StartActivity(intent);
        }
       
        private void FillList()
        {
            try
            {
                var machines = DataManager.machines;
                if (machines.Count <= 0) return;

                _adapter.UpdateList(DataManager.machines.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

