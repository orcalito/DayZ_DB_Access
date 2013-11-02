using System;
using System.Drawing;
using System.IO;

namespace DBAccess
{
    //
    //
    //
    public class VirtualMap
    {
        public VirtualMap()
        {
            Position = Tool.Point.Empty;
            Size = new Tool.Size(400, 400);
        }

        public bool Enabled { get { return nfo.depth > 0; } }

        public Tool.Point Position;
        public Tool.Size Size
        {
            get { return _size; }
            set { _size = value; UpdateData(); }
        }
        public BitmapNfo nfo = new BitmapNfo();

        public Tool.Size SizeCorrected { get { return _sizeCorrected; } }
        public Tool.Size DefTileSize { get { return nfo.defTileSize; } }
        public Tool.Size TileCount { get { return _tileCount; } }
        public Tool.Size TileSize { get { return _tileSize; } }
        public int Depth { get { return _depth; } }
        public Rectangle TileRectangle(Tool.Point p)
        {
            return new Rectangle(Position + p * _tileSize, _tileSize);
        }
        public Tool.Point UnitToPanel(Tool.Point from)
        {
            //                return Position + from * SizeCorrected;
            Tool.Size ratio = this.nfo.dbMapSize / this.nfo.dbRefMapSize;
            Tool.Point unitPos = from * ratio + this.nfo.dbMapOffsetUnit;

            return this.Position + unitPos * this.SizeCorrected;
        }
        public Tool.Point UnitToPanel(iconDB from)
        {
            Tool.Size ratio = this.nfo.dbMapSize / this.nfo.dbRefMapSize;
            Tool.Point unitPos = from.pos * ratio + this.nfo.dbMapOffsetUnit;

            return this.Position + unitPos * this.SizeCorrected - from.icon.Size * 0.5f;
        }
        public Tool.Point PanelToUnit(Tool.Point from)
        {
            Tool.Point unitInMap = (Tool.Point)((from - this.Position) / this.SizeCorrected);
            unitInMap = (Tool.Point)(unitInMap - this.nfo.dbMapOffsetUnit);
            Tool.Size ratio = this.nfo.dbRefMapSize / this.nfo.dbMapSize;

            Tool.Point pt = unitInMap * ratio;
            pt.Y = 1.0f - pt.Y;

            return pt;
        }
        public Tool.Point UnitToDB(Tool.Point from)
        {
            return from * this.nfo.dbRefMapSize;
        }
        public Tool.Point UnitToMap(Tool.Point from)
        {
            Tool.Point pt = from;
            pt.Y = 1.0f - pt.Y;
            return pt * this.nfo.dbRefMapSize;
        }
        public float ResizeFromZoom(float zoom)
        {
            Tool.Size minSize = nfo.defTileSize * (float)Math.Pow(2, nfo.min_depth);

            Tool.Size maxSize = new Tool.Size((int)nfo.defTileSize.Width << (nfo.depth - 1),
                                              (int)nfo.defTileSize.Height << (nfo.depth - 1));

            Tool.Size temp = nfo.defTileSize * zoom;

            Size = Tool.Size.Max(minSize, Tool.Size.Min(maxSize, temp));

            return Size.Width / nfo.defTileSize.Width;
        }
        public void SetRatio(Tool.Size ratio) { _ratio = ratio; }

        //
        private int _depth;
        private Tool.Size _size;
        private Tool.Size _sizeCorrected;
        private Tool.Size _tileCount;
        private Tool.Size _tileSize;
        private Tool.Size _ratio;

        private void UpdateData()
        {
            Tool.Size reqTileCount = (_size / nfo.defTileSize).UpperPowerOf2;

            _depth = (int)Math.Log(Math.Max(reqTileCount.Width, reqTileCount.Height), 2);

            // Clamp to min depth
            _depth = Math.Max(_depth, nfo.min_depth); 

            // Clamp to max depth
            _depth = Math.Min(_depth, nfo.depth - 1);

            // Clamp tile count
            Tool.Size maxSize = new Tool.Size(1 << _depth, 1 << _depth);

            _tileCount = Tool.Size.Min(reqTileCount, maxSize);

            _tileSize = (_size / _tileCount).Ceiling;

            // and re-adjust size from new tile size
            _size = _tileSize * _tileCount;
            _sizeCorrected = (_size * _ratio).Ceiling;
        }

        public void Calibrate()
        {
            nfo.min_depth = 0;
            while (Directory.Exists(nfo.tileBasePath + nfo.min_depth) == false)
                nfo.min_depth++;
        }

        public class BitmapNfo
        {
            public string tileBasePath = "";
            public int depth = 0;
            public int min_depth = 0;
            public Tool.Size defTileSize = new Tool.Size(1, 1);
            public Tool.Size dbMapSize = new Tool.Size(1, 1);
            public Tool.Point dbMapOffsetUnit = Tool.Point.Empty;
            public Tool.Size dbRefMapSize = new Tool.Size(1, 1);
        }
    }
}
