using UnityEditor;
using UnityEngine;

namespace Inheo.UParser
{
    internal class CreateMenuContextWindow : EditorWindow
    {
        private static EditorWindow _window;

        private int tabIndex = 0;
        private Vector2 scrollPosition;
        private JsonDrawer jsonDrawer;
        private XmlDrawer xmlDrawer;

        private void OnEnable()
        {
            jsonDrawer = new JsonDrawer();
            xmlDrawer = new XmlDrawer();
        }

        [MenuItem("Window/UParser")]
        private static void ShowWindow()
        {
            if (_window != null)
                return;

            _window = GetWindow(typeof(CreateMenuContextWindow));
            _window.titleContent = new GUIContent("UParser");
            _window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;

            tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Json", "XML" });
            switch (tabIndex)
            {
                case 0:
                    jsonDrawer.Draw();
                    break;
                case 1:
                    xmlDrawer.Draw();
                    break;
            }
            EditorGUI.indentLevel = indent;

            EditorGUILayout.EndScrollView();
        }

        private void OnDestroy()
        {
            _window = null;
        }
    }
}