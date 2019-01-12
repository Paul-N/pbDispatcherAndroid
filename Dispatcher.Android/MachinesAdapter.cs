using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Graphics;
using Android.Support.V7.Util;
using Android.Support.V7.Widget;
using Android.Views;
using DataUtils;
using Dispatcher.Android.Helpers;
using Java.Lang;

namespace Dispatcher.Android
{
    public class MachinesAdapter : RecyclerView.Adapter
    {
        private List<Machine> _machines;
        public event Action<int> ItemClicked;

        public MachinesAdapter(List<Machine> machines)
        {
            _machines = machines;
        }

        public void UpdateList(List<Machine> newList)
        {
            if (!_machines.Any())
            {
                _machines.AddRange(newList);
                NotifyDataSetChanged();
            }
            else
            {
                DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new MachineDiffCallback(_machines, newList), true);

                _machines.Clear();
                _machines = null;
                _machines = newList;

                result.DispatchUpdatesTo(this);
            }
        }

        public void ClearList()
        {
            var count = _machines.Count;
            _machines.Clear();

            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.item_machine, parent, false);
            
            var vh = new MachineViewHolder(itemView, OnClick);
            return vh;
        }              

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                if (!(holder is MachineViewHolder cell) || 
                    _machines.Count <= position) return;

                var item = _machines[position];

                cell.MachineIcon.SetImageBitmap(ResourcesHelper.GetImageFromResources(item.type.iconName));

                cell.MachineStateIcon.SetImageBitmap(item.serviceState.code > MachineServiceStateCodes.Work
                    ? ResourcesHelper.GetImageFromResources(item.serviceState.iconName)
                    : ResourcesHelper.GetImageFromResources(item.state.iconName));

                cell.Title.Text = item.GetNameStr();
                cell.Description.Text = item.GetDivisionStr();           

                if (item.sensors.Count != 0)
                {
                    var sensor = item.sensors[0];

                    cell.SensorIconCell.SetImageBitmap(ResourcesHelper.GetImageFromResources(sensor.type.iconName));
                    cell.MainValueCell.Text = sensor.mainValue.ToString("F2");
                    cell.MainValueSymbolCell.Text = sensor.type.mainValueSymbol;

                    var numberFormatInfo = new CultureInfo("en-Us", false).NumberFormat;
                    numberFormatInfo.NumberGroupSeparator = " ";
                    numberFormatInfo.NumberDecimalSeparator = ",";

                    cell.AdditionalValueCell.Text = sensor.additionalValue.ToString("N", numberFormatInfo);
                    cell.AdditionalValueSymbolCell.Text = sensor.type.additionalValueSymbol;
                }
                else
                {
                    cell.SensorIconCell.SetImageBitmap(null);
                    cell.MainValueCell.Text = "";
                    cell.MainValueSymbolCell.Text = "";
                    cell.AdditionalValueCell.Text = "";
                    cell.AdditionalValueSymbolCell.Text = "";
                }

                cell.SetColor(Color.Black);

                if (item.state.code == MachineStateCodes.Failure || item.serviceState.code == MachineServiceStateCodes.Broken)
                    cell.SetColor(Color.Red);
                else if (item.divisionPosition.ID != item.divisionOwner.ID && (item.divisionPosition.ID != 0))
                    cell.SetColor(Color.Argb(128, 255, 0, 0));

                if (item.sensors.Count != 0)
                {
                    if ((DateTime.Now - item.sensors[0].lastTime).TotalMinutes > Settings.greyOfflineMinutes)
                        cell.SetColor(Color.Gray);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
               
        public override int ItemCount => _machines.Count;

        private void OnClick(int position)
        {
            ItemClicked?.Invoke(position);
        }
    }
}