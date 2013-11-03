using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public class tileReq
    {
        public tileReq(int x, int y, int depth, bool bKeepLoaded)
        {
            this.x = x;
            this.y = y;
            this.depth = depth;
            this.bKeepLoaded = bKeepLoaded;
        }

        public string path;
        public Rectangle rec;
        public int x;
        public int y;
        public int depth;
        public bool bKeepLoaded;
    }
    class tileNfo
    {
        public tileNfo(tileReq req)
        {
            if (File.Exists(req.path))
            {
                this.bKeepLoaded = req.bKeepLoaded;
                this.path = req.path;
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

        public string path;
        public Bitmap bitmap;
        public long ticks;
        public bool bKeepLoaded = false;
    }
}
