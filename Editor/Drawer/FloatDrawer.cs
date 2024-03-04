using System;
using GMToolKit.Inspector.UndoSystem;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(float))]
    internal class FloatDrawer : Drawer
    {
        FloatNumberField view;
        public override VisualElement Initialize()
        {
            view = new FloatNumberField(this.Entry.memberInfo.Name);

            view.RegisterValueChangedCallback(e =>
            {
                this.Entry.Value = view.value;
            });

            view.SetEnabled(this.Entry.IsSettable());

            view.OnEdited += this.OnEdited;

            return view;
        }

        private void OnEdited(float previous, float newValue)
        {
            var undoCommand = new UndoCommand()
            {
                Do = () => { this.Entry.Value = newValue; },
                Undo = () => { this.Entry.Value = previous; }
            };
            UndoSystem.UndoSystem.Record(undoCommand);
        }

        public override void Tick()
        {
            var sourceValue = (float)this.Entry.Value;
            var textValue = this.view.value;
            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}