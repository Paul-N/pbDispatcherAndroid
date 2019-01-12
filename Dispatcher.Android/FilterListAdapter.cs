using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Dispatcher.Android.Helpers;

namespace Dispatcher.Android
{
    public class FilterListAdapter : BaseAdapter<Filter>
    {
        private readonly Activity _activity;
        private readonly List<Filter> _filters;

        public FilterListAdapter(Activity context, List<Filter> filters)
        {
            _activity = context;
            _filters = filters;
        }
        
        public override int Count => _filters.Count;

        public override long GetItemId (int position)
        {
            return position;
        }

        public override Filter this[int index] => _filters [index];

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? 
                       _activity.LayoutInflater.Inflate (Resource.Layout.item_filter, parent, false);

            if (Count <= position) return view;
            
            var item = this [position];

            var tvFilterName = view.FindViewById<TextView>(Resource.Id.tvFilterName);
            if (position == 0)
            {
                item.Description = "Все";
                tvFilterName.Gravity = GravityFlags.Center;
            }
            else            
                tvFilterName.Gravity = GravityFlags.Left;
            
            
            tvFilterName.Text = item.Description;

            view.FindViewById<ImageView>(Resource.Id.ivFilterThumb)
                .SetImageBitmap(ResourcesHelper.GetImageFromResources(item.ImageName));
            
            return view;
        }
    }

    public class Filter
    {
        public string ImageName { get; set; }
        public string Description { get; set; }
    }
}