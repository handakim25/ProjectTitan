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

        public string SeletedGuid {get; private set;}

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

        // For testing
        // Replace after
        [MenuItem("Tools/DialogueSystem")]
        public static void Open()
        {
            // Load data from guid

            // Load graph from data

            var window = CreateWindow<DialogueEditorWindow>(kTitleName);
            window.Init(); // Init with data
        }

        /// <summary>
        /// Load data from guid
        /// Create elements
        /// </summary>
        private void Init()
        {
            EditorView = new DialogueEditorView(this);
        }

        private void OnEnable()
        {
            Debug.Log($"OnEnable / EditorView : {_editorView}");
            if(EditorView == null)
            {
                Init();
                Debug.Log("Re-Init");
            }
        }

        private void OnDisable()
        {
            Debug.Log($"OnDisable / EditorView : {_editorView}");
        }
    }
}
