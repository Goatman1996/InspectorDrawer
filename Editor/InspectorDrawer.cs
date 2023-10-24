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
            var showFieldList = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var serializedObj = new SerializedObject(this.mono);
            foreach (var showEntity in showFieldList)
            {
                var needInspect = InspectorIfUtil.CheckInspectIf(showEntity, this.mono);
                if (!needInspect)
                {
                    continue;
                }
                var inspectAttr = showEntity.GetCustomAttribute<Inspect>();

                var oldValue = showEntity.GetValue(this.mono);
                var monoInstanceId = this.mono.GetInstanceID().ToString();
                var cacheKey = monoInstanceId;
                var showiingName = string.IsNullOrEmpty(inspectAttr.showingName) ? showEntity.Name : inspectAttr.showingName;
                var newValue = InspectorDrawerUtility.DrawField(showiingName, showEntity.FieldType, oldValue, cacheKey, out bool changed);
                if (changed)
                {
                    showEntity.SetValue(this.mono, newValue);

                    if (serializedObj.FindProperty(showEntity.Name) != null)
                    {
                        EditorUtility.SetDirty(this.mono);
                    }
                }
            }

            var showPropertyList = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var showEntity in showPropertyList)
            {
                var needInspect = InspectorIfUtil.CheckInspectIf(showEntity, this.mono);
                if (!needInspect)
                {
                    continue;
                }
                var inspectAttr = showEntity.GetCustomAttribute<Inspect>();

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
                var newValue = InspectorDrawerUtility.DrawField(showiingName, showEntity.PropertyType, oldValue, cacheKey, out bool changed);

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
            var showFieldList = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var showEntity in showFieldList)
            {
                var needInspect = InspectorIfUtil.CheckInspectIf(showEntity, this.mono);
                if (!needInspect)
                {
                    continue;
                }
                return true;
            }

            var showPropertyList = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
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
