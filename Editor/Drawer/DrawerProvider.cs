using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using Codice.Client.BaseCommands;

namespace GMToolKit.Inspector
{
    public static class DrawerProvider
    {
        private class Provider
        {
            private Type visibleType;
            private Dictionary<Type, Type> specialDrawerDic;
            private Type defaultDrawerType;

            public Provider(Type visibleType)
            {
                this.visibleType = visibleType;
                specialDrawerDic = new Dictionary<Type, Type>();
            }

            public void Add(Type drawerType)
            {
                var drawerAttribute = drawerType.GetCustomAttribute<DrawerAttribute>();
                if (drawerAttribute.special == null)
                {
                    this.defaultDrawerType = drawerType;
                }
                else
                {
                    specialDrawerDic.Add(drawerAttribute.special, drawerType);
                }
            }

            public Type GetDrawerType(MemberInfo memberInfo)
            {
                var inspectAttributeList = memberInfo.GetCustomAttributes<InspectAttribute>(true);
                foreach (var inspectAttribute in inspectAttributeList)
                {
                    var specialType = inspectAttribute.GetType();
                    if (specialType != typeof(InspectAttribute))
                    {
                        if (this.specialDrawerDic.ContainsKey(specialType))
                        {
                            return this.specialDrawerDic[specialType];
                        }
                    }
                }

                return this.defaultDrawerType;
            }
        }

        private static Dictionary<Type, Provider> drawerProviderDic;

        private static void Initialize()
        {
            drawerProviderDic = new Dictionary<Type, Provider>();
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
                    if (!drawerProviderDic.ContainsKey(visibleType))
                    {
                        // 显示类型，绘制器类型
                        drawerProviderDic.Add(visibleType, new Provider(visibleType));
                    }
                    var provider = drawerProviderDic[visibleType];
                    provider.Add(drawerType);
                }
            }
        }

        public static Drawer CreateDrawer(MemberInfo memberInfo, Type visibleType)
        {
            if (drawerProviderDic == null)
            {
                Initialize();
            }

            if (drawerProviderDic.ContainsKey(visibleType))
            {
                var provider = drawerProviderDic[visibleType];
                var drawerType = provider.GetDrawerType(memberInfo);
                var drawer = (Drawer)ReflectionUtil.CreateInstance(drawerType);
                return drawer;
            }

            return null;
        }

        public static bool CanDrawerType(Type type)
        {
            if (drawerProviderDic == null)
            {
                Initialize();
            }

            return drawerProviderDic.ContainsKey(type);
        }
    }
}