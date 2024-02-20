using System;
using GMToolKit.Inspector.UndoSystem;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(int))]
    internal class IntDrawer : Drawer
    {
        IntegerField view;
        private int previousValue;

        public override VisualElement Initialize()
        {
            view = new IntegerField(this.Entry.memberInfo.Name);

            view.RegisterValueChangedCallback((e) =>
            {
                this.Entry.Value = e.newValue;
            });

            view.SetEnabled(this.Entry.IsSettable());

            var labelView = view.labelElement;
            labelView.RegisterCallback<MouseDownEvent>(this.OnDrag);
            labelView.RegisterCallback<MouseUpEvent>(this.OnDrop);
            var inputView = view.Q("unity-text-input");
            inputView.RegisterCallback<FocusInEvent>(this.OnFocusIn);
            inputView.RegisterCallback<FocusOutEvent>(this.OnFocusOut);
            return view;
        }

        private void OnFocusOut(FocusOutEvent evt)
        {
            var newValue = this.view.value;
            var previous = previousValue;

            var undoCommand = new UndoCommand()
            {
                Do = () => { this.Entry.Value = newValue; },
                Undo = () => { this.Entry.Value = previous; }
            };
            UndoSystem.UndoSystem.Record(undoCommand);
        }

        private void OnFocusIn(FocusInEvent evt)
        {
            previousValue = this.view.value;
        }

        private void OnDrop(MouseUpEvent evt)
        {
            var newValue = this.view.value;
            var previous = previousValue;

            var undoCommand = new UndoCommand()
            {
                Do = () => { this.Entry.Value = newValue; },
                Undo = () => { this.Entry.Value = previous; }
            };

            UndoSystem.UndoSystem.Record(undoCommand);
        }

        private void OnDrag(MouseDownEvent evt)
        {
            previousValue = this.view.value;
        }

        public override void Tick()
        {
            var sourceValue = (int)this.Entry.Value;
            if (this.view.value != sourceValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}