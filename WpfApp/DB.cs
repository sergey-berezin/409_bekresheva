using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YOLOv4MLNet.DataStructures;

namespace WpfApp
{
    public class DB
    {
        //ViewModel vm;
        PicturesContext db;
        public DB() {
            db = new PicturesContext();
        }
        public bool IsReady()
        {
            if (/*vm==null ||*/ db==null)
                return false;
            return true;
        }
        
        //public void Reset (/*ViewModel _vm,*/ PicturesContext _db)
        //{
        //    //vm = _vm;
        //    db = _db;
        //}
        public void SaveOneToDB(YoloV4Result _res, string image_name)
        {
            lock (db)
            {
                var res = new Result { Label = _res.Label, x1 = _res.BBox[0], y1 = _res.BBox[1], x2 = _res.BBox[2], y2 = _res.BBox[3] };
                //Picture не сохранена!!!
                //var r = db.Results.Where(r => !r.Label.Equals(res.Label)).Where(r => !r.x1.Equals(res.x1)).Where(r => !r.x2.Equals(res.x2)).Where(r => !r.y1.Equals(res.y1)).Where(r => !r.y2.Equals(res.y2));
                //if (r.Count() == 0)
                    db.Results.Add(res);
                //else
                //{
                //    var r2 = r.Include(r => r.Picture).Where(r => !r.Picture.Photo.SequenceEqual(res.Picture.Photo));
                //    if (r2.Count() == 0)
                //        db.Add(res);
                //}
                db.SaveChanges();
            }
        }
        public void DeleteFromDB(Result res) 
        {
            foreach(var r in db.Results.Where(r => r.Label.Equals(res.Label)).Where(r => r.x1.Equals(res.x1)).Where(r => r.x2.Equals(res.x2)).Where(r => r.y1.Equals(res.y1)).Where(r => r.y2.Equals(res.y2)))
            {
                db.Entry(r).Reference(r => r.Picture).Load();
                if (r.Picture.Photo.SequenceEqual(res.Picture.Photo))
                    db.Remove(r);
            }
            db.SaveChanges();
        }
    }
}
