using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan.DialogueSystem.Data
{
    using Graph;

    /// <summary>
    /// Dialogue editor window
    /// </summary>
    public class DialogueEditorWindow : EditorWindow
    {
        private const string kTitleName = "Dialogue Editor";

        private DialogueGraphView _graphView;
        internal DialogueGraphView GraphView => _graphView;

        // For testing
        // Replace
        [MenuItem("Tools/DialogueSystem")]
        public static void Open()
        {
            // GetWindow<DialogueEditorWindow>(kTitleName);
            var window = CreateWindow<DialogueEditorWindow>(kTitleName);
            window.Init(); // Init with data
        }

        private void CreateGUI()
        {
            DialogueGraphView graphView = new DialogueGraphView(this);


        }

        private void Init()
        {
            Debug.Log("Init");
        }
    }
}
