using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Text;

namespace DBAccess
{
    //
    //
    //
    internal class EntryConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is Entry)
            {
                Entry entry = (Entry)value;
                return entry.count.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
    //
    //
    //
    internal class NullConverter<T> : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string))
            {
                if (value is Cargo)
                {
                    Cargo cargo = value as Cargo;
                    return "total = " + cargo.Total;
                }
                else if (value is T)
                {
                    return "";
                }
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
    //
    //
    //
    internal class NullEditor : CollectionEditor
    {
        public NullEditor(Type type) : base(type) { }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
    }
    //
    //
    //
    internal class CargoPropertyDescriptor : PropertyDescriptor
    {
        private Cargo collection = null;
        private int index = -1;

        public CargoPropertyDescriptor(Cargo coll, int idx) :
            base("#" + idx.ToString(), null)
        {
            this.collection = coll;
            this.index = idx;
        }

        public override AttributeCollection Attributes { get { return new AttributeCollection(null); } }
        public override bool CanResetValue(object component) { return true; }
        public override Type ComponentType { get { return this.collection.GetType(); } }

        public override string DisplayName
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                Entry entry = this.collection[index];
                return entry.name;
            }
        }

        public override string Description
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                Entry entry = this.collection[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(entry.name + " : " + entry.count);
                return sb.ToString();
            }
        }

        public override object GetValue(object component) { return this.collection[index]; }
        public override bool IsReadOnly { get { return true; } }
        public override string Name { get { return "#" + index.ToString(); } }
        public override Type PropertyType { get { return this.collection[index].GetType(); } }
        public override void ResetValue(object component) { }
        public override bool ShouldSerializeValue(object component) { return true; }
        public override void SetValue(object component, object value) { }
    };
    //
    //
    //
    [TypeConverterAttribute(typeof(NullConverter<Cargo>)), EditorAttribute(typeof(NullEditor), typeof(UITypeEditor))]
    public class Cargo : CollectionBase, ICustomTypeDescriptor
    {
        #region Collection Implementation

        public void Add(Entry zone) { this.List.Add(zone); }
        public void Remove(Entry zone) { this.List.Remove(zone); }
        public Entry this[int index] { get { return (Entry)this.List[index]; } }

        #endregion

        // Implementation of interface ICustomTypeDescriptor 
        #region ICustomTypeDescriptor impl

        public String GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
        public String GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }
        public PropertyDescriptorCollection GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++)
            {
                // Create a property descriptor for the zone item and add to the property descriptor collection
                CargoPropertyDescriptor pd = new CargoPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection
            return pds;
        }

        #endregion

        public int Total
        {
            get
            {
                int total = 0;
                foreach (Entry entry in List)
                    total += entry.count;
                return total;
            }
        }
    }
    //
    //
    //
    [TypeConverter(typeof(EntryConverter))]
    public class Entry
    {
        public Entry(string n = "", int c = 0)
        {
            name = n;
            count = c;
        }

        public string name { get; set; }
        public int count { get; set; }
    };
    //
    //
    //
    [TypeConverterAttribute(typeof(NullConverter<Storage>))]
    public class Storage
    {
        public Storage()
        {
            weapons = new Cargo();
            items = new Cargo();
            bags = new Cargo();
        }

        public Cargo weapons { get; set; }
        public Cargo items { get; set; }
        public Cargo bags { get; set; }
    };

    //
    //
    //
    internal class PartsPropertyDescriptor : PropertyDescriptor
    {
        private VParts collection = null;
        private int index = -1;

        public PartsPropertyDescriptor(VParts coll, int idx) :
            base("#" + idx.ToString(), null)
        {
            this.collection = coll;
            this.index = idx;
        }

        public override AttributeCollection Attributes { get { return new AttributeCollection(null); } }
        public override bool CanResetValue(object component) { return true; }
        public override Type ComponentType { get { return this.collection.GetType(); } }

        public override string DisplayName
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                VEntry entry = this.collection[index];
                return entry.name;
            }
        }

        public override string Description
        {
            get
            {
                if (index >= this.collection.Count)
                    return "";

                VEntry entry = this.collection[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(entry.name + " : " + entry.damage);
                return sb.ToString();
            }
        }

        public override object GetValue(object component) { return this.collection[index]; }
        public override bool IsReadOnly { get { return true; } }
        public override string Name { get { return "#" + index.ToString(); } }
        public override Type PropertyType { get { return this.collection[index].GetType(); } }
        public override void ResetValue(object component) { }
        public override bool ShouldSerializeValue(object component) { return true; }
        public override void SetValue(object component, object value) { }
    };
    //
    //
    //
    internal class VEntryConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is VEntry)
            {
                VEntry entry = (VEntry)value;
                int damage = (int)((1.0f - entry.damage) * 100.0f);
                return damage + " %";
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
    //
    //
    //
    [TypeConverter(typeof(VEntryConverter))]
    public class VEntry
    {
        public VEntry(string n = "", float d = 0)
        {
            name = n;
            damage = d;
        }

        public string name { get; set; }
        public float damage { get; set; }
    };
    [TypeConverterAttribute(typeof(NullConverter<VParts>)), EditorAttribute(typeof(NullEditor), typeof(UITypeEditor))]
    public class VParts : CollectionBase, ICustomTypeDescriptor
    {
        #region Collection Implementation

        public void Add(VEntry zone) { this.List.Add(zone); }
        public void Remove(VEntry zone) { this.List.Remove(zone); }
        public VEntry this[int index] { get { return (VEntry)this.List[index]; } }

        #endregion

        // Implementation of interface ICustomTypeDescriptor 
        #region ICustomTypeDescriptor impl

        public String GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
        public String GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }
        public PropertyDescriptorCollection GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++)
            {
                // Create a property descriptor for the zone item and add to the property descriptor collection
                PartsPropertyDescriptor pd = new PartsPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection
            return pds;
        }

        #endregion

        public int Total { get { return List.Count; } }
    }
}
