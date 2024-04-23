using GMToolKit.Inspector.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(Vector2Int))]
    internal class Vector2IntDrawer : Drawer
    {
        Vector2IntField view;
        Vector2Int previousValue;
        public override VisualElement Initialize()
        {
            view = new Vector2IntField(this.Entry.memberInfo.Name);

            previousValue = (Vector2Int)this.Entry.Value;

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
            var sourceValue = (Vector2Int)this.Entry.Value;
            var textValue = this.view.value;

            if (sourceValue != textValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}