using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using DataUtils;
using Dispatcher.Android.Helpers;

namespace Dispatcher.Android
{
    public class MachineStatesAdapter : RecyclerView.Adapter
    {
        private readonly List<MachineStatesLogElement> _statesLogs;
        public event Action<int> ItemClicked;
        
        public MachineStatesAdapter(List<MachineStatesLogElement> statesLogs)
        {
            _statesLogs = statesLogs;
        }
        
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.item_machine_sates_log, parent, false);
            
            var vh = new MachineStateViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is MachineStateViewHolder cell) || 
                _statesLogs.Count <= position) return;

            var item = _statesLogs[position];
            
            cell.IvState.SetImageBitmap(ResourcesHelper.GetImageFromResources(item.state.iconName));
            cell.TvSateName.Text = !string.IsNullOrEmpty(item.description) 
                ? item.description
                : item.state.name;
            cell.TvStartDate.Text = item.timeStart.ToString ("dd.MM.yy  HH:mm:ss");


            if (item.timeEnd > item.timeStart)
            {
                cell.TvPeriod.Text = FormatUtils.PeriodStr(item.timeStart, item.timeEnd);
                cell.TvHourGlass.Visibility = ViewStates.Visible;
            }
            else
            {
                cell.TvPeriod.Text = null;
                cell.TvHourGlass.Visibility = ViewStates.Invisible;
            }
            
            cell.TvUser.Text = item.userName;
        }

        public override int ItemCount => _statesLogs.Count;
        
        private void OnClick(int position)
        {
            ItemClicked?.Invoke(position);
        }
    }
}