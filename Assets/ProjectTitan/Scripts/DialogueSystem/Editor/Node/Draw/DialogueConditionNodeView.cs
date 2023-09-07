using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// Logic Node에서 Condition을 나타내는 노드
    /// </summary>
    public class DialogueConditionNodeView : DialogueBaseNodeView
    {
        protected override void BuildView()
        {
            base.BuildView();

            var outputPort = CreatePort(DialoguePortType.Condition, Direction.Output, Port.Capacity.Multi);
            outputPort.portName = "Condition";
            outputContainer.Add(outputPort);
        }
    }
}
