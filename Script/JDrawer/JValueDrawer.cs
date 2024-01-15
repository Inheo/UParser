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
            token.Value = EditorGUILayout.TextArea(token.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }
}