using GMToolKit.Inspector.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(BoundsInt))]
    internal class BoundsIntDrawer : Drawer
    {
        BoundsIntField view;
        BoundsInt previousValue;
        public override VisualElement Initialize()
        {
            view = new BoundsIntField(this.Entry.memberInfo.Name);

            previousValue = (BoundsInt)this.Entry.Value;

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
            var sourceValue = (BoundsInt)this.Entry.Value;
            var textValue = this.view.value;

            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}