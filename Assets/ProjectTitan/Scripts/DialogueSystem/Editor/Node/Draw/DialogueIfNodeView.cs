using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Nodes
{
    public class DialogueIfNodeView : DialougeLogicNodeView
    {
        protected override void BuildView()
        {
            base.BuildView();

            var inputLogic = CreatePort(DialoguePortType.Condition, Direction.Input, Port.Capacity.Single);
            inputLogic.portName = "Condition";
            inputContainer.Add(inputLogic);
        }
    }
}
