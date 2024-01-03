using System.Reflection;
using UnityEngine;

namespace GMToolKit.Inspector
{
    public class InspectorIfUtil
    {
        private static bool CheckInspectIf(InspectIfAttribute inspectIFAttr, object mono)
        {
            var bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            var condition = inspectIFAttr.condition;
            var optionalValue = inspectIFAttr.optionalValue;

            var conditionalField = InspectorReflectionUtil.FindField(mono.GetType(), condition);
            if (conditionalField != null)
            {
                object conditionalValue = null;
                if (conditionalField.IsStatic)
                {
                    conditionalValue = conditionalField.GetValue(null);
                }
                else
                {
                    conditionalValue = conditionalField.GetValue(mono);
                }
                if (conditionalValue is bool conditionalBool)
                {
                    if (optionalValue is bool optionalBool)
                    {
                        return conditionalBool == optionalBool;
                    }
                    return conditionalBool;
                }
                else
                {
                    return object.Equals(optionalValue, conditionalValue);
                }
            }

            var conditionalProperty = InspectorReflectionUtil.FindProperty(mono.GetType(), condition);
            if (conditionalProperty != null && conditionalProperty.GetMethod != null)
            {
                object conditionalValue = null;
                var getMethod = conditionalProperty.GetMethod;

                if (getMethod.IsStatic)
                {
                    conditionalValue = getMethod.Invoke(null, default);
                }
                else
                {
                    conditionalValue = getMethod.Invoke(mono, default);
                }
                if (conditionalValue is bool conditionalBool)
                {
                    if (optionalValue is bool optionalBool)
                    {
                        return conditionalBool == optionalBool;
                    }
                    return conditionalBool;
                }
                else
                {
                    return object.Equals(optionalValue, conditionalValue);
                }
            }
            return false;
        }

        public static bool CheckInspectIf(FieldInfo field, object mono)
        {
            var inspectAttr = field.GetCustomAttribute<InspectAttribute>();
            if (inspectAttr == null)
            {
                return false;
            }

            var inspectIFAttr = field.GetCustomAttribute<InspectIfAttribute>();
            if (inspectIFAttr == null)
            {
                return true;
            }

            return CheckInspectIf(inspectIFAttr, mono);
        }

        public static bool CheckInspectIf(PropertyInfo propertyInfo, object mono)
        {
            var inspectAttr = propertyInfo.GetCustomAttribute<InspectAttribute>();
            if (inspectAttr == null)
            {
                return false;
            }

            var inspectIFAttr = propertyInfo.GetCustomAttribute<InspectIfAttribute>();
            if (inspectIFAttr == null)
            {
                return true;
            }

            return CheckInspectIf(inspectIFAttr, mono);
        }
    }
}