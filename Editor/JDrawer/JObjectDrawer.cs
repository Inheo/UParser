using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser.JDrawer
{
    internal class JObjectDrawer : JBaseDrawer
    {
        private Dictionary<int, bool> isExpandedMap;

        public JObjectDrawer()
        {
            isExpandedMap = new Dictionary<int, bool>();
        }

        internal override void Draw(string label, JToken token)
        {
            if (!isExpandedMap.ContainsKey(token.GetHashCode()))
                isExpandedMap.Add(token.GetHashCode(), true);

            var indentOffset = (15 * EditorGUI.indentLevel);
            var color = new Color(0.2509804f, 0.2509804f, 0.2509804f, 1f);
            GUILayout.Space(5f);
            var rect = EditorGUILayout.BeginVertical();
            rect.width -= indentOffset;
            rect.x += indentOffset;
            EditorGUI.DrawRect(rect, color);

            DrawLabel(label, token);

            CheckLabelIsClicked(token);

            if (isExpandedMap[token.GetHashCode()])
                DrawBody(token);

            EditorGUILayout.EndVertical();
        }

        private void DrawLabel(string label, JToken token)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.boldLabel);
            GUILayout.Space(15 * EditorGUI.indentLevel);
            GUILayout.Label(label, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label(isExpandedMap[token.GetHashCode()] ? "Collapse" : "Expand");

            GUILayout.Space(10f);
            EditorGUILayout.EndHorizontal();
        }

        private void CheckLabelIsClicked(JToken token)
        {
            var labelRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
            {
                isExpandedMap[token.GetHashCode()] = !isExpandedMap[token.GetHashCode()];
                Event.current.Use();
            }
        }

        internal override void DrawBody(JToken token)
        {
            var jObject = (JObject)token;
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;

            foreach (var item in jObject)
            {
                JDrawerDefineder.Find(item.Value.Type).Draw(item.Key, item.Value);
            }

            EditorGUI.indentLevel = indent;
        }
    }
}