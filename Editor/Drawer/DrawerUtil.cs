
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace GMToolKit.Inspector
{
    internal static class DrawerUtil
    {
        private static void Initialize()
        {
            exitsDrawers = new Dictionary<Type, Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass) continue;
                    if (!type.IsSubclassOf(typeof(Drawer))) continue;

                    var drawerAttribute = type.GetCustomAttribute<DrawerAttribute>();
                    if (drawerAttribute == null) return;

                    var drawerType = type;
                    var visibleType = drawerAttribute.type;
                    if (!exitsDrawers.ContainsKey(visibleType))
                    {
                        // 显示类型，绘制器类型
                        exitsDrawers.Add(visibleType, drawerType);
                    }
                }
            }
        }


        public static Dictionary<Type, Type> exitsDrawers;

        public static Drawer CreateDrawer(Type type)
        {
            if (exitsDrawers == null)
            {
                Initialize();
            }

            if (exitsDrawers.ContainsKey(type))
            {
                var drawerType = exitsDrawers[type];
                var drawer = (Drawer)ReflectionUtil.CreateInstance(drawerType);
                return drawer;
            }

            return null;
        }

        public static bool CanDrawerType(Type type)
        {
            if (exitsDrawers == null)
            {
                Initialize();
            }

            return exitsDrawers.ContainsKey(type);
        }
    }
}