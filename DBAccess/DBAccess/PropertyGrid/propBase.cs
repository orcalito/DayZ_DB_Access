
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
