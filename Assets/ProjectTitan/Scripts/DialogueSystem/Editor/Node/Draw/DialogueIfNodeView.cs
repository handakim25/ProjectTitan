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

        public override Condition[] GetConditions()
        {
            foreach(Port port in _graphView.ports)
            {
                var portData = port.userData as PortData;
                if(portData.PortID == _conditionInputPortData.ConnectedPortID)
                {
                    var conditionNode = port.node as DialogueEventNodeView;
                    var retCondition = new Condition() { TriggerName = conditionNode.TriggerName };
                    retCondition.ExpectedBool = GetExpectedCondition();
                    return new Condition[] { retCondition };
                }
            }
            return null;
        }
    }
}
