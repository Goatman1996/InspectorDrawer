using GMToolKit.Inspector.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(Vector4))]
    internal class Vector4Drawer : Drawer
    {
        Vector4Field view;
        Vector4 previousValue;
        public override VisualElement Initialize()
        {
            view = new Vector4Field(this.Entry.memberInfo.Name);

            previousValue = (Vector4)this.Entry.Value;

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
            var sourceValue = (Vector4)this.Entry.Value;
            var textValue = this.view.value;

            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}