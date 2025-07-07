using System;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    public abstract class Drawer
    {
        internal IDrawerEntry Entry { get; set; }
        public abstract VisualElement Initialize();
        public abstract void Tick();
    }
}