using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace GMToolKit.Inspector
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class InspectorDrawer : UnityEditor.Editor
    {
        public static int DrawerLevel = 0;

        private MonoBehaviour mono { get => target as MonoBehaviour; }

        public override void OnInspectorGUI()
        {
            this.DrawSerializedProperty();

            InspectorDrawer.DrawerLevel = 0;

            this.DrawShowInInspector();
            this.DrawButton();
        }

        private void DrawSerializedProperty()
        {
            SerializedObject m_SerializedObject = new SerializedObject(mono);
            m_SerializedObject.Update();
            SerializedProperty m_SerializedProperty = m_SerializedObject.GetIterator();

            m_SerializedProperty.NextVisible(true);
            GUI.enabled = false;
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.PropertyField(rect, m_SerializedProperty);
            GUI.enabled = true;

            while (m_SerializedProperty.NextVisible(false))
            {
                EditorGUILayout.PropertyField(m_SerializedProperty);
            }

            m_SerializedObject.ApplyModifiedProperties();
        }

        private void DrawShowInInspector()
        {
            var type = this.mono.GetType();
            var showFieldList = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var showEntity in showFieldList)
            {
                var inspectAttr = showEntity.GetCustomAttribute<Inspect>();
                if (inspectAttr == null)
                {
                    continue;
                }
                var oldValue = showEntity.GetValue(this.mono);
                var monoInstanceId = this.mono.GetInstanceID().ToString();
                var cacheKey = monoInstanceId;
                var showiingName = string.IsNullOrEmpty(inspectAttr.showingName) ? showEntity.Name : inspectAttr.showingName;
                var newValue = InspectorDrawerUtility.DrawField(showiingName, showEntity.FieldType, oldValue, cacheKey, out bool changed);
                if (changed)
                {
                    showEntity.SetValue(this.mono, newValue);

                    if (Application.isEditor)
                    {
                        EditorUtility.SetDirty(this.mono);
                    }
                }
            }

            var showPropertyList = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var showEntity in showPropertyList)
            {
                var inspectAttr = showEntity.GetCustomAttribute<Inspect>();
                if (inspectAttr == null)
                {
                    continue;
                }
                var getMethod = showEntity.GetMethod;
                if (getMethod == null)
                {
                    continue;
                }
                var oldValue = getMethod.Invoke(this.mono, default);
                var monoInstanceId = this.mono.GetInstanceID().ToString();
                var cacheKey = monoInstanceId + showEntity.GetHashCode().ToString() + showEntity.Name;
                var showiingName = string.IsNullOrEmpty(inspectAttr.showingName) ? showEntity.Name : inspectAttr.showingName;
                var newValue = InspectorDrawerUtility.DrawField(showiingName, showEntity.PropertyType, oldValue, cacheKey, out bool changed);

                var setMethod = showEntity.SetMethod;
                if (setMethod == null)
                {
                    continue;
                }
                if (changed)
                {
                    setMethod.Invoke(this.mono, new object[] { newValue });
                }
            }
        }

        private void DrawButton()
        {
            var type = this.mono.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
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
                    var paramArray = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var param = parameters[i];
                        var paramKey = cachePreKey + param.Name;
                        var paramValue = InspectorDrawerCache.Instance.Get(paramKey);
                        paramArray[i] = paramValue;
                    }
                    method.Invoke(this.mono, paramArray);
                }
            }
        }
    }
}