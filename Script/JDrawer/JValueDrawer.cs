using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Inheo.UParser.JDrawer
{
    internal class JValueDrawer : JDrawer
    {
        internal override void Draw(string label, JToken token)
        {
            Draw(label, (JValue)token);
        }

        private void Draw(string label, JValue token)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            DrawValue(token);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawValue(JValue token)
        {
            var value = token.ToString();
            switch (token.Type)
            {
                case JTokenType.Boolean:
                    token.Value = EditorGUILayout.Toggle(bool.Parse(value));
                    break;
                case JTokenType.Integer:
                    token.Value = EditorGUILayout.IntField(int.Parse(value));
                    break;
                case JTokenType.Float:
                    token.Value = EditorGUILayout.FloatField(float.Parse(value));
                    break;
                default:
                    token.Value = EditorGUILayout.TextArea(value);
                    break;
            }
        }
    }
}