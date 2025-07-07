using System;
using System.Reflection;
using GMToolKit.Inspector.UndoSystem;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(int), typeof(RangeAttribute))]
    internal class IntRangeDrawer : Drawer
    {
        SliderInt view;
        IntegerField intField;
        private int min, max;
        private int previousValue;

        public override VisualElement Initialize()
        {
            var rangeAttr = this.Entry.GetEntryAttribute<RangeAttribute>();
            this.min = Mathf.CeilToInt(rangeAttr.min);
            this.max = Mathf.FloorToInt(rangeAttr.max);
            view = new SliderInt(this.Entry.EntryName, min, max);

            view.RegisterValueChangedCallback((e) =>
            {
                ApplyClampedValue(e.newValue);
            });

            var dragger = view.Q("unity-drag-container");
            dragger.RegisterCallback<MouseDownEvent>(this.OnSliderDrag);
            dragger.parent.RegisterCallback<MouseUpEvent>(this.OnSliderDrop);

            intField = new IntegerField();
            intField.style.minWidth = 50;
            intField.RegisterValueChangedCallback((e) =>
            {
                ApplyClampedValue(e.newValue);
            });
            var inputView = intField.Q("unity-text-input");
            inputView.RegisterCallback<FocusInEvent>(this.OnFocusIn);
            inputView.RegisterCallback<FocusOutEvent>(this.OnFocusOut);

            view.Add(intField);

            return view;
        }

        private void ApplyClampedValue(int value)
        {
            this.Entry.Value = Mathf.Clamp(value, this.min, this.max);
        }

        private void OnFocusOut(FocusOutEvent evt)
        {
            var newValue = this.view.value;
            var previous = previousValue;

            var undoCommand = new UndoCommand()
            {
                Do = () => { ApplyClampedValue(newValue); },
                Undo = () => { ApplyClampedValue(previous); }
            };
            UndoSystem.UndoSystem.Record(undoCommand);
        }

        private void OnFocusIn(FocusInEvent evt)
        {
            previousValue = this.view.value;
        }

        private void OnSliderDrop(MouseUpEvent evt)
        {
            var newValue = this.view.value;
            var previous = previousValue;

            var undoCommand = new UndoCommand()
            {
                Do = () => { ApplyClampedValue(newValue); },
                Undo = () => { ApplyClampedValue(previous); }
            };
            UndoSystem.UndoSystem.Record(undoCommand);
        }

        private void OnSliderDrag(MouseDownEvent evt)
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
            if (this.intField.value != sourceValue)
            {
                this.intField.SetValueWithoutNotify(sourceValue);
            }
        }
    }
}