using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Inheo.UParser
{
    internal class CreateMenuContextWindow : EditorWindow
    {
        class ReorderableJArray
        {
            private const float elementHeight = 20;
            public JArray JArray;
            public ReorderableList ReorderableList;

            public ReorderableJArray(JArray jArray, ReorderableList reorderableList)
            {
                JArray = jArray;
                ReorderableList = reorderableList;
                ReorderableList.elementHeight = elementHeight;
            }
        }

        private int tabIndex = 0;
        private static EditorWindow _window;
        private TextAsset _textFile;

        private JObject _currentJson;
        private Dictionary<string, ReorderableJArray> arrayTokens;

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
            //Editor.CreateEditor();
            arrayTokens = new Dictionary<string, ReorderableJArray>();
        }

        private void OnGUI()
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
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
            EditorGUI.indentLevel = indent;
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
                    DrawArrayToken((JArray)token, key, value);
                    break;
                default:
                    DrawTextField(key, value.ToString());
                    break;
            }
        }

        private void DrawArrayToken(JArray jArray, string key, JToken value)
        {
            var indent = EditorGUI.indentLevel;
            if (!arrayTokens.ContainsKey(key))
            {
                var reorderableList = new ReorderableList(jArray, typeof(JArray), true, true, true, true);
                arrayTokens[key] = new ReorderableJArray(jArray, reorderableList);

                var reorderableJArray = arrayTokens[key];

                reorderableList.drawHeaderCallback += rect => { EditorGUI.LabelField(rect, key); };
                reorderableList.drawElementCallback += (rect, i, isActive, isFocused) =>
                {
                    EditorGUI.indentLevel = indent + 1;
                    jArray[i] = EditorGUI.TextField(rect, jArray[i].ToString());
                    EditorGUI.indentLevel = indent;
                };

                reorderableList.onAddCallback += D;
            }
            arrayTokens[key].ReorderableList.DoLayoutList();
        }

        private void D(ReorderableList list)
        {
            list.list.Add(default);
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