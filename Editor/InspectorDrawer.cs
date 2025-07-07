using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System;

namespace GMToolKit.Inspector
{
    // [CustomEditor(typeof(MonoBehaviour), true)]
    // public class MonoBehaviourDrawer : InspectorDrawer { }

    // [CustomEditor(typeof(ScriptableObject), true)]
    // public class ScriptableObjectDrawer : InspectorDrawer { }

    // [CustomEditor(typeof(MonoBehaviour), true)]
    public class InspectorDrawerElement : UnityEditor.Editor
    {
        private DrawerManifest manifest;
        private VisualElement root;

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            this.AddScriptViewToRoot(root);

            var imgui = new IMGUIContainer();
            imgui.onGUIHandler += this.Tick;
            root.Add(imgui);

            this.manifest = DrawerManifest.CreateManifest(this.target);
            foreach (var entry in manifest.entryList)
            {
                entry.drawer.Entry = entry;
                root.Add(entry.drawer.Initialize());
            }

            return root;
        }

        private void AddScriptViewToRoot(VisualElement root)
        {
            var serializedObj = new SerializedObject(this.target);
            var scriptField = new ObjectField();
            scriptField.label = "Script";
            scriptField.objectType = typeof(MonoBehaviour);
            var property = serializedObj.FindProperty("m_Script");
            scriptField.value = property.objectReferenceValue;

            root.Add(scriptField);

            scriptField.AddToClassList("unity-disabled");
            var selector = scriptField.Q(null, "unity-object-field__selector");
            selector.SetEnabled(false);

        }

