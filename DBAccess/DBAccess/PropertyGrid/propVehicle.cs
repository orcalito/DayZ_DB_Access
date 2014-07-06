using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace DBAccess
{
    public class Vehicle : PropObjectBase
    {
        public Vehicle(iconDB idb)
            : base(idb)
        {
            this.inventory = new Storage();
            this.parts = new VParts();
        }

        [ReadOnlyAttribute(true)]
        public string classname { get; set; }
        [ReadOnlyAttribute(true)]
        public UInt64 uid { get; set; }
        [ReadOnlyAttribute(true)]
        public UInt64 spawn_id { get; set; }
        [ReadOnlyAttribute(true)]
        public Tool.Point position { get; set; }
        [ReadOnlyAttribute(true)]
        public string fuel { get { return (int)(_fuel * 100.0f) + " %"; } }
        private double _fuel;
        [ReadOnlyAttribute(true)]
        public string damage { get { return (int)(_damage * 100.0f) + " %"; } }
        private double _damage;
        [ReadOnlyAttribute(true)]
        public Storage inventory { get; set; }
        [ReadOnlyAttribute(true)]
        public VParts parts { get; set; }
        public override void Rebuild()
        {
            this.inventory.weapons.Clear();
            this.inventory.items.Clear();
            this.inventory.bags.Clear();
            this.parts.Clear();

            try
            {
                this.uid = idb.row.Field<UInt64>("id");
                this.spawn_id = idb.row.Field<UInt64>("spawn_id");
                this._fuel = idb.row.Field<double>("fuel");
                this._damage = idb.row.Field<double>("damage");
                this.classname = idb.row.Field<string>("class_name");

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
                        if (aCount[i] as string != "")
                            this.inventory.weapons.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));

                    aItems = arr[1] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        if (aCount[i] as string != "")
                            this.inventory.items.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));

                    aItems = arr[2] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        if (aCount[i] as string != "")
                            this.inventory.bags.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                }

                arr = Tool.ParseInventoryString(idb.row.Field<string>("parts"));
                if (arr.Count > 0)
                {
                    for (int i = 0; i < arr.Count; i++)
                    {
                        var items = arr[i] as ArrayList;
                        var aPart = items[0] as string;
                        var aDamage = items[1] as string;
                        float damage = (float)(double.Parse(aDamage, CultureInfo.InvariantCulture.NumberFormat));
                        this.parts.Add(new VEntry(aPart, damage));
                    }
                }
            }
            catch
            {
            }
        }
    }
}
