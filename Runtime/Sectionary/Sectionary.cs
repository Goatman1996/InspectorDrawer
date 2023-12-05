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

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keyList.Clear();
            valueList.Clear();
            keyList.AddRange(showingDic.Keys);
            valueList.AddRange(showingDic.Values);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
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

        public Dictionary<KeyT, ValueT> Dictionary => showingDic;

        public ValueT this[KeyT key]
        {
            get => showingDic[key];
            set => showingDic[key] = value;
        }

        public bool ContainsKey(KeyT key)
        {
            return showingDic.ContainsKey(key);
        }

        public bool Remove(KeyT key)
        {
            return showingDic.Remove(key);
        }

        public bool TryGetValue(KeyT key, out ValueT value)
        {
            return showingDic.TryGetValue(key, out value);
        }

        public void Add(KeyT key, ValueT value)
        {
            showingDic.Add(key, value);
        }

        public void Clear()
        {
            showingDic.Clear();
        }
    }
}

