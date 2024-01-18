using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser.JDrawer
{
    internal class JArrayDrawer : JBaseDrawer
    {
        private Dictionary<int, bool> isFoldout;

        public JArrayDrawer()
        {
            isFoldout = new();
        }

        internal override void Draw(string label, JToken token)
        {
            var tokenCode = token.GetHashCode();
            if (!isFoldout.ContainsKey(tokenCode))
                isFoldout[tokenCode] = true;

            var tmpFoldoutState = isFoldout[tokenCode];
            tmpFoldoutState = EditorGUILayout.Foldout(tmpFoldoutState, label, true, EditorStyles.foldoutHeader);

            isFoldout[tokenCode] = tmpFoldoutState;

            if (!tmpFoldoutState) return;
            DrawBody(token);
        }

        internal override void DrawBody(JToken token)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;

            var style = new GUIStyle(EditorStyles.helpBox);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = EditorStyles.boldLabel.fontSize;
            style.margin.left = (indent + 1) * 15;
            style.padding.left = -(indent + 1) * 15;

            foreach (var item in token)
            {
                EditorGUILayout.BeginVertical(style);
                DrawerDefineder.Find(item.Type).DrawBody(item);
                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel = indent;
        }
    }
}