using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public class Tool
    {
        public struct Point
        {
            public static Point Empty = new Point(0, 0);

            public Point(float x, float y) { X = x; Y = y; }
            public Point(System.Drawing.PointF p) { X = p.X; Y = p.Y; }

            public static implicit operator System.Drawing.Point(Point p) { return new System.Drawing.Point((int)p.X, (int)p.Y); }
            public static implicit operator System.Drawing.PointF(Point p) { return new System.Drawing.PointF(p.X, p.Y); }
            public static explicit operator Size(Point p) { return new Size(p.X, p.Y); }
            public static implicit operator Point(System.Drawing.Point p) { return new Point(p.X, p.Y); }

            public static Point operator +(Point p1, Point p2) { return new Point(p1.X + p2.X, p1.Y + p2.Y); }
            public static Point operator +(Point pt, Size sz) { return new Point(pt.X + sz.Width, pt.Y + sz.Height); }
            public static Point operator -(Point pt, Size sz) { return new Point(pt.X - sz.Width, pt.Y - sz.Height); }
            public static Size operator -(Point p1, Point p2) { return new Size(p1.X - p2.X, p1.Y - p2.Y); }
            public static Point operator *(Point pt, float f) { return new Point(pt.X * f, pt.Y * f); }
            public static Point operator *(Point pt, Size sz) { return new Point(pt.X * sz.Width, pt.Y * sz.Height); }
            public static Point operator /(Point pt, float f) { return new Point(pt.X / f, pt.Y / f); }
            public static Point operator /(Point pt, Size sz) { return new Point(pt.X / sz.Width, pt.Y / sz.Height); }
            public static bool operator !=(Point left, Point right) { return !(left == right); }
            public static bool operator ==(Point left, Point right) { return (left.X == right.X) && (left.Y == right.Y); }

            public static Point Min(Point p1, Point p2) { return new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y)); }
            public static Point Max(Point p1, Point p2) { return new Point(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y)); }
            public static float Distance(Point p1, Point p2) { return (p1 - p2).Lenght; }
           
            public Point Floor { get { return new Point((float)Math.Floor(X), (float)Math.Floor(Y)); } }
            public Point Ceiling { get { return new Point((float)Math.Ceiling(X), (float)Math.Ceiling(Y)); } }
            public Point Truncate { get { return new Point((float)Math.Truncate(X), (float)Math.Truncate(Y)); } }

            public bool IsNaN { get { return float.IsNaN(X) || float.IsNaN(Y); } }

            public override bool Equals(object obj)
            {
                if (obj is Point)
                    return ((Point)obj == this);

                return false;
            }
            public override int GetHashCode() { return ((int)(X * 16777216) ^ ((int)(Y * 16777216)) << 8); }
            public override string ToString() { return "(" + X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + ")"; }
            public string ToStringInt() { return "(" + (int)X + "," + (int)Y + ")"; }

            public float X;
            public float Y;
        }
        public struct Size
        {
            public static Size Empty = new Size(0, 0);

            public Size(float w, float h) { Width = w; Height = h; }
            public Size(System.Drawing.SizeF sz) { Width = sz.Width; Height = sz.Height; }

            public static implicit operator System.Drawing.Size(Size sz) { return new System.Drawing.Size((int)sz.Width, (int)sz.Height); }
            public static implicit operator System.Drawing.SizeF(Size sz) { return new System.Drawing.SizeF(sz.Width, sz.Height); }
            public static explicit operator Point(Size sz) { return new Point(sz.Width, sz.Height); }
            public static implicit operator Size(System.Drawing.Size sz) { return new Size(sz.Width, sz.Height); }

            public static Size operator +(Size s1, Size s2) { return new Size(s1.Width + s2.Width, s1.Height + s2.Height); }
            public static Size operator -(Size s1, Size s2) { return new Size(s1.Width - s2.Width, s1.Height - s2.Height); }
            public static Size operator *(Size sz, float f) { return new Size(sz.Width * f, sz.Height * f); }
            public static Size operator *(Size s1, Size s2) { return new Size(s1.Width * s2.Width, s1.Height * s2.Height); }
            public static Size operator /(Size sz, float f) { return new Size(sz.Width / f, sz.Height / f); }
            public static Size operator /(Size s1, Size s2) { return new Size(s1.Width / s2.Width, s1.Height / s2.Height); }
            public static bool operator !=(Size left, Size right) { return !(left == right); }
            public static bool operator ==(Size left, Size right) { return (left.Width == right.Width) && (left.Height == right.Height); }

            public static Size Min(Size p1, Size p2) { return new Size(Math.Min(p1.Width, p2.Width), Math.Min(p1.Height, p2.Height)); }
            public static Size Max(Size p1, Size p2) { return new Size(Math.Max(p1.Width, p2.Width), Math.Max(p1.Height, p2.Height)); }

            public Size Floor { get { return new Size((float)Math.Floor(Width), (float)Math.Floor(Height)); } }
            public Size Ceiling { get { return new Size((float)Math.Ceiling(Width), (float)Math.Ceiling(Height)); } }
            public Size BelowPowerOf2 { get { return new Size(BelowPowerOf2(Width), BelowPowerOf2(Height)); } }
            public Size UpperPowerOf2 { get { return new Size(UpperPowerOf2(Width), UpperPowerOf2(Height)); } }
            public float Lenght { get { return (float)Math.Sqrt(Width * Width + Height * Height); } }

            public override bool Equals(object obj)
            {
                if (obj is Size)
                    return ((Size)obj == this);

                return false;
            }
            public override int GetHashCode() { return ((int)(Width * 16777216) ^ ((int)(Height * 16777216)) << 8); }
            public override string ToString() { return "(" + Width.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + Height.ToString(CultureInfo.InvariantCulture.NumberFormat) + ")"; }

            public float Width;
            public float Height;
        }
        public static int BelowPowerOf2(int v)
        {
            int r = 30;
            while (v < (1 << r))
                r--;
            return (1 << r);
        }
        public static int BelowPowerOf2(float v)
        {
            int r = 30;
            while (v < (float)(1 << r))
                r--;
            return (1 << r);
        }
        public static int UpperPowerOf2(int v)
        {
            int r = 0;
            while (v > (1 << r))
                r++;
            return (1 << r);
        }
        public static int UpperPowerOf2(float f)
        {
            int r = 0;
            while (f > (float)(1 << r))
                r++;
            return (1 << r);
        }
        public static ArrayList ParseInventoryString(string str)
        {
            Stack<ArrayList> stack = new Stack<ArrayList>();
            ArrayList main = null;
            ArrayList curr = null;
            string value = "";
            bool bValue = false;

            foreach (char c in str)
            {
                switch (c)
                {
                    case '[':
                        if (curr != null) stack.Push(curr);
                        curr = new ArrayList();
                        if (stack.Count > 0) stack.Peek().Add(curr);
                        if (main == null)
                            main = curr;
                        break;

                    case ']':
                        if (value != "") curr.Add(value);
                        value = "";
                        bValue = false;
                        if (stack.Count > 0) curr = stack.Pop();
                        else curr = null;
                        break;

                    case '"':
                        bValue = true;
                        break;

                    case ',':
                        if (bValue) curr.Add(value);
                        value = "";
                        bValue = false;
                        break;

                    default:
                        bValue = true;
                        value += c;
                        break;
                }
            }

            return main;
        }
        public static bool NullOrEmpty(string str)
        {
            return ((str == null) || (str == ""));
        }
        public static void SaveJpeg(string path, Bitmap img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size, bool keepRatio)
        {
            Size sourceSize = imgToResize.Size;
            Size destSize = size;

            if (keepRatio)
            {
                Size nPercentSize = size / sourceSize;
                float nPercent = Math.Min(nPercentSize.Width, nPercentSize.Height);
                destSize = sourceSize * nPercent;
            }

            Bitmap b = new Bitmap((int)destSize.Width, (int)destSize.Height);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.Bilinear/*NearestNeighbor*/;

            g.DrawImage(imgToResize, 0, 0, (int)destSize.Width, (int)destSize.Height);
            g.Dispose();

            return b;
        }
        public static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpCrop = img.Clone(cropArea, img.PixelFormat);
            return bmpCrop;
        }
        public static Bitmap IncreaseImageSize(Bitmap imgToResize, Size newSize)
        {
            Bitmap b = new Bitmap((int)newSize.Width, (int)newSize.Height);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.Clear(Color.White);
            g.DrawImage(imgToResize, 0, 0, imgToResize.Width, imgToResize.Height);
            g.Dispose();

            return b;
        }
        public static void CreateBitmapFromTiles(string dstFilePath, string srcDirPath, int tileCountX, int tileCountY, Tool.Size tileSize)
        {
            Bitmap result = new Bitmap((int)tileSize.Width * tileCountX,
                                       (int)tileSize.Height * tileCountY);
            Graphics g = Graphics.FromImage((Image)result);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.Clear(Color.White);

            for (int y = 0; y < tileCountY; y++)
            {
                for (int x = 0; x < tileCountX; x++)
                {
                    Bitmap input = new Bitmap(srcDirPath + x +"_" + y + ".png");
                    g.DrawImage(input, new Rectangle((int)(x * tileSize.Width), (int)(y * tileSize.Height), (int)tileSize.Width, (int)tileSize.Height));
                    input.Dispose();
                }
            }

            SaveJpeg(dstFilePath, result, 90);
            result.Dispose();
        }
        public static int maxSize = 8192;
        public static Tuple<Size, Size, Size> CreateTiles(string filepath, string basepath, int limit)
        {
            Bitmap input = new Bitmap(filepath);
            
            float ratio = (input.VerticalResolution / input.HorizontalResolution);
            if( Math.Abs(1.0f - ratio) > 0.01f )
            {
                // Ratio isn't squared
                Bitmap input2 = ResizeImage(input, new Size(input.Width * ratio, input.Height), false);
                input = input2;
            }

            Size inSize = input.Size;
            Size sqSize = inSize.UpperPowerOf2;

            double iMax = 1.0 / Math.Min(sqSize.Width, sqSize.Height);
            Size limits = sqSize * (float)(limit * iMax);

            // Cut in Xk*Xk blocks
            Rectangle rXk = new Rectangle(0, 0, maxSize, maxSize);

            Size blkCount = sqSize / maxSize;

            if (Directory.Exists(basepath + "X") == false)
                Directory.CreateDirectory(basepath + "X");

            for (int y = 0; y < sqSize.Height / maxSize; y++)
            {
                for (int x = 0; x < sqSize.Width / maxSize; x++)
                {
                    rXk.Location = new Point(x * maxSize, y * maxSize);

                    Size blkSize = new Size(Math.Min(input.Width - x * maxSize, maxSize), 
                                            Math.Min(input.Height - y * maxSize, maxSize));
                    rXk.Size = blkSize;

                    if ((blkSize.Width > 0) && (blkSize.Height > 0))
                    {
                        Bitmap blk = CropImage(input, rXk);

                        if ((blkSize.Width < maxSize) || (blkSize.Height < maxSize))
                        {
                            blk = IncreaseImageSize(blk, new Size(maxSize, maxSize));
                        }

                        SaveJpeg(basepath + "X\\Blk_" + y.ToString("00") + x.ToString("00") + ".jpg", blk, 90);
                        blk.Dispose();
                    }
                }
            }
            input.Dispose();

            int depth = (int)Math.Log(Math.Max(blkCount.Width, blkCount.Height), 2);

            rXk.Size = new Size(maxSize, maxSize);
            for (int y = 0; y < sqSize.Height / maxSize; y++)
            {
                for (int x = 0; x < sqSize.Width / maxSize; x++)
                {
                    string BlkPath = basepath + "X\\Blk_" + y.ToString("00") + x.ToString("00") + ".jpg";
                    if (File.Exists(BlkPath))
                    {
                        input = new Bitmap(BlkPath);

                        rXk.Location = new System.Drawing.Point(x * maxSize, y * maxSize);
                        Point location = new Point(x, y);

                        RecursCreateTiles(location, Size.Empty, basepath, "Tile", input, limits, depth);

                        input.Dispose();
                    }
                }
            }

            DirectoryInfo di = new DirectoryInfo(basepath + "X");
            if (di.Exists)
                di.Delete(true);

            return new Tuple<Size, Size, Size>(inSize, sqSize, limits);
        }
