using System.Collections;
using System.ComponentModel;
using System.Data;

namespace DBAccess
{
    public class Deployable : PropObjectBase
    {
        public Deployable(iconDB idb)
            : base(idb)
        {
            this.inventory = new Storage();
        }

        [ReadOnlyAttribute(true)]
        public string name { get; set; }
        [ReadOnlyAttribute(true)]
        public string epochKey { get; set; }
        [ReadOnlyAttribute(true)]
        public Storage inventory { get; set; }

        internal void ComputeEpochKey(uint key)
        {
            if (key == 0)
            {
                this.epochKey = "-";
            }
            else
            {
                this.epochKey = "code=" + key.ToString() + " or ";
                
                if (key <= 12500)
                {
                    if (key > 10000) this.epochKey += "KeyBlack" + (key - 10000).ToString();
                    else if (key > 7500) this.epochKey += "KeyYellow" + (key - 7500).ToString();
                    else if (key > 5000) this.epochKey += "KeyBlue" + (key - 5000).ToString();
                    else if (key > 2500) this.epochKey += "KeyRed" + (key - 2500).ToString();
                    else if (key > 0) this.epochKey += "KeyGreen" + key.ToString();
                }
            }
        }
        public override void Rebuild()
        {
            this.inventory.weapons.Clear();
            this.inventory.items.Clear();
            this.inventory.bags.Clear();

            try
            {
                this.name = idb.row.Field<string>("class_name");

                ComputeEpochKey(idb.row.Field<uint>("keycode"));

                ArrayList arr = Tool.ParseInventoryString(idb.row.Field<string>("inventory"));
                ArrayList aItems;
                ArrayList aTypes;
                ArrayList aCount;

                if (arr.Count > 0)
                {
                    // arr[0] = weapons
                    // arr[1] = items
                    // arr[2] = bags
                    aItems = arr[0] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.inventory.weapons.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));

                    aItems = arr[1] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.inventory.items.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));

                    aItems = arr[2] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.inventory.bags.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                }
            }
            catch
            {
            }
        }
    }
}
