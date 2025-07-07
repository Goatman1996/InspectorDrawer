using System;

namespace GMToolKit.Inspector
{
    internal interface IDrawerEntry
    {
        public string EntryName { get; }
        public object Value { get; set; }
        public Type EntryType { get; }

        public bool IsSettable();
        public T GetEntryAttribute<T>() where T : Attribute;
    }
}