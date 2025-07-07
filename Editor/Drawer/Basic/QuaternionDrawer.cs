using GMToolKit.Inspector.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(Quaternion))]
    internal class QuaternionDrawer : Drawer
    {
        Vector4Field view;
        Quaternion previousValue;
        public override VisualElement Initialize()
        {
            view = new Vector4Field(this.Entry.EntryName);
            previousValue = (Quaternion)this.Entry.Value;

            view.RegisterValueChangedCallback(e =>
            {
                var prevous = previousValue;
                var newValue = Convertor(e.newValue);

                this.Entry.Value = newValue;
                var undoCommand = new UndoCommand()
                {
                    Do = () => { this.Entry.Value = newValue; },
                    Undo = () => { this.Entry.Value = prevous; }
                };
                UndoSystem.UndoSystem.Record(undoCommand);

                previousValue = newValue;
            });

            return view;
        }

        public override void Tick()
        {
            var sourceValue = (Quaternion)this.Entry.Value;
            var textValue = Convertor(this.view.value);

            // if (sourceValue != textValue)
            if (!CustomEquals(sourceValue, textValue))
            {
                this.view.SetValueWithoutNotify(Convertor(sourceValue));
            }
        }

        private Quaternion Convertor(Vector4 v)
        {
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        private Vector4 Convertor(Quaternion v)
        {
            return new Vector4(v.x, v.y, v.z, v.w);
        }

        public bool CustomEquals(Quaternion a, Quaternion b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }
    }
}