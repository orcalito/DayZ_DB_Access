using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

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
        public Storage inventory { get; set; }
        public override void Rebuild()
        {
            this.inventory.weapons.Clear();
            this.inventory.items.Clear();
            this.inventory.bags.Clear();

            this.name = idb.row.Field<string>("class_name");

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
    }
}
