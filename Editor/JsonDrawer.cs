using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Inheo.UParser
{
    internal class JsonDrawer : BaseFileDrawer
    {
        private JObject currentJson;

        protected override bool IsNeedUpdateConditions => currentJson == null;
        protected override string FileKey => "JsonFilePath";
        protected override string FileExtensions => ".json";

        protected override void DrawBody()
        {
            DrawCurrentJson();
        }

        private void DrawCurrentJson()
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

        protected override void UpdateCurrentData()
        {
            SaveJsonFilePath();
            currentJson = ParseJson(FileText);
        }

        private JObject ParseJson(string json) => JObject.Parse(json);
    }
}