using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Util;
using Android.Views;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Extensions;

namespace Dispatcher.Android
{
    public class MachineDiffCallback : DiffUtil.Callback
    {
        private List<Machine> oldList;
        private List<Machine> newList;

        public MachineDiffCallback(List<Machine> oldList, List<Machine> newList)
        {
            this.oldList = oldList;
            this.newList = newList;
        }

        public override int NewListSize => newList.Count;

        public override int OldListSize => newList.Count;

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            return oldList[oldItemPosition].ID == newList[newItemPosition].ID;
        }

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            if (oldList.Count <= oldItemPosition) return false;

            return oldList[oldItemPosition].IsSame(newList[newItemPosition]);
        }
    }
}