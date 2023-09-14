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

        DialogueGraphObject _graphObject;
        public DialogueGraphObject GraphObject
        {
            get => _graphObject;
            private set
            {
                if(_graphObject != null)
                {
                    Destroy(_graphObject);
                }
                _graphObject = value;
                SelectedGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_graphObject));
            }
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
                _editorView.SaveRequest = () => SaveAsset();
                
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
            GraphObject = graphObject;
            EditorView = new DialogueEditorView(this, GraphObject);
            UpdateTitle();

            graphObject.LoadData(EditorView.GraphView);
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

        public void UpdateTitle()
        {
            string titleText = string.IsNullOrEmpty(GraphObject.name) ? kTitleName : GraphObject.name;
            titleContent = new GUIContent(titleText);
        }

        public bool SaveAsset()
        {
            Debug.Log("SaveAsset");

            // Json으로 직접 직렬화 하는 것은 아니니까 그냥 Scriptable Object 저장 루틴을 따른다.
            // 추후에는 Dialgoue Graph Object를 통해서 데이터를 저장하고 그것을 View에서 Visualize하는 루틴으로 수정할 것
            GraphObject.SaveData(EditorView.GraphView);

            EditorUtility.SetDirty(GraphObject);
            GraphObject.UpdateDialogueObject(EditorView.GraphView);

            AssetDatabase.SaveAssetIfDirty(GraphObject);

            return true;
        }
    }
}