/*        public static Tuple<Size, Size, Size> CreateTiles(string filepath, string basepath, int limit)
        {
            Bitmap input = new Bitmap(filepath);

            Size inSize = input.Size;
            Size sqSize = inSize.UpperPowerOf2;

            if (sqSize.Width * sqSize.Height > (16384 * 16384))
                throw new Exception("Input bitmap is too large, don't use bitmaps larger than 16384 * 16384");

            Bitmap sqInput = IncreaseImageSize(input, sqSize);

            input.Dispose();

            double iMax = 1.0 / Math.Min(sqSize.Width, sqSize.Height);

            Size limits = sqSize * (float)(limit * iMax);

            RecursCreateTiles(Point.Empty, Size.Empty, basepath, "Tile", sqInput, limits, 0);
            sqInput.Dispose();

            return new Tuple<Size, Size, Size>(inSize, sqSize, limits);
        }*/
        private static void RecursCreateTiles(Point father, Size child, string basepath, string name, Bitmap input, Size limits, int recCnt)
        {
            Point pos = father + child;

            Bitmap resized = ResizeImage(input, limits, true);
            //  TEST
            bool bReject = true;
            {
                Bitmap b = new Bitmap(4, 4, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.DrawImage(resized, 0, 0, 4, 4);
                g.Dispose();
                BitmapData data = b.LockBits(new Rectangle(0, 0, 4, 4), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                IntPtr ptr = data.Scan0;
                int numBytes = (data.Stride * b.Height);
                byte[] rgbValues = new byte[numBytes];
                Marshal.Copy(ptr, rgbValues, 0, numBytes);

                float fError = 0;
                for (int i = 0; i < numBytes; i++)
                    fError += 255 - rgbValues[i];

                fError /= numBytes;
                if(fError >= 0.5)
                    bReject = false;

                // Unlock the bits.
                b.UnlockBits(data);

            }
            if (!bReject)
            {
                if (recCnt >= 0)
                {
                    if (Directory.Exists(basepath + recCnt) == false)
                        Directory.CreateDirectory(basepath + recCnt);

                    SaveJpeg(basepath + recCnt + "\\" + name + pos.Y.ToString("000") + pos.X.ToString("000") + ".jpg", resized, 90);
                    resized.Dispose();
                }

                bool bSplitH = (input.Width > limits.Width);
                bool bSplitV = (input.Height > limits.Height);

                if (bSplitH || bSplitV)
                {
                    recCnt++;
                    Size cropSize = Size.Max((Size)input.Size / 2, limits);

                    father = pos * 2;

                    RecursCreateTiles(father, new Size(0, 0), basepath, name, CropImage(input, new Rectangle(new Point(0, 0), cropSize)), limits, recCnt);

                    if (bSplitH)
                        RecursCreateTiles(father, new Size(1, 0), basepath, name, CropImage(input, new Rectangle(new Point(cropSize.Width, 0), cropSize)), limits, recCnt);

                    if (bSplitV)
                        RecursCreateTiles(father, new Size(0, 1), basepath, name, CropImage(input, new Rectangle(new Point(0, cropSize.Height), cropSize)), limits, recCnt);

                    if (bSplitH && bSplitV)
                        RecursCreateTiles(father, new Size(1, 1), basepath, name, CropImage(input, new Rectangle(new Point(cropSize.Width, cropSize.Height), cropSize)), limits, recCnt);
                }
            }
            else
            {
                resized.Dispose();
            }

            input.Dispose();
        }
        private static ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
        //
        //
        //
        private static Point[] ptsWorld1_Grid = new Point[]
        {
            new Point(14000,2360),
            new Point(1000,2360),
            new Point(1000,3360),
            new Point(14000,3360),
            new Point(14000,4360),
            new Point(1000,4360),
            new Point(1000,5360),
            new Point(14000,5360),
            new Point(14000,6360),
            new Point(1000,6360),
            new Point(1000,7360),
            new Point(14000,7360),
            new Point(14000,8360),
            new Point(1000,8360),
            new Point(1000,9360),
            new Point(14000,9360),
            new Point(14000,10360),
            new Point(1000,10360),
            new Point(1000,11360),
            new Point(14000,11360),
            new Point(14000,12360),
            new Point(1000,12360),
            new Point(1000,13360),
            new Point(14000,13360),
            
            new Point(14000,1360),
            new Point(13000,1360),
            new Point(13000,13360),
            new Point(12000,13360),
            new Point(12000,1360),
            new Point(11000,1360),
            new Point(11000,13360),
            new Point(10000,13360),
            new Point(10000,1360),
            new Point(9000,1360),
            new Point(9000,13360),
            new Point(8000,13360),
            new Point(8000,1360),
            new Point(7000,1360),
            new Point(7000,13360),
            new Point(6000,13360),
            new Point(6000,1360),
            new Point(5000,1360),
            new Point(5000,13360),
            new Point(4000,13360),
            new Point(4000,1360),
            new Point(3000,1360),
            new Point(3000,13360),
            new Point(2000,13360),
            new Point(2000,1360),
            new Point(1000,1360),
            new Point(1000,13360)
        };
        private static Point[] ptsWorld1_Skalisty = new Point[]
        {
            new Point(13440,3537),
            new Point(13393,3508),
            new Point(13345,3436),
            new Point(13306,3409),
            new Point(13263,3381),
            new Point(13194,3365),
            new Point(13155,3349),
            new Point(13127,3343),
            new Point(13109,3326),
            new Point(13049,3306),
            new Point(12992,3246),
            new Point(12960,3205),
            new Point(12978,3163),
            new Point(13036,3129),
            new Point(13093,3083),
            new Point(13150,3053),
            new Point(13173,3014),
            new Point(13214,2998),
            new Point(13221,3042),
            new Point(13286,3019),
            new Point(13336,3015),
            new Point(13419,3033),
            new Point(13433,3012),
            new Point(13432,2927),
            new Point(13440,2875),
            new Point(13426,2842),
            new Point(13373,2836),
            new Point(13334,2824),
            new Point(13249,2767),
            new Point(13228,2728),
            new Point(13251,2693),
            new Point(13293,2675),
            new Point(13355,2673),
            new Point(13421,2721),
            new Point(13503,2757),
            new Point(13568,2748),
            new Point(13662,2707),
            new Point(13701,2675),
            new Point(13832,2751),
            new Point(13971,2728),
            new Point(14027,2691),
            new Point(14070,2654),
            new Point(14130,2645),
            new Point(14180,2604),
            new Point(14205,2611),
            new Point(14224,2661),
            new Point(14221,2739),
            new Point(14194,2847),
            new Point(14150,2897),
            new Point(14158,2978),
            new Point(14075,3012),
            new Point(13994,3097),
            new Point(13951,3104),
            new Point(13859,3072),
            new Point(13770,3097),
            new Point(13740,3216),
            new Point(13678,3406),
            new Point(13625,3425),
            new Point(13568,3460),
            new Point(13497,3526),
            new Point(13464,3540),
            new Point(13437,3535)
        };
        private static Point[] ptsWorld1_NWAF = new Point[]
        {
            new Point(5055, 9732),
            new Point(4778, 9572),
            new Point(4057, 10820),
            new Point(4335, 10979),
            new Point(5055, 9732)
        };
        private static Tool.Point[] ptsWorld9_HighwayWest = new Tool.Point[]
        {
            new Tool.Point(3595,51),
            new Tool.Point(3351,624),
            new Tool.Point(2893,1253),
            new Tool.Point(2786,1392),
            new Tool.Point(1781,3414),
            new Tool.Point(1655,3800),
            new Tool.Point(1530,4818),
            new Tool.Point(1433,5369),
            new Tool.Point(1215,6108),
            new Tool.Point(1175,6482),
            new Tool.Point(1212,6832),
            new Tool.Point(1585,8978),
            new Tool.Point(1725,9294),
            new Tool.Point(1884,9430),
            new Tool.Point(2306,9654),
            new Tool.Point(2583,9886),
            new Tool.Point(2960,10385),
            new Tool.Point(3333,10764),
            new Tool.Point(3725,11264),
            new Tool.Point(3965,11473),
            new Tool.Point(4785,11980),
            new Tool.Point(5048,12216)
        };
        private static Point[] ptsWorld9_SouthAF = new Point[]
        {
            new Point(6982,1091),
            new Point(8145,1289),
            new Point(8089,1392),
            new Point(7953,1392),
            new Point(7853,1491),
            new Point(7657,1451),
            new Point(7591,1315),
            new Point(7008,1201),
            new Point(6974,1095)
        };
        private static Point[] ptsWorld9_NorthAF = new Point[]
        {
            new Point(10379,11317),
            new Point(11889,11317),
            new Point(11889,11281),
            new Point(11841,11229),
            new Point(11206,11222),
            new Point(11202,11130),
            new Point(11180,11090),
            new Point(11054,11023),
            new Point(11080,10961),
            new Point(10859,10843),
            new Point(10777,10976),
            new Point(10796,10987),
            new Point(10777,11042),
            new Point(10984,11137),
            new Point(11047,11053),
            new Point(11165,11119),
            new Point(11169,11226),
            new Point(10471,11226),
            new Point(10382,11281),
            new Point(10379,11317)
        };
        //
        public static List<List<Point[]>> MapHelperDefs = new List<List<Point[]>>
        {
            // Chernarus
            new List<Point[]>
            {
                ptsWorld1_Grid,
                ptsWorld1_NWAF,
                ptsWorld1_Skalisty
            },
            new List<Point[]>
            {
            },
            new List<Point[]>
            {
            },
            new List<Point[]>
            {
            },
            new List<Point[]>
            {
            },
            new List<Point[]>
            {
            },
            new List<Point[]>
            {
            },
            new List<Point[]>
            {
            },
            // Celle 2
            new List<Point[]>
            {
                ptsWorld9_HighwayWest,
                ptsWorld9_SouthAF,
                ptsWorld9_NorthAF
            },
            // Taviana
            new List<Point[]>
            {
            }
        };
    }
}

