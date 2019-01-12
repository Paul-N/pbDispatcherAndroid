using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Dispatcher.Android
{
    public class MachineStateViewHolder : RecyclerView.ViewHolder
    {
        public ImageView IvState { get; }
        public TextView TvSateName { get; }
        public TextView TvStartDate { get; }
        public TextView TvHourGlass { get; }
        public TextView TvPeriod { get; }
        public TextView TvUser { get; }
        
        public MachineStateViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            IvState = itemView.FindViewById<ImageView>(Resource.Id.ivState);
            TvSateName = itemView.FindViewById<TextView>(Resource.Id.tvStateName);
            TvStartDate = itemView.FindViewById<TextView>(Resource.Id.tvStartTime);
            TvPeriod = itemView.FindViewById<TextView>(Resource.Id.tvTimePeriod);
            TvUser = itemView.FindViewById<TextView>(Resource.Id.tvUser);
            TvHourGlass = itemView.FindViewById<TextView>(Resource.Id.tvHourGlass);
            
            itemView.Click += (sender, e) => listener(LayoutPosition);
        }
    }
}