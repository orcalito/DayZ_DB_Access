using System;
using System.Threading;

namespace DBAccess
{
    //
    //
    //
    public class MapZoom
    {
        public MapZoom(EventWaitHandle evtHandle)
        {
            this.evtHandle = evtHandle;
        }

        public void Start(VirtualMap map, Tool.Point center, int depthDir)
        {
            this.centerUnit = center / map.SizeCorrected;

            int newDepth = this.destDepth + depthDir;

            if (newDepth >= 0 && newDepth <= map.nfo.depth - 1)
            {
                this.destDepth = newDepth;
                this.evtHandle.Set();
            }
        }

        internal bool Update(VirtualMap map)
        {
            Tool.Point center = (centerUnit * map.SizeCorrected).Truncate;

            double deltaDepth = this.destDepth - this.currDepth;
            if (Math.Abs(deltaDepth) > depthSpeed)
            {
                map.ResizeFromZoom((float)Math.Pow(2, currDepth));

                Tool.Point newPos = (centerUnit * map.SizeCorrected).Truncate;

                map.Position = map.Position - (newPos - center);

                this.currDepth += Math.Sign(deltaDepth) * depthSpeed;

                return true;
            }
            else
            {
                this.currDepth = this.destDepth;
                map.ResizeFromZoom((float)Math.Pow(2, currDepth));

                Tool.Point newPos = (centerUnit * map.SizeCorrected).Truncate;

                map.Position = map.Position - (newPos - center);
            }

            return false;
        }

        public Tool.Point centerUnit;
        public double currDepth = 0;
        public int destDepth = 0;

        private EventWaitHandle evtHandle;
        private static float depthSpeed = 0.08f;
    }
}
