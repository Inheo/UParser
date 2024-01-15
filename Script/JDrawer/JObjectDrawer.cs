using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Inheo.UParser.JDrawer
{
    internal class JObjectDrawer : JDrawer
    {
        private readonly JValueDrawer jValueDrawer;
        private readonly JArrayDrawer jArrayDrawer;

        public JObjectDrawer()
        {
            jValueDrawer = new JValueDrawer();
            jArrayDrawer = new JArrayDrawer();
        }

        internal override void Draw(string label, JToken token)
        {
            Draw(label, (JObject)token);
        }

        private void Draw(string label, JObject token)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var style = EditorStyles.helpBox;
            style.fontStyle = UnityEngine.FontStyle.Bold;
            style.fontSize = EditorStyles.boldLabel.fontSize;
            EditorGUILayout.LabelField(label, style);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;

            foreach (var item in token)
            {
                switch (item.Value.Type)
                {
                    case JTokenType.None:
                        break;
                    case JTokenType.String:
                    case JTokenType.Integer:
                    case JTokenType.Float:
                    case JTokenType.Boolean:
                    case JTokenType.Guid:
                        jValueDrawer.Draw(item.Key, item.Value);
                        break;
                    case JTokenType.Object:
                        Draw(item.Key, item.Value);
                        break;
                    case JTokenType.Array:
                        jArrayDrawer.Draw(item.Key, item.Value);
                        break;
                    default:
                        EditorGUILayout.LabelField(item.Value.Type.ToString());
                        break;
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUILayout.EndVertical();
        }
    }
}