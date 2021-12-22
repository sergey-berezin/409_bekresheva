using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
    }
}
