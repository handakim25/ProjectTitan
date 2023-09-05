using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan.DialogueSystem.Data
{
    using View;

    /// <summary>
    /// Dialogue editor window
    /// </summary>
    // DialogueEditorWindow <- DialogueEditorView <- toolbar, content(DialogueGraphView, inspector, blackboard)
    // DialogueEditorView <- Search Window
    public class DialogueEditorWindow : EditorWindow
    {
        public static string StyleSheetsPath => "Assets/ProjectTitan/Styles/";

        private const string kTitleName = "Dialogue Editor";

        private string _selectedGuid;

        public string SelectedGuid {
            get => _selectedGuid;
            private set => _selectedGuid = value;
        }

        private DialogueEditorView _editorView;
        public DialogueEditorView EditorView 
        {
            get
            {
                return _editorView;
            }
            set
            {
                if(_editorView != null)
                {
                    _editorView.RemoveFromHierarchy();
                }
                
                _editorView = value;
                
                rootVisualElement.Add(_editorView);
            }
        }

        public static void Open(DialogueGraphObject graphObject)
        {
            // Load data from guid

            // Load graph from data

            var window = CreateWindow<DialogueEditorWindow>(kTitleName);
            window.Init(graphObject); // Init with data
        }

        /// <summary>
        /// Load data from guid
        /// Create elements
        /// </summary>
        private void Init(DialogueGraphObject graphObject)
        {
            SelectedGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(graphObject));
            EditorView = new DialogueEditorView(this, graphObject);
        }

        private void OnEnable()
        {
            Debug.Log($"OnEnable / EditorView : {_editorView}");
            if(EditorView == null)
            {
                if(string.IsNullOrEmpty(SelectedGuid) == false)
                {
                    var graphObject = AssetDatabase.LoadAssetAtPath<DialogueGraphObject>(AssetDatabase.GUIDToAssetPath(SelectedGuid));
                    EditorView = new DialogueEditorView(this, graphObject);
                    Debug.Log($"Re-Init with GUID : {SelectedGuid}");
                }
                Debug.Log("Re-Init");
            }
        }

        private void OnDisable()
        {
            Debug.Log($"OnDisable / EditorView : {_editorView}");
        }
    }
}
