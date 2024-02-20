using System;
using System.Collections.Generic;
using System.Reflection;

namespace GMToolKit.Inspector
{
    internal class DrawerManifest
    {
        public List<DrawerEntry> entryList;

        public static DrawerManifest CreateManifest(object target)
        {
            var manifest = new DrawerManifest();

            var type = target.GetType();

            var inheritedTypeList = new List<Type>();
            ReflectionUtil.GetInheritedTypeList(type, inheritedTypeList);

            manifest.entryList = new List<DrawerEntry>();

            var result = new List<MemberInfo>();

            foreach (var t in inheritedTypeList)
            {
                ReflectionUtil.GetVisibleMember(t, result);
            }
            foreach (var memberInfo in result)
            {
                var drawerEntry = DrawerEntry.CreateEntry(memberInfo, target);

                manifest.entryList.Add(drawerEntry);
            }

            return manifest;

        }
    }
}