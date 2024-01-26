using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Inheo.UParser.JDrawer
{
    internal class JValueDrawer : JBaseDrawer
    {
        internal override void Draw(string label, JToken token)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            DrawBody(token);
            EditorGUILayout.EndHorizontal();
        }

        internal override void DrawBody(JToken token)
        {
            var jValueToke = (JValue)token;
            var value = token.ToString();
            switch (token.Type)
            {
                case JTokenType.Boolean:
                    jValueToke.Value = EditorGUILayout.Toggle(bool.Parse(value));
                    break;
                case JTokenType.Integer:
                    jValueToke.Value = EditorGUILayout.IntField(int.Parse(value));
                    break;
                case JTokenType.Float:
                    jValueToke.Value = EditorGUILayout.FloatField(float.Parse(value));
                    break;
                default:
                    jValueToke.Value = EditorGUILayout.TextArea(value);
                    break;
            }
        }
    }
}