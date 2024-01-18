using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser.JDrawer
{
    internal class JObjectDrawer : JDrawer
    {
        private bool isExpanded = true;

        internal override void Draw(string label, JToken token)
        {
            Draw(label, (JObject)token);
        }

        private void Draw(string label, JObject token)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawLabel(label);

            CheckLabelIsClicked();

            if (isExpanded)
                DrawBody(token);

            EditorGUILayout.EndVertical();
        }

        private void DrawLabel(string label)
        {
            var style = EditorStyles.helpBox;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = EditorStyles.boldLabel.fontSize;

            EditorGUILayout.BeginHorizontal(style);
            GUILayout.Space(2f);

            GUILayout.Label(label);
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

        private void DrawBody(JObject token)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            foreach (var item in token)
            {
                var jDrawer = DrawerDefineder.Find(item.Value.Type);
                if (jDrawer == null)
                    EditorGUILayout.LabelField(item.Value.Type.ToString());
                else
                    jDrawer.Draw(item.Key, item.Value);
            }

            EditorGUI.indentLevel = indent;
        }

    }
}