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
            view.label = this.Entry.EntryName;
            view.RegisterValueChangedCallback((e) =>
            {
                var newValue = e.newValue;
                var undoCommand = new UndoCommand
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
    }
}