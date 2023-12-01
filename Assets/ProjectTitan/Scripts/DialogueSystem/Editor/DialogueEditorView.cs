using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Titan.DialogueSystem.Data.View
{
    /// <summary>
    /// 모든 UI 요소를 담는 클래스
    /// e.g. Toolbar, GraphView, Blackboard, Inspector
    /// </summary>
    public class DialogueEditorView : VisualElement
    {
        /// <summary>
        /// Paretnt Window
        /// </summary>
        private DialogueEditorWindow _window;

        // View Elements
        /// <summary>
        /// Editor View 중에서 Graph를 표현하는 Graph View
        /// </summary>
        private DialogueGraphView _graphView;
        /// <summary>
        /// GraphView에서 생성 Window를 띄울 때 사용하는 Search Window
        /// </summary>
        private DialogueSearchWindow _searchWindow;

        // Toolbar Buttons
        private Button _blackBoardButton;
        private Button _inspectorButton;

        // Callbacks
        public Action SaveRequest;
        public Action SaveAsRequest;

        public DialogueGraphView GraphView => _graphView;

        public DialogueEditorView(DialogueEditorWindow editorWindow, DialogueGraphObject graphObject)
        {
            _window = editorWindow;

            // Load Styel Sheet
            var styleVar = AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueCssVar.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueEditorView.uss");
            styleSheets.Add(styleSheet);
            styleSheets.Add(styleVar);

            // Create Elements
            CreateToolbar();
            CreateContent();
            CreateSearchWindow();
        }

        #region Create Elements
        
        /// <summary>
        /// 최상단 툴바 생성
        /// </summary>
        private void CreateToolbar()
        {
            var toolbar = new Toolbar() {name = "toolbar"};
            toolbar.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueToolbar.uss"));

            var leftGroup = new VisualElement() {name = "leftGroup"};
            
            var saveButton = new Button(() => SaveRequest?.Invoke()) {text = "Save"};
            var saveAsButton = new Button() {text = "Save As"};
            var showInProjectButton = new Button() {text = "Show In Project"};

            leftGroup.Add(saveButton);
            leftGroup.Add(saveAsButton);
            leftGroup.Add(showInProjectButton);
            leftGroup.Children().First().AddToClassList("toolbar__button-first");
            toolbar.Add(leftGroup);

            var rightGroup = new VisualElement() {name = "rightGroup"};

            _blackBoardButton = new Button(() =>
            {
                ToggleBlackBoard(); 
            }) { text = "Blackboard" };
            _inspectorButton = new Button(() =>
            {
                ToggleInspector(); 
            }) {text = "Inspector"};

            rightGroup.Add(_blackBoardButton);
            rightGroup.Add(_inspectorButton);
            rightGroup.Children().Last().AddToClassList("toolbar__button-last");
            toolbar.Add(rightGroup);

            toolbar.Query<Button>().ForEach((Button button) => button.AddToClassList("toolbar__button"));

            Add(toolbar);
        }

        /// <summary>
        /// 중간에 들어가는 Content 생성
        /// <para>- GraphView</para>
        /// <para>- Blackboard</para>
        /// <para>- Inspector</para>
        /// </summary>
        private void CreateContent()
        {
            // Content로 분리함으로써 나중에 하단바를 추가할 수도 있다.
            // Graph View를 담는 Element
            var content = new VisualElement() { name = "content" };

            // Graph View
            _graphView = new DialogueGraphView() { name = "GraphView" };
            _graphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale); // Zoom
            _graphView.AddManipulator(new ContentDragger()); // 그래프 이동
            _graphView.AddManipulator(new SelectionDragger()); // 노드를 선택해서 이동 가능
            _graphView.AddManipulator(new RectangleSelector());  // 드래그해서 사각형 영역 선택

            content.Add(_graphView);

            Add(content);
        }

        /// <summary>
        /// 스페이스 바를 누르거나 아니면 Create Node를 눌렀을 때 되는 콜백 함수이다.
        /// </summary>
        // Graph View에서 Node 생성 Request할 때 호출되는 함수
        public void CreateSearchWindow()
        {
            _searchWindow = ScriptableObject.CreateInstance<DialogueSearchWindow>();
            _searchWindow.Initialize(_window, _graphView, _window.GraphObject);
            _graphView.nodeCreationRequest = NodeCreationRequest;
        }
        
        #endregion Create Elements

        #region Callbacks
        
        /// <summary>
        /// Blackboard를 토글한다. 현재 기능은 없다.
        /// </summary>
        private void ToggleBlackBoard()
        {
            _blackBoardButton.ToggleInClassList("toolbar__button-selected");
        }

        /// <summary>
        /// Inspector를 토글한다. 현재 기능은 없다.
        /// </summary>
        private void ToggleInspector()
        {
            _inspectorButton.ToggleInClassList("toolbar__button-selected");
        }
        
        /// <summary>
        /// Create 메뉴를 누르면 호출 된다.
        /// </summary>
        /// <param name="context"></param>
        public void NodeCreationRequest(NodeCreationContext context)
        {
            // Draw Search Window
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }
        #endregion Callbacks

    }
}
