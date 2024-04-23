using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace GMToolKit.Inspector
{
    internal class ReflectionUtil
    {
        public static object CreateInstance(Type type)
        {
            object ret = null;
            var defaultConstructor = type.GetConstructor(new Type[0]);
            if (defaultConstructor != null)
            {
                ret = Activator.CreateInstance(type);
            }
            else
            {
                ret = FormatterServices.GetUninitializedObject(type);
            }
            return ret;
        }

        private static BindingFlags DefaultBindingFlags =>
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        public static void GetInheritedTypeList(Type type, List<Type> result)
        {
            var targetType = type;
            while (ValidInherit(targetType))
            {
                result.Add(targetType);
                targetType = targetType.BaseType;
            }

            result.Reverse();
        }

        private static bool ValidInherit(Type type)
        {
            if (type == typeof(UnityEngine.MonoBehaviour)) return false;
            if (type == typeof(UnityEngine.ScriptableObject)) return false;
            if (type == typeof(ValueType)) return false;
            if (type == typeof(object)) return false;
            if (type == null) return false;

            return true;
        }

        public static void GetVisibleMember(Type type, List<MemberInfo> result)
        {
            type.GetMembers(DefaultBindingFlags)
                .Where(x => IsVisibleMember(x))
                .ToList()
                .ForEach(x => result.Add(x));
        }

        public static bool IsVisibleMember(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return IsVisibleField(fieldInfo);
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                return IsVisibleProperty(propertyInfo);
            }

            return false;
        }

        private static bool IsVisibleField(FieldInfo fieldInfo)
        {
            if (!fieldInfo.IsPublic)
            {
                var inspectAttr = fieldInfo.GetCustomAttribute<InspectAttribute>();
                var serializeFieldAttr = fieldInfo.GetCustomAttribute<UnityEngine.SerializeField>();
                if (inspectAttr == null && serializeFieldAttr == null) return false;
            }
            return DrawerProvider.CanDrawerType(fieldInfo.FieldType);
        }

        private static bool IsVisibleProperty(PropertyInfo propertyInfo)
        {
            return DrawerProvider.CanDrawerType(propertyInfo.PropertyType);
        }

        public static HashSet<Type> DefaultVisibleTypes = new HashSet<Type>
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(long),
            typeof(ulong),

            typeof(float),
            typeof(double),

            typeof(Color),
            typeof(Color32),
            typeof(Gradient),
            typeof(Quaternion),

            typeof(Vector2),
            typeof(Vector2Int),
            typeof(Vector3),
            typeof(Vector3Int),
            typeof(Vector4),
            typeof(Rect),
            typeof(RectInt),
            typeof(Bounds),
            typeof(BoundsInt),

            typeof(AnimationCurve),

            typeof(UnityEngine.Object),
        };
    }
}