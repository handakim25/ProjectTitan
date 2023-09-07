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
        protected override void BuildView()
        {
            base.BuildView();
            var inputChoice = CreatePort(DialoguePortType.Choice, Direction.Input, Port.Capacity.Single);
            inputChoice.portName = "Choice";
            inputContainer.Add(inputChoice);

            var outputTrue = CreatePort(DialoguePortType.Choice, Direction.Output, Port.Capacity.Single);
            outputTrue.portName = "True Choice";
            outputContainer.Add(outputTrue);
        }
    }
}
