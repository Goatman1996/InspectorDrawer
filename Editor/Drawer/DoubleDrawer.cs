using System;
using GMToolKit.Inspector.UndoSystem;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(double))]
    internal class DoubleDrawer : Drawer
    {
        DoubleNumberField view;
        public override VisualElement Initialize()
        {
            view = new DoubleNumberField(this.Entry.memberInfo.Name);

            view.RegisterValueChangedCallback(e =>
            {
                this.Entry.Value = view.value;
            });

            view.SetEnabled(this.Entry.IsSettable());

            view.OnEdited += this.OnEdited;

            return view;
        }

        private void OnEdited(double previous, double newValue)
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
            var sourceValue = (double)this.Entry.Value;
            var textValue = this.view.value;
            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}