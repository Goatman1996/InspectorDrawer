using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    internal class DoubleNumberField : DoubleField
    {
        private double previousValue;
        public event Action<double, double> OnEdited;

        public DoubleNumberField(string label = null) : base(label)
        {
            this.Init();
        }

        private void Init()
        {
            var labelView = this.labelElement;
            labelElement.RegisterCallback<MouseDownEvent>(this.OnDrag);
            labelElement.RegisterCallback<MouseUpEvent>(this.OnDrop);

            var input = this.textInputBase;
            input.RegisterCallback<FocusInEvent>(this.OnFocusIn);
            input.RegisterCallback<FocusOutEvent>(this.OnFocusOut);
        }

        private void OnFocusOut(FocusOutEvent evt)
        {
            var newValue = this.value;
            var previous = previousValue;
            OnEdited?.Invoke(previous, newValue);
        }

        private void OnFocusIn(FocusInEvent evt)
        {
            previousValue = this.value;
        }

        private void OnDrop(MouseUpEvent evt)
        {
            var newValue = this.value;
            var previous = previousValue;
            OnEdited?.Invoke(previous, newValue);
        }

        private void OnDrag(MouseDownEvent evt)
        {
            previousValue = this.value;
        }
    }
}