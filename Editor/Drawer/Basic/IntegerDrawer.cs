
using System;
using System.Numerics;
using GMToolKit.Inspector.UndoSystem;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    internal abstract class IntegerDrawer<T> : Drawer
    {
        protected abstract BigInteger Convertor(T v);
        protected abstract T Convertor(BigInteger v);
        protected abstract T MinValue { get; }
        protected abstract T MaxValue { get; }
        protected abstract bool Equals(T left, T right);

        private BigIntegerField view;
        public override VisualElement Initialize()
        {
            var min = Convertor(MinValue);
            var max = Convertor(MaxValue);
            this.view = new BigIntegerField(this.Entry.EntryName, min, max);

            view.RegisterValueChangedCallback(e =>
            {
                this.Entry.Value = Convertor(e.newValue);
            });

            view.SetEnabled(this.Entry.IsSettable());

            view.OnEdited += this.OnEdited;

            return view;
        }

        private void OnEdited(BigInteger previous, BigInteger newValue)
        {
            var undoCommand = new UndoCommand()
            {
                Do = () => { this.Entry.Value = Convertor(newValue); },
                Undo = () => { this.Entry.Value = Convertor(previous); }
            };
            UndoSystem.UndoSystem.Record(undoCommand);
        }

        public override void Tick()
        {
            var sourceValue = (T)this.Entry.Value;
            var textValue = Convertor(this.view.value);
            if (!Equals(sourceValue, textValue))
            {
                this.view.SetValueWithoutNotify(Convertor(sourceValue));
            }
        }
    }
}