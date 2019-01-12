using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Dispatcher.Android
{
    public class MachineViewHolder : RecyclerView.ViewHolder
    {
        public ImageView MachineIcon { get; private set; }

        public ImageView MachineStateIcon { get; private set; }

        public ImageView SensorIconCell { get; private set; }     

        public TextView Title { get; private set; }
        public TextView Description { get; private set; }

        public TextView MainValueCell { get; private set; }
        public TextView MainValueSymbolCell { get; private set; }

        public TextView AdditionalValueCell { get; private set; }
        public TextView AdditionalValueSymbolCell { get; private set; }       

        public MachineViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MachineIcon = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            SensorIconCell = itemView.FindViewById<ImageView>(Resource.Id.stateImageView);
            MachineStateIcon = itemView.FindViewById<ImageView>(Resource.Id.sensorImageView);

            Title = itemView.FindViewById<TextView>(Resource.Id.nameTextView);
            Description = itemView.FindViewById<TextView>(Resource.Id.descTextView);
            MainValueCell = itemView.FindViewById<TextView>(Resource.Id.mainValueTextView);
            MainValueSymbolCell = itemView.FindViewById<TextView>(Resource.Id.mainValueSymbolTextView);
            AdditionalValueCell = itemView.FindViewById<TextView>(Resource.Id.addValueTextView);
            AdditionalValueSymbolCell = itemView.FindViewById<TextView>(Resource.Id.addValueSymbolTextView);

            itemView.Click += (senser, args) => listener(LayoutPosition);
        }      

        public void SetColor(Color color)
        {
            Title.SetTextColor(color);
            Description.SetTextColor(color);
            MainValueCell.SetTextColor(color);
            MainValueSymbolCell.SetTextColor(color);
            AdditionalValueCell.SetTextColor(color);
            AdditionalValueSymbolCell.SetTextColor(color);         
        }
    }
}