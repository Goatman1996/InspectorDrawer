using System;
using System.Numerics;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    internal class BigIntegerField : TextValueField<BigInteger>
    {
        internal BigIntegerField(BigInteger min, BigInteger max)
        : this(null, min, max) { }

        internal BigIntegerField(string label, BigInteger min, BigInteger max)
        : this(label, -1, new BigIntegerInput())
        {
            this.value = 0;
            this.MinValue = min;
            this.MaxValue = max;
        }

        private BigIntegerField(string label, int maxLength, BigIntegerInput input)
        : base(label, maxLength, input)
        {
            this.label = label;
            this.AddLabelDragger<BigInteger>();

            var labelView = this.labelElement;
            labelElement.RegisterCallback<MouseDownEvent>(this.OnDrag);
            labelElement.RegisterCallback<MouseUpEvent>(this.OnDrop);

            input.RegisterCallback<FocusInEvent>(this.OnFocusIn);
            input.RegisterCallback<FocusOutEvent>(this.OnFocusOut);
        }

        private BigInteger previousValue;
        public event Action<BigInteger, BigInteger> OnEdited;

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

        private BigInteger MinValue, MaxValue;
        private BigIntegerInput bigInput => (BigIntegerInput)textInputBase;

        public override void ApplyInputDeviceDelta(UnityEngine.Vector3 delta, DeltaSpeed speed, BigInteger startValue)
        {
            bigInput.ApplyInputDeviceDelta(delta, speed, startValue);
        }

        protected override BigInteger StringToValue(string str)
        {
            if (IntegerExpressionEvaluator.EvaluateInteger(str, out BigInteger result))
            {
                result = GetClampedValue(result);
                return result;
            }
            return this.MinValue;
        }

        protected override string ValueToString(BigInteger value)
        {
            return value.ToString();
        }

        private BigInteger GetClampedValue(BigInteger value)
        {
            if (value > this.MaxValue)
            {
                return this.MaxValue;
            }
            if (value < this.MinValue)
            {
                return this.MinValue;
            }
            return value;
        }

        private class BigIntegerInput : TextValueInput
        {
            protected override string allowedCharacters => "0123456789-*/+%^()";

            private BigIntegerField parentField => (BigIntegerField)parent;

            public override void ApplyInputDeviceDelta(UnityEngine.Vector3 delta, DeltaSpeed speed, BigInteger startValue)
            {
                var previousValue = this.StringToValue(text);
                var deltaValue = GetSpeededDelta(delta.x, speed);
                var nextValue = previousValue + deltaValue;
                nextValue = this.parentField.GetClampedValue(nextValue);

                if (parentField.isDelayed)
                {
                    this.text = ValueToString(nextValue);
                }
                else
                {
                    parentField.value = nextValue;
                }
            }

            private BigInteger GetSpeededDelta(float delta, DeltaSpeed speed)
            {
                if (speed == DeltaSpeed.Normal)
                {
                    return new BigInteger(delta);
                }
                else if (speed == DeltaSpeed.Fast)
                {
                    return new BigInteger(delta * 4);
                }
                else
                {
                    return new BigInteger(delta / 4);
                }
            }

            protected override string ValueToString(BigInteger value)
            {
                return value.ToString();
            }

            protected override BigInteger StringToValue(string str)
            {
                if (IntegerExpressionEvaluator.EvaluateInteger(str, out BigInteger result))
                {
                    result = this.parentField.GetClampedValue(result);
                    return result;
                }
                return this.parentField.MinValue;
            }
        }
    }
}