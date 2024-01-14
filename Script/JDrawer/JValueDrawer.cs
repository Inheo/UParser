using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Inheo.UParser.JDrawer
{
    internal class JValueDrawer : JDrawer
    {
        internal override void Draw(string label, JToken token)
        {
            NewMethod(label, (JValue)token);
        }

        private static void NewMethod(string label, JValue token)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            token.Value = EditorGUILayout.TextArea(token.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }
}