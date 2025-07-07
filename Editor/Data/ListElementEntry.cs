using System;
using System.Collections;
using UnityEngine.UI;

namespace GMToolKit.Inspector
{
    internal sealed class ListElementEntry : IDrawerEntry
    {
        public IList sourceList { get; set; }
        public Type elementType { get; set; }
        public int index { get; set; }

        public string EntryName => index.ToString();

        public Type EntryType => throw new NotImplementedException();

        public T GetEntryAttribute<T>() where T : Attribute
        {
            return null;
        }

        public object Value { get => Getter(); set => throw new NotImplementedException(); }

        private object Getter()
        {
            // var targetList = 
            return default;
        }

        public bool IsSettable()
        {
            throw new NotImplementedException();
        }
    }
}