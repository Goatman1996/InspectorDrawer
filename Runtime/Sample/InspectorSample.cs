using System.Collections.Generic;
using UnityEngine;
using System;

namespace GMToolKit.Inspector
{
    public class InspectorSample : MonoBehaviour
    {
        [Inspect("这是一个静态字段")]
        private static int staticInt = 4;
        [Inspect("这是一个静态属性")]
        private static int staticIntPro
        {
            get => staticInt;
            set => staticInt = value;
        }


        [SerializeField, HideInInspector, Inspect]
        private Sectionary<TestClass, int> i;

        [SerializeField, Inspect("这是一个可以序列化的字典")] private Sectionary<int, int> sectionary;

        [Inspect("这是一个不可序列化的类")] private TestClass originClass;

        [Inspect("这是一个Int")] private int intTest;
        [Inspect] private uint uintTest;
        [Inspect] private long longTest;
        [Inspect] private ulong uongTest;
        [Inspect] private string stringTest;
        [Inspect] private float floatTest;
        [Inspect] private double doubleTest;

        [Inspect] private TestEnum enumTest;
        [Inspect] private Texture unityObjectTest;
        [Inspect] private Color colorTest;
        [Inspect] private AnimationCurve curveTest;
        [Inspect] private Vector2 v2Test;
        [Inspect] private Vector3 v3Test;
        [Inspect] private Vector4 v4Test;

        [Inspect] private TestEnum[][] enumArrayTest;
        [Inspect] private List<List<TestClass>> doubleListTest;

        [Inspect] private Dictionary<TestClass, Texture> dictionaryTest;
        [Inspect] private Dictionary<int, TestEnum> dictionaryTest1;
        [Inspect] private Dictionary<UnityEngine.Vector2, TestClass> dictionaryTest2;

        [Button("这是一个方法")]
        private void B()
        {
            Debug.Log("B");
        }
        [Button]
        private void B(int param)
        {
            Debug.Log(param);
        }
        [Button]
        private void B(TestEnum param)
        {
            Debug.Log(param);
        }
        [Button]
        private void B(TestClass param)
        {
            Debug.Log(param);
        }
        [Button]
        private void B(List<TestClass> param)
        {
            Debug.Log(param.Count);
        }
        [Button]
        private void B(TestEnum[] param)
        {
            Debug.Log(param.Length);
        }
        [Button]
        private void B(Dictionary<TestClass, TestEnum> param)
        {
            Debug.Log(param.Count);
        }
        [Button]
        private void B(Dictionary<UnityEngine.Vector2, TestClass> param, List<TestClass> param1)
        {
            Debug.Log(param.Count + param1.Count);
        }
        [Button]
        private static void StatcMethod()
        {
            Debug.Log("StatcMethod");
        }
    }

    public enum TestEnum
    {
        A, B, C, D, F, G
    }

    [Serializable]
    public class TestClass
    {
        public int a;
        public int B;
    }
}