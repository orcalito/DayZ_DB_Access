using System;
using System.Collections.Generic;
using System.Drawing;

namespace DBAccess
{
    //
    //
    //
    public class MapHelper
    {
        public Tool.Point[] defBoundaries = new Tool.Point[2];
        public Tool.Point[] boundaries = new Tool.Point[2];
        public Tool.Point[] controls = new Tool.Point[4];
        public int isDraggingCtrlPoint;
        public bool enabled;
        private void AddPathDef(Tool.Point[] path, Pen pen = null)
        {
            PathDef def = new PathDef(pen);
            foreach (Tool.Point pt in path)
                def.points.Add(pt);
            paths.Add(def);
        }
        public MapHelper(VirtualMap map, int worldId)
        {
            this.map = map;

            foreach (Tool.Point[] arr in Tool.MapHelperDefs[worldId - 1])
                AddPathDef(arr);

            Tool.Point min = new Tool.Point(9999999, 9999999);
            Tool.Point max = new Tool.Point(-9999999, -9999999);
            foreach (PathDef _def in paths)
            {
                foreach (Tool.Point pt in _def.points)
                {
                    min = Tool.Point.Min(min, pt);
                    max = Tool.Point.Max(max, pt);
                }
            }
            Tool.Size size = (max - min);

            //  DB Map boundaries
            {
                Tool.Point[] points = new Tool.Point[]
                {
                    new Tool.Point(0, map.nfo.dbRefMapSize.Height),
                    new Tool.Point(map.nfo.dbRefMapSize.Width, map.nfo.dbRefMapSize.Height),
                    new Tool.Point(map.nfo.dbRefMapSize.Width, 0),
                    new Tool.Point(0, 0),
                    new Tool.Point(0, map.nfo.dbRefMapSize.Height)
                };
                AddPathDef(points, new Pen(Color.Red, 2));
            }

            foreach (PathDef _def in paths)
            {
                for (int i = 0; i < _def.points.Count; i++)
                {
                    Tool.Point pt = _def.points[i];
                    pt = (Tool.Point)((pt - min) / size);
                    _def.points[i] = pt;
                }
            }

            defBoundaries[0] = (Tool.Point)((new Tool.Point(0, map.nfo.dbRefMapSize.Height) - min) / size);
            defBoundaries[1] = (Tool.Point)((new Tool.Point(map.nfo.dbRefMapSize.Width, 0) - min) / size);

            //  DB bounding box
            {
                Tool.Point[] points = new Tool.Point[]
                {
                    new Tool.Point(0, 0),
                    new Tool.Point(0, 1),
                    new Tool.Point(1, 1),
                    new Tool.Point(1, 0),
                    new Tool.Point(0, 0)
                };
                AddPathDef(points, new Pen(Color.Green, 1));
            }

            // DB map boundaries
            boundaries[0] = map.nfo.dbMapOffsetUnit;
            boundaries[1] = boundaries[0] + map.nfo.dbMapSize / map.nfo.dbRefMapSize;

            // Control points
            Tool.Size Csize = (boundaries[1] - boundaries[0]) / (defBoundaries[1] - defBoundaries[0]);

            controls[0] = (Tool.Point)(boundaries[0] - defBoundaries[0] * Csize);
            controls[1] = controls[0] + Csize;

            controls[2] = new Tool.Point(controls[1].X, controls[0].Y);
            controls[3] = new Tool.Point(controls[0].X, controls[1].Y);
        }
        public int IntersectControl(Tool.Point pos, float radius)
        {
            for (int i = 0; i < 4; i++)
            {
                Tool.Point posInMap = controls[i] * map.SizeCorrected;

                float distance = (posInMap - pos).Lenght;

                if (distance <= radius)
                    return i;
            }

            return -1;
        }
        public void Display(Graphics gfx)
        {
            Tool.Point offset = controls[0] * map.SizeCorrected + map.Position;

            Tool.Size size = (controls[1] - controls[0]) * map.SizeCorrected;

            foreach (PathDef def in paths)
            {
                def.path.Reset();

                Tool.Point last = offset + def.points[0] * size;
                foreach (Tool.Point point in def.points)
                {
                    Tool.Point newpt = offset + point * size;
                    def.path.AddLine(last, newpt);
                    last = newpt;
                }

                gfx.DrawPath(def.pen, def.path);
            }

            int j = 0;
            foreach (Tool.Point point in controls)
            {
                Tool.Point pt = (point * map.SizeCorrected + map.Position).Truncate;

                Brush brush = (isDraggingCtrlPoint == j) ? brushSelected : brushUnselected;
                gfx.FillEllipse(brush, new Rectangle((int)pt.X - 5, (int)pt.Y - 5, 11, 11));
                j++;
            }
        }
        public void ControlPointUpdated(int idx)
        {
            if (idx < 2)
            {
                // Update 2 & 3
                controls[2] = new Tool.Point(controls[1].X, controls[0].Y);
                controls[3] = new Tool.Point(controls[0].X, controls[1].Y);
            }
            else
            {
                // Update 1 & 2
                controls[0] = new Tool.Point(controls[3].X, controls[2].Y);
                controls[1] = new Tool.Point(controls[2].X, controls[3].Y);
            }

            Tool.Size size = (controls[1] - controls[0]);

            boundaries[0] = controls[0] + defBoundaries[0] * size;
            boundaries[1] = controls[0] + defBoundaries[1] * size;
        }

        private SolidBrush brushUnselected = new SolidBrush(Color.Red);
        private SolidBrush brushSelected = new SolidBrush(Color.Green);
        private List<PathDef> paths = new List<PathDef>();
        private VirtualMap map;
    }
}
