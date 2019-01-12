using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Dispatcher.Android.Helpers
{
    public static class ResourcesHelper
    {
        private static readonly AssetManager Assets = Application.Context.Assets;
        private static readonly Dictionary<string, Bitmap> CachedImages = new Dictionary<string, Bitmap>();

        public static Bitmap GetImageFromResources(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            if (CachedImages.ContainsKey(name))
            {
                return CachedImages[name];
            }

            try
            {
                var stream = Assets.List("").Contains(name) 
                    ? Assets.Open(name) 
                    : Assets.Open(name + ".png");
                
                var bitmap = BitmapFactory.DecodeStream(stream);

                if (!CachedImages.ContainsKey(name))
                    CachedImages.Add(name, bitmap);

                return bitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}