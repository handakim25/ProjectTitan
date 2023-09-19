using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// SelectorNode에서 Senetence를 표현할 지 안 할 지를 나타내는 노드
    /// </summary>
    public abstract class DialougeLogicNodeView : DialogueBaseNodeView
    {
        public PortData _choiceInputPortData;
        // 동시 연결은 불가능
        public PortData trueOutputPortData; // True일 경우 출력
        public PortData falseOutputPortData; // False일 경우 출력

        protected override void BuildView()
        {
            base.BuildView();
            var inputChoice = CreatePort(DialoguePortType.Choice, Direction.Input, Port.Capacity.Single, ref _choiceInputPortData);
            inputChoice.portName = "Choice";
            inputContainer.Add(inputChoice);

            var outputTrue = CreatePort(DialoguePortType.Choice, Direction.Output, Port.Capacity.Single, ref trueOutputPortData);
            outputTrue.portName = "True Choice";
            outputContainer.Add(outputTrue);

            var outputFalse = CreatePort(DialoguePortType.Choice, Direction.Output, Port.Capacity.Single, ref falseOutputPortData);
            outputFalse.portName = "False Choice";
            outputContainer.Add(outputFalse);
        }

        // 전제 조건
        // true 혹은 output이 존재해야 한다.
        public bool GetExpectedBool()
        {
            if(string.IsNullOrEmpty(trueOutputPortData.ConnectedPortID))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsValid()
        {
            if(string.IsNullOrEmpty(trueOutputPortData.ConnectedPortID) && string.IsNullOrEmpty(falseOutputPortData.ConnectedPortID))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public abstract Condition GetCondtion();

        // 현재 Node가 Graph View에 접근할 수 있는 방법이 없다.
        // 추후에 구조를 개선할 것
        // 각각 노드에서 처리하는 것이 더 좋은 방법이다.
        public virtual List<PortData> GetConditionPortsData()
        {
            return null;
        }
    }
}
