using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GMToolKit.Inspector.UndoSystem;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(Enum), inherited = true)]
    internal class EnumDrawer : Drawer
    {
        EnumFlagsField enumFlagsField;
        EnumField view;
        Enum previousValue;
        public override VisualElement Initialize()
        {
            var flags = this.Entry.EntryType.GetCustomAttribute<FlagsAttribute>();
            if (flags != null)
            {
                enumFlagsField = new EnumFlagsField(this.Entry.EntryName);
                enumFlagsField.Init((Enum)this.Entry.Value, true);
                previousValue = (Enum)this.Entry.Value;
                enumFlagsField.RegisterValueChangedCallback((e) =>
                {
                    var newValue = e.newValue;
                    var previous = previousValue;
                    this.Entry.Value = newValue;

                    var undoCommand = new UndoCommand
                    {
                        Do = () => this.Entry.Value = newValue,
                        Undo = () => this.Entry.Value = previous,
                    };
                    UndoSystem.UndoSystem.Record(undoCommand);
                    previousValue = newValue;
                });
                return enumFlagsField;
            }
            else
            {
                view = new EnumField(this.Entry.EntryName);
                view.Init((Enum)this.Entry.Value, true);
                previousValue = (Enum)this.Entry.Value;
                view.RegisterValueChangedCallback((e) =>
                {
                    var newValue = e.newValue;
                    var previous = previousValue;
                    this.Entry.Value = newValue;

                    var undoCommand = new UndoCommand
                    {
                        Do = () => this.Entry.Value = newValue,
                        Undo = () => this.Entry.Value = previous,
                    };
                    UndoSystem.UndoSystem.Record(undoCommand);
                    previousValue = newValue;
                });
                return view;
            }
        }

        public override void Tick()
        {
            var sourceValue = (Enum)this.Entry.Value;
            Enum textValue;
            if (this.view == null)
            {
                textValue = this.enumFlagsField.value;
            }
            else
            {
                textValue = this.view.value;
            }
            if (sourceValue != textValue)
            {
                if (this.view == null)
                {
                    this.enumFlagsField.SetValueWithoutNotify(sourceValue);
                }
                else
                {
                    this.view.SetValueWithoutNotify(sourceValue);
                }
            }
        }
    }
}