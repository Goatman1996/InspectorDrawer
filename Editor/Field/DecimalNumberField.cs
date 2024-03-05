using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    internal class DecimalNumberField : TextValueField<decimal>
    {
        internal DecimalNumberField() : this(null) { }
        internal DecimalNumberField(string label) : this(label, -1, new DecimalNumberInput()) { }

        private DecimalNumberField(string label, int maxLength, DecimalNumberInput input) : base(label, maxLength, input)
        {
            this.label = label;
            this.AddLabelDragger<decimal>();

            var labelView = this.labelElement;
            labelElement.RegisterCallback<MouseDownEvent>(this.OnDrag);
            labelElement.RegisterCallback<MouseUpEvent>(this.OnDrop);

            input.RegisterCallback<FocusInEvent>(this.OnFocusIn);
            input.RegisterCallback<FocusOutEvent>(this.OnFocusOut);
        }

        private decimal previousValue;
        public event Action<decimal, decimal> OnEdited;

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

        private DecimalNumberInput decimalInput => (DecimalNumberInput)textInputBase;

        public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, decimal startValue)
        {
            decimalInput.ApplyInputDeviceDelta(delta, speed, startValue);
        }

        protected override decimal StringToValue(string str)
        {
            if (DecimalExpressionEvaluator.EvaluateDecimal(str, out decimal result))
            {
                return result;
            }
            return default;
        }

        protected override string ValueToString(decimal value)
        {
            return value.ToString();
        }

        private class DecimalNumberInput : TextValueInput
        {
            protected override string allowedCharacters => "inftynaeINFTYNAE0123456789.,-*/+%mM()";
            private DecimalNumberField parentField => (DecimalNumberField)parent;

            public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, decimal startValue)
            {
                var previousValue = this.StringToValue(text);
                var deltaValue = GetSpeededDelta(delta.x, speed);
                var nextValue = previousValue + deltaValue;

                if (parentField.isDelayed)
                {
                    this.text = ValueToString(nextValue);
                }
                else
                {
                    parentField.value = nextValue;
                }
            }

            private decimal GetSpeededDelta(float delta, DeltaSpeed speed)
            {
                delta = delta / 100f;
                if (speed == DeltaSpeed.Normal)
                {
                    return (decimal)delta;
                }
                else if (speed == DeltaSpeed.Fast)
                {
                    return (decimal)(delta * 4);
                }
                else
                {
                    return new decimal(delta / 4);
                }
            }

            protected override string ValueToString(decimal value)
            {
                return value.ToString();
            }

            protected override decimal StringToValue(string str)
            {
                if (DecimalExpressionEvaluator.EvaluateDecimal(str, out decimal result))
                {
                    return result;
                }
                return default;
            }
        }
    }
}