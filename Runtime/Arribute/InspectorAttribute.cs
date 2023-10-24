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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectIf : Attribute
    {
        public string condition = null;
        public object optionalValue = null;

        public InspectIf(string showIf, object showIfParam = null)
        {
            this.condition = showIf;
            this.optionalValue = showIfParam;
        }
    }
}

