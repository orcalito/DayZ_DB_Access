using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public class Spawn : PropObjectBase
    {
        public Spawn(iconDB idb)
            : base(idb)
        {
            this.inventory = new Storage();
        }

        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public string classname { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public UInt64 uid { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public Tool.Point position { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public Decimal chance { get; set; }
        [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
        public Storage inventory { get; set; }
        public override void Rebuild()
        {
            this.inventory.weapons.Clear();
            this.inventory.items.Clear();
            this.inventory.bags.Clear();

            this.classname = idb.row.Field<string>("class_name");
            this.uid = idb.row.Field<UInt64>("id");
            this.chance = idb.row.Field<Decimal>("chance");

            ArrayList arr = Tool.ParseInventoryString(idb.row.Field<string>("worldspace"));
            //  arr[0] = angle
            //  arr[1] = [x, y, z]
            double x = double.Parse((arr[1] as ArrayList)[0] as string, CultureInfo.InvariantCulture.NumberFormat);
            double y = double.Parse((arr[1] as ArrayList)[1] as string, CultureInfo.InvariantCulture.NumberFormat);
            this.position = new Tool.Point((float)x, (float)y);

            arr = Tool.ParseInventoryString(idb.row.Field<string>("inventory"));
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
