using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(AnimationCurve))]
    internal class AnimationCurveDrawer : Drawer
    {
        CurveField view;
        public override VisualElement Initialize()
        {
            view = new CurveField(this.Entry.EntryName);
            view.RegisterValueChangedCallback(e =>
            {
                this.Entry.Value = e.newValue;
            });

            return view;
        }

        public override void Tick()
        {
            var sourceValue = (AnimationCurve)this.Entry.Value;
            var textValue = this.view.value;
            if (!sourceValue.Equals(textValue))
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}