using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMToolKit.Inspector
{
    public class InspectorSampleBase : MonoBehaviour
    {
        [Inspect("这是一个基类中的判断,是否显示Base")] private bool ShowBase;
        [Inspect("这是一个基类中的判断,是否显示Base")] private bool ShowBaseProperty { get; set; }

        [Inspect("基类中的Int"), InspectIf("ShowBaseProperty")]
        private int BaseInt;

        [Inspect("常量Int")]
        private const int ConstInt = 0;
        [Inspect("只读Int")]
        private readonly int readonlyInt = 0;
    }
}