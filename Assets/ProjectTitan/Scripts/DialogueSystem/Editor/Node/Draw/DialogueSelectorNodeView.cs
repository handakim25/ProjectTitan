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
        public List<PortDataPair> selectionPortData = new();

        private VisualElement bodyContainer;
        private VisualElement _selectorInputContaienr;
        private VisualElement _selectorOutputContainer;

        // 코드 구조가 이러한 형식에 따라 중복이 발생한다.
        // 다른 곳에서 언급했듯이 이러한 공통 형식을 추후에 slot으로 만들 것
        public PortData dialogueInputPortData;

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

            var inputPort = CreatePort(DialoguePortType.Dialogue, Direction.Input, Port.Capacity.Multi, ref dialogueInputPortData);
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

            var selectorAddButton = new Button(() => {
                var portDataPair = new PortDataPair();
                selectionPortData.Add(portDataPair);
                AddSelection(portDataPair);
                ReOrderNumber();
            })
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

            if(selectionPortData.Count == 0)
            {
                selectionPortData.Add(new PortDataPair());
            }

            foreach(var SelectionPortData in selectionPortData)
            {
                AddSelection(SelectionPortData);
            }
            ReOrderNumber();

            bodyContainer.Add(selectorContainer);
        }

        private void AddSelection(PortDataPair selectionPortData)
        {
            var selectorInputPort = CreatePort(DialoguePortType.Choice, Direction.Input, Port.Capacity.Single, ref selectionPortData.inputPortData);
            selectorInputPort.portName = $"Selector";
            _selectorInputContaienr.Add(selectorInputPort);

            var selectorOutputPort = CreatePort(DialoguePortType.Dialogue, Direction.Output, Port.Capacity.Single, ref selectionPortData.outputPortData);
            selectorOutputPort.portName = $"Next Dialogue";
            var selectDeleteButton = new Button(() => {
                    if(this.selectionPortData.Count == 1)
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

                    this.selectionPortData.Remove(selectionPortData);
                    // _selectorCount will be updated in ReOrderNumber()
                    ReOrderNumber();
            }){ text = "X" };
            selectorOutputPort.Add(selectDeleteButton);

            _selectorOutputContainer.Add(selectorOutputPort);
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
        }
    }
}
