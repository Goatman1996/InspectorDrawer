using System;

namespace GMToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class DrawerProvider : Attribute
    {
        public abstract Type drawerType { get; }
    }
}