using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Nodes
{
    public class DialogueIfNodeView : DialougeLogicNodeView
    {
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
