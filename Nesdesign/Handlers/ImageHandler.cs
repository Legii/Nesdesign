using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nesdesign
{
    public static class ImageHandler
    {
        private static readonly Dictionary<string, BitmapImage> _cache = new Dictionary<string, BitmapImage>();
        public static string imageTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "nesdesign_sp_z_o_o_logo.jpg");
        public static ImageSource LOGO = Get(imageTemplatePath);


        public static ImageSource Get(string path)
        {
            if(string.IsNullOrEmpty(path))
                return LOGO;
            if (_cache.TryGetValue(path, out var img))
                return img;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
      
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();

            _cache[path] = bitmap;
            return bitmap;
        }

        public static void Unload(string path)
        {
            if (_cache.ContainsKey(path))
                _cache.Remove(path);
        }

        public static ImageSource SafeGet(string path)
        {
            return File.Exists(path) ? Get(path) : LOGO;
        }
    }


    
}
