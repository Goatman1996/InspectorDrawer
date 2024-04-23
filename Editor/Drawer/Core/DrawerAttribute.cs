using System;

namespace GMToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerAttribute : Attribute
    {
        public Type type;
        public Type special;
        public bool inherited = false;

        public DrawerAttribute(Type type, Type specialDrawer = null)
        {
            this.type = type;
            this.special = specialDrawer;
        }
    }
}