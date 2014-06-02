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
        public static int max_depth;

        public tileReq(int x, int y, int depth, bool bKeepLoaded, bool bDontDisplay)
        {
            this.x = x;
            this.y = y;
            this.depth = depth;
            this.bKeepLoaded = bKeepLoaded;
            this.bDontDisplay = bDontDisplay;
        }

        public string path;
        public Rectangle rec;
        public int x;
        public int y;
        public int depth;
        public bool bKeepLoaded;
        public bool bDontDisplay;

        public int Key
        {
            get
            {
                return (x << 0 | y << 12 | depth << 24);
            }
        }
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
            timeOut = (tileReq.max_depth - req.depth) * 10000000L;  // lowest level = smallest timeOut
        }
        ~tileNfo()
        {
            if (bitmap != null)
                bitmap.Dispose();
        }

        public string path;
        public Bitmap bitmap;
        public long ticks;
        public long timeOut;
        public bool bKeepLoaded = false;
    }
}
