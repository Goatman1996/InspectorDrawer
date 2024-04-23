using GMToolKit.Inspector.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(RectInt))]
    internal class RectIntDrawer : Drawer
    {
        RectIntField view;
        RectInt previousValue;
        public override VisualElement Initialize()
        {
            view = new RectIntField(this.Entry.memberInfo.Name);

            previousValue = (RectInt)this.Entry.Value;

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
            var sourceValue = (RectInt)this.Entry.Value;
            var textValue = this.view.value;

            if (!sourceValue.Equals(textValue))
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}