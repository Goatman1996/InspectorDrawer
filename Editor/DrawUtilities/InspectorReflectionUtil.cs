using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMToolKit.Inspector
{
    public class InspectorReflectionUtil
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

        public static FieldInfo[] GetFields(Type type)
        {
            var ret = new List<FieldInfo>();
            while (type != typeof(object) && type != typeof(MonoBehaviour) && type != null)
            {
                var fields = type.GetFields(DefaultBindingFlags);
                if (fields != null)
                {
                    ret.AddRange(fields);
                }
                type = type.BaseType;
            }
            return ret.ToArray();
        }

        public static FieldInfo FindField(Type type, string fieldName)
        {
            while (type != typeof(object) && type != typeof(MonoBehaviour) && type != null)
            {
                var field = type.GetField(fieldName, DefaultBindingFlags);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }

        public static PropertyInfo[] GetProperties(Type type)
        {
            var ret = new List<PropertyInfo>();
            while (type != typeof(object) && type != typeof(MonoBehaviour) && type != null)
            {
                var properties = type.GetProperties(DefaultBindingFlags);
                if (properties != null)
                {
                    ret.AddRange(properties);
                }
                type = type.BaseType;
            }
            return ret.ToArray();
        }

        public static PropertyInfo FindProperty(Type type, string propertyName)
        {
            while (type != typeof(object) && type != typeof(MonoBehaviour) && type != null)
            {
                var property = type.GetProperty(propertyName, DefaultBindingFlags);
                if (property != null) return property;
                type = type.BaseType;
            }
            return null;
        }
    }
}