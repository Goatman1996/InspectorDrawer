using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GMToolKit.Inspector
{
    internal sealed class DrawerEntry : IDrawerEntry
    {
        public Drawer drawer;
        public Type EntryType { get; private set; }
        public bool isDirty;
        public MemberInfo memberInfo;
        public string EntryName { get => this.memberInfo.Name; }

        private object instance;
        private bool IsStatic { get; set; }

        private DrawerEntry() { }

        public static DrawerEntry CreateEntry(MemberInfo memberInfo, object instance)
        {
            var drawerEntry = new DrawerEntry();
            drawerEntry.memberInfo = memberInfo;
            drawerEntry.IsStatic = MemberUtil.IsMemberStatic(memberInfo);
            drawerEntry.EntryType = MemberUtil.GetMemberValueType(memberInfo);
            if (!drawerEntry.IsStatic)
            {
                drawerEntry.instance = instance;
            }

            drawerEntry.drawer = DrawerProvider.CreateDrawer(drawerEntry.memberInfo, drawerEntry.EntryType);

            return drawerEntry;
        }

        public object Value
        {
            get => this.Getter();
            set => this.Setter(value);
        }

        private object Getter()
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(instance);
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                var getMethod = propertyInfo.GetMethod;
                return getMethod.Invoke(instance, default);
            }
            else
            {
                Debug.LogError($"{memberInfo.DeclaringType}.{memberInfo.Name} Not Found Getter");
                return null;
            }
        }

        private void Setter(object newValue)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                // readonly
                if (fieldInfo.IsInitOnly) return;
                // const
                if (fieldInfo.IsLiteral) return;

                fieldInfo.SetValue(instance, newValue);
                this.isDirty = true;
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                var setMethod = propertyInfo.SetMethod;
                // get only
                if (setMethod == null) return;

                setMethod.Invoke(instance, new object[] { newValue });
                this.isDirty = true;
            }
            else
            {
                Debug.LogWarning($"{memberInfo.DeclaringType}.{memberInfo.Name} Not Found Setter");
                return;
            }
        }

        public bool IsSettable()
        {
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                var fieldInfo = memberInfo as FieldInfo;
                // readonly
                if (fieldInfo.IsInitOnly) return false;
                // const
                if (fieldInfo.IsLiteral) return false;

                return true;
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberInfo as PropertyInfo;
                var setMethod = propertyInfo.SetMethod;
                // get only
                if (setMethod == null) return false;

                return true;
            }

            return false;
        }

        public T GetEntryAttribute<T>() where T : Attribute
        {
            return this.memberInfo.GetCustomAttribute<T>();
        }
    }
}