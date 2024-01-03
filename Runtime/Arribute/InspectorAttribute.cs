using System;
using UnityEngine;

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
    public class InspectAttribute : Attribute
    {
        public string showingName = null;
        public bool isReadOnly;
        public int color;

        public InspectAttribute() { }

        public InspectAttribute(string showingName)
        {
            this.showingName = showingName;
        }

        public InspectAttribute(string showingName, bool isReadOnly)
        {
            this.showingName = showingName;
            this.isReadOnly = isReadOnly;
        }

        public InspectAttribute(bool isReadOnly)
        {
            this.isReadOnly = isReadOnly;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectIfAttribute : Attribute
    {
        public string condition = null;
        public object optionalValue = null;

        public InspectIfAttribute(string showIf, object showIfParam = null)
        {
            this.condition = showIf;
            this.optionalValue = showIfParam;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectColorAttribute : Attribute
    {
        private readonly Color color;

        public InspectColorAttribute(UnityEngine.Color color)
        {
            this.color = color;
        }
    }
}

