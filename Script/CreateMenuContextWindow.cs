using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;

namespace Inheo.UParser
{
    internal class CreateMenuContextWindow : EditorWindow
    {
        private int tabIndex = 0;
        private static EditorWindow _window;
        private TextAsset _textFile;

        private JObject _currentJson;
        private SerializedObject so;

        [MenuItem("Window/UParser")]
        private static void ShowWindow()
        {
            if (_window != null)
                return;

            _window = GetWindow(typeof(CreateMenuContextWindow));
            _window.titleContent = new GUIContent("UParser");
            _window.Show();
        }

        private void OnEnable()
        {
            ScriptableObject target = this;
            so = new SerializedObject(target);
            //Editor.CreateEditor();
        }

        private void OnGUI()
        {
            tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "File", "PlayerPrefs" });
            switch (tabIndex)
            {
                case 0:
                    DrawForFileEditor();
                    break;
                case 1:
                    DrawForPlayerPrefsEditor();
                    break;
            }
        }

        private void DrawForFileEditor()
        {
            EditorGUI.BeginChangeCheck();
            _textFile = (TextAsset)EditorGUILayout.ObjectField("Text Asset", _textFile, typeof(TextAsset), false);
            if (_textFile == null)
                return;

            if (EditorGUI.EndChangeCheck())
                UpdateCurrentJson();

            if (_currentJson == null && _textFile != null)
                UpdateCurrentJson();

            EditorGUILayout.Space();

            DrawCurrentJson();

            TrySaveCurrentJsonIntoTextFile();
            TryUpdateCurrentJson();
        }

        private void DrawCurrentJson()
        {
            foreach (var tokenPair in _currentJson)
            {
                DrawToken(_currentJson[tokenPair.Key], tokenPair.Key, tokenPair.Value);
                //_currentJson[tokenPair.Key] = EditorGUILayout.TextField(tokenPair.Key, value);
            }
        }

        private void DrawToken(JToken token, string key, JToken value)
        {
            switch (token.Type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.Array:
                    DrawArrayToken(token, key, value);
                    break;
                default:
                    DrawTextField(key, value.ToString());
                    break;
            }
        }

        private void DrawArrayToken(JToken token, string key, JToken value)
        {
            var index = 0;
            //var reorderableList = new ReorderableList(so, );
            foreach (var item in new List<JToken>(token))
            {
                token[index] = EditorGUILayout.TextField(key, item.ToString());
                index++;
            }
        }

        private void DrawTextField(string key, string value)
        {
            EditorGUILayout.BeginHorizontal();
            _currentJson[key] = EditorGUILayout.TextField(key, value);
            EditorGUILayout.EndHorizontal();
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

        private void DrawForPlayerPrefsEditor()
        {
        }

        private void OnDisable()
        {
            _window = null;
        }

        private JObject ParseJson(string json) => JObject.Parse(json);
        private void UpdateCurrentJson() => _currentJson = ParseJson(_textFile.text);
    }
}