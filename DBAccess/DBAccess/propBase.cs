using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public abstract class PropObjectBase
    {
        public PropObjectBase(iconDB idb)
        {
            this.idb = idb;
        }

        public iconDB idb;

        public abstract void Rebuild();
    }
}
