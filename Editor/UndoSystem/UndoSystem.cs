using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace GMToolKit.Inspector.UndoSystem
{
    public class UndoSystem
    {
        private static bool Initialized { get; set; } = false;
        private static Dictionary<int, IUndoCommand> commmandDic;

        private static void Initialize()
        {
            commmandDic = new Dictionary<int, IUndoCommand>();
            UnityEditor.Undo.undoRedoPerformed += OnUndoPerformed;
        }

        public static void Record(IUndoCommand command)
        {
            if (!Initialized)
            {
                Initialize();
                Initialized = true;
            }

            var commandHashCode = command.GetHashCode();
            commmandDic.Add(commandHashCode, command);

            UndoRegisterProxy.Proxy.i = command.GetHashCode();

            UnityEditor.Undo.RegisterCompleteObjectUndo(UndoRegisterProxy.Proxy, commandHashCode.ToString());

            UndoRegisterProxy.Proxy.i = -command.GetHashCode();
        }

        private static void OnUndoPerformed()
        {
            var proxyId = UndoRegisterProxy.Proxy.i;

            // undoing
            if (commmandDic.ContainsKey(proxyId))
            {
                var command = commmandDic[proxyId];
                command.Undo?.Invoke();
                return;
            }
            proxyId = -proxyId;
            // redoing
            if (commmandDic.ContainsKey(proxyId))
            {
                var command = commmandDic[proxyId];
                command.Do?.Invoke();
                return;
            }
        }

        private class UndoRegisterProxy : UnityEngine.ScriptableObject
        {
            private static UndoRegisterProxy _Proxy;
            public static UndoRegisterProxy Proxy
            {
                get
                {
                    if (_Proxy == null)
                    {
                        _Proxy = CreateInstance<UndoRegisterProxy>();
                    }
                    return _Proxy;
                }
            }

            public int i;
        }
    }
}