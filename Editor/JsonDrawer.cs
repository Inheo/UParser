using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser
{
    internal class JsonDrawer : BaseDrawer
    {
        private JObject _currentJson;
        private TextAsset _textFile;

        internal override void Draw()
        {
            DrawForFileEditor();
        }

        private void DrawForFileEditor()
        {
            EditorGUI.BeginChangeCheck();
            _textFile = (TextAsset)EditorGUILayout.ObjectField("Json File:", _textFile, typeof(TextAsset), false);

            if (_textFile == null)
                return;

            if (EditorGUI.EndChangeCheck())
                UpdateCurrentJson();

            if (_currentJson == null && _textFile != null)
                UpdateCurrentJson();

            EditorGUILayout.Space();

            DrawCurrentJson();

            EditorGUILayout.BeginHorizontal();
            TrySaveCurrentJsonIntoTextFile();
            TryUpdateCurrentJson();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCurrentJson()
        {
            foreach (var tokenPair in _currentJson)
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

        private void TrySaveCurrentJsonIntoTextFile()
        {
            if (GUILayout.Button("Save"))
            {
                File.WriteAllText(AssetDatabase.GetAssetPath(_textFile), _currentJson.ToString());
            }
        }

        private void TryUpdateCurrentJson()
        {
            if (GUILayout.Button("Update"))
            {
                UpdateCurrentJson();
            }
        }

        private JObject ParseJson(string json) => JObject.Parse(json);
        private void UpdateCurrentJson() => _currentJson = ParseJson(_textFile.text);
    }
}