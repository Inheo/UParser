using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser
{
    internal class JsonDrawer : BaseDrawer
    {
        private const string JsonFilePath = "JsonFilePath";
        private JObject _currentJson;
        private TextAsset _jsonFile;
        private string jsonFilePath;
        private string[] jsonFiles;

        public JsonDrawer()
        {
            LoadSavedJsonFilePath();
            LoadJsonFiles();
            EditorApplication.projectChanged += OnProjectChanged;
        }

        internal override void Draw()
        {
            DrawForFileEditor();
        }

        private void DrawForFileEditor()
        {
            EditorGUI.BeginChangeCheck();

            DrawJsonFile();

            if (_jsonFile == null)
                return;

            if (EditorGUI.EndChangeCheck())
                UpdateCurrentJson();

            if (_currentJson == null && _jsonFile != null)
                UpdateCurrentJson();

            EditorGUILayout.Space();

            DrawCurrentJson();

            EditorGUILayout.BeginHorizontal();
            TrySaveCurrentJsonIntoTextFile();
            TryUpdateCurrentJson();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawJsonFile()
        {
            EditorGUILayout.BeginHorizontal();
            var tmp = (TextAsset)EditorGUILayout.ObjectField("Json File:", _jsonFile, typeof(TextAsset), false);

            if (tmp != null)
            {
                string path = AssetDatabase.GetAssetPath(tmp);

                if (!string.IsNullOrEmpty(path) && !path.EndsWith(".json"))
                {
                    tmp = null;
                    Debug.LogWarning("Please select a JSON file.");
                }
                else
                {
                    _jsonFile = tmp;
                }
            }

            if (jsonFiles != null && jsonFiles.Length > 0)
            {
                int selectedIndex = EditorGUILayout.Popup(Array.IndexOf(jsonFiles, AssetDatabase.GetAssetPath(_jsonFile)), jsonFiles);
                if (selectedIndex >= 0 && selectedIndex < jsonFiles.Length)
                {
                    _jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonFiles[selectedIndex]);
                }
            }
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
                File.WriteAllText(AssetDatabase.GetAssetPath(_jsonFile), _currentJson.ToString());
            }
        }

        private void TryUpdateCurrentJson()
        {
            if (GUILayout.Button("Update"))
            {
                UpdateCurrentJson();
            }
        }

        private void LoadJsonFiles()
        {
            string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets" });
            System.Collections.Generic.List<string> jsonFilesList = new System.Collections.Generic.List<string>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.ToLower().EndsWith(".json"))
                {
                    jsonFilesList.Add(path);
                }
            }

            jsonFiles = jsonFilesList.ToArray();
        }

        private void OnProjectChanged() => LoadJsonFiles();

        private void UpdateCurrentJson()
        {
            SaveJsonFilePath();
            _currentJson = ParseJson(_jsonFile.text);
        }

        private JObject ParseJson(string json) => JObject.Parse(json);

        private void SaveJsonFilePath()
        {
            if (_jsonFile != null)
            {
                jsonFilePath = AssetDatabase.GetAssetPath(_jsonFile);
                EditorPrefs.SetString(JsonFilePath, jsonFilePath);
            }
        }

        private void LoadSavedJsonFilePath()
        {
            jsonFilePath = EditorPrefs.GetString(JsonFilePath, "");
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                _jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonFilePath);
            }
        }
    }
}