using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Titan.DialogueSystem.Data.Nodes;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// Node Data를 저장하는 클래스
    /// UnityJsonUtility의 한계로 인해 지금은
    /// </summary>
    [System.Serializable]
    public class DialogueNodeData
    {
        public Vector2 position;
        // ID를 기준으로 연결을 한다.
        public string ID;
        public List<string> outputPortIDs = new();

        public string Text;

        public virtual DialogueBaseNodeView GetNodeView()
        {
            return null;
        }

        public void SetOutputPortID(DialogueBaseNodeView view)
        {
            foreach(var port in view.outputContainer.Children().Cast<Port>())
            {
                if(port.connected)
                {
                    var outputNode = port.connections.First().output.node as DialogueBaseNodeView;
                    outputPortIDs.Add(outputNode.ID);
                }
            }
        }
    }
}
