using UnityEditor;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GMToolKit.Inspector
{
    internal static class InspectorDrawerUtility
    {
        public static readonly GUIStyle leftSpaceStyle = new GUIStyle() { margin = new RectOffset(30, 0, 0, 0) };

        public static bool IsCollection(Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type);
        }

        public static void DrawHorizontalLine(UnityEngine.Color color)
        {
            var colorTemp = GUI.color;
            GUI.color = color;
            GUIStyle line = new GUIStyle();
            line.normal.background = EditorGUIUtility.whiteTexture;
            line.margin = new RectOffset(0, 0, 3, 3);
            line.fixedHeight = 1;
            GUILayout.Box(GUIContent.none, line);
            GUI.color = colorTemp;
        }

        public static void DrawVerticalLine(UnityEngine.Color color)
        {
            var colorTemp = GUI.color;
            GUI.color = color;
            GUIStyle line = new GUIStyle();
            line.normal.background = EditorGUIUtility.whiteTexture;
            line.margin = new RectOffset(0, 0, 3, 3);
            line.fixedHeight = 1;
            GUILayout.Box(GUIContent.none, line, GUILayout.ExpandHeight(true));
            GUI.color = colorTemp;
        }

        public static void DrawLable(string content, Color color)
        {
            var colorTemp = GUI.color;
            GUI.color = color;
            GUILayout.Label(content);
            GUI.color = colorTemp;
        }

        public static object DrawField(string name, Type type, object oldValue, string paramCacheKey, out bool changed)
        {
            changed = false;
            if (InspectorDrawer.DrawerLevel >= 7)
            {
                EditorGUILayout.LabelField($"绘制深度不能超过7");
                return null;
            }

            if (type == typeof(bool)) return DrawBool(name, oldValue, out changed);
            else if (type == typeof(int)) return DrawInt(name, oldValue, out changed);
            else if (type == typeof(uint)) return DrawUInt(name, oldValue, out changed);
            else if (type == typeof(long)) return DrawLong(name, oldValue, out changed);
            else if (type == typeof(ulong)) return DrawULong(name, oldValue, out changed);
            else if (type == typeof(string)) return DrawString(name, oldValue, out changed);
            else if (type == typeof(float)) return DrawFloat(name, oldValue, out changed);
            else if (type == typeof(double)) return DrawDouble(name, oldValue, out changed);
            else if (type.IsEnum) return DrawEnum(name, type, oldValue, out changed);

            else if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return DrawUnityObject(name, type, oldValue, out changed);
            else if (type == typeof(UnityEngine.Color)) return DrawColor(name, oldValue, out changed);
            else if (type == typeof(UnityEngine.AnimationCurve)) return DrawAnimationCurve(name, oldValue, out changed);
            else if (type == typeof(UnityEngine.Vector2)) return DrawVector2(name, oldValue, out changed);
            else if (type == typeof(UnityEngine.Vector3)) return DrawVector3(name, oldValue, out changed);
            else if (type == typeof(UnityEngine.Vector4)) return DrawVector4(name, oldValue, out changed);

            else if (type.IsArray) return DrawArray(name, type, oldValue, paramCacheKey, out changed);
            else if (typeof(IList).IsAssignableFrom(type)) return DrawList(name, type, oldValue, paramCacheKey, out changed);
            else if (typeof(IDictionary).IsAssignableFrom(type)) return DrawDictionary(name, type, oldValue, paramCacheKey, out changed);

            else return DrawAsClassOrStruct(name, type, oldValue, paramCacheKey, out changed);
        }

        public static bool IsKeyTypeChangeable(Type type)
        {
            return type.IsClass;
        }

        public static bool DrawBool(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = false;
            var content = new GUIContent(name);
            bool newValue = EditorGUILayout.Toggle(content, (bool)oldValue);
            changed = newValue != (bool)oldValue;
            return newValue;
        }

        public static int DrawInt(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = 0;
            var content = new GUIContent(name);
            int ret = EditorGUILayout.IntField(content, (int)oldValue);

            changed = ret != (int)oldValue;

            return ret;
        }

        public static uint DrawUInt(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = (uint)0;
            var content = new GUIContent(name);
            string uintString = EditorGUILayout.TextField(content, oldValue.ToString());
            uintString = System.Text.RegularExpressions.Regex.Replace(uintString, @"[^0-9]+", "");
            if (uint.TryParse(uintString, out uint u))
            {
                changed = (uint)oldValue != u;
                oldValue = u;
            }
            else
            {
                changed = (uint)oldValue != 0;
                oldValue = (uint)0;
            }



            return (uint)oldValue;
        }

        public static long DrawLong(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = 0;
            var content = new GUIContent(name);
            long newValue = EditorGUILayout.LongField(content, (long)oldValue);
            changed = newValue != (long)oldValue;
            return newValue;
        }

        public static ulong DrawULong(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = (ulong)0;
            var content = new GUIContent(name);
            var uLongString = EditorGUILayout.TextField(content, oldValue.ToString());
            uLongString = System.Text.RegularExpressions.Regex.Replace(uLongString, @"[^0-9]+", "");
            if (ulong.TryParse(uLongString, out ulong u))
            {
                changed = (ulong)oldValue != u;
                oldValue = u;
            }
            else
            {
                changed = (ulong)oldValue != 0;
                oldValue = (ulong)0;
            }
            return (ulong)oldValue;
        }

        public static string DrawString(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = string.Empty;
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.TextField(content, (string)oldValue);
            changed = newValue != (string)oldValue;
            return newValue;
        }

        public static float DrawFloat(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = 0f;
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.FloatField(content, (float)oldValue);
            changed = newValue != (float)oldValue;
            return newValue;
        }

        public static double DrawDouble(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = 0;
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.DoubleField(content, (double)oldValue);
            changed = newValue != (double)oldValue;
            return newValue;
        }

        public static Enum DrawEnum(string name, Type type, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = Enum.GetValues(type).GetValue(0);
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.EnumPopup(content, (Enum)oldValue);
            changed = !newValue.Equals(oldValue);
            return newValue;
        }

        public static UnityEngine.Object DrawUnityObject(string name, Type type, object oldValue, out bool changed)
        {
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.ObjectField(content, (UnityEngine.Object)oldValue, type, true);
            changed = newValue != (UnityEngine.Object)oldValue;
            return newValue;
        }

        public static UnityEngine.Color DrawColor(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = Color.white;
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.ColorField(content, (Color)oldValue);
            changed = !newValue.Equals(oldValue);
            return newValue;
        }

        public static UnityEngine.AnimationCurve DrawAnimationCurve(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = new AnimationCurve();
            var content = new GUIContent(name);
            var temp = new AnimationCurve(((AnimationCurve)oldValue).keys);
            var newValue = EditorGUILayout.CurveField(content, temp);
            changed = !newValue.Equals(oldValue);
            return newValue;
        }

        public static UnityEngine.Vector2 DrawVector2(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = Vector2.zero;
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.Vector2Field(content, (Vector2)oldValue);
            changed = !newValue.Equals(oldValue);
            return newValue;
        }

        public static UnityEngine.Vector3 DrawVector3(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = Vector3.zero;
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.Vector3Field(content, (Vector3)oldValue);
            changed = !newValue.Equals(oldValue);
            return newValue;
        }

        public static UnityEngine.Vector4 DrawVector4(string name, object oldValue, out bool changed)
        {
            if (oldValue == null) oldValue = Vector4.zero;
            var content = new GUIContent(name);
            var newValue = EditorGUILayout.Vector4Field(content, (Vector4)oldValue);
            changed = !newValue.Equals(oldValue);
            return newValue;
        }

        public static Array DrawArray(string name, Type type, object oldValue, string paramCacheKey, out bool changed)
        {
            var nullCheckerValue = DrawAsNull(name, type, oldValue, out changed);
            if (nullCheckerValue == null)
            {
                return null;
            }
            oldValue = nullCheckerValue;

            paramCacheKey += name;
            var elementType = type.GetElementType();

            Array array = (Array)oldValue;

            var isFoldOutKey = paramCacheKey + "IsFoldOut";
            var isFoldOut = InspectorDrawerCache.Instance.Get<bool>(isFoldOutKey);
            EditorGUILayout.BeginHorizontal();
            if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal("HelpBox");

            GUILayout.Space(13);
            isFoldOut = EditorGUILayout.Foldout(isFoldOut, name, true, new GUIStyle("Foldout"));

            if (GUILayout.Button("Set Null", GUILayout.Width(100)))
            {
                oldValue = null;
                changed = true;
                return null;
            }

            if (isFoldOut)
            {
                var pageIndexKey = paramCacheKey + "pageIndex";
                var pageIndex = InspectorDrawerCache.Instance.Get<int>(pageIndexKey);
                var length = array.Length;
                var maxPageIndex = (length - 1) / 10;

                if (maxPageIndex >= 1)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("", new GUIStyle("ArrowNavigationLeft")))
                    {
                        pageIndex--;
                    }
                    pageIndex = EditorGUILayout.IntField(pageIndex, GUILayout.Width(30));
                    GUILayout.Label($"/{maxPageIndex}");
                    if (GUILayout.Button("", new GUIStyle("ArrowNavigationRight")))
                    {
                        pageIndex++;
                    }
                }
                pageIndex = Mathf.Clamp(pageIndex, 0, maxPageIndex);
                InspectorDrawerCache.Instance.Set(pageIndexKey, pageIndex);
            }

            int newLength = EditorGUILayout.DelayedIntField(array.Length, GUILayout.Width(50));
            if (newLength != array.Length)
            {
                if (newLength > array.Length)
                {
                    Array newArray = Array.CreateInstance(elementType, newLength);
                    for (int i = 0; i < newLength; i++)
                    {
                        var newValue = i < array.Length ? array.GetValue(i) : default;
                        newArray.SetValue(newValue, i);
                    }
                    array = newArray;
                    oldValue = array;
                    changed = true;
                }
                else
                {
                    Array newArray = Array.CreateInstance(elementType, newLength);
                    Array.Copy(array, newArray, newLength);
                    array = newArray;
                    oldValue = array;
                    changed = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            InspectorDrawerCache.Instance.Set(isFoldOutKey, isFoldOut);

            if (isFoldOut)
            {
                EditorGUILayout.BeginHorizontal();
                if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                EditorGUILayout.BeginVertical("RL Background");
                InspectorDrawer.DrawerLevel++;

                var pageIndexKey = paramCacheKey + "pageIndex";
                int pageIndex = InspectorDrawerCache.Instance.Get<int>(pageIndexKey);

                for (int index = pageIndex * 10; index < pageIndex * 10 + 10; index++)
                {
                    if (index >= array.Length)
                    {
                        break;
                    }
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    object newValue = null;
                    newValue = DrawField($"Element {index}", elementType, array.GetValue(index), paramCacheKey + index, out bool c1);
                    if (c1)
                    {
                        array.SetValue(newValue, index);
                        changed = true;
                    }
                    EditorGUILayout.EndVertical();

                    var oldC = GUI.color;
                    GUI.color = new Color(200 / 255f, 70 / 255f, 70 / 255f);
                    var needDelete = GUILayout.Button("X", GUILayout.Width(20));
                    GUI.color = oldC;
                    EditorGUILayout.EndHorizontal();

                    if (needDelete)
                    {
                        Array newArray = Array.CreateInstance(elementType, array.Length - 1);
                        for (int i = 0; i < array.Length - 1; i++)
                        {
                            var newValueCopy = i < index ? array.GetValue(i) : array.GetValue(i + 1);
                            newArray.SetValue(newValueCopy, i);
                        }
                        array = newArray;
                        oldValue = array;
                        changed = true;
                        break;
                    }
                }
                InspectorDrawer.DrawerLevel--;

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

            }
            return (Array)oldValue;
        }

        public static object DrawList(string name, Type type, object oldValue, string paramCacheKey, out bool changed)
        {
            var nullCheckerValue = DrawAsNull(name, type, oldValue, out changed);
            if (nullCheckerValue == null)
            {
                return nullCheckerValue;
            }
            oldValue = nullCheckerValue;

            IList ilist = (IList)oldValue;

            paramCacheKey += name;
            var genericType = type.GetGenericArguments()[0];

            var isFoldOutKey = paramCacheKey + "IsFoldOut";
            var isFoldOut = InspectorDrawerCache.Instance.Get<bool>(isFoldOutKey);
            EditorGUILayout.BeginHorizontal();
            if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal("HelpBox");

            GUILayout.Space(13);
            isFoldOut = EditorGUILayout.Foldout(isFoldOut, name, true, new GUIStyle("Foldout"));

            if (GUILayout.Button("Set Null", GUILayout.Width(100)))
            {
                oldValue = null;
                changed = true;
                return oldValue;
            }

            if (isFoldOut)
            {
                var pageIndexKey = paramCacheKey + "pageIndex";
                var pageIndex = InspectorDrawerCache.Instance.Get<int>(pageIndexKey);
                var length = ilist.Count;
                var maxPageIndex = (length - 1) / 10;

                if (maxPageIndex >= 1)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("", new GUIStyle("ArrowNavigationLeft")))
                    {
                        pageIndex--;
                    }
                    pageIndex = EditorGUILayout.IntField(pageIndex, GUILayout.Width(30));
                    GUILayout.Label($"/{maxPageIndex}");
                    if (GUILayout.Button("", new GUIStyle("ArrowNavigationRight")))
                    {
                        pageIndex++;
                    }
                }
                pageIndex = Mathf.Clamp(pageIndex, 0, maxPageIndex);
                InspectorDrawerCache.Instance.Set(pageIndexKey, pageIndex);
            }

            int newLength = EditorGUILayout.DelayedIntField(ilist.Count, GUILayout.Width(50));
            if (newLength != ilist.Count)
            {
                if (newLength > ilist.Count)
                {
                    var moreCount = newLength - ilist.Count;
                    for (int count = 0; count < moreCount; count++)
                    {
                        ilist.Add(Activator.CreateInstance(genericType));
                    }
                }
                else
                {
                    var lessCount = ilist.Count - newLength;
                    for (int count = 0; count < lessCount; count++)
                    {
                        ilist.RemoveAt(ilist.Count - 1);
                    }
                }
                changed = true;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            InspectorDrawerCache.Instance.Set(isFoldOutKey, isFoldOut);

            if (isFoldOut)
            {
                EditorGUILayout.BeginHorizontal();
                if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                EditorGUILayout.BeginVertical("RL Background");
                InspectorDrawer.DrawerLevel++;

                var length = ilist.Count;

                var pageIndexKey = paramCacheKey + "pageIndex";
                int pageIndex = InspectorDrawerCache.Instance.Get<int>(pageIndexKey);

                for (int index = pageIndex * 10; index < pageIndex * 10 + 10; index++)
                {
                    if (index >= length)
                    {
                        break;
                    }
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    var value = ilist[index];
                    value = DrawField($"Element {index}", genericType, value, paramCacheKey + index, out bool c1);
                    if (c1)
                    {
                        ilist[index] = value;
                        changed = true;
                    }
                    EditorGUILayout.EndVertical();

                    var oldC = GUI.color;
                    GUI.color = new Color(200 / 255f, 70 / 255f, 70 / 255f);
                    var needDelete = GUILayout.Button("X", GUILayout.Width(20));
                    GUI.color = oldC;
                    EditorGUILayout.EndHorizontal();

                    if (needDelete)
                    {
                        ilist.RemoveAt(index);
                        changed = true;
                        break;
                    }
                }

                InspectorDrawer.DrawerLevel--;
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            return oldValue;
        }

        public static object DrawDictionary(string name, Type type, object oldValue, string paramCacheKey, out bool changed)
        {
            var nullCheckerValue = DrawAsNull(name, type, oldValue, out changed);
            if (nullCheckerValue == null)
            {
                return nullCheckerValue;
            }
            oldValue = nullCheckerValue;

            IDictionary iDictionary = (IDictionary)oldValue;

            paramCacheKey += name;
            var genericType = type.GetGenericArguments();
            var keyType = genericType[0];
            var valueType = genericType[1];

            var isFoldOutKey = paramCacheKey + "IsFoldOut";
            var isFoldOut = InspectorDrawerCache.Instance.Get<bool>(isFoldOutKey);
            EditorGUILayout.BeginHorizontal();
            if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal("HelpBox");

            GUILayout.Space(13);
            isFoldOut = EditorGUILayout.Foldout(isFoldOut, name, true, new GUIStyle("Foldout"));

            if (GUILayout.Button("Set Null", GUILayout.Width(100)))
            {
                oldValue = null;
                changed = true;
                return oldValue;
            }

            int length = iDictionary.Count;

            int pageIndex = 0;
            var maxPageIndex = (length - 1) / 10;
            if (isFoldOut)
            {
                if (maxPageIndex >= 1)
                {
                    GUILayout.FlexibleSpace();
                    var pageIndexKey = paramCacheKey + "pageIndex";
                    pageIndex = InspectorDrawerCache.Instance.Get<int>(pageIndexKey);

                    // EditorGUILayout.BeginHorizontal();
                    // pageIndex = EditorGUILayout.DelayedIntField("Page index : ", pageIndex);
                    if (GUILayout.Button("", new GUIStyle("ArrowNavigationLeft")))
                    {
                        pageIndex--;
                    }
                    pageIndex = EditorGUILayout.IntField(pageIndex, GUILayout.Width(30));
                    GUILayout.Label($"/{maxPageIndex}");
                    if (GUILayout.Button("", new GUIStyle("ArrowNavigationRight")))
                    {
                        pageIndex++;
                    }
                    pageIndex = Mathf.Clamp(pageIndex, 0, maxPageIndex);
                    // EditorGUILayout.EndHorizontal();
                    InspectorDrawerCache.Instance.Set(pageIndexKey, pageIndex);
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            InspectorDrawerCache.Instance.Set(isFoldOutKey, isFoldOut);



            if (isFoldOut)
            {
                EditorGUILayout.BeginHorizontal();
                if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                EditorGUILayout.BeginVertical("RL Background");
                InspectorDrawer.DrawerLevel++;

                var needAddKey = paramCacheKey + "Need Add";
                var needAdd = InspectorDrawerCache.Instance.Get<bool>(needAddKey);
                EditorGUILayout.BeginHorizontal();
                if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                EditorGUILayout.BeginHorizontal("HelpBox");
                GUILayout.Space(13);
                needAdd = EditorGUILayout.Foldout(needAdd, "Need Add", true, new GUIStyle("Foldout"));

                InspectorDrawerCache.Instance.Set(needAddKey, needAdd);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();

                if (needAdd)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                    EditorGUILayout.BeginVertical("RL Background");

                    var addKeyCache = paramCacheKey + "AddKey";
                    var addKey = InspectorDrawerCache.Instance.Get(addKeyCache);
                    var addKeyName = "Key";

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    addKey = InspectorDrawerUtility.DrawField(addKeyName, keyType, addKey, addKeyCache, out bool c1);
                    if (c1)
                    {
                        InspectorDrawerCache.Instance.Set(addKeyCache, addKey);
                    }
                    EditorGUILayout.EndVertical();

                    var oColor = GUI.color;
                    GUI.color = Color.green;
                    var addBtn = GUILayout.Button("Add", GUILayout.Width(100));
                    GUI.color = oColor;
                    EditorGUILayout.EndHorizontal();

                    if (addBtn)
                    {
                        if (addKey == null)
                        {
                            Debug.LogError($"不可以添加空的Key");
                        }
                        else
                        {
                            var hasKey = iDictionary.Contains(addKey);
                            if ((bool)hasKey)
                            {
                                Debug.LogError($"Key 已存在");
                            }
                            else
                            {
                                object addValue = valueType.IsClass ? null : Activator.CreateInstance(valueType);
                                iDictionary.Add(addKey, addValue);
                                InspectorDrawerCache.Instance.Set(addKeyCache, null);
                                changed = true;
                                return oldValue;
                            }
                        }
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                DrawHorizontalLine(Color.gray);

                // int length = iDictionary.Count;

                // int pageIndex = 0;
                // var maxPageIndex = (length - 1) / 10;
                // if (maxPageIndex >= 1)
                // {
                //     var pageIndexKey = paramCacheKey + "pageIndex";
                //     pageIndex = InspectorDrawerCache.Instance.Get<int>(pageIndexKey);

                //     EditorGUILayout.BeginHorizontal();
                //     pageIndex = EditorGUILayout.DelayedIntField("Page index : ", pageIndex);
                //     if (GUILayout.Button("", new GUIStyle("ArrowNavigationLeft")))
                //     {
                //         pageIndex--;
                //     }
                //     if (GUILayout.Button("", new GUIStyle("ArrowNavigationRight")))
                //     {
                //         pageIndex++;
                //     }
                //     pageIndex = Mathf.Clamp(pageIndex, 0, maxPageIndex);
                //     EditorGUILayout.EndHorizontal();
                //     InspectorDrawerCache.Instance.Set(pageIndexKey, pageIndex);
                // }

                var keys = iDictionary.Keys;

                var showingKeyList = new List<object>();
                int index = 0;
                foreach (var key in keys)
                {
                    if (pageIndex * 10 <= index && index < pageIndex * 10 + 10)
                    {
                        showingKeyList.Add(key);
                    }
                    index++;
                }

                object keyToDelete = null;
                index = pageIndex * 10;
                foreach (var key in showingKeyList)
                {
                    EditorGUILayout.Space(7);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();

                    GUI.enabled = IsKeyTypeChangeable(keyType);
                    DrawField("Key", keyType, key, paramCacheKey + index + "Key", out bool c2);
                    if (c2)
                    {
                        changed = true;
                    }
                    GUI.enabled = true;
                    // EditorGUILayout.EndVertical();

                    // DrawVerticalLine(Color.gray);

                    // EditorGUILayout.BeginVertical();
                    var value = iDictionary[key];
                    value = DrawField("Value", valueType, value, paramCacheKey + index + "Value", out bool c3);
                    if (c3)
                    {
                        iDictionary[key] = value;
                        changed = true;
                    }
                    EditorGUILayout.EndVertical();

                    var originColor = GUI.color;
                    GUI.color = Color.red;
                    var delete = GUILayout.Button("Delete");
                    GUI.color = originColor;

                    EditorGUILayout.EndHorizontal();
                    if (delete)
                    {
                        keyToDelete = key;
                        break;
                    }
                    index++;

                }

                if (keyToDelete != null)
                {
                    var removeMethods = type.GetMethods();
                    MethodInfo removeMethod = null;
                    foreach (var m in removeMethods)
                    {
                        if (m.Name == "Remove" && m.IsVirtual)
                        {
                            removeMethod = m;
                        }
                    }
                    iDictionary.Remove(keyToDelete);
                    changed = true;
                }

                InspectorDrawer.DrawerLevel--;
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            return oldValue;
        }

        public static object DrawAsClassOrStruct(string name, Type type, object oldValue, string paramCacheKey, out bool changed)
        {
            if (type.IsClass)
            {
                var nullCheckerValue = DrawAsNull(name, type, oldValue, out changed);
                if (nullCheckerValue == null)
                {
                    return nullCheckerValue;
                }
                oldValue = nullCheckerValue;
            }
            else
            {
                changed = false;
            }

            paramCacheKey += name;

            var isFoldOutKey = paramCacheKey + "IsFoldOut";
            var isFoldOut = InspectorDrawerCache.Instance.Get<bool>(isFoldOutKey);
            EditorGUILayout.BeginHorizontal();
            if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal("HelpBox");

            GUILayout.Space(13);
            isFoldOut = EditorGUILayout.Foldout(isFoldOut, name, true, new GUIStyle("Foldout"));

            if (type.IsClass && GUILayout.Button("Set Null", GUILayout.Width(100)))
            {
                oldValue = null;
                changed = true;
                return oldValue;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            InspectorDrawerCache.Instance.Set(isFoldOutKey, isFoldOut);

            if (isFoldOut)
            {
                EditorGUILayout.BeginHorizontal();
                if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                EditorGUILayout.BeginVertical("RL Background");
                InspectorDrawer.DrawerLevel++;

                var showFieldList = InspectorReflectionUtil.GetFields(type);
                foreach (var showEntity in showFieldList)
                {
                    var inspectAttr = showEntity.GetCustomAttribute<Inspect>();
                    if (inspectAttr == null && showEntity.IsPrivate)
                    {
                        continue;
                    }
                    var oldField = showEntity.GetValue(oldValue);
                    var cacheKey = paramCacheKey + showEntity.Name;
                    var showiingName = inspectAttr == null || string.IsNullOrEmpty(inspectAttr.showingName) ? showEntity.Name : inspectAttr.showingName;
                    var newField = InspectorDrawerUtility.DrawField(showiingName, showEntity.FieldType, oldField, cacheKey, out bool c1);
                    if (c1)
                    {
                        changed = true;
                        showEntity.SetValue(oldValue, newField);
                    }
                }

                var showPropertyList = InspectorReflectionUtil.GetProperties(type);
                foreach (var showEntity in showPropertyList)
                {
                    var inspectAttr = showEntity.GetCustomAttribute<Inspect>();
                    if (inspectAttr == null && showEntity.GetMethod != null && showEntity.GetMethod.IsPrivate)
                    {
                        continue;
                    }
                    var getMethod = showEntity.GetMethod;
                    if (getMethod == null)
                    {
                        continue;
                    }
                    var oldProperty = getMethod.Invoke(oldValue, default);
                    var cacheKey = paramCacheKey + showEntity.Name;
                    var showiingName = inspectAttr == null || string.IsNullOrEmpty(inspectAttr.showingName) ? showEntity.Name : inspectAttr.showingName;
                    var newProperty = InspectorDrawerUtility.DrawField(showiingName, showEntity.PropertyType, oldProperty, cacheKey, out bool c2);

                    var setMethod = showEntity.SetMethod;
                    if (setMethod == null)
                    {
                        continue;
                    }
                    if (c2)
                    {
                        setMethod.Invoke(oldValue, new object[] { newProperty });
                    }
                }

                InspectorDrawer.DrawerLevel--;
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }


            return oldValue;
        }

        public static object DrawAsNull(string name, Type type, object value, out bool changed)
        {
            var ret = value;
            changed = false;

            if (value == null)
            {
                var index = EditorGUILayout.Popup(name, 0, new string[] { "Null", type.Name });
                if (index != 0)
                {
                    var constrcutorArray = type.GetConstructors();
                    foreach (var constrcutor in constrcutorArray)
                    {
                        if (constrcutor.GetParameters().Length == 0)
                        {
                            ret = Activator.CreateInstance(type);
                        }
                        if (constrcutor.GetParameters().Length == 1 && constrcutor.GetParameters()[0].ParameterType == typeof(int))
                        {
                            ret = Activator.CreateInstance(type, 0);
                        }
                    }

                    changed = true;
                }
            }
            return ret;
        }
    }
}