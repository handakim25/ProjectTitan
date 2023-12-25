using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// SelectorNode에서 Senetence를 표현할 지 안 할 지를 나타내는 Base Node
    /// </summary>
    public abstract class DialougeLogicNodeView : DialogueBaseNodeView
    {
        /// <summary>
        /// 선택지 정보
        /// </summary>
        // @Warning
        // 직렬화되어 있는 변수를 함부로 변경하면 호환성 문제가 발생한다.
        // public naming convention에 어긋나지만 호환성 유지를 위해 이름을 변경하지 않는다.
        public PortData _choiceInputPortData;
        // 동시 연결은 불가능
        public PortData trueOutputPortData; // True일 경우 출력
        public PortData falseOutputPortData; // False일 경우 출력

        /// <summary>
        /// <para>Input1 : Choice</para>
        /// <para>Output1 : True</para>
        /// <para>Output2 : False</para>
        /// </summary>
        protected override void BuildView()
        {
            base.BuildView();
            Debug.Log($"Choice Input Port Data : {_choiceInputPortData} / Node ID : {ID}");
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

        /// <summary>
        /// True 혹은 False가 가능한지 확인
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 자신의 Class에 따라 적합한 Condition을 반환
        /// Requirement를 구하는 것은 DialgoueBuilder가 하고 있다.
        /// 그 이유는 Node가 Graph 정보에 접근할 수 없고 Port ID만 가지고 있기 때문이다.
        /// </summary>
        /// <returns></returns>
        public abstract Condition GetCondtion();

        // 현재 Node가 Graph View에 접근할 수 있는 방법이 없다.
        // 추후에 구조를 개선할 것
        // 각각 노드에서 처리하는 것이 더 좋은 방법이다.
        /// <summary>
        /// 자신의 Condition Port들을 반환
        /// </summary>
        /// <returns></returns>
        public virtual List<PortData> GetConditionPortsData()
        {
            return null;
        }
    }
}
