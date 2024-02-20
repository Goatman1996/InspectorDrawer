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
        private bool dragging;
        private bool editing;
        private int previousValue;

        public override VisualElement Initialize()
        {
            view = new IntegerField();
            view.label = this.Entry.memberInfo.Name;

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

            var undoCommand = new IntUndoCommand()
            {
                Do = () => { this.Entry.Value = newValue; },
                Undo = () => { this.Entry.Value = previous; }
            };
            UndoSystem.UndoSystem.Record(undoCommand);
            editing = false;
        }

        private void OnFocusIn(FocusInEvent evt)
        {
            editing = true;
            previousValue = this.view.value;
        }

        private void OnDrop(MouseUpEvent evt)
        {
            var newValue = this.view.value;
            var previous = previousValue;

            var undoCommand = new IntUndoCommand()
            {
                Do = () => { this.Entry.Value = newValue; },
                Undo = () => { this.Entry.Value = previous; }
            };

            UndoSystem.UndoSystem.Record(undoCommand);
            dragging = false;
        }

        private void OnDrag(MouseDownEvent evt)
        {
            dragging = true;
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

        private class IntUndoCommand : IUndoCommand
        {
            public Action Undo { get; set; }
            public Action Do { get; set; }
        }
    }
}