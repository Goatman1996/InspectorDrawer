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
            view.value = (Gradient)this.Entry.Value;
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
            if (!Compare(sourceValue, textValue))
            {
                Debug.Log("1");
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
        private bool Compare(Gradient a, Gradient b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            if (b == null) return false;

            var a_colorKeys = a.colorKeys;
            var b_colorKeys = b.colorKeys;

            if (a_colorKeys.Length != b_colorKeys.Length) return false;

            for (var i = 0; i < a_colorKeys.Length; i++)
            {
                if (a_colorKeys[i].time != b_colorKeys[i].time) return false;
                if (a_colorKeys[i].color != b_colorKeys[i].color) return false;
            }

            var a_alphaKeys = a.alphaKeys;
            var b_alphaKeys = b.alphaKeys;

            if (a_alphaKeys.Length != b_alphaKeys.Length) return false;

            for (var i = 0; i < a_alphaKeys.Length; i++)
            {
                if (a_alphaKeys[i].time != b_alphaKeys[i].time) return false;
                if (a_alphaKeys[i].alpha != b_alphaKeys[i].alpha) return false;
            }

            if (a.mode != b.mode) return false;

            return true;
        }
    }
}