using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// If 노드를 표현하는 노드, 한 가지 조건식을 포함할 수 있다.
    /// </summary>
    public class DialogueIfNodeView : DialougeLogicNodeView
    {
        /// <summary>
        /// 조건식을 입력받는 Port
        /// </summary>
        public PortData _conditionInputPortData;

        protected override void BuildView()
        {
            base.BuildView();

            var inputLogic = CreatePort(DialoguePortType.Condition, Direction.Input, Port.Capacity.Single, ref _conditionInputPortData);
            inputLogic.portName = "Condition";
            inputContainer.Add(inputLogic);
        }

        public override Condition GetCondtion()
        {
            return new Condition()
            {
                Type = Condition.ConditionType.If,
                ExpectedBool = GetExpectedBool(),
            };
        }

        public override List<PortData> GetConditionPortsData()
        {
            return new List<PortData>() { _conditionInputPortData };
        }
    }
}
