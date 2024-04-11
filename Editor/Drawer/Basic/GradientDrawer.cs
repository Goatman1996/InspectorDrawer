using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(Gradient))]
    internal class GradientDrawer : Drawer
    {
        GradientField view;
        public override VisualElement Initialize()
        {
            view = new GradientField(this.Entry.memberInfo.Name);
            view.RegisterValueChangedCallback(e =>
            {
                this.Entry.Value = e.newValue;
            });

            return view;
        }

        public override void Tick()
        {
            var sourceValue = (Gradient)this.Entry.Value;
            var textValue = this.view.value;
            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}