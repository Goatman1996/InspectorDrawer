using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GMToolKit.Inspector
{
    /// <summary>
    /// Serializable Dictionary
    /// </summary>
    [Serializable]
    public class Sectionary<KeyT, ValueT> : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private List<KeyT> keyList = new List<KeyT>();
        [SerializeField, HideInInspector] private List<ValueT> valueList = new List<ValueT>();

        [Inspect] private Dictionary<KeyT, ValueT> showingDic = new Dictionary<KeyT, ValueT>();

        public void OnBeforeSerialize()
        {
            keyList.Clear();
            valueList.Clear();
            keyList.AddRange(showingDic.Keys);
            valueList.AddRange(showingDic.Values);
        }

        public void OnAfterDeserialize()
        {
            showingDic.Clear();

            int elementCount = keyList.Count;
            for (int index = 0; index < elementCount; index++)
            {
                var key = keyList[index];
                var value = valueList[index];
                showingDic[key] = value;
            }
        }
    }
}

