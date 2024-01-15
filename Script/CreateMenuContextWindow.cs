using System.IO;
using Inheo.UParser.JDrawer;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Inheo.UParser
{
    internal class CreateMenuContextWindow : EditorWindow
    {
        private static EditorWindow _window;
        private int tabIndex = 0;
        private TextAsset _textFile;

        private Vector2 scrollPosition;
        private JObject _currentJson;

        private JValueDrawer jValueDrawer;
        private JArrayDrawer jArrayDrawer;
        private JObjectDrawer jObjectDrawer;

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
            jValueDrawer = new JValueDrawer();
            jArrayDrawer = new JArrayDrawer();
            jObjectDrawer = new JObjectDrawer();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
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

            EditorGUILayout.EndScrollView();
        }

        private void DrawForFileEditor()
        {
            EditorGUI.BeginChangeCheck();
            _textFile = (TextAsset)EditorGUILayout.ObjectField("Text Asset", _textFile, typeof(TextAsset), false);

            if (_textFile == null)
                return;

            if (EditorGUI.EndChangeCheck())
                UpdateCurrentJson();

            if(_currentJson == null && _textFile != null)
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
            switch (value.Type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                case JTokenType.Guid:
                    jValueDrawer.Draw(key, value);
                    break;
                case JTokenType.Object:
                    jObjectDrawer.Draw(key, value);
                    break;
                case JTokenType.Array:
                    jArrayDrawer.Draw(key, value);
                    break;
                default:
                    EditorGUILayout.LabelField("None");
                    break;
            }
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