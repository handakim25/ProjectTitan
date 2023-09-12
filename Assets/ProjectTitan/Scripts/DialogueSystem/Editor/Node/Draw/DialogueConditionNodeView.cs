using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// Logic Node에서 Condition을 나타내는 노드
    /// </summary>
    public abstract class DialogueConditionNodeView : DialogueBaseNodeView
    {
        [SerializeField] private PortData _conditionOutputPortData;

        protected override void BuildView()
        {
            base.BuildView();

            var outputPort = CreatePort(DialoguePortType.Condition, Direction.Output, Port.Capacity.Multi, ref _conditionOutputPortData);
            outputPort.portName = "Condition";
            outputContainer.Add(outputPort);
        }
    }
}
