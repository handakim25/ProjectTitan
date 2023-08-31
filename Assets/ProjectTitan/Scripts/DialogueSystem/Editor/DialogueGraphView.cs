using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Graph
{
    public class DialogueGraphView : GraphView
    {
        private EditorWindow _window;
        public DialogueGraphView(EditorWindow editorWindow)
        {
            _window = editorWindow;
            var test = editorWindow as DialogueEditorWindow;
            
        }
    }
}
