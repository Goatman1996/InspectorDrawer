using System;
using GMToolKit.Inspector.UndoSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(bool))]
    internal class BoolDrawer : Drawer
    {
        Toggle view;
        public override VisualElement Initialize()
        {
            view = new Toggle();
            view.label = this.Entry.memberInfo.Name;
            view.RegisterValueChangedCallback((e) =>
            {
                var newValue = e.newValue;
                var undoCommand = new BoolUndoCommand
                {
                    Do = () => this.Entry.Value = newValue,
                    Undo = () => this.Entry.Value = !newValue,
                };
                undoCommand.Do();
                UndoSystem.UndoSystem.Record(undoCommand);
            });

            view.SetEnabled(this.Entry.IsSettable());

            return view;
        }

        public override void Tick()
        {
            var sourceValue = (bool)this.Entry.Value;
            if (this.view.value != sourceValue)
            {
                this.view.SetValueWithoutNotify(sourceValue);
            }
        }

        private class BoolUndoCommand : UndoSystem.IUndoCommand
        {
            public Action Do { get; set; }
            public Action Undo { get; set; }
        }
    }
}