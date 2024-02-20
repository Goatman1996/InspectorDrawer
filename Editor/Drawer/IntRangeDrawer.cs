using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(int), typeof(RangeAttribute))]
    internal class IntRangeDrawer : Drawer
    {
        SliderInt view;
        IntegerField intField;

        public override VisualElement Initialize()
        {
            var rangeAttr = this.Entry.memberInfo.GetCustomAttribute<RangeAttribute>();
            var min = Mathf.CeilToInt(rangeAttr.min);
            var max = Mathf.FloorToInt(rangeAttr.max);
            view = new SliderInt(this.Entry.memberInfo.Name, min, max);
            intField = new IntegerField();
            intField.style.minWidth = 50;
            view.Add(intField);
            return view;
        }

        public override void Tick()
        {
        }
    }
}