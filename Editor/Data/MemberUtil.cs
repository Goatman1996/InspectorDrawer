using System;
using System.Reflection;

namespace GMToolKit.Inspector
{
    internal static class MemberUtil
    {
        public static bool IsMemberStatic(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.IsStatic;
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.GetMethod.IsStatic;
            }
            else if (memberInfo is MethodInfo methodInfo)
            {
                return methodInfo.IsStatic;
            }
            return false;
        }

        public static Type GetMemberValueType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }
            return null;
        }
    }
}