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
        /// <summary>
        /// 선택지 자료를 저장. Pair로 쌍으로 저장되어 있다.
        /// </summary>
        public List<PortDataPair> selectionPortData = new();

        private VisualElement _bodyContainer;
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
            _bodyContainer = topContainer.parent;

            BuildAddSelection();

            // Sentence로부터 들어오는 Port
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
            // 구분줄을 위해 divider를 추가하고 그 뒤에 Add 버튼을 추가
            var divider = new VisualElement { name = "divider" };
            divider.AddToClassList("horizontal");
            _bodyContainer.Add(divider);

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
            _bodyContainer.Add(selectorAddContainer);
        }

        /// <summary>
        /// 선택지를 생성
        /// </summary>
        private void BuildSelector()
        {
            var divider = new VisualElement { name = "divider" };
            divider.AddToClassList("horizontal");
            _bodyContainer.Add(divider);

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

            _bodyContainer.Add(selectorContainer);
        }

        /// <summary>
        /// Selection 부분에 Port를 추가, 존재하는 PortDataPair를 통해서 생성된다.
        /// </summary>
        /// <param name="selectionPortData"></param>
        private void AddSelection(PortDataPair selectionPortData)
        {
            // Selector Input Port
            var selectorInputPort = CreatePort(DialoguePortType.Choice, Direction.Input, Port.Capacity.Single, ref selectionPortData.inputPortData);
            selectorInputPort.portName = $"Selector";
            _selectorInputContaienr.Add(selectorInputPort);

            // Selector Output Port
            var selectorOutputPort = CreatePort(DialoguePortType.Dialogue, Direction.Output, Port.Capacity.Single, ref selectionPortData.outputPortData);
            selectorOutputPort.portName = $"Next Dialogue";
            var selectDeleteButton = new Button(() => {
                    // 최소한 하나의 선택지는 있어야 한다.
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

        /// <summary>
        /// Selector들의 숫자를 재정렬. 시작 인덱스는 0부터 시작한다.
        /// </summary>
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
