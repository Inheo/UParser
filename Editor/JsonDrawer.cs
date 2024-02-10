using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser
{
    internal class JsonDrawer : BaseDrawer
    {
        private const string JsonFilePath = "JsonFilePath";
        private JObject currentJson;
        private TextAsset file;
        private string currentfilePath;
        private string[] files;

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

        private void SaveJsonFilePath()
        {
            if (file != null)
            {
                currentfilePath = AssetDatabase.GetAssetPath(file);
                EditorPrefs.SetString(JsonFilePath, currentfilePath);
            }
        }

        private void LoadSavedJsonFilePath()
        {
            currentfilePath = EditorPrefs.GetString(JsonFilePath, "");
            if (!string.IsNullOrEmpty(currentfilePath))
            {
                file = AssetDatabase.LoadAssetAtPath<TextAsset>(currentfilePath);
            }
        }

        private void LoadJsonFiles()
        {
            string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets" });
            List<string> jsonFilesList = new List<string>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.ToLower().EndsWith(".json"))
                {
                    jsonFilesList.Add(path);
                }
            }

            files = jsonFilesList.ToArray();
        }

        private void DrawForFileEditor()
        {
            EditorGUI.BeginChangeCheck();

            DrawJsonFileSection();

            if (file == null)
                return;

            TryUpdateJson();

            EditorGUILayout.Space();

            DrawCurrentJson();

            EditorGUILayout.BeginHorizontal();
            SaveCurrentJsonButton();
            UpdateCurrentJsonButton();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawJsonFileSection()
        {
            EditorGUILayout.BeginHorizontal();
            DrawFile();

            DrawFilePopup();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFile()
        {
            var tmp = (TextAsset)EditorGUILayout.ObjectField("Json File:", file, typeof(TextAsset), false);

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
                    file = tmp;
                }
            }
        }

        private void DrawFilePopup()
        {
            if (files != null && files.Length > 0)
            {
                int selectedIndex = EditorGUILayout.Popup(Array.IndexOf(files, AssetDatabase.GetAssetPath(file)), files);
                if (selectedIndex >= 0 && selectedIndex < files.Length)
                {
                    file = AssetDatabase.LoadAssetAtPath<TextAsset>(files[selectedIndex]);
                }
            }
        }

        private void TryUpdateJson()
        {
            if (EditorGUI.EndChangeCheck())
                UpdateCurrentJson();

            if (currentJson == null && file != null)
                UpdateCurrentJson();
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

        private void SaveCurrentJsonButton()
        {
            if (GUILayout.Button("Save"))
            {
                File.WriteAllText(AssetDatabase.GetAssetPath(file), currentJson.ToString());
            }
        }

        private void UpdateCurrentJsonButton()
        {
            if (GUILayout.Button("Update"))
            {
                UpdateCurrentJson();
            }
        }

        private void UpdateCurrentJson()
        {
            SaveJsonFilePath();
            currentJson = ParseJson(file.text);
        }

        private void OnProjectChanged() => LoadJsonFiles();
        private JObject ParseJson(string json) => JObject.Parse(json);
    }
}