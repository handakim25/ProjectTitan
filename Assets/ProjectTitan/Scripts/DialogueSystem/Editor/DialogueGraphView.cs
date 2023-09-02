using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Titan.DialogueSystem.Data.View
{
    /// <summary>
    /// Dialogue Graph View
    /// </summary>
    public class DialogueGraphView : GraphView
    {
        public DialogueGraphView()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueGraphView.uss"));
        }

        // Context Menu를 만드는데는 2가지 방법이 있다.
        // Context Menu는 우클릭 했을 때 나오는 메뉴이다.
        // 1. 첫번째는 이 함수를 오버라이드하는 것.
        // 2. 두번째는 ContextualMenuManipulator를 이용하는 것이다.
        // 타겟에 대해 다른 메뉴를 띄우기에는 아무래도 첫번째 방법이 더 적합하다.
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            Debug.Log("BuildContextualMenu");
            base.BuildContextualMenu(evt);

            // 추후에 노드인지 뭐 다른 거인지 다 고려해서 메뉴 생성
            // 여기에도 2가지 방법이 있다. 우선은 Graph View에서 일괄적으로 처리
            // 두번째는 각각의 요소가 적절한 메뉴를 출력하는 것이다.
        }
    }
}
