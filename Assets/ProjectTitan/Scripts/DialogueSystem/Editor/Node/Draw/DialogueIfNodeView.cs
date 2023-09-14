using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Nodes
{
    public class DialogueIfNodeView : DialougeLogicNodeView
    {
        public PortData _conditionInputPortData;

        public override Condition[] GetConditions()
        {
            throw new System.NotImplementedException();
        }

        protected override void BuildView()
        {
            base.BuildView();

            var inputLogic = CreatePort(DialoguePortType.Condition, Direction.Input, Port.Capacity.Single, ref _conditionInputPortData);
            inputLogic.portName = "Condition";
            inputContainer.Add(inputLogic);
        }
    }
}
