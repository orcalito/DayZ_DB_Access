using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    class tileReq
    {
        public string path;
        public Rectangle rec;
    }
    class tileNfo
    {
        public tileNfo(string path)
        {
            bFileExists = File.Exists(path);
            if (bFileExists)
            {
                this.path = path;
                //this.bitmap = new Bitmap(path);
                using (var bmpTemp = new Bitmap(path))
                {
                    this.bitmap = new Bitmap(bmpTemp);
                }
            }
            ticks = DateTime.Now.Ticks;
        }
        ~tileNfo()
        {
            if (bitmap != null)
                bitmap.Dispose();
        }

        public bool bFileExists = false;
        public string path;
        public Bitmap bitmap;
        public long ticks;
    }
}
