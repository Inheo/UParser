using System;
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
        [Serializable]
        class ReorderableJArray
        {
            public bool IsExpand = true;
            public Rect HeaderRect;
            public Rect TextRect;
            public readonly JArray JArray;
            public readonly ReorderableList ReorderableList;

            public ReorderableJArray(JArray jArray, ReorderableList reorderableList)
            {
                JArray = jArray;
                ReorderableList = reorderableList;
                ReorderableList.displayAdd = IsExpand;
                ReorderableList.displayRemove = IsExpand;
                ReorderableList.draggable = IsExpand;
                ReorderableList.list = IsExpand ? JArray : new List<JToken>(0);
            }

            public void UpdateExpandState(Event e)
            {
                if (e.type == EventType.MouseDown && HeaderRect.Contains(e.mousePosition))
                {
                    IsExpand = !IsExpand;
                    ReorderableList.displayAdd = IsExpand;
                    ReorderableList.displayRemove = IsExpand;
                    ReorderableList.draggable = IsExpand;
                    ReorderableList.list = IsExpand ? JArray : new List<JToken>(0);
                    e.Use();
                }
            }
        }

        private static EditorWindow _window;
        [SerializeField] private int tabIndex = 0;
        [SerializeField] private TextAsset _textFile;

        [SerializeField] private Vector2 scrollPosition;
        [SerializeField] private JObject _currentJson;
        [SerializeField] private Dictionary<string, ReorderableJArray> arrayTokens;

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
            Undo.RecordObject(this, "changed file");
            var ischecked = EditorGUI.EndChangeCheck();
            if (ischecked)
            {
                EditorUtility.SetDirty(this);
            }

            if (_textFile == null)
                return;

            if (ischecked)
            {
                UpdateCurrentJson();
            }

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
                case JTokenType.String:
                    DrawSimpleText(key, value.ToString());
                    break;
                case JTokenType.Array:
                    DrawArrayToken((JArray)token, key, value);
                    break;
                default:
                    EditorGUILayout.LabelField("None");
                    break;
            }
        }

        private void DrawArrayToken(JArray jArray, string key, JToken value)
        {
            var indent = EditorGUI.indentLevel;
            var verticalSpacing = EditorGUIUtility.standardVerticalSpacing * 2;

            if (!arrayTokens.ContainsKey(key))
            {
                var reorderableList = new ReorderableList(jArray, typeof(JArray), true, true, true, true);
                var rJAray = new ReorderableJArray(jArray, reorderableList);
                arrayTokens[key] = rJAray;

                var reorderableJArray = arrayTokens[key];

                reorderableList.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, key + $" (Count: {rJAray.JArray.Count})");
                    rJAray.HeaderRect = rect;
                    rJAray.UpdateExpandState(Event.current);
                };
                reorderableList.drawElementCallback = (rect, i, isActive, isFocused) =>
                {
                    rJAray.TextRect = rect;
                    if (!rJAray.IsExpand) return;
                    EditorGUI.indentLevel = indent + 1;
                    rect.height = GetElementClearHeight(jArray[i].ToString(), rect);
                    rect.y += verticalSpacing;
                    jArray[i] = EditorGUI.TextArea(rect, jArray[i].ToString());
                    EditorGUI.indentLevel = indent;
                };

                reorderableList.drawNoneElementCallback = rect =>
                {
                    reorderableList.elementHeight = -10;
                };

                reorderableList.onAddCallback = list => list.list.Add(default);
                reorderableList.elementHeightCallback = i =>
                    GetElementHeight(jArray[i].ToString(), rJAray.IsExpand, verticalSpacing, rJAray.TextRect);
            }

            arrayTokens[key].ReorderableList.DoLayoutList();
        }

        private void DrawSimpleText(string key, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(key);
            _currentJson[key] = EditorGUILayout.TextArea(value);
            EditorGUILayout.EndHorizontal();
        }

        private float GetElementHeight(string text, bool isExpand, float offset, Rect rect)
        {
            if (!isExpand)
                return 0;

            float height = GetElementClearHeight(text, rect) + offset;
            return height;
        }

        private float GetElementClearHeight(string text, Rect rect)
        {
            float textHeight = EditorStyles.textArea.CalcHeight(new GUIContent(text), rect.width);
            return textHeight + EditorGUIUtility.standardVerticalSpacing * 2;
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