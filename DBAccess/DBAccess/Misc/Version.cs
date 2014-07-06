using System;

namespace DBAccess
{
    [Serializable]
    public class ModuleVersion : ICloneable, IComparable
    {
        private int major;
        private int minor;
        public int Major
        {
            get
            {
                return major;
            }
            set
            {
                major = value;
            }
        }
        public int Minor
        {
            get
            {
                return minor;
            }
            set
            {
                minor = value;
            }
        }
        public ModuleVersion()
        {
            this.major = 0;
            this.minor = 0;
        }
        public ModuleVersion(int major, int minor)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", "ArgumentOutOfRange_Version");
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", "ArgumentOutOfRange_Version");
            }
            this.major = major;
            this.minor = minor;
        }
        #region ICloneable Members
        public object Clone()
        {
            ModuleVersion version1 = new ModuleVersion();
            version1.major = this.major;
            version1.minor = this.minor;
            return version1;
        }
        #endregion
        #region IComparable Members
        public int CompareTo(object version)
        {
            if (version == null)
            {
                return 1;
            }
            if (!(version is ModuleVersion))
            {
                throw new ArgumentException("Arg_MustBeVersion");
            }
            ModuleVersion version1 = (ModuleVersion)version;
            if (this.major != version1.Major)
            {
                if (this.major > version1.Major)
                {
                    return 1;
                }
                return -1;
            }
            if (this.minor != version1.Minor)
            {
                if (this.minor > version1.Minor)
                {
                    return 1;
                }
                return -1;
            }
            return 0;
        }
        #endregion
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is ModuleVersion))
            {
                return false;
            }
            ModuleVersion version1 = (ModuleVersion)obj;
            if ((this.major == version1.Major) && (this.minor == version1.Minor))
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int num1 = 0;
            num1 |= ((this.major & 15) << 0x1c);
            num1 |= ((this.minor & 0xff) << 20);
            return num1;
        }
        public static bool operator ==(ModuleVersion v1, ModuleVersion v2)
        {
            if (v1 is ModuleVersion)
                return v1.Equals(v2);
            return (object)v1 == (object)v2;
        }
        public static bool operator >(ModuleVersion v1, ModuleVersion v2)
        {
            return (v2 < v1);
        }
        public static bool operator >=(ModuleVersion v1, ModuleVersion v2)
        {
            return (v2 <= v1);
        }
        public static bool operator !=(ModuleVersion v1, ModuleVersion v2)
        {
            return (v1 != v2);
        }
        public static bool operator <(ModuleVersion v1, ModuleVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) < 0);
        }
        public static bool operator <=(ModuleVersion v1, ModuleVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) <= 0);
        }
    }
}
