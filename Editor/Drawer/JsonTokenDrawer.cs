using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Inheo.UParser
{
    internal class JsonTokenDrawer
    {
        private JObject currentJson;

        public bool IsCurrentNull => currentJson == null;
        public string Text => currentJson.ToString();

        internal void Draw()
        {
            foreach (var tokenPair in currentJson)
            {
                DrawToken(tokenPair.Key, tokenPair.Value);
            }
        }

        private void DrawToken(string key, JToken value)
        {
            var jDrawer = JDrawerDefineder.Find(value.Type);
            if (jDrawer == null)
                EditorGUILayout.LabelField("None");
            else
                jDrawer.Draw(key, value);
        }

        internal void Load(string json)
        {
            currentJson = JObject.Parse(json);
        }
    }
}