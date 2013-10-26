using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DBAccess
{
    //
    //
    //
    internal class MySplitContainer : SplitContainer
    {
        public MySplitContainer()
        {
            MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
            mi.Invoke(this.Panel1, args);
            mi.Invoke(this.Panel2, args);
        }
    }
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
            Tool.Size maxSize = new Tool.Size(/*2**/(int)nfo.defTileSize.Width << (nfo.depth - 1),
                /*2**/(int)nfo.defTileSize.Height << (nfo.depth - 1));

            Tool.Size temp = nfo.defTileSize * zoom;

            Size = Tool.Size.Max(nfo.defTileSize, Tool.Size.Min(maxSize, temp));

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

        public class BitmapNfo
        {
            public string tileBasePath = "";
            public int depth = 0;
            public Tool.Size defTileSize = new Tool.Size(1, 1);
            public Tool.Size dbMapSize = new Tool.Size(1, 1);
            public Tool.Point dbMapOffsetUnit = Tool.Point.Empty;
            public Tool.Size dbRefMapSize = new Tool.Size(1, 1);
        }
    }
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

        private class PathDef
        {
            private static Pen defPen = new Pen(Color.Black, 1);

            public PathDef(Pen pen = null)
            {
                if (pen == null)
                    pen = defPen;

                this.pen = pen;
            }
            public Pen pen;
            public GraphicsPath path = new GraphicsPath();
            public List<Tool.Point> points = new List<Tool.Point>();
        }
        private SolidBrush brushUnselected = new SolidBrush(Color.Red);
        private SolidBrush brushSelected = new SolidBrush(Color.Green);
        private List<PathDef> paths = new List<PathDef>();
        private VirtualMap map;
    }
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
    //
    //
    //
    public class DragNDrop
    {
        public void Start(List<Tool.Point> refPos)
        {
            refPositions = refPos;
            offset = Tool.Size.Empty;
            lastPositionMouse = Form.MousePosition;
        }
        public void Start(Point refPos)
        {
            refPositions = new List<Tool.Point>();
            refPositions.Add(refPos);
            offset = Tool.Size.Empty;
            lastPositionMouse = Form.MousePosition;
        }
        public void Update()
        {
            offset = Form.MousePosition - lastPositionMouse;
        }
        public void Stop()
        {
            offset = Tool.Size.Empty;
        }

        public Tool.Size Offset { get { return offset; } }
        public Tool.Point Position(int idx) { return refPositions[idx] + offset; }

        private List<Tool.Point> refPositions;
        private Tool.Size offset = Tool.Size.Empty;
        private Tool.Point lastPositionMouse;
    }
    //
    //
    //
    public class myConfig
    {
        public myConfig()
        {
        }
        public myConfig(ModuleVersion curVers)
        {
            cfgVersion = curVers;
            worlds_def = new DataSet();
            vehicle_types = new DataSet();
            deployable_types = new DataSet();
        }
        public string game_type;
        public string url;
        public string port;
        public string basename;
        public string username;
        public string password;
        public string instance_id;
        public string vehicle_limit;
        public string body_time_limit;
        public string tent_time_limit;
        public string online_time_limit;
        public string customscript1;
        public string customscript2;
        public string customscript3;
        public ModuleVersion cfgVersion { get; set; }
        public DataSet worlds_def { get; set; }
        public DataSet vehicle_types { get; set; }
        public DataSet deployable_types { get; set; }
        //
        [XmlIgnore]
        public UInt16 world_id = 0;
    }
    //
    //
    //
    public class myIcon
    {
        public myIcon()
        {
        }

        // Wrap event invocations inside a protected virtual method 
        // to allow derived classes to override the event invocation behavior 
        public void OnClick(Control parent, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left) && (Click != null))
            {
                Click(this, e);
            }
            else if (e.Button.HasFlag(MouseButtons.Right) && (contextMenuStrip != null))
            {
                contextMenuStrip.Show(parent, e.Location);
            }
        }

        public Image image;
        public iconDB iconDB;
        public Rectangle rectangle;
        public Tool.Point Location { get { return rectangle.Location; } set { rectangle.Location = value; } }
        public Tool.Size Size { get { return rectangle.Size; } set { rectangle.Size = value; } }

        public event MouseEventHandler Click;
        public ContextMenuStrip contextMenuStrip;
    }
    //
    //
    //
    public class UIDGraph
    {
        public static Tool.Point InvalidPos = new Tool.Point(float.NaN, float.NaN);

        public UIDGraph(Pen pen)
        {
            this.pen = pen;
        }
        public void AddPoint(Tool.Point pos)
        {
            if ((positions.Count == 0) || (pos != positions.Last()))
                positions.Add(pos);
        }
        public void DisplayInMap(Graphics gfx, VirtualMap map)
        {
            if (positions.Count > 1)
            {
                path.Reset();

                Tool.Point last = InvalidPos;
                foreach (Tool.Point pt in positions)
                {
                    Tool.Point newpt = map.UnitToPanel(pt);

                    // if a point is invalid, break the continuity
                    if (!(last.IsNaN || pt.IsNaN))
                        path.AddLine((System.Drawing.Point)last, (System.Drawing.Point)newpt);

                    last = newpt;
                }

                gfx.DrawPath(pen, path);
            }
        }

        internal GraphicsPath path = new GraphicsPath();
        internal Pen pen;
        internal List<Tool.Point> positions = new List<Tool.Point>();

        internal void RemoveLastPoint()
        {
            if (positions.Count > 0)
                positions.RemoveAt(positions.Count - 1);
        }
    }
    //
    //
    //
    public enum UIDType : ulong
    {
        TypePlayer = 1,
        TypeVehicle,
        TypeSpawn,
        TypeDeployable
    };
    //
    //
    //
    public class iconDB
    {
        public myIcon icon;
        public DataRow row;
        public Tool.Point pos;
        public string uid = "0";
        public UIDType type;
    };

    #region PropertyGrid specific classes
    //
    //
    //
    internal class EntryConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is Entry)
            {
                Entry entry = (Entry)value;
                return entry.count.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
    //
    //
    //
    internal class NullConverter<T> : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string))
            {
                if (value is Cargo)
                {
                    Cargo cargo = value as Cargo;
                    return "total = " + cargo.Total;
                }
                else if (value is T)
                {
                    return "";
                }
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
    //
    //
    //
    internal class NullEditor : CollectionEditor
    {
        public NullEditor(Type type) : base(type) { }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
    }
    //
    //
    //
    internal class CargoPropertyDescriptor : PropertyDescriptor
    {
        private Cargo collection = null;
        private int index = -1;

        public CargoPropertyDescriptor(Cargo coll, int idx) :
            base("#" + idx.ToString(), null)
        {
            this.collection = coll;
            this.index = idx;
        }

        public override AttributeCollection Attributes { get { return new AttributeCollection(null); } }
        public override bool CanResetValue(object component) { return true; }
        public override Type ComponentType { get { return this.collection.GetType(); } }

        public override string DisplayName
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                Entry entry = this.collection[index];
                return entry.name;
            }
        }

        public override string Description
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                Entry entry = this.collection[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(entry.name + " : " + entry.count);
                return sb.ToString();
            }
        }

        public override object GetValue(object component) { return this.collection[index]; }
        public override bool IsReadOnly { get { return true; } }
        public override string Name { get { return "#" + index.ToString(); } }
        public override Type PropertyType { get { return this.collection[index].GetType(); } }
        public override void ResetValue(object component) { }
        public override bool ShouldSerializeValue(object component) { return true; }
        public override void SetValue(object component, object value) { }
    };
    //
    //
    //
    [TypeConverterAttribute(typeof(NullConverter<Cargo>)), EditorAttribute(typeof(NullEditor), typeof(UITypeEditor))]
    public class Cargo : CollectionBase, ICustomTypeDescriptor
    {
        #region Collection Implementation

        public void Add(Entry zone) { this.List.Add(zone); }
        public void Remove(Entry zone) { this.List.Remove(zone); }
        public Entry this[int index] { get { return (Entry)this.List[index]; } }

        #endregion

        // Implementation of interface ICustomTypeDescriptor 
        #region ICustomTypeDescriptor impl

        public String GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
        public String GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }
        public PropertyDescriptorCollection GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++)
            {
                // Create a property descriptor for the zone item and add to the property descriptor collection
                CargoPropertyDescriptor pd = new CargoPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection
            return pds;
        }

        #endregion

        public int Total
        {
            get
            {
                int total = 0;
                foreach (Entry entry in List)
                    total += entry.count;
                return total;
            }
        }
    }
    //
    //
    //
    [TypeConverter(typeof(EntryConverter))]
    public class Entry
    {
        public Entry(string n = "", int c = 0)
        {
            name = n;
            count = c;
        }

        public string name { get; set; }
        public int count { get; set; }
    };
    //
    //
    //
    [TypeConverterAttribute(typeof(NullConverter<Storage>))]
    public class Storage
    {
        public Storage()
        {
            weapons = new Cargo();
            items = new Cargo();
            bags = new Cargo();
        }

        public Cargo weapons { get; set; }
        public Cargo items { get; set; }
        public Cargo bags { get; set; }
    };

    //
    //
    //
    internal class PartsPropertyDescriptor : PropertyDescriptor
    {
        private VParts collection = null;
        private int index = -1;

        public PartsPropertyDescriptor(VParts coll, int idx) :
            base("#" + idx.ToString(), null)
        {
            this.collection = coll;
            this.index = idx;
        }

        public override AttributeCollection Attributes { get { return new AttributeCollection(null); } }
        public override bool CanResetValue(object component) { return true; }
        public override Type ComponentType { get { return this.collection.GetType(); } }

        public override string DisplayName
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                VEntry entry = this.collection[index];
                return entry.name;
            }
        }

        public override string Description
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                VEntry entry = this.collection[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(entry.name + " : " + entry.damage);
                return sb.ToString();
            }
        }

        public override object GetValue(object component) { return this.collection[index]; }
        public override bool IsReadOnly { get { return true; } }
        public override string Name { get { return "#" + index.ToString(); } }
        public override Type PropertyType { get { return this.collection[index].GetType(); } }
        public override void ResetValue(object component) { }
        public override bool ShouldSerializeValue(object component) { return true; }
        public override void SetValue(object component, object value) { }
    };
    //
    //
    //
    internal class VEntryConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is VEntry)
            {
                VEntry entry = (VEntry)value;
                return entry.damage.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
    //
    //
    //
    [TypeConverter(typeof(VEntryConverter))]
    public class VEntry
    {
        public VEntry(string n = "", float d = 0)
        {
            name = n;
            damage = d;
        }

        public string name { get; set; }
        public float damage { get; set; }
    };
    [TypeConverterAttribute(typeof(NullConverter<VParts>)), EditorAttribute(typeof(NullEditor), typeof(UITypeEditor))]
    public class VParts : CollectionBase, ICustomTypeDescriptor
    {
        #region Collection Implementation

        public void Add(VEntry zone) { this.List.Add(zone); }
        public void Remove(VEntry zone) { this.List.Remove(zone); }
        public VEntry this[int index] { get { return (VEntry)this.List[index]; } }

        #endregion

        // Implementation of interface ICustomTypeDescriptor 
        #region ICustomTypeDescriptor impl

        public String GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
        public String GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }
        public PropertyDescriptorCollection GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++)
            {
                // Create a property descriptor for the zone item and add to the property descriptor collection
                PartsPropertyDescriptor pd = new PartsPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection
            return pds;
        }

        #endregion

        public int Total { get { return List.Count; } }
    }

    #endregion
}