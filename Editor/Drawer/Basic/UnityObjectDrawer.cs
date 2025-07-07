using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(UnityEngine.Object), inherited = true)]
    internal class UnityObjectDrawer : Drawer
    {
        ObjectField view;
        public override VisualElement Initialize()
        {
            view = new ObjectField(this.Entry.EntryName);
            view.allowSceneObjects = true;
            view.objectType = this.Entry.EntryType;
            view.RegisterValueChangedCallback(e =>
            {
                this.Entry.Value = e.newValue;
            });

            return view;
        }

        public override void Tick()
        {
            var sourceValue = (UnityEngine.Object)this.Entry.Value;
            var textValue = this.view.value;
            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}