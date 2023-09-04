using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.View
{
    using Nodes;

    // 참고 링크
    // SearchWindowProvider : https://github.com/Unity-Technologies/Graphics/blob/e7b7590646a976e80e01c4df841959ef6b27134d/Packages/com.unity.shadergraph/Editor/Drawing/SearchWindowProvider.cs
    // SearchWindowAdapter : https://github.com/Unity-Technologies/Graphics/blob/e7b7590646a976e80e01c4df841959ef6b27134d/Packages/com.unity.shadergraph/Editor/Drawing/SearchWindowAdapter.cs
    public class DialogueSearchWindow : ScriptableObject, ISearchWindowProvider
    {   
        // Node를 추가하기 위해서는 GraphView가 필요
        private EditorWindow _editorWindow;
        private DialogueGraphView _graphView;
        public void Initialize(DialogueEditorWindow window, DialogueGraphView graphView)
        {
            _editorWindow = window;
            _graphView = graphView;
        }

        /// <summary>
        /// Search Tree 메뉴를 만드는 함수
        /// </summary>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // SearchTreeGroupEntry
            // 하나의 그룹을 표현한다.
            // SearchTreeEntry
            // 하나의 요소를 표현한다.
            // - GUIContent : 화면에 보여지는 하나의 그룹
            // - level : 요소의 level, level이 같으면 같은 그룹으로 묶인다. 근데 어디로 묶이는 건 어떻게 정하지?
            
            // @refactor
            // 지금은 하드 코딩해서 들어가고 추후에 개선할 것
            // 생각할 수 있느 것은 Reflection
            List<SearchTreeEntry> searchTreeEntries = new()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element"), 0),
                new SearchTreeEntry(new GUIContent("Sentence"))
                {
                    level = 1,
                    userData = typeof(DialogueSentenceNodeView),
                },
                new SearchTreeEntry(new GUIContent("Choice"))
                {
                    level = 1,
                    userData = typeof(DialogueChoiceNodeView),
                },
                new SearchTreeEntry(new GUIContent("Selector"))
                {
                    level = 1,
                    userData = typeof(DialogueSelectorNodeView),
                },
                new SearchTreeGroupEntry(new GUIContent("Logics"), 1),
                new SearchTreeEntry(new GUIContent("If"))
                {
                    level = 2,
                    userData = typeof(DialogueIfNodeView),
                },
                new SearchTreeEntry(new GUIContent("Any"))
                {
                    level = 2,
                    userData = typeof(DialogueAnyNodeView),
                },
                new SearchTreeEntry(new GUIContent("All"))
                {
                    level = 2,
                    userData = typeof(DialogueAllNodeView),
                },
                new SearchTreeGroupEntry(new GUIContent("Conditions"), 1),
                new SearchTreeEntry(new GUIContent("Quest State Condition"))
                {
                    level = 2,
                },
                new SearchTreeEntry(new GUIContent("Item Condition"))
                {
                    level = 2,
                },
                new SearchTreeEntry(new GUIContent("Event Condition"))
                {
                    level = 2,
                },
                new SearchTreeGroupEntry(new GUIContent("Actions"), 1),
                new SearchTreeEntry(new GUIContent("Trigger Quest"))
                {
                    level = 2,
                },
                new SearchTreeEntry(new GUIContent("Trigger Event"))
                {
                    level = 2,
                },
                new SearchTreeEntry(new GUIContent("Give Item"))
                {
                    level = 2,
                },
            };
            return searchTreeEntries;
        }

        /// <summary>
        /// Action을 정의
        /// </summary>
        /// <param name="SearchTreeEntry"></param>
        /// <param name="context"></param>
        /// <returns>true : close / false : still open</returns>
        // Shader Graph의 생성 루틴
        // 1. Material Node를 생성
        // 2. Material Node를 GraphData에 추가
        // 3. Graph Data에 추가
        // 4. Graph Data에 AddedNode에 추가 됨
        // 5. MaterialGraphEditWindow의 Update에서 변경을 감지하고 Graph Editor View의 HandleGraphChanges를 호출
        // 6. HandleGraphChanges에서 AddNode를 호출, Material Node에 따라서 적절한 Node View를 생성(e.g. property ndoe -> property node view)
        // 6. 여기서 호출되고 나면 graph data의 ClearChanges를 호출해서 reset
        // 크게 잡아서 설명을 하자면 Material Node(data) -> Data Holder Graph Data > Graph Editor Handle Graph Changes -> Graph View에 추가
        // 여기서도 다시 설명을 하자면 지금은 여기서 View만을 생성한다. DialogueBaseNode관련 설명 참고
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // Create Node
            System.Type type = SearchTreeEntry.userData as System.Type;
            DialogueBaseNodeView node = type != null ? System.Activator.CreateInstance(type) as DialogueBaseNodeView : new DialogueSentenceNodeView();
            node.Initialize(_graphView);
            
            node.SetPosition(new Rect(CalculateScreenPos(context.screenMousePosition), Vector2.zero));
            _graphView.AddElement(node);
            return true;
        }

        // 만약 나중에 다른 곳에서도 사용된다면 Utility 쪽으로 옮길 것
        // 혹은 Graph View로 옮길 것
        private Vector2 CalculateScreenPos(Vector2 screenMousePos)
        {
            var windowRoot = _editorWindow.rootVisualElement;
            // screenMousePos - _editorWindow.position.position == editorwindow Position을 기준으로 위치
            // editorWindow position : 좌측 상단 스크린 좌표
            // screenMousePos : 스크린 좌표
            // contentViewContainer : 고정 좌표이다. 왜냐하면 에디터에서 항상 고정된 위치에 있기 때문
            // var windowMousePos = windowRoot.ChangeCoordinatesTo(windowRoot.parent, screenMousePos - _editorWindow.position.position);
            var windowMousePos = screenMousePos - _editorWindow.position.position;

            // Debug.Log($"windowMousePos : {windowMousePos}");
            // Debug.Log($"editor pos : {_editorWindow.position.position}");
            // Debug.Log($"contaienr pos : {_graphView.contentViewContainer.worldBound.position}");
            // 툴바 같은 것은 어떻게 되지?
            var graphMousePos = _graphView.contentViewContainer.WorldToLocal(windowMousePos);

            return graphMousePos;
        }
    }
}
