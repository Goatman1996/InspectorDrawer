using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(IList), inherited = true)]
    internal class ListDrawer : Drawer
    {
        IListView view;
        public override VisualElement Initialize()
        {
            view = new IListView(this.Entry.memberInfo.Name);
            // view.itemsSource = (IList)this.Entry.Value;
            // view.makeItem += this.MakeItem;
            // view.bindItem += this.BindItem;

            // RefreshListView();

            return view;
        }

        //         private void RefreshListView()
        //         {
        //             Debug.Log("RefreshListView");
        // #if UNITY_2021_1_OR_NEWER
        //             view.Rebuild();
        // #else
        //             view.Refresh();
        // #endif
        //         }

        // private void BindItem(VisualElement element, int arg2)
        // {

        //     (element as Label).text = arg2.ToString();
        // }

        // private VisualElement MakeItem()
        // {
        //     Debug.Log("MakeItem");
        //     return new Label();
        // }

        public override void Tick()
        {
            // var sourceValue = (AnimationCurve)this.Entry.Value;
            // var textValue = this.view.value;
            // if (sourceValue != textValue)
            // {
            //     this.view.SetValueWithoutNotify(sourceValue);
            // }
        }

        private class IListView : VisualElement
        {
            public IListView(string name)
            {
                Add(new Label(name));
            }
        }
    }
}