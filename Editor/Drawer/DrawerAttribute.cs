using System;

namespace GMToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerAttribute : Attribute
    {
        public Type type;

        public DrawerAttribute(Type type)
        {
            this.type = type;
        }
    }
}