using GMToolKit.Inspector.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(Vector3))]
    internal class Vector3Drawer : Drawer
    {
        Vector3Field view;
        Vector3 previousValue;
        public override VisualElement Initialize()
        {
            view = new Vector3Field(this.Entry.EntryName);

            previousValue = (Vector3)this.Entry.Value;

            view.RegisterValueChangedCallback(e =>
            {
                var prevous = previousValue;
                var newValue = e.newValue;

                this.Entry.Value = newValue;
                var undoCommand = new UndoCommand()
                {
                    Do = () => { this.Entry.Value = newValue; },
                    Undo = () => { this.Entry.Value = prevous; }
                };
                UndoSystem.UndoSystem.Record(undoCommand);

                previousValue = newValue;
            });
            return view;
        }

        public override void Tick()
        {
            var sourceValue = (Vector3)this.Entry.Value;
            var textValue = this.view.value;

            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}