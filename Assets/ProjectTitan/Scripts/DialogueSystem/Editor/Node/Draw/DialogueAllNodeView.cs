using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Titan.DialogueSystem.Data.Nodes
{
    public class DialogueAllNodeView : DialougeLogicNodeView
    {
        [SerializeField] protected List<PortData> _conditionInputPortDataList = new List<PortData>();

        protected override void BuildView()
        {
            // 코드가 굉장히 중복된다.
            // 이래서 ShaderGraph가 Material Slot을 이용해서 Port를 제어하는 것 같다.
            // 일단은 어쩔 수 없다.
            base.BuildView();
            var conditionAddContainer = new VisualElement() { name = "condition-add" };
            var conditionAddButton = new Button(() =>
            {
                PortData portData = AddCondition(null);
                _conditionInputPortDataList.Add(portData);
            })
            { text = "Add Condition" };

            conditionAddContainer.Add(conditionAddButton);
            topContainer.parent.Add(conditionAddContainer);
            topContainer.PlaceInFront(conditionAddContainer);

            var divider = new VisualElement() {name = "divider"};
            divider.AddToClassList("horizontal");
            topContainer.parent.Add(divider);
            divider.PlaceInFront(conditionAddContainer);

            if(_conditionInputPortDataList.Count == 0)
            {
                var portData = AddCondition(null);
                _conditionInputPortDataList.Add(portData);
            }
            else
            {
                foreach(var portData in _conditionInputPortDataList)
                {
                    AddCondition(portData);
                }
            }
        }

        private PortData AddCondition(PortData portData)
        {
            var inputLogicPort = CreatePort(DialoguePortType.Condition, Direction.Input, Port.Capacity.Single, ref portData);
            inputLogicPort.portName = "Condition";

            {
                var deleteButton = new Button(() =>
                {
                    if (_conditionInputPortDataList.Count <= 1)
                    {
                        return;
                    }
                    if (inputLogicPort.connected)
                    {
                        // Disconnect할 필요는 없나?
                        _graphView.DeleteElements(inputLogicPort.connections);
                    }
                    inputContainer.Remove(inputLogicPort);
                    _conditionInputPortDataList.Remove(portData);
                })
                { text = "X" };

                inputLogicPort.Add(deleteButton);
            }

            inputContainer.Add(inputLogicPort);

            return portData;
        }

        public override Condition GetCondtion()
        {
            throw new System.NotImplementedException();
        }
    }
}
