using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace DBAccess
{
    //
    //
    //
    public class PathDef
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
            player_state = new DataSet();
        }
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
        public int filter_last_updated;
        public int bitmap_mag_level;
        public int world_id;
        public decimal db_refreshrate;
        public decimal be_refreshrate;
        public string rcon_port;
        public string rcon_url;
        public string rcon_password;
        public string rcon_adminname;
        public ModuleVersion cfgVersion { get; set; }
        public DataSet worlds_def { get; set; }
        public DataSet vehicle_types { get; set; }
        public DataSet deployable_types { get; set; }
        public DataSet player_state { get; set; }
        //
        //[XmlIgnore]
    }
    //
    //
    //
    public class myIcon
    {
        public myIcon()
        {
        }

        public void OnClick(Control parent, MouseEventArgs e)
        {
            if (/*e.Button.HasFlag(MouseButtons.Left) &&*/ (Click != null))
            {
                Click(this, e);
            }
            if (e.Button.HasFlag(MouseButtons.Right) && (contextMenuStrip != null))
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
        public static Tool.Point InvalidPos = new Tool.Point(0, 1);

        public UIDGraph(Pen pen)
        {
            this.pen = pen;
        }
        public void AddPoint(Tool.Point pos)
        {
            if ((paths.Count == 0) || (pos == InvalidPos))
            {
                paths.Add(new PathDef(pen));
            }
            if (pos != InvalidPos)
            {
                if ((paths.Last().points.Count == 0) || (Tool.Point.Distance(pos, paths.Last().points.Last()) > 0.0002614f))
                    paths.Last().points.Add(pos);
                else
                {
                    paths.Last().points.RemoveAt(paths.Last().points.Count-1);
                    paths.Last().points.Add(pos);
                }
            }
        }
        public void DisplayOnMap(Graphics gfx, VirtualMap map)
        {
            foreach(PathDef def in paths)
            {
                if (def.points.Count > 1)
                {
                    def.path.Reset();

                    var pts_u2p = new Point[def.points.Count];
                    for (int i = 0; i < def.points.Count; i++ )
                        pts_u2p[i] = map.UnitToPanel(def.points[i]);

                    def.path.AddLines(pts_u2p);

                    gfx.DrawPath(pen, def.path);
                }
            }
        }
        public void RemoveLastPoint()
        {
            if (paths.Count > 0)
            {
                PathDef def = paths.Last();
                
                if (def.points.Count > 0)
                    def.points.Remove(def.points.Last());

                if (def.points.Count == 0)
                    paths.Remove(def);
            }
        }
        public void ResetPaths()
        {
            foreach (PathDef def in paths)
                def.path.Reset();
        }
        internal List<PathDef> paths = new List<PathDef>();
        internal Pen pen;
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
}
