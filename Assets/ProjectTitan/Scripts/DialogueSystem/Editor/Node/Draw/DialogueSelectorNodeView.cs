using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// 선택지를 표현하는 노드.
    /// </summary>
    public class DialogueSelectorNodeView : DialogueBaseNodeView
    {
        [SerializeField] protected int _selectorCount = 1;

        private VisualElement bodyContainer;
        private VisualElement _selectorInputContaienr;
        private VisualElement _selectorOutputContainer;
        
        protected override void BuildView()
        {
            base.BuildView();
            // Load Style Sheet
            // @refactor
            // 굳이 이런식으로 하면 실수할 가능성이 높다.
            // 따로 메소드로 분리하는 것이 나아 보인다.
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueSelectorNodeView.uss"));
            bodyContainer = topContainer.parent;

            BuildAddSelection();

            var inputPort = CreatePort(DialoguePortType.Dialogue, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);
            
            BuildSelector();
        }

        /// <summary>
        /// 선택지 추가 버튼을 생성
        /// </summary>
        private void BuildAddSelection()
        {
            var divider = new VisualElement { name = "divider" };
            divider.AddToClassList("horizontal");
            bodyContainer.Add(divider);

            var selectorAddContainer = new VisualElement() { name = "selector-add"};

            var selectorAddButton = new Button(() => AddSelection())
            {
                text = "Add Selector"
            };

            selectorAddContainer.Add(selectorAddButton);
            bodyContainer.Add(selectorAddContainer);
        }

        /// <summary>
        /// 선택지를 생성
        /// </summary>
        private void BuildSelector()
        {
            var divider = new VisualElement { name = "divider" };
            divider.AddToClassList("horizontal");
            bodyContainer.Add(divider);

            var selectorContainer = new VisualElement() { name = "selector" };

            _selectorInputContaienr = new VisualElement() { name = "input" };
            var selectorDivider = new VisualElement { name = "divider" };
            selectorDivider.AddToClassList("vertical");
            _selectorOutputContainer = new VisualElement() { name = "output" };

            selectorContainer.Add(_selectorInputContaienr);
            selectorContainer.Add(selectorDivider);
            selectorContainer.Add(_selectorOutputContainer);

            int count = _selectorCount > 0 ? _selectorCount : 1;
            _selectorCount = 0;
            for (int i = 0; i < count; i++)
            {
                AddSelection();
            }
            bodyContainer.Add(selectorContainer);
        }

        private void AddSelection()
        {
            var selectorInputPort = CreatePort(DialoguePortType.Choice, Direction.Input, Port.Capacity.Single);
            selectorInputPort.portName = $"Selector {_selectorCount}";
            _selectorInputContaienr.Add(selectorInputPort);

            var selectorOutputPort = CreatePort(DialoguePortType.Dialogue, Direction.Output, Port.Capacity.Single);
            selectorOutputPort.portName = $"Next Dialogue {_selectorCount}";
            var selectDeleteButton = new Button(() => {
                    if(_selectorCount <= 1)
                    {
                        return;
                    }

                    if(selectorInputPort.connected)
                    {
                        _graphView.DeleteElements(selectorInputPort.connections);
                    }
                    _selectorInputContaienr.Remove(selectorInputPort);
                    
                    if(selectorOutputPort.connected)
                    {
                        _graphView.DeleteElements(selectorOutputPort.connections);
                    }
                    _selectorOutputContainer.Remove(selectorOutputPort);
                    // _selectorCount will be updated in ReOrderNumber()
                    ReOrderNumber();
            }){ text = "X" };
            selectorOutputPort.Add(selectDeleteButton);

            _selectorOutputContainer.Add(selectorOutputPort);
            _selectorCount++;
        }

        private void ReOrderNumber()
        {
            if(_selectorInputContaienr.childCount != _selectorOutputContainer.childCount)
            {
                Debug.LogError("Input/Output Port가 맞지 않습니다.");
                return;
            }

            for(int i = 0; i < _selectorInputContaienr.childCount; i++)
            {
                var inputPort = _selectorInputContaienr[i] as Port;
                inputPort.portName = $"Selector {i}";
                var outputPort = _selectorOutputContainer[i] as Port;
                outputPort.portName = $"Next Dialogue {i}";
            }
            _selectorCount = _selectorInputContaienr.childCount;
        }
    }
}
