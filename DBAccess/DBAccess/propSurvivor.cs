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
    public class Survivor : PropObjectBase
    {
        public Survivor(iconDB idb)
            : base(idb)
        {
            this.inventory = new Cargo();
            this.backpack = new Cargo();
            this.tools = new Cargo();
        }

        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public string name { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public string uid { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public int humanity { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public int blood { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public string hunger { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public string thirst { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public string medical { get; set; }
        [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
        public string weapon { get; set; }
        [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
        public Cargo inventory { get; set; }
        [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
        public string backpackclass { get; set; }
        [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
        public Cargo backpack { get; set; }
        [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
        public Cargo tools { get; set; }
        public override void Rebuild()
        {
            this.inventory.Clear();
            this.backpack.Clear();
            this.tools.Clear();

            Dictionary<string, int> dicInventory = new Dictionary<string, int>();

            ArrayList arr = Tool.ParseInventoryString(idb.row.Field<string>("medical"));
            // arr[0] = is dead
            // arr[1] = unconscious
            // arr[2] = infected
            // arr[3] = injured
            // arr[4] = in pain
            // arr[5] = is cardiac
            // arr[6] = low blood
            // arr[7] = blood quantity
            // arr[8] = [wounds]
            // arr[9] = [legs, arms]
            // arr[10] = unconscious time
            // arr[11] = [hunger, thirst]

            this.medical = "";
            if (bool.Parse(arr[1] as string)) this.medical += " unconscious, ";
            if (bool.Parse(arr[2] as string)) this.medical += " infected, ";
            if (bool.Parse(arr[3] as string)) this.medical += " injured, ";
            if (bool.Parse(arr[4] as string)) this.medical += " in pain, ";
            if (bool.Parse(arr[6] as string)) this.medical += " low blood, ";
            this.medical = this.medical.Trim();
            this.medical = this.medical.TrimEnd(',');

            this.blood = (int)double.Parse(arr[7] as string, CultureInfo.InvariantCulture.NumberFormat);
            this.hunger = ((int)(double.Parse((arr[11] as ArrayList)[0] as string, CultureInfo.InvariantCulture.NumberFormat) / 21.60f)).ToString() + "%";
            this.thirst = ((int)(double.Parse((arr[11] as ArrayList)[1] as string, CultureInfo.InvariantCulture.NumberFormat) / 14.40f)).ToString() + "%";

            arr = Tool.ParseInventoryString(idb.row.Field<string>("inventory"));

            if (arr.Count > 0)
            {
                // arr[0] = tools
                // arr[1] = items

                foreach (string o in arr[0] as ArrayList)
                    this.tools.Add(new Entry(o, 1));

                foreach (object o in (arr[1] as ArrayList))
                {
                    if (o is string)
                    {
                        string name = o as string;
                        if (dicInventory.ContainsKey(name) == false)
                            dicInventory.Add(name, 1);
                        else
                            dicInventory[name]++;
                    }
                    else if (o is ArrayList)
                    {
                        ArrayList oal = o as ArrayList;

                        string name = oal[0] as string;
                        if (dicInventory.ContainsKey(name) == false)
                            dicInventory.Add(name, 1);
                        else
                            dicInventory[name]++;
                    }
                }

                foreach (KeyValuePair<string, int> pair in dicInventory)
                    this.inventory.Add(new Entry(pair.Key, pair.Value));
            }

            arr = Tool.ParseInventoryString(idb.row.Field<string>("backpack"));
            // arr[0] = backpack's class
            // arr[1] = weapons
            // arr[2] = items

            if (arr.Count > 0)
            {
                this.backpackclass = arr[0] as string;

                {
                    ArrayList aWeapons = arr[1] as ArrayList;
                    ArrayList aTypes = aWeapons[0] as ArrayList;
                    ArrayList aCount = aWeapons[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.backpack.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                }

                {
                    ArrayList aItems = arr[2] as ArrayList;
                    ArrayList aTypes = aItems[0] as ArrayList;
                    ArrayList aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.backpack.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                }
            }

            arr = Tool.ParseInventoryString(idb.row.Field<string>("state"));

            if (arr.Count > 0)
            {
                this.weapon = arr[0] as string;
                if (this.weapon == "")
                    this.weapon = "none";
            }

            this.name = idb.row.Field<string>("name");
            this.uid = idb.row.Field<string>("unique_id");
            this.humanity = idb.row.Field<int>("humanity");
        }
    }
}
