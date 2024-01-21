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


            foreach (var item in token)
            {
                var drawer = DrawerDefineder.Find(item.Type);
                var isNeedDrawRect = !(drawer is JValueDrawer);

                if (isNeedDrawRect)
                {
                    BeginVertical();
                }

                drawer.DrawBody(item);

                if (isNeedDrawRect)
                {
                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);
            }

            EditorGUI.indentLevel = indent;
        }

        private static void BeginVertical()
        {
            var indentOffset = (15 * EditorGUI.indentLevel);
            var style = new GUIStyle();
            style.padding = new RectOffset(0, 0, 5, 5);
            style.padding = new RectOffset(0, 0, 5, 5);
            var groupRect = EditorGUILayout.BeginVertical(style);
            groupRect.width -= indentOffset;
            groupRect.x += indentOffset;
            var color = new Color(0.2509804f, 0.2509804f, 0.2509804f, 1f);
            EditorGUI.DrawRect(groupRect, color);
        }
    }
}