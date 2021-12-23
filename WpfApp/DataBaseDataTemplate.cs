using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using YOLOv4MLNet.DataStructures;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace WpfApp
{
    public class Result
    {
        public int ResultId { get; set; }
        [ConcurrencyCheck]
        public string Label { get; set; }
        
        [ConcurrencyCheck]
        public float x1 { get; set; }
        [ConcurrencyCheck]
        public float y1 { get; set; }
        [ConcurrencyCheck]
        public float x2 { get; set; }
        [ConcurrencyCheck]
        public float y2 { get; set; }
        [ConcurrencyCheck]
        virtual public Picture Picture { get; set; }
        public override string ToString()
        {
            return Label + $"x1={x1}, x2={x2}, y1={y1}, y2={y2}";
        }
    }
    public class Picture
    {
        public int PictureId { get; set; }
        [ConcurrencyCheck]
        public byte[] Photo { get; set; }

    }
    public class PicturesContext: DbContext
    {
        public DbSet<Result> Results { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlite("Data Source=library.db");
        private static byte[] ImageToBytes(CroppedBitmap img)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
        public void SaveOneToDB(YoloV4Result _res, string image_name)
        {
            lock (this)
            {
                var fullBitmap = new BitmapImage(new Uri(image_name));
                var ph = new CroppedBitmap(fullBitmap, new Int32Rect((int)_res.BBox[0], (int)_res.BBox[1], (int)(_res.BBox[2]-_res.BBox[0]), (int)(_res.BBox[3]- _res.BBox[1])));
                var pic = new Picture { Photo = ImageToBytes(ph) };
                var res = new Result { Label = _res.Label, x1 = _res.BBox[0], y1 = _res.BBox[1], x2 = _res.BBox[2], y2 = _res.BBox[3], Picture = pic};

                var r = Results.Where(r => r.Label.Equals(res.Label)).Where(r => r.x1.Equals(res.x1)).Where(r => r.x2.Equals(res.x2)).Where(r => r.y1.Equals(res.y1)).Where(r => r.y2.Equals(res.y2));
                if (r.Count() == 0)
                    Results.Add(res);
                else
                {
                    var r2 = r.Include(r => r.Picture).Where(r => r.Picture.Photo.SequenceEqual(res.Picture.Photo));
                    if (r2.Count() == 0)
                        Add(res);
                }
                SaveChanges();
            }
        }
    }
}
