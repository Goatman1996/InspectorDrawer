using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using Codice.Client.BaseCommands;
using UnityEngine;

namespace GMToolKit.Inspector
{
    public static class DrawerProvider
    {
        private class Provider
        {
            private Type visibleType;
            private Dictionary<Type, Type> specialDrawerDic;
            private Type defaultDrawerType;
            public bool inherited { get; private set; }

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

                this.inherited = drawerAttribute.inherited;
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

            public bool IsInherited(Type type)
            {
                return this.visibleType.IsAssignableFrom(type);
            }
        }

        private static Dictionary<Type, Provider> drawerProviderDic;
        private static HashSet<Provider> inheritedProviderList;

        private static void Initialize()
        {
            inheritedProviderList = new HashSet<Provider>();
            drawerProviderDic = new Dictionary<Type, Provider>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass) continue;
                    if (!type.IsSubclassOf(typeof(Drawer))) continue;

                    var drawerAttribute = type.GetCustomAttribute<DrawerAttribute>();
                    if (drawerAttribute == null) continue;

                    var drawerType = type;
                    var visibleType = drawerAttribute.type;
                    if (!drawerProviderDic.ContainsKey(visibleType))
                    {
                        // 显示类型，绘制器类型
                        var newProvider = new Provider(visibleType);
                        drawerProviderDic.Add(visibleType, newProvider);

                    }
                    var provider = drawerProviderDic[visibleType];
                    provider.Add(drawerType);
                    if (provider.inherited)
                    {
                        inheritedProviderList.Add(provider);
                    }
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

            foreach (var inheritedProvider in inheritedProviderList)
            {
                var IsInherited = inheritedProvider.IsInherited(visibleType);
                if (IsInherited)
                {
                    var drawerType = inheritedProvider.GetDrawerType(memberInfo);
                    var drawer = (Drawer)ReflectionUtil.CreateInstance(drawerType);
                    return drawer;
                }
            }

            return null;
        }

        public static bool CanDrawerType(Type type)
        {
            if (drawerProviderDic == null)
            {
                Initialize();
            }

            var hasDrawer = drawerProviderDic.ContainsKey(type);
            var hasInheritedDrawer = false;

            foreach (var inheritedProvider in inheritedProviderList)
            {
                var IsInherited = inheritedProvider.IsInherited(type);
                if (IsInherited)
                {
                    hasInheritedDrawer = true;
                    break;
                }
            }

            return hasDrawer || hasInheritedDrawer;
        }
    }
}