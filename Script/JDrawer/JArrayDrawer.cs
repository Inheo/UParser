using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Inheo.UParser.JDrawer
{
    internal class JArrayDrawer : JDrawer
    {
        [Serializable]
        private class ReorderableJArray
        {
            public bool IsExpand = true;
            public Rect HeaderRect;
            public Rect TextRect;
            public readonly JArray JArray;
            public readonly ReorderableList ReorderableList;

            public ReorderableJArray(JArray jArray, ReorderableList reorderableList)
            {
                JArray = jArray;
                ReorderableList = reorderableList;
                ReorderableList.displayAdd = IsExpand;
                ReorderableList.displayRemove = IsExpand;
                ReorderableList.draggable = IsExpand;
                ReorderableList.list = IsExpand ? JArray : new List<JToken>(0);
            }

            public void UpdateExpandState(Event e)
            {
                if (e.type == EventType.MouseDown && HeaderRect.Contains(e.mousePosition))
                {
                    IsExpand = !IsExpand;
                    ReorderableList.displayAdd = IsExpand;
                    ReorderableList.displayRemove = IsExpand;
                    ReorderableList.draggable = IsExpand;
                    ReorderableList.list = IsExpand ? JArray : new List<JToken>(0);
                    e.Use();
                }
            }
        }

        [SerializeField] private readonly Dictionary<string, ReorderableJArray> arrayTokens;

        internal JArrayDrawer()
        {
            arrayTokens = new Dictionary<string, ReorderableJArray>();
        }

        internal override void Draw(string label, JToken token)
        {
            var padding = 15 * EditorGUI.indentLevel;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(padding);
            EditorGUILayout.BeginVertical();

            DrawArrayToken(label, (JArray)token);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawArrayToken(string key, JArray jArray)
        {
            var verticalSpacing = EditorGUIUtility.standardVerticalSpacing * 2;

            if (!arrayTokens.ContainsKey(key))
            {
                var reorderableList = new ReorderableList(jArray, typeof(JArray), true, true, true, true);
                var rJAray = new ReorderableJArray(jArray, reorderableList);
                arrayTokens[key] = rJAray;

                var reorderableJArray = arrayTokens[key];

                reorderableList.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, key + $" (Count: {rJAray.JArray.Count})");
                    rJAray.HeaderRect = rect;
                    rJAray.UpdateExpandState(Event.current);
                };
                reorderableList.drawElementCallback = (rect, i, isActive, isFocused) =>
                {
                    rJAray.TextRect = rect;
                    if (!rJAray.IsExpand) return;
                    rect.height = GetElementClearHeight(jArray[i].ToString(), rect);
                    rect.y += verticalSpacing;
                    jArray[i] = EditorGUI.TextArea(rect, jArray[i].ToString());
                };

                reorderableList.drawNoneElementCallback = rect =>
                {
                    reorderableList.elementHeight = -10;
                };

                reorderableList.onAddCallback = list => list.list.Add(default);
                reorderableList.elementHeightCallback = i =>
                    GetElementHeight(jArray[i].ToString(), rJAray.IsExpand, verticalSpacing, rJAray.TextRect);
            }

            arrayTokens[key].ReorderableList.DoLayoutList();
        }

        private float GetElementHeight(string text, bool isExpand, float offset, Rect rect)
        {
            if (!isExpand)
                return 0;

            float height = GetElementClearHeight(text, rect) + offset;
            return height;
        }

        private float GetElementClearHeight(string text, Rect rect)
        {
            float textHeight = EditorStyles.textArea.CalcHeight(new GUIContent(text), rect.width);
            return textHeight + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}