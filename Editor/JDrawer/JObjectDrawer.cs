using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser.JDrawer
{
    internal class JObjectDrawer : JBaseDrawer
    {
        private bool isExpanded = true;

        internal override void Draw(string label, JToken token)
        {
            var indentOffset = (15 * EditorGUI.indentLevel);
            var color = new Color(0.2509804f, 0.2509804f, 0.2509804f, 1f);
            GUILayout.Space(5f);
            var rect = EditorGUILayout.BeginVertical();
            rect.width -= indentOffset;
            rect.x += indentOffset;
            EditorGUI.DrawRect(rect, color);

            DrawLabel(label);

            CheckLabelIsClicked();

            if (isExpanded)
                DrawBody(token);

            EditorGUILayout.EndVertical();
        }

        private void DrawLabel(string label)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.boldLabel);
            GUILayout.Space(15 * EditorGUI.indentLevel);
            GUILayout.Label(label, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label(isExpanded ? "Collapse" : "Expand");

            GUILayout.Space(10f);
            EditorGUILayout.EndHorizontal();
        }

        private void CheckLabelIsClicked()
        {
            var labelRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
            {
                isExpanded = !isExpanded;
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