        private void Tick()
        {
            bool isDirty = false;
            foreach (var entry in this.manifest.entryList)
            {
                entry.drawer.Tick();
                if (entry.isDirty) isDirty = true;
                entry.isDirty = false;
            }
            if (isDirty) EditorUtility.SetDirty(this.target);
        }
    }

    public abstract class InspectorDrawer : UnityEditor.Editor
    {
        public static int DrawerLevel = 0;

        private UnityEngine.Object mono { get => target as UnityEngine.Object; }

        public override void OnInspectorGUI()
        {
            this.DrawSerializedProperty();

            InspectorDrawer.DrawerLevel = 0;

            if (HasAnyWillDraw())
            {
                InspectorDrawerUtility.DrawHorizontalLine(Color.gray);
                InspectorDrawerUtility.DrawLable("Draw By Inspecter Drawer", Color.cyan);
                InspectorDrawerUtility.DrawHorizontalLine(Color.gray);
            }

            this.DrawShowInInspector();
            this.DrawButton();
        }

        private void DrawSerializedProperty()
        {
            this.DrawDefaultInspector();
            return;
        }

        private void DrawShowInInspector()
        {
            var type = this.mono.GetType();
            var showFieldList = InspectorReflectionUtil.GetFields(type);
            var serializedObj = new SerializedObject(this.mono);
            foreach (var showEntity in showFieldList)
            {
                var needInspect = InspectorIfUtil.CheckInspectIf(showEntity, this.mono);
                if (!needInspect)
                {
                    continue;
                }
                var inspectAttr = showEntity.GetCustomAttribute<InspectAttribute>();

                var oldValue = showEntity.GetValue(this.mono);
                var monoInstanceId = this.mono.GetInstanceID().ToString();
                var cacheKey = monoInstanceId;
                var showiingName = string.IsNullOrEmpty(inspectAttr.showingName) ? showEntity.Name : inspectAttr.showingName;
                var isReadOnly = inspectAttr.isReadOnly;
                GUI.enabled = !isReadOnly;
                var newValue = InspectorDrawerUtility.DrawField(showiingName, showEntity.FieldType, oldValue, cacheKey, out bool changed);
                GUI.enabled = true;
                if (changed)
                {
                    if (showEntity.IsInitOnly) continue;
                    if (showEntity.IsLiteral) continue;

                    showEntity.SetValue(this.mono, newValue);

                    if (serializedObj.FindProperty(showEntity.Name) != null)
                    {
                        EditorUtility.SetDirty(this.mono);
                    }
                }
            }

            var showPropertyList = InspectorReflectionUtil.GetProperties(type);
            foreach (var showEntity in showPropertyList)
            {
                var needInspect = InspectorIfUtil.CheckInspectIf(showEntity, this.mono);
                if (!needInspect)
                {
                    continue;
                }
                var inspectAttr = showEntity.GetCustomAttribute<InspectAttribute>();

                var getMethod = showEntity.GetMethod;
                if (getMethod == null)
                {
                    continue;
                }
                object oldValue = null;
                if (getMethod.IsStatic)
                {
                    oldValue = getMethod.Invoke(null, default);
                }
                else
                {
                    oldValue = getMethod.Invoke(this.mono, default);
                }
                var monoInstanceId = this.mono.GetInstanceID().ToString();
                var cacheKey = monoInstanceId + showEntity.GetHashCode().ToString() + showEntity.Name;
                var showiingName = string.IsNullOrEmpty(inspectAttr.showingName) ? showEntity.Name : inspectAttr.showingName;
                var isReadOnly = inspectAttr.isReadOnly;
                GUI.enabled = !isReadOnly;
                var newValue = InspectorDrawerUtility.DrawField(showiingName, showEntity.PropertyType, oldValue, cacheKey, out bool changed);
                GUI.enabled = true;
                var setMethod = showEntity.SetMethod;
                if (setMethod == null)
                {
                    continue;
                }
                if (changed)
                {
                    if (setMethod.IsStatic)
                    {
                        setMethod.Invoke(null, new object[] { newValue });
                    }
                    else
                    {
                        setMethod.Invoke(this.mono, new object[] { newValue });
                    }
                }
            }
        }

        private void DrawButton()
        {
            var type = this.mono.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute == null)
                {
                    continue;
                }
                var monoInstanceId = this.mono.GetInstanceID().ToString();
                var methodHashCode = method.GetHashCode().ToString();
                var cachePreKey = monoInstanceId + methodHashCode;
                var functionName = string.IsNullOrEmpty(buttonAttribute.functionName) ? method.Name : buttonAttribute.functionName;
                bool isStatic = method.IsStatic;
                bool onClick = false;
                var parameters = method.GetParameters();
                if (parameters.Length != 0)
                {
                    var isFoldOutKey = cachePreKey + "IsFoldOut";
                    var isFoldOut = InspectorDrawerCache.Instance.Get<bool>(isFoldOutKey);
                    EditorGUILayout.BeginHorizontal();
                    if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                    EditorGUILayout.BeginHorizontal("HelpBox");

                    GUILayout.Space(13);
                    isFoldOut = EditorGUILayout.Foldout(isFoldOut, "[Button] " + functionName + " Params", true, new GUIStyle("Foldout"));
                    onClick = GUILayout.Button("Invoke", GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndHorizontal();
                    InspectorDrawerCache.Instance.Set(isFoldOutKey, isFoldOut);


                    if (isFoldOut)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (InspectorDrawer.DrawerLevel != 0) GUILayout.Space(10f);
                        EditorGUILayout.BeginVertical("RL Background");
                        InspectorDrawer.DrawerLevel++;

                        foreach (var param in parameters)
                        {
                            var paramCacheKey = cachePreKey + param.Name;
                            var paramValue = InspectorDrawerCache.Instance.Get(paramCacheKey);
                            var paramType = param.ParameterType;
                            var paramName = param.Name;
                            paramValue = InspectorDrawerUtility.DrawField(paramName, paramType, paramValue, paramCacheKey, out bool changed);
                            InspectorDrawerCache.Instance.Set(paramCacheKey, paramValue);
                        }

                        InspectorDrawer.DrawerLevel--;
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    onClick = GUILayout.Button("[Button] " + functionName);
                }

                if (onClick)
                {
                    object[] paramArray = new object[parameters.Length]; ;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var param = parameters[i];
                        var paramKey = cachePreKey + param.Name;
                        var paramValue = InspectorDrawerCache.Instance.Get(paramKey);
                        paramArray[i] = paramValue;
                    }
                    if (isStatic)
                    {
                        method.Invoke(null, paramArray);
                    }
                    else
                    {
                        method.Invoke(this.mono, paramArray);
                    }

                }
            }
        }

        private bool HasAnyWillDraw()
        {
            var type = this.mono.GetType();
            var showFieldList = InspectorReflectionUtil.GetFields(type);
            foreach (var showEntity in showFieldList)
            {
                var needInspect = InspectorIfUtil.CheckInspectIf(showEntity, this.mono);
                if (!needInspect)
                {
                    continue;
                }
                return true;
            }

            var showPropertyList = InspectorReflectionUtil.GetProperties(type);
            foreach (var showEntity in showPropertyList)
            {
                var needInspect = InspectorIfUtil.CheckInspectIf(showEntity, this.mono);
                if (!needInspect)
                {
                    continue;
                }
                var getMethod = showEntity.GetMethod;
                if (getMethod == null)
                {
                    continue;
                }
                return true;
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute == null)
                {
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}
