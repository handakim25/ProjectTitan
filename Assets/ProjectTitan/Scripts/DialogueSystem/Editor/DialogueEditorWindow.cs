using UnityEngine;
using UnityEditor;

namespace Titan.DialogueSystem.Data
{
    using View;

    /// <summary>
    /// DialogueGraph를 편집하기 위한 Editor
    /// </summary>
    // DialogueEditorWindow <- DialogueEditorView <- toolbar, content(DialogueGraphView, inspector, blackboard)
    // DialogueEditorView <- Search Window
    public class DialogueEditorWindow : EditorWindow
    {
        public static string StyleSheetsPath => "Assets/ProjectTitan/Styles/";

        private const string kTitleName = "Dialogue Editor";

        /// <summary>
        /// 현재 편집 중인 Dialogue Graph Object의 GUID
        /// </summary>
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

        /// <summary>
        /// Toolbar, Content 등을 담고 있는 View
        /// </summary>
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

        /// <summary>
        /// Graph Object를 통해서 Editor를 연다.
        /// </summary>
        /// <param name="graphObject"></param>
        public static void Open(DialogueGraphObject graphObject)
        {
            var window = CreateWindow<DialogueEditorWindow>(kTitleName);
            window.Init(graphObject); // Init with data
        }

        /// <summary>
        /// GraphObject로부터 Editor를 초기화 한다.
        /// Create elements
        /// </summary>
        private void Init(DialogueGraphObject graphObject)
        {
            GraphObject = graphObject;
            EditorView = new DialogueEditorView(this, GraphObject);
            UpdateTitle();

            graphObject.LoadData(EditorView.GraphView);
        }

        // Asset refresh 시에 무효화되는 문제가 있다.
        // 이를 위해 Log를 통해서 추적하도록 한다.
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
            Debug.Log("Save Dialogue Graph");

            // @memo
            // 추후에는 Dialgoue Graph Object를 통해서 데이터를 저장하고 그것을 View에서 Visualize하는 루틴으로 수정할 것
            
            // Json으로 직접 직렬화 하는 것은 아니니까 그냥 Scriptable Object 저장 루틴을 따른다.
            GraphObject.SaveData(EditorView.GraphView);

            EditorUtility.SetDirty(GraphObject);
            GraphObject.UpdateDialogueObject(EditorView.GraphView);

            AssetDatabase.SaveAssetIfDirty(GraphObject);

            return true;
        }
    }
}
