using System;

namespace GMToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public string functionName = null;

        public ButtonAttribute() { }

        public ButtonAttribute(string functionName)
        {
            this.functionName = functionName;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Inspect : Attribute
    {
        public string showingName = null;

        public Inspect() { }

        public Inspect(string showingName)
        {
            this.showingName = showingName;
        }
    }
}

