using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.DialogueSystem.Data.View;
using Titan.DialogueSystem.Data.Nodes;
using System;

namespace Titan.DialogueSystem.Data
{
    /// <summary>
    /// Dialogue Graph를 저장하는 Object.
    /// 에디팅은 <see cref="DialogueEditorWindow"/>에서 이루어진다.
    /// 현재는 Save 버튼을 누르면 Snap Shot을 찍듯이 저장한다.
    /// </summary>
    public class DialogueGraphObject : ScriptableObject
    {
        public List<DialogueBaseNodeData> Nodes = new List<DialogueBaseNodeData>();
        // public List<DialogueBaseNodeConnectionData> Connections = new List<DialogueBaseNodeConnectionData>();

        public string GraphName;

        public void Init(string graphName)
        {
            GraphName = graphName;
        }

        public void SaveData(DialogueGraphView graph)
        {
            graph.nodes.ForEach((node) =>
            {
                if (node is DialogueBaseNodeView nodeView)
                {
                    Nodes.Add(nodeView.GetNodeData());
                }
            });
        }

        public void LoadData(DialogueGraphView graph)
        {
            foreach (var nodeData in Nodes)
            {
                // var node = graph.CreateNode(nodeData.GetType(), nodeData.ID, nodeData.position);
                // if (node is DialogueBaseNodeView nodeView)
                // {
                //     nodeView.LoadNodeData(nodeData);
                // }
            }
        }
    }
}
