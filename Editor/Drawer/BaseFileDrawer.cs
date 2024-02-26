using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser
{
    internal abstract class BaseFileDrawer
    {
        private TextAsset file;
        private string currentfilePath;
        private string[] files;

        protected abstract bool IsNeedUpdateConditions { get; }
        protected abstract string FileKey { get; }
        protected abstract string FileExtensions { get; }
        protected abstract string CurrentText { get; }
        protected string FileText => file.text;

        public BaseFileDrawer()
        {
            LoadSavedFilePath();
            LoadFiles();
            EditorApplication.projectChanged += OnProjectChanged;
        }

        private void OnProjectChanged() => LoadFiles();

        private void LoadFiles()
        {
            string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets" });
            List<string> filesList = new List<string>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.ToLower().EndsWith(FileExtensions))
                {
                    filesList.Add(path);
                }
            }

            files = filesList.ToArray();
        }

        protected void SaveFilePath()
        {
            if (file != null)
            {
                currentfilePath = AssetDatabase.GetAssetPath(file);
                EditorPrefs.SetString(FileKey, currentfilePath);
            }
        }

        protected void LoadSavedFilePath()
        {
            currentfilePath = EditorPrefs.GetString(FileKey, "");
            if (!string.IsNullOrEmpty(currentfilePath))
            {
                file = AssetDatabase.LoadAssetAtPath<TextAsset>(currentfilePath);
            }
        }

        internal void Draw()
        {
            EditorGUI.BeginChangeCheck();

            DrawFileSection();

            if (file == null)
                return;

            var isChanged = EditorGUI.EndChangeCheck();
            TryUpdate(isChanged);

            EditorGUILayout.Space();

            DrawBody();

            EditorGUILayout.BeginHorizontal();
            SaveCurrentButton();
            UpdateCurrentButton();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFileSection()
        {
            EditorGUILayout.BeginHorizontal();
            DrawFile();

            DrawFilePopup();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFile()
        {
            var tmp = (TextAsset)EditorGUILayout.ObjectField("File:", file, typeof(TextAsset), false);

            if (tmp != null)
            {
                string path = AssetDatabase.GetAssetPath(tmp);

                if (!string.IsNullOrEmpty(path) && !path.EndsWith(FileExtensions))
                {
                    tmp = null;
                    Debug.LogWarning($"Please select a {FileExtensions} file.");
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

        private void TryUpdate(bool isChanged)
        {
            if (isChanged)
                UpdateCurrentData();

            if (IsNeedUpdateConditions && file != null)
                UpdateCurrentData();
        }

        protected abstract void UpdateCurrentData();

        private void SaveCurrentButton()
        {
            if (GUILayout.Button("Save"))
            {
                File.WriteAllText(AssetDatabase.GetAssetPath(file), CurrentText);
            }
        }

        private void UpdateCurrentButton()
        {
            if (GUILayout.Button("Update"))
            {
                UpdateCurrentData();
            }
        }

        protected abstract void DrawBody();
    }
